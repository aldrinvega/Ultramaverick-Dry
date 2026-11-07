using System.Collections.Generic;
using System.Net.Http;
using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.CORE.INTERFACES.ORDER_HUB;
using ELIXIR.DATA.CORE.INTERFACES.ORDERING_INTERFACE;
using ELIXIR.DATA.SERVICES;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace ELIXIR.Tests;

/// <summary>
/// Boots the real <c>Startup</c> pipeline with the data layer and SignalR hub context replaced by
/// substitutes. No SQL Server connection is made and no real appsettings/user-secrets are read
/// (environment "Testing", in-memory config only).
/// </summary>
public sealed class ElixirApiFactory : WebApplicationFactory<ELIXIR.API.Program>
{
    /// <summary>What the browser/axios frontend sends by default.</summary>
    public const string FrontendAccept = "application/json, text/plain, */*";

    public IOrdering Order { get; private set; } = null!;
    public IUnitOfWork UnitOfWork { get; private set; } = null!;
    public IHubContext<OrderHub, IOrderHub> Hub { get; private set; } = null!;

    public ElixirApiFactory() => Reset();

    /// <summary>Fresh substitutes; call at the start of every test.</summary>
    public void Reset()
    {
        Order = Substitute.For<IOrdering>();
        UnitOfWork = Substitute.For<IUnitOfWork>();
        UnitOfWork.Order.Returns(_ => Order);
        Hub = Substitute.For<IHubContext<OrderHub, IOrderHub>>();
    }

    public HttpClient CreateFrontendClient()
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("Accept", FrontendAccept);
        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtConfig:Key"] = "test-only-signing-key-not-a-secret-0123456789",
                ["ConnectionStrings:LiveConnectionDevelopment"] = "Server=unused.invalid;Database=unused;"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddScoped(_ => UnitOfWork);
            services.AddTransient(_ => Hub);
        });
    }
}
