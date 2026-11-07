using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using NSubstitute;
using Xunit;

namespace ELIXIR.Tests.Ordering;

/// <summary>Contract tests for SPEC.md endpoints #32–42 (move-order approval lists, print, view, cancel).</summary>
[Trait("Group", "C")]
public sealed class GroupC_ApprovalTests : OrderingContractTestBase
{
    public GroupC_ApprovalTests(ElixirApiFactory factory) : base(factory) { }

    private static PagedList<MoveOrderDto> Page() => TestData.LastPage(TestData.MoveOrderDto());

    private static async Task AssertMoveOrderPage(HttpResponseMessage response, PagedList<MoveOrderDto> page)
    {
        Contract.AssertPaginationHeader(response, Contract.LastPageHeader);
        await Contract.AssertJsonLiteral(response, HttpStatusCode.OK, Contract.PagedBody("moveorder", page));
    }

    private static UserParams RequestedPageTwoParams() => Arg.Is<UserParams>(p => p.PageNumber == 2 && p.PageSize == 1);

    // #32 GET GetAllForApprovalMoveOrderPagination
    [Fact]
    public async Task GetAllForApprovalMoveOrderPagination_returns_header_and_paged_body()
    {
        var page = Page();
        Order.ForApprovalMoveOrderPagination(Arg.Any<UserParams>()).Returns(page);

        var response = await Get("GetAllForApprovalMoveOrderPagination?PageNumber=2&PageSize=1");

        await AssertMoveOrderPage(response, page);
        await Order.Received(1).ForApprovalMoveOrderPagination(RequestedPageTwoParams());
    }

    // #33 GET GetAllForApprovalMoveOrderPaginationOrig
    [Fact]
    public async Task GetAllForApprovalMoveOrderPaginationOrig_without_search_falls_back_to_unfiltered()
    {
        var page = Page();
        Order.ForApprovalMoveOrderPagination(Arg.Any<UserParams>()).Returns(page);

        var response = await Get("GetAllForApprovalMoveOrderPaginationOrig?PageNumber=2&PageSize=1");

        await AssertMoveOrderPage(response, page);
        await Order.Received(1).ForApprovalMoveOrderPagination(RequestedPageTwoParams());
        await Order.DidNotReceive().ForApprovalMoveOrderPaginationOrig(Arg.Any<UserParams>(), Arg.Any<string>());
    }

    [Fact]
    public async Task GetAllForApprovalMoveOrderPaginationOrig_with_search_uses_filtered()
    {
        var page = Page();
        Order.ForApprovalMoveOrderPaginationOrig(Arg.Any<UserParams>(), "farm").Returns(page);

        var response = await Get("GetAllForApprovalMoveOrderPaginationOrig?PageNumber=2&PageSize=1&search=farm");

        await AssertMoveOrderPage(response, page);
        await Order.DidNotReceive().ForApprovalMoveOrderPagination(Arg.Any<UserParams>());
    }

    // #34 GET PrintMultipleMoveOrderSlip
    [Fact]
    public async Task PrintMultipleMoveOrderSlip_binds_repeated_ids_and_returns_json()
    {
        var list = new List<MultiplePrintingDTO> { new() { OrderNo = 1, Orders = new List<MultiplePrintingDTO.Order> { new() { FarmCode = "F01" } } } };
        Order.MultiplePrintingForMOS(Arg.Any<List<int>>()).Returns(list);

        var response = await Get("PrintMultipleMoveOrderSlip?orderIds=1&orderIds=2");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
        await Order.Received(1).MultiplePrintingForMOS(Arg.Is<List<int>>(l => l.SequenceEqual(new[] { 1, 2 })));
    }

    [Fact]
    public async Task PrintMultipleMoveOrderSlip_without_ids_returns_400_text()
    {
        var response = await Get("PrintMultipleMoveOrderSlip");

        await Contract.AssertText(response, HttpStatusCode.BadRequest, "No order IDs provided");
        await Order.DidNotReceive().MultiplePrintingForMOS(Arg.Any<List<int>>());
    }

    // #35 GET ViewMoveOrderForApproval
    [Fact]
    public async Task ViewMoveOrderForApproval_returns_primary_list_when_not_empty()
    {
        var list = new List<MoveOrderDto> { TestData.MoveOrderDto() };
        Order.ViewMoveOrderForApproval(4).Returns(list);

        var response = await Get("ViewMoveOrderForApproval?id=4");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
        await Order.DidNotReceive().ViewMoveOrderForApprovalOriginal(Arg.Any<int>());
    }

    [Fact]
    public async Task ViewMoveOrderForApproval_falls_back_to_original_when_empty()
    {
        var original = new List<MoveOrderDto> { TestData.MoveOrderDto(7) };
        Order.ViewMoveOrderForApproval(4).Returns(new List<MoveOrderDto>());
        Order.ViewMoveOrderForApprovalOriginal(4).Returns(original);

        var response = await Get("ViewMoveOrderForApproval?id=4");

        await Contract.AssertJson(response, HttpStatusCode.OK, original);
    }

    // #36 GET ApprovedMoveOrderPagination
    [Fact]
    public async Task ApprovedMoveOrderPagination_returns_page_and_passes_dates()
    {
        var page = Page();
        Order.ApprovedMoveOrderPagination(Arg.Any<UserParams>(), Arg.Any<string>(), Arg.Any<string>()).Returns(page);

        var response = await Get("ApprovedMoveOrderPagination?PageNumber=2&PageSize=1&dateFrom=2024-01-01&dateTo=2024-01-31");

        await AssertMoveOrderPage(response, page);
        await Order.Received(1).ApprovedMoveOrderPagination(RequestedPageTwoParams(), "2024-01-01", "2024-01-31");
    }

    // #37 GET ApprovedMoveOrderPaginationOrig
    [Fact]
    public async Task ApprovedMoveOrderPaginationOrig_without_search_falls_back_with_dates()
    {
        var page = Page();
        Order.ApprovedMoveOrderPagination(Arg.Any<UserParams>(), Arg.Any<string>(), Arg.Any<string>()).Returns(page);

        var response = await Get("ApprovedMoveOrderPaginationOrig?PageNumber=2&PageSize=1&dateFrom=2024-01-01&dateTo=2024-01-31");

        await AssertMoveOrderPage(response, page);
        await Order.Received(1).ApprovedMoveOrderPagination(RequestedPageTwoParams(), "2024-01-01", "2024-01-31");
        await Order.DidNotReceive().ApprovedMoveOrderPaginationOrig(Arg.Any<UserParams>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task ApprovedMoveOrderPaginationOrig_with_search_passes_search_and_dates()
    {
        var page = Page();
        Order.ApprovedMoveOrderPaginationOrig(Arg.Any<UserParams>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(page);

        var response = await Get("ApprovedMoveOrderPaginationOrig?PageNumber=2&PageSize=1&search=farm&dateFrom=2024-01-01&dateTo=2024-01-31");

        await AssertMoveOrderPage(response, page);
        await Order.Received(1).ApprovedMoveOrderPaginationOrig(RequestedPageTwoParams(), "farm", "2024-01-01", "2024-01-31");
    }

    // #38 GET RejectedMoveOrderPagination
    [Fact]
    public async Task RejectedMoveOrderPagination_returns_header_and_paged_body()
    {
        var page = Page();
        Order.RejectedMoveOrderPagination(Arg.Any<UserParams>()).Returns(page);

        var response = await Get("RejectedMoveOrderPagination?PageNumber=2&PageSize=1");

        await AssertMoveOrderPage(response, page);
        await Order.Received(1).RejectedMoveOrderPagination(RequestedPageTwoParams());
    }

    // #39 GET RejectedMoveOrderPaginationOrig
    [Fact]
    public async Task RejectedMoveOrderPaginationOrig_without_search_falls_back_to_unfiltered()
    {
        var page = Page();
        Order.RejectedMoveOrderPagination(Arg.Any<UserParams>()).Returns(page);

        var response = await Get("RejectedMoveOrderPaginationOrig?PageNumber=2&PageSize=1");

        await AssertMoveOrderPage(response, page);
        await Order.DidNotReceive().RejectedMoveOrderPaginationOrig(Arg.Any<UserParams>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RejectedMoveOrderPaginationOrig_with_search_uses_filtered()
    {
        var page = Page();
        Order.RejectedMoveOrderPaginationOrig(Arg.Any<UserParams>(), "farm").Returns(page);

        var response = await Get("RejectedMoveOrderPaginationOrig?PageNumber=2&PageSize=1&search=farm");

        await AssertMoveOrderPage(response, page);
        await Order.DidNotReceive().RejectedMoveOrderPagination(Arg.Any<UserParams>());
    }

    // #40 GET GetAllApprovedMoveOrder
    [Fact]
    public async Task GetAllApprovedMoveOrder_returns_single_object_json()
    {
        var dto = TestData.MoveOrderDto(2);
        Order.GetAllApprovedMoveOrder(2).Returns(dto);

        var response = await Get("GetAllApprovedMoveOrder?id=2");

        await Contract.AssertJson(response, HttpStatusCode.OK, dto);
    }

    [Fact]
    public async Task GetAllApprovedMoveOrder_null_returns_204_empty()
    {
        Order.GetAllApprovedMoveOrder(Arg.Any<int>()).Returns((MoveOrderDto)null!);

        var response = await Get("GetAllApprovedMoveOrder?id=2");

        await Contract.AssertEmpty(response, HttpStatusCode.NoContent);
    }

    // #41 PUT CancelOrdersInMoveOrder/{id}
    [Fact]
    public async Task CancelOrdersInMoveOrder_binds_route_id_and_reason_and_saves()
    {
        var response = await Send(HttpMethod.Put, "CancelOrdersInMoveOrder/7", new ReasontDTO { Reason = "Truck unavailable" });

        await Contract.AssertText(response, HttpStatusCode.OK, "Successfully cancel orders");
        await Order.Received(1).CancelControlInMoveOrder(7, Arg.Is<ReasontDTO>(r => r.Reason == "Truck unavailable"));
        await AssertCompleted(1);
    }

    // #42 GET GetAllApprovedOrderCalendar
    [Fact]
    public async Task GetAllApprovedOrderCalendar_returns_json()
    {
        var list = new List<OrderDto> { TestData.OrderDto() };
        Order.GetAllApprovedOrdersForCalendar().Returns(list);

        var response = await Get("GetAllApprovedOrderCalendar");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }
}
