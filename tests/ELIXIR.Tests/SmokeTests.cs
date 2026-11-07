using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using NSubstitute;
using Xunit;

namespace ELIXIR.Tests;

public sealed class SmokeTests : IClassFixture<ElixirApiFactory>
{
    private readonly ElixirApiFactory _factory;

    public SmokeTests(ElixirApiFactory factory)
    {
        _factory = factory;
        _factory.Reset();
    }

    [Fact]
    public async Task Host_boots_with_substituted_data_layer()
    {
        _factory.Order.GetAllListForApprovalOfSchedule().Returns(new List<OrderDto>());
        var client = _factory.CreateFrontendClient();

        var response = await client.GetAsync("api/Ordering/GetAllListForScheduleApproval");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await _factory.Order.Received(1).GetAllListForApprovalOfSchedule();
    }
}
