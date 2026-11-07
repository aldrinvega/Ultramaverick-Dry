using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;
using OrderingEntity = ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL.Ordering;

namespace ELIXIR.Tests.Ordering;

/// <summary>Contract tests for SPEC.md endpoints #43–52 (transact, SignalR preparation lock, allocation).</summary>
[Trait("Group", "D")]
public sealed class GroupD_TransactAllocationTests : OrderingContractTestBase
{
    public GroupD_TransactAllocationTests(ElixirApiFactory factory) : base(factory) { }

    // #43 GET GetTotalListForMoveOrder
    [Fact]
    public async Task GetTotalListForMoveOrder_returns_json_and_passes_status()
    {
        var list = new List<OrderDto> { TestData.OrderDto() };
        Order.TotalListForTransactMoveOrder(true).Returns(list);

        var response = await Get("GetTotalListForMoveOrder?status=true");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    [Fact]
    public async Task GetTotalListForMoveOrder_missing_status_binds_false()
    {
        Order.TotalListForTransactMoveOrder(Arg.Any<bool>()).Returns(new List<OrderDto>());

        await Contract.AssertJsonLiteral(await Get("GetTotalListForMoveOrder"), HttpStatusCode.OK, "[]");
        await Order.Received(1).TotalListForTransactMoveOrder(false);
    }

    // #44 GET ListOfMoveOrdersForTransact
    [Fact]
    public async Task ListOfMoveOrdersForTransact_returns_json_and_passes_orderid()
    {
        var list = new List<MoveOrderDto> { TestData.MoveOrderDto() };
        Order.ListOfMoveOrdersForTransact(5).Returns(list);

        var response = await Get("ListOfMoveOrdersForTransact?orderid=5");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    [Fact]
    public async Task ListOfMoveOrdersForTransact_missing_orderid_binds_zero()
    {
        Order.ListOfMoveOrdersForTransact(Arg.Any<int>()).Returns(new List<MoveOrderDto>());

        await Contract.AssertJsonLiteral(await Get("ListOfMoveOrdersForTransact"), HttpStatusCode.OK, "[]");
        await Order.Received(1).ListOfMoveOrdersForTransact(0);
    }

    // #45 POST TransactListOfMoveOrders
    [Fact]
    public async Task TransactListOfMoveOrders_sets_flags_and_prepared_date_then_echoes_input()
    {
        var input = new[]
        {
            new TransactMoveOrder { Id = 1, OrderNo = 11, FarmName = "Farm A", IsActive = false, PreparedBy = "clerk" },
            new TransactMoveOrder { Id = 2, OrderNo = 12, FarmName = "Farm B", IsActive = false, PreparedBy = "clerk" }
        };
        var before = DateTime.Now;

        var response = await Send(HttpMethod.Post, "TransactListOfMoveOrders", input);

        var after = DateTime.Now;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(Contract.JsonContentType, response.Content.Headers.ContentType?.ToString());
        var body = await response.Content.ReadAsStringAsync();
        var echoed = JsonConvert.DeserializeObject<TransactMoveOrder[]>(body, Contract.MvcSettings)!;
        Assert.Equal(body, Contract.MvcJson(echoed)); // body is exactly the Newtonsoft form of the echoed items
        Assert.Equal(new[] { 1, 2 }, echoed.Select(e => e.Id));
        Assert.All(echoed, e =>
        {
            Assert.True(e.IsActive);
            Assert.True(e.IsTransact);
            Assert.Equal("clerk", e.PreparedBy);
            Assert.InRange(e.PreparedDate!.Value, before.AddSeconds(-1), after.AddSeconds(1));
        });
        await Order.Received(2).TransanctListOfMoveOrders(Arg.Is<TransactMoveOrder>(t => t.IsActive && t.IsTransact && t.PreparedDate != null));
        await AssertCompleted(1);
    }

    // #46 POST SetBeingPrepared
    [Fact]
    public async Task SetBeingPrepared_first_success_broadcasts_once_and_stops()
    {
        Order.SetBeingPrepared(Arg.Is<OrderingEntity>(o => o.Id == 1)).Returns(false);
        Order.SetBeingPrepared(Arg.Is<OrderingEntity>(o => o.Id == 2)).Returns(true);

        var response = await Send(HttpMethod.Post, "SetBeingPrepared", new[] { TestData.Order(1), TestData.Order(2), TestData.Order(3) });

        await Contract.AssertEmpty(response, HttpStatusCode.OK);
        await Order.Received(2).SetBeingPrepared(Arg.Any<OrderingEntity>());
        await Order.DidNotReceive().SetBeingPrepared(Arg.Is<OrderingEntity>(o => o.Id == 3));
        await Factory.Hub.Clients.All.Received(1).SetBeingPrepared(Arg.Any<OrderingEntity>());
        await Factory.Hub.Clients.All.Received(1).SetBeingPrepared(Arg.Is<OrderingEntity>(o => o.Id == 2));
    }

    [Fact]
    public async Task SetBeingPrepared_none_succeed_returns_json_string_without_broadcast()
    {
        Order.SetBeingPrepared(Arg.Any<OrderingEntity>()).Returns(false);

        var response = await Send(HttpMethod.Post, "SetBeingPrepared", new[] { TestData.Order(1), TestData.Order(2) });

        await Contract.AssertJsonString(response, "Someone is already preparing!");
        await Factory.Hub.Clients.All.DidNotReceive().SetBeingPrepared(Arg.Any<OrderingEntity>());
    }

    [Fact]
    public async Task SetBeingPrepared_empty_array_returns_json_string_without_broadcast()
    {
        var response = await Send(HttpMethod.Post, "SetBeingPrepared", Array.Empty<OrderingEntity>());

        await Contract.AssertJsonString(response, "Someone is already preparing!");
        await Order.DidNotReceive().SetBeingPrepared(Arg.Any<OrderingEntity>());
        await Factory.Hub.Clients.All.DidNotReceive().SetBeingPrepared(Arg.Any<OrderingEntity>());
    }

    // #47 POST UnsetBeingPrepared
    [Fact]
    public async Task UnsetBeingPrepared_handles_only_first_item_and_broadcasts_it()
    {
        Order.UnsetBeingPrepared(Arg.Any<OrderingEntity>()).Returns(false); // bool result is never null: first item always "succeeds"

        var response = await Send(HttpMethod.Post, "UnsetBeingPrepared", new[] { TestData.Order(1), TestData.Order(2) });

        await Contract.AssertEmpty(response, HttpStatusCode.OK);
        await Order.Received(1).UnsetBeingPrepared(Arg.Any<OrderingEntity>());
        await Order.Received(1).UnsetBeingPrepared(Arg.Is<OrderingEntity>(o => o.Id == 1));
        await Factory.Hub.Clients.All.Received(1).SetBeingPrepared(Arg.Is<OrderingEntity>(o => o.Id == 1));
    }

    [Fact]
    public async Task UnsetBeingPrepared_empty_array_returns_json_string()
    {
        var response = await Send(HttpMethod.Post, "UnsetBeingPrepared", Array.Empty<OrderingEntity>());

        await Contract.AssertJsonString(response, "Something Wrong");
        await Factory.Hub.Clients.All.DidNotReceive().SetBeingPrepared(Arg.Any<OrderingEntity>());
    }

    // #48 PUT Allocate
    [Fact]
    public async Task Allocate_returns_allocation_results_and_saves()
    {
        var results = new List<AllocationResult> { new() { AllocatedQuantity = 5, CustomerName = "Farm A", OrderNo = 1 } };
        Order.AllocateOrdersPerItems(Arg.Any<AllocationFinalResult>()).Returns(results);

        var response = await Send(HttpMethod.Put, "Allocate", new AllocationFinalResult { SOH = 20, Allocations = new List<AllocationDTO>() });

        await Contract.AssertJson(response, HttpStatusCode.OK, results);
        await Order.Received(1).AllocateOrdersPerItems(Arg.Is<AllocationFinalResult>(a => a.SOH == 20));
        await AssertCompleted(1);
    }

    // #49 GET GetAllListOfOrdersForAllocationPagination
    [Fact]
    public async Task GetAllListOfOrdersForAllocationPagination_returns_header_and_paged_body()
    {
        var page = TestData.LastPage(TestData.OrderDto());
        Order.GetAllListofOrdersForAllocationPagination(Arg.Any<UserParams>()).Returns(page);

        var response = await Get("GetAllListOfOrdersForAllocationPagination?PageNumber=2&PageSize=1");

        Contract.AssertPaginationHeader(response, Contract.LastPageHeader);
        await Contract.AssertJsonLiteral(response, HttpStatusCode.OK, Contract.PagedBody("orders", page));
        await Order.Received(1).GetAllListofOrdersForAllocationPagination(Arg.Is<UserParams>(p => p.PageNumber == 2 && p.PageSize == 1));
    }

    // #50 GET GetAllListofOrdersForAllocation
    [Fact]
    public async Task GetAllListofOrdersForAllocation_returns_json_and_passes_item_code()
    {
        var list = new List<OrderDto> { TestData.OrderDto() };
        Order.GetAllListofOrdersAllocation("RM-001").Returns(list);

        var response = await Get("GetAllListofOrdersForAllocation?itemCode=RM-001");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    // #51 POST CancelForPendingAllocation
    [Fact]
    public async Task CancelForPendingAllocation_success_returns_empty_200()
    {
        Order.CancelForPendingAllocation("Farm A").Returns(true);

        var response = await SendNoBody(HttpMethod.Post, "CancelForPendingAllocation?customer=Farm%20A");

        await Contract.AssertEmpty(response, HttpStatusCode.OK);
        await AssertCompleted(0);
    }

    [Fact]
    public async Task CancelForPendingAllocation_failure_returns_400_text()
    {
        Order.CancelForPendingAllocation(Arg.Any<string>()).Returns(false);

        var response = await SendNoBody(HttpMethod.Post, "CancelForPendingAllocation?customer=x");

        await Contract.AssertText(response, HttpStatusCode.BadRequest, "Something went wrong");
    }

    // #52 PUT ManualAllocation
    [Fact]
    public async Task ManualAllocation_success_returns_text_after_saving()
    {
        Order.ManualAllocationForOrders(Arg.Any<List<ManualAllocation>>()).Returns(true);

        var response = await Send(HttpMethod.Put, "ManualAllocation", new[] { new ManualAllocation { Id = 1, ItemCode = "RM-001", QuantityOrdered = 4 } });

        await Contract.AssertText(response, HttpStatusCode.OK, "Manual Allocation Complete");
        await Order.Received(1).ManualAllocationForOrders(Arg.Is<List<ManualAllocation>>(l => l.Single().QuantityOrdered == 4));
        await AssertCompleted(1);
    }

    [Fact]
    public async Task ManualAllocation_failure_returns_json_string_after_saving()
    {
        Order.ManualAllocationForOrders(Arg.Any<List<ManualAllocation>>()).Returns(false);

        var response = await Send(HttpMethod.Put, "ManualAllocation", new[] { new ManualAllocation { Id = 1 } });

        await Contract.AssertJsonString(response, "Something Wrong");
        await AssertCompleted(1);
    }
}
