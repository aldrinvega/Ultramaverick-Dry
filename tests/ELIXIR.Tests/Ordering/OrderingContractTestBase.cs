using System.Net.Http;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.CORE.INTERFACES.ORDERING_INTERFACE;
using NSubstitute;
using Xunit;

namespace ELIXIR.Tests.Ordering;

/// <summary>
/// Base for the Ordering contract tests. Tests call the HTTP surface only, so the same tests pin the MVC
/// controller and the Minimal API replacement.
/// </summary>
public abstract class OrderingContractTestBase : IClassFixture<ElixirApiFactory>
{
    protected const string BasePath = "api/Ordering/";

    protected OrderingContractTestBase(ElixirApiFactory factory)
    {
        Factory = factory;
        Factory.Reset();
        Client = factory.CreateFrontendClient();
    }

    protected ElixirApiFactory Factory { get; }
    protected HttpClient Client { get; }
    protected IOrdering Order => Factory.Order;
    protected IUnitOfWork UnitOfWork => Factory.UnitOfWork;

    protected Task<HttpResponseMessage> Get(string pathAndQuery) => Client.GetAsync(BasePath + pathAndQuery);

    protected Task<HttpResponseMessage> Send(HttpMethod method, string pathAndQuery, object body)
        => Client.SendAsync(Contract.Request(method, BasePath + pathAndQuery, Contract.Body(body)));

    protected Task<HttpResponseMessage> SendRaw(HttpMethod method, string pathAndQuery, string json)
        => Client.SendAsync(Contract.Request(method, BasePath + pathAndQuery, Contract.RawBody(json)));

    protected Task<HttpResponseMessage> SendNoBody(HttpMethod method, string pathAndQuery)
        => Client.SendAsync(Contract.Request(method, BasePath + pathAndQuery));

    protected Task AssertCompleted(int times) => UnitOfWork.Received(times).CompleteAsync();
}
