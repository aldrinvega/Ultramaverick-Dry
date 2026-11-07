using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using NSubstitute;
using Xunit;
using OrderingEntity = ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL.Ordering;

namespace ELIXIR.Tests.Ordering;

/// <summary>Contract tests for SPEC.md endpoints #14–31 (move-order preparation and approval).</summary>
[Trait("Group", "B")]
public sealed class GroupB_MoveOrderTests : OrderingContractTestBase
{
    public GroupB_MoveOrderTests(ElixirApiFactory factory) : base(factory) { }

    // #14 GET GetAllListForMoveOrderPagination
    [Fact]
    public async Task GetAllListForMoveOrderPagination_returns_header_and_paged_body_and_passes_dates()
    {
        var page = TestData.PageTwo(new CustomersForMoveOrderDTO { CustomerId = 3, Farm = "Farm A" });
        Order.GetAllListForMoveOrderPagination(Arg.Any<UserParams>(), Arg.Any<string>(), Arg.Any<string>()).Returns(page);

        var response = await Get("GetAllListForMoveOrderPagination?PageNumber=2&PageSize=1&dateFrom=2024-01-01&dateTo=2024-01-31");

        Contract.AssertPaginationHeader(response, Contract.PageTwoHeader);
        await Contract.AssertJsonLiteral(response, HttpStatusCode.OK, Contract.PagedBody("orders", page));
        await Order.Received(1).GetAllListForMoveOrderPagination(
            Arg.Is<UserParams>(p => p.PageNumber == 2 && p.PageSize == 1), "2024-01-01", "2024-01-31");
    }

    // #15 GET GetAllListOfApprovedPreparedForMoveOrder
    [Fact]
    public async Task GetAllListOfApprovedPreparedForMoveOrder_returns_json()
    {
        var list = new List<TotalListOfPreparedDateDTO> { new() { Id = 1, FarmName = "Farm A", QuantityOrder = 5 } };
        Order.TotalListOfApprovedPreparedDate("Farm A").Returns(list);

        var response = await Get("GetAllListOfApprovedPreparedForMoveOrder?farm=Farm%20A");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    [Fact]
    public async Task GetAllListOfApprovedPreparedForMoveOrder_null_returns_400_text()
    {
        Order.TotalListOfApprovedPreparedDate(Arg.Any<string>()).Returns((IReadOnlyList<TotalListOfPreparedDateDTO>)null!);

        var response = await Get("GetAllListOfApprovedPreparedForMoveOrder?farm=x");

        await Contract.AssertText(response, HttpStatusCode.BadRequest, "No Orders Currently Available");
    }

    // #16 GET DetailedListOfOrders
    [Fact]
    public async Task DetailedListOfOrders_returns_json_and_passes_farm()
    {
        var list = new List<OrderDto> { TestData.OrderDto() };
        Order.DetailedListOfOrders("Farm A").Returns(list);

        var response = await Get("DetailedListOfOrders?farm=Farm%20A");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    // #17 GET GetAllListForScheduleApproval
    [Fact]
    public async Task GetAllListForScheduleApproval_returns_json()
    {
        var list = new List<OrderDto> { TestData.OrderDto(1), TestData.OrderDto(2) };
        Order.GetAllListForApprovalOfSchedule().Returns(list);

        var response = await Get("GetAllListForScheduleApproval");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    // #18 GET GetAllOrdersForScheduleApproval
    [Fact]
    public async Task GetAllOrdersForScheduleApproval_returns_json_and_passes_id()
    {
        var list = new List<OrderDto> { TestData.OrderDto() };
        Order.GetAllOrdersForScheduleApproval(5).Returns(list);

        var response = await Get("GetAllOrdersForScheduleApproval?id=5");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    [Fact]
    public async Task GetAllOrdersForScheduleApproval_missing_id_binds_zero()
    {
        Order.GetAllOrdersForScheduleApproval(Arg.Any<int>()).Returns(new List<OrderDto>());

        var response = await Get("GetAllOrdersForScheduleApproval");

        await Contract.AssertJsonLiteral(response, HttpStatusCode.OK, "[]");
        await Order.Received(1).GetAllOrdersForScheduleApproval(0);
    }

    // #19 GET GetAllOutOfStockByItemCodeAndOrderDate
    [Fact]
    public async Task GetAllOutOfStockByItemCodeAndOrderDate_returns_json_and_passes_both_params()
    {
        var list = new List<OrderDto> { TestData.OrderDto() };
        Order.GetAllOutOfStockByItemCodeAndOrderDate("RM-001", "2024-01-02").Returns(list);

        var response = await Get("GetAllOutOfStockByItemCodeAndOrderDate?itemcode=RM-001&orderdate=2024-01-02");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    // #20 POST PrepareItemsForMoveOrder
    [Fact]
    public async Task PrepareItemsForMoveOrder_copies_order_details_and_echoes_saved_order()
    {
        var details = TestData.OrderDto(9);
        details.PreparedDateTime = new DateTime(2024, 1, 4, 9, 0, 0);
        Order.GetMoveOrderDetailsForMoveOrder(41).Returns(details);
        var input = TestData.MoveOrder(1); // OrderNoPKey = 41

        var response = await Send(HttpMethod.Post, "PrepareItemsForMoveOrder", input);

        var expected = TestData.MoveOrder(1);
        expected.TimeStamp = input.TimeStamp; // entity default is DateTime.Now; the bound input keeps the sent value
        expected.OrderNoPKey = details.Id;
        expected.OrderDate = details.OrderDateTime;
        expected.DateNeeded = details.DateNeededDateTime;
        expected.PreparedDate = details.PreparedDateTime;
        expected.FarmName = details.Farm;
        expected.FarmCode = details.FarmCode;
        expected.FarmType = details.FarmType;
        expected.ItemCode = details.ItemCode;
        expected.ItemDescription = details.ItemDescription;
        expected.Uom = details.Uom;
        expected.Category = details.Category;
        expected.IsActive = true;
        expected.IsPrepared = true;
        await Contract.AssertJson(response, HttpStatusCode.OK, expected);
        await Order.Received(1).PrepareItemForMoveOrder(Arg.Is<MoveOrder>(m => m.OrderNoPKey == 9 && m.IsPrepared));
        await AssertCompleted(1);
    }

    // #21 GET GetAllListOfOrdersForMoveOrder
    [Fact]
    public async Task GetAllListOfOrdersForMoveOrder_returns_json_and_passes_id()
    {
        var list = new List<OrderDto> { TestData.OrderDto() };
        Order.ListOfOrdersForMoveOrder(6).Returns(list);

        var response = await Get("GetAllListOfOrdersForMoveOrder?id=6");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    [Fact]
    public async Task GetAllListOfOrdersForMoveOrder_missing_id_binds_zero()
    {
        Order.ListOfOrdersForMoveOrder(Arg.Any<int>()).Returns(new List<OrderDto>());

        await Contract.AssertJsonLiteral(await Get("GetAllListOfOrdersForMoveOrder"), HttpStatusCode.OK, "[]");
        await Order.Received(1).ListOfOrdersForMoveOrder(0);
    }

    // #22 GET GetAvailableStockFromWarehouse
    [Fact]
    public async Task GetAvailableStockFromWarehouse_returns_stock_and_first_expiry_warehouse()
    {
        var stocks = new ItemStocks { WarehouseId = 3, ItemCode = "RM-001", Remaining = 4.5m, ManufacturingDate = new DateTime(2024, 1, 1) };
        Order.GetActualItemQuantityInWarehouse(3, "RM-001").Returns(stocks);
        Order.GetFirstExpiry("RM-001").Returns(new ItemStocks { WarehouseId = 9 });

        var response = await Get("GetAvailableStockFromWarehouse?id=3&itemcode=RM-001");

        await Contract.AssertJson(response, HttpStatusCode.OK, new { orders = stocks, WarehouseId = 9 });
        Assert.EndsWith(",\"warehouseId\":9}", await response.Content.ReadAsStringAsync());
    }

    // #23 GET ListOfPreparedItemsForMoveOrder
    [Fact]
    public async Task ListOfPreparedItemsForMoveOrder_returns_json_and_passes_id()
    {
        var list = new List<MoveOrderDto> { TestData.MoveOrderDto() };
        Order.ListOfPreparedItemsForMoveOrder(8).Returns(list);

        var response = await Get("ListOfPreparedItemsForMoveOrder?id=8");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    [Fact]
    public async Task ListOfPreparedItemsForMoveOrder_missing_id_binds_zero()
    {
        Order.ListOfPreparedItemsForMoveOrder(Arg.Any<int>()).Returns(new List<MoveOrderDto>());

        await Contract.AssertJsonLiteral(await Get("ListOfPreparedItemsForMoveOrder"), HttpStatusCode.OK, "[]");
        await Order.Received(1).ListOfPreparedItemsForMoveOrder(0);
    }

    // #24 PUT CancelPreparedItems
    [Fact]
    public async Task CancelPreparedItems_returns_text_and_saves()
    {
        var response = await Send(HttpMethod.Put, "CancelPreparedItems", TestData.MoveOrder(2));

        await Contract.AssertText(response, HttpStatusCode.OK, "Successfully cancel prepared items");
        await Order.Received(1).CancelMoveOrder(Arg.Is<MoveOrder>(m => m.Id == 2));
        await AssertCompleted(1);
    }

    // #25 PUT AddPlateNumberInMoveOrder
    [Fact]
    public async Task AddPlateNumberInMoveOrder_updates_each_item_and_saves_once()
    {
        var first = TestData.Order(1);
        first.PlateNumber = "ABC-123";

        var response = await Send(HttpMethod.Put, "AddPlateNumberInMoveOrder", new[] { first, TestData.Order(2) });

        await Contract.AssertJsonString(response, "Successfully added plate number!");
        await Order.Received(1).AddPlateNumberInMoveOrder(Arg.Is<OrderingEntity>(o => o.Id == 1 && o.PlateNumber == "ABC-123"));
        await Order.Received(1).AddPlateNumberInMoveOrder(Arg.Is<OrderingEntity>(o => o.Id == 2));
        await AssertCompleted(1);
    }

    // #26 PUT AddDeliveryStatus
    [Fact]
    public async Task AddDeliveryStatus_updates_each_item_and_saves_once()
    {
        var first = TestData.OrderDto(1);
        first.DeliveryStatus = "Delivered";

        var response = await Send(HttpMethod.Put, "AddDeliveryStatus", new[] { first, TestData.OrderDto(2) });

        await Contract.AssertJsonString(response, "Successfully added delivery status!");
        await Order.Received(1).AddDeliveryStatus(Arg.Is<OrderDto>(o => o.Id == 1 && o.DeliveryStatus == "Delivered"));
        await Order.Received(1).AddDeliveryStatus(Arg.Is<OrderDto>(o => o.Id == 2));
        await AssertCompleted(1);
    }

    // #27 PUT ApproveListOfMoveOrder
    [Fact]
    public async Task ApproveListOfMoveOrder_returns_json_string_and_saves()
    {
        var response = await Send(HttpMethod.Put, "ApproveListOfMoveOrder", new[] { TestData.MoveOrder(1), TestData.MoveOrder(2) });

        await Contract.AssertJsonString(response, "Successfully approved list for move order!");
        await Order.Received(1).ApprovalForMoveOrder(Arg.Is<IEnumerable<MoveOrder>>(m => m.Count() == 2));
        await AssertCompleted(1);
    }

    // #28 PUT RejectListOfMoveOrder
    [Fact]
    public async Task RejectListOfMoveOrder_returns_json_string_and_saves()
    {
        var response = await Send(HttpMethod.Put, "RejectListOfMoveOrder", TestData.MoveOrder(3));

        await Contract.AssertJsonString(response, "Successfully reject list for move order!");
        await Order.Received(1).RejectForMoveOrder(Arg.Is<MoveOrder>(m => m.Id == 3));
        await AssertCompleted(1);
    }

    // #29 PUT RejectApproveListOfMoveOrder
    [Fact]
    public async Task RejectApproveListOfMoveOrder_returns_json_string_and_saves()
    {
        var response = await Send(HttpMethod.Put, "RejectApproveListOfMoveOrder", TestData.MoveOrder(4));

        await Contract.AssertJsonString(response, "Successfully reject approved list for move order!");
        await Order.Received(1).RejectApproveMoveOrder(Arg.Is<MoveOrder>(m => m.Id == 4));
        await AssertCompleted(1);
    }

    // #30 PUT ReturnMoveOrderForApproval
    [Fact]
    public async Task ReturnMoveOrderForApproval_returns_json_string_and_saves()
    {
        var response = await Send(HttpMethod.Put, "ReturnMoveOrderForApproval", TestData.MoveOrder(5));

        await Contract.AssertJsonString(response, "Successfully return list for move order!");
        await Order.Received(1).ReturnMoveOrderForApproval(Arg.Is<MoveOrder>(m => m.Id == 5));
        await AssertCompleted(1);
    }

    // #31 PUT UpdatePrintStatus
    [Fact]
    public async Task UpdatePrintStatus_echoes_ids_without_saving()
    {
        var response = await Send(HttpMethod.Put, "UpdatePrintStatus", new[] { 1, 2, 3 });

        await Contract.AssertJsonLiteral(response, HttpStatusCode.OK, "[1,2,3]");
        await Order.Received(1).UpdatePrintStatus(Arg.Is<int[]>(a => a.SequenceEqual(new[] { 1, 2, 3 })));
        await AssertCompleted(0);
    }
}
