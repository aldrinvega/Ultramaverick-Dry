using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using NSubstitute;
using Xunit;
using OrderingEntity = ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL.Ordering;

namespace ELIXIR.Tests.Ordering;

/// <summary>Contract tests for SPEC.md endpoints #1–13 (orders and scheduling).</summary>
[Trait("Group", "A")]
public sealed class GroupA_OrdersTests : OrderingContractTestBase
{
    public GroupA_OrdersTests(ElixirApiFactory factory) : base(factory) { }

    // #1 GET GetAllListofOrders
    [Fact]
    public async Task GetAllListofOrders_returns_list_as_json_and_passes_farms()
    {
        var list = new List<PreparationScheduleDto> { new() { Id = 1, Farm = "Farm A", OrderDate = "01/02/2024" } };
        Order.GetAllListOfOrders("Farm A").Returns(list);

        var response = await Get("GetAllListofOrders?farms=Farm%20A");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    [Fact]
    public async Task GetAllListofOrders_null_result_returns_204_empty()
    {
        Order.GetAllListOfOrders(Arg.Any<string>()).Returns((IReadOnlyList<PreparationScheduleDto>)null!);

        var response = await Get("GetAllListofOrders");

        await Contract.AssertEmpty(response, HttpStatusCode.NoContent);
        await Order.Received(1).GetAllListOfOrders(null!);
    }

    // #2 PUT EditOrderQuantity
    [Fact]
    public async Task EditOrderQuantity_success_returns_json_string_and_saves()
    {
        Order.EditQuantityOrder(Arg.Any<OrderingEntity>()).Returns(true);

        var response = await Send(HttpMethod.Put, "EditOrderQuantity", TestData.Order());

        await Contract.AssertJsonString(response, "Successfully edit order quantity!");
        await Order.Received(1).EditQuantityOrder(Arg.Is<OrderingEntity>(o =>
            o.Id == 1 && o.QuantityOrdered == 10.5m && o.OrderDate == new DateTime(2024, 1, 2)));
        await AssertCompleted(1);
    }

    [Fact]
    public async Task EditOrderQuantity_rejected_returns_400_text_and_does_not_save()
    {
        Order.EditQuantityOrder(Arg.Any<OrderingEntity>()).Returns(false);

        var response = await Send(HttpMethod.Put, "EditOrderQuantity", TestData.Order());

        await Contract.AssertText(response, HttpStatusCode.BadRequest,
            "Quantity must not transcend the value of allocated quantity!");
        await AssertCompleted(0);
    }

    [Fact]
    public async Task EditOrderQuantity_binds_lenient_json_pascal_case_and_us_date_string()
    {
        Order.EditQuantityOrder(Arg.Any<OrderingEntity>()).Returns(true);

        var response = await SendRaw(HttpMethod.Put, "EditOrderQuantity",
            "{\"Id\":9,\"QuantityOrdered\":\"3.5\",\"OrderDate\":\"01/02/2024\"}");

        await Contract.AssertJsonString(response, "Successfully edit order quantity!");
        await Order.Received(1).EditQuantityOrder(Arg.Is<OrderingEntity>(o =>
            o.Id == 9 && o.QuantityOrdered == 3.5m && o.OrderDate == new DateTime(2024, 1, 2)));
    }

    // #3 PUT SchedulePreparedOrderedDate
    [Fact]
    public async Task SchedulePreparedOrderedDate_saves_number_before_stamping_items_and_saves_twice()
    {
        // With EF the generated key only exists after SaveChanges, so the substitute assigns it in CompleteAsync.
        GenerateOrderNo? generated = null;
        Order.GenerateNumber(Arg.Do<GenerateOrderNo>(g => generated = g)).Returns(true);
        UnitOfWork.CompleteAsync().Returns(_ =>
        {
            if (generated is not null) generated.Id = 77;
            return Task.CompletedTask;
        });

        var response = await Send(HttpMethod.Put, "SchedulePreparedOrderedDate",
            new[] { TestData.Order(1), TestData.Order(2) });

        await Contract.AssertJsonString(response, "Successfully scheduled orders");
        await Order.Received(1).GenerateNumber(Arg.Is<GenerateOrderNo>(g => g.IsActive));
        await Order.Received(1).SchedulePreparedDate(Arg.Is<OrderingEntity>(o => o.Id == 1 && o.OrderNoPKey == 77));
        await Order.Received(1).SchedulePreparedDate(Arg.Is<OrderingEntity>(o => o.Id == 2 && o.OrderNoPKey == 77));
        await AssertCompleted(2);
    }

    [Fact]
    public async Task SchedulePreparedOrderedDate_repository_failure_midway_returns_500_after_first_save_only()
    {
        Order.GenerateNumber(Arg.Any<GenerateOrderNo>()).Returns(true);
        Order.SchedulePreparedDate(Arg.Is<OrderingEntity>(o => o.Id == 2))
            .Returns<bool>(_ => throw new InvalidOperationException("boom"));

        var response = await Send(HttpMethod.Put, "SchedulePreparedOrderedDate",
            new[] { TestData.Order(1), TestData.Order(2), TestData.Order(3) });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.StartsWith("System.InvalidOperationException: boom", await response.Content.ReadAsStringAsync());
        await Order.DidNotReceive().SchedulePreparedDate(Arg.Is<OrderingEntity>(o => o.Id == 3));
        await AssertCompleted(1);
    }

    // #4 PUT ApprovePreparedDate
    [Fact]
    public async Task ApprovePreparedDate_returns_json_string_and_saves()
    {
        var response = await Send(HttpMethod.Put, "ApprovePreparedDate", new[] { TestData.Order(1), TestData.Order(2) });

        await Contract.AssertJsonString(response, "Successfully approved prepared date!");
        await Order.Received(1).ApprovePreparedDate(Arg.Is<List<OrderingEntity>>(l => l.Count == 2 && l[1].Id == 2));
        await AssertCompleted(1);
    }

    // #5 PUT RejectPreparedDate
    [Fact]
    public async Task RejectPreparedDate_returns_json_string_and_saves()
    {
        var response = await Send(HttpMethod.Put, "RejectPreparedDate", new[] { TestData.Order(3) });

        await Contract.AssertJsonString(response, "Successfully reject prepared date!");
        await Order.Received(1).RejectPreparedDate(Arg.Is<List<OrderingEntity>>(l => l.Single().Id == 3));
        await AssertCompleted(1);
    }

    // #6 PUT GetAllListofPreparedDate
    [Fact]
    public async Task GetAllListofPreparedDate_is_a_put_returning_fixed_message()
    {
        var response = await SendNoBody(HttpMethod.Put, "GetAllListofPreparedDate");

        await Contract.AssertJsonString(response, "Successfully schedules ordered");
        await Order.Received(1).GetAllListOfPreparedDate();
        await AssertCompleted(1);
    }

    [Fact]
    public async Task GetAllListofPreparedDate_get_is_not_allowed()
    {
        var response = await Get("GetAllListofPreparedDate");

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    // #7 GET OrderSummary
    [Fact]
    public async Task OrderSummary_returns_json_and_passes_date_range()
    {
        var list = new List<OrderDto> { TestData.OrderDto() };
        Order.OrderSummary("2024-01-01", "2024-01-31").Returns(list);

        var response = await Get("OrderSummary?dateFrom=2024-01-01&dateTo=2024-01-31");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    [Fact]
    public async Task OrderSummary_formats_dates_decimals_and_nulls_like_newtonsoft()
    {
        Order.OrderSummary(Arg.Any<string>(), Arg.Any<string>()).Returns(new List<OrderDto> { TestData.OrderDto() });

        var body = await (await Get("OrderSummary")).Content.ReadAsStringAsync();

        Assert.Contains("\"orderDateTime\":\"2024-01-02T03:04:05\"", body);
        Assert.Contains("\"quantityOrder\":12.25", body);
        Assert.Contains("\"stockOnHand\":0.0", body);
        Assert.Contains("\"preparedDateTime\":null", body);
        Assert.Contains("\"cancelBy\":null", body);
        Assert.Contains("\"isBeingPrepared\":true", body);
        Assert.Contains("\"casting\":0", body);
    }

    // #8 POST ValidateNewOrders
    [Fact]
    public async Task ValidateNewOrders_all_valid_returns_text_and_saves()
    {
        AllValidatorsReturn(true);

        var response = await Send(HttpMethod.Post, "ValidateNewOrders", new[] { TestData.Order(1), TestData.Order(2) });

        await Contract.AssertText(response, HttpStatusCode.OK, "Successfully add new orders!");
        await Order.Received(2).ValidateNewOrders(Arg.Any<OrderingEntity>());
        await Order.Received(1).ValidateIfForAllocation(Arg.Is<List<OrderingEntity>>(l => l.Count == 2));
        await AssertCompleted(2);
    }

    [Fact]
    public async Task ValidateNewOrders_invalid_returns_400_with_all_seven_lists()
    {
        AllValidatorsReturn(true);
        Order.ValidateExistingOrders(Arg.Is<OrderingEntity>(o => o.Id == 1)).Returns(false);
        Order.ValidateCustomerName(Arg.Is<OrderingEntity>(o => o.Id == 2)).Returns(false);
        Order.ValidateCustomerCode(Arg.Is<OrderingEntity>(o => o.Id == 3)).Returns(false);
        Order.ValidateRawMaterial(Arg.Is<OrderingEntity>(o => o.Id == 4)).Returns(false);
        Order.ValidateUom(Arg.Is<OrderingEntity>(o => o.Id == 5)).Returns(false);
        var orders = Enumerable.Range(1, 6).Select(TestData.Order).ToArray();

        var response = await Send(HttpMethod.Post, "ValidateNewOrders", orders);

        await Contract.AssertJson(response, HttpStatusCode.BadRequest, new
        {
            duplicateList = new[] { orders[0] },
            filteredOrders = new[] { orders[5] },
            notExistFarmName = new[] { orders[1] },
            notExistFarmCode = new[] { orders[2] },
            notExistRawMats = new[] { orders[3] },
            notExistUom = new[] { orders[4] },
            previousdateNeeded = Array.Empty<OrderingEntity>()
        });
        await Order.Received(6).ValidateNewOrders(Arg.Any<OrderingEntity>());
        await Order.DidNotReceive().ValidateIfForAllocation(Arg.Any<List<OrderingEntity>>());
        await AssertCompleted(0);
    }

    /// <summary>Each validator on its own must block the save (one row per <c>X.Count == 0</c> check).</summary>
    public static TheoryData<string, string> SingleFailingValidator() => new()
    {
        { "ValidateExistingOrders", "duplicateList" },
        { "ValidateCustomerName", "notExistFarmName" },
        { "ValidateCustomerCode", "notExistFarmCode" },
        { "ValidateRawMaterial", "notExistRawMats" },
        { "ValidateUom", "notExistUom" },
    };

    [Theory]
    [MemberData(nameof(SingleFailingValidator))]
    public async Task ValidateNewOrders_any_single_failing_validator_blocks_the_save(string validator, string list)
    {
        AllValidatorsReturn(true);
        FailValidator(validator);
        var order = TestData.Order(1);

        var response = await Send(HttpMethod.Post, "ValidateNewOrders", new[] { order });

        var lists = new Dictionary<string, OrderingEntity[]>
        {
            ["duplicateList"] = Array.Empty<OrderingEntity>(),
            ["filteredOrders"] = Array.Empty<OrderingEntity>(),
            ["notExistFarmName"] = Array.Empty<OrderingEntity>(),
            ["notExistFarmCode"] = Array.Empty<OrderingEntity>(),
            ["notExistRawMats"] = Array.Empty<OrderingEntity>(),
            ["notExistUom"] = Array.Empty<OrderingEntity>(),
            ["previousdateNeeded"] = Array.Empty<OrderingEntity>(),
        };
        lists[list] = new[] { order };
        await Contract.AssertJsonLiteral(response, HttpStatusCode.BadRequest,
            "{" + string.Join(",", lists.Select(kv => "\"" + kv.Key + "\":" + Contract.MvcJson(kv.Value))) + "}");
        await Order.DidNotReceive().ValidateIfForAllocation(Arg.Any<List<OrderingEntity>>());
        await AssertCompleted(0);
    }

    [Fact]
    public async Task ValidateNewOrders_order_failing_several_validators_goes_to_first_matching_list()
    {
        AllValidatorsReturn(true);
        FailValidator("ValidateExistingOrders");
        FailValidator("ValidateCustomerName");
        FailValidator("ValidateUom");
        var order = TestData.Order(1);

        var response = await Send(HttpMethod.Post, "ValidateNewOrders", new[] { order });

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.StartsWith("{\"duplicateList\":[" + Contract.MvcJson(order) + "],\"filteredOrders\":[],\"notExistFarmName\":[],", body);
        Assert.EndsWith(",\"notExistUom\":[],\"previousdateNeeded\":[]}", body);
    }

    private void FailValidator(string validator)
    {
        switch (validator)
        {
            case "ValidateExistingOrders": Order.ValidateExistingOrders(Arg.Any<OrderingEntity>()).Returns(false); break;
            case "ValidateCustomerName": Order.ValidateCustomerName(Arg.Any<OrderingEntity>()).Returns(false); break;
            case "ValidateCustomerCode": Order.ValidateCustomerCode(Arg.Any<OrderingEntity>()).Returns(false); break;
            case "ValidateRawMaterial": Order.ValidateRawMaterial(Arg.Any<OrderingEntity>()).Returns(false); break;
            case "ValidateUom": Order.ValidateUom(Arg.Any<OrderingEntity>()).Returns(false); break;
            default: throw new ArgumentOutOfRangeException(nameof(validator), validator, null);
        }
    }

    private void AllValidatorsReturn(bool value)
    {
        Order.ValidateExistingOrders(Arg.Any<OrderingEntity>()).Returns(value);
        Order.ValidateCustomerName(Arg.Any<OrderingEntity>()).Returns(value);
        Order.ValidateCustomerCode(Arg.Any<OrderingEntity>()).Returns(value);
        Order.ValidateRawMaterial(Arg.Any<OrderingEntity>()).Returns(value);
        Order.ValidateUom(Arg.Any<OrderingEntity>()).Returns(value);
    }

    // #9 POST AddNewOrder
    [Fact]
    public async Task AddNewOrder_returns_empty_200_without_saving()
    {
        var response = await Send(HttpMethod.Post, "AddNewOrder", new[] { TestData.Order(1), TestData.Order(2) });

        await Contract.AssertEmpty(response, HttpStatusCode.OK);
        await Order.Received(1).AddNewOrders(Arg.Is<OrderingEntity[]>(a => a.Length == 2));
        await AssertCompleted(0);
    }

    // #10 GET GetAllListOfOrdersPagination
    [Fact]
    public async Task GetAllListOfOrdersPagination_returns_header_and_paged_body()
    {
        var page = TestData.LastPage(new CustomerListForPreparationSchedule { Id = 5, CustomerName = "Farm A" });
        Order.GetAllListofOrdersPagination(Arg.Any<UserParams>()).Returns(page);

        var response = await Get("GetAllListOfOrdersPagination?PageNumber=2&PageSize=1");

        Contract.AssertPaginationHeader(response, Contract.LastPageHeader);
        await Contract.AssertJsonLiteral(response, HttpStatusCode.OK, Contract.PagedBody("orders", page));
        await Order.Received(1).GetAllListofOrdersPagination(Arg.Is<UserParams>(p => p.PageNumber == 2 && p.PageSize == 1));
    }

    [Fact]
    public async Task GetAllListOfOrdersPagination_header_keeps_swapped_next_previous_flags()
    {
        var page = TestData.PageOne(new CustomerListForPreparationSchedule { Id = 5 });
        Order.GetAllListofOrdersPagination(Arg.Any<UserParams>()).Returns(page);

        var response = await Get("GetAllListOfOrdersPagination?PageNumber=1&PageSize=1");

        Contract.AssertPaginationHeader(response, Contract.PageOneHeader);
        var body = await response.Content.ReadAsStringAsync();
        Assert.EndsWith(",\"hasNextPage\":true,\"hasPreviousPage\":false}", body);
    }

    [Fact]
    public async Task GetAllListOfOrdersPagination_defaults_and_clamps_user_params()
    {
        Order.GetAllListofOrdersPagination(Arg.Any<UserParams>())
            .Returns(TestData.PageOne(new CustomerListForPreparationSchedule()));

        await Get("GetAllListOfOrdersPagination");
        await Get("GetAllListOfOrdersPagination?pagenumber=3&pagesize=5000");

        await Order.Received(1).GetAllListofOrdersPagination(Arg.Is<UserParams>(p => p.PageNumber == 1 && p.PageSize == 10));
        await Order.Received(1).GetAllListofOrdersPagination(Arg.Is<UserParams>(p => p.PageNumber == 3 && p.PageSize == 2000));
    }

    // #11 PUT CancelOrders
    [Fact]
    public async Task CancelOrders_success_returns_text_and_saves()
    {
        Order.CancelOrders(Arg.Any<OrderingEntity[]>()).Returns(true);

        var response = await Send(HttpMethod.Put, "CancelOrders", new[] { TestData.Order() });

        await Contract.AssertText(response, HttpStatusCode.OK, "Successfully cancel orders");
        await Order.Received(1).CancelOrders(Arg.Is<OrderingEntity[]>(a => a.Single().Id == 1));
        await AssertCompleted(1);
    }

    [Fact]
    public async Task CancelOrders_missing_returns_400_text_and_does_not_save()
    {
        Order.CancelOrders(Arg.Any<OrderingEntity[]>()).Returns(false);

        var response = await Send(HttpMethod.Put, "CancelOrders", new[] { TestData.Order() });

        await Contract.AssertText(response, HttpStatusCode.BadRequest, "Orders is not exist!");
        await AssertCompleted(0);
    }

    // #12 GET GetAllListOfCancelledOrders
    [Fact]
    public async Task GetAllListOfCancelledOrders_returns_json()
    {
        var list = new List<OrderDto> { TestData.OrderDto(1), TestData.OrderDto(2) };
        Order.GetAllListOfCancelledOrders().Returns(list);

        var response = await Get("GetAllListOfCancelledOrders");

        await Contract.AssertJson(response, HttpStatusCode.OK, list);
    }

    // #13 PUT ReturnCancelledOrders
    [Fact]
    public async Task ReturnCancelledOrders_success_returns_text_and_saves()
    {
        Order.ReturnCancellOrdersInList(Arg.Any<OrderingEntity>()).Returns(true);

        var response = await Send(HttpMethod.Put, "ReturnCancelledOrders", TestData.Order(4));

        await Contract.AssertText(response, HttpStatusCode.OK, "Successfully return orders");
        await Order.Received(1).ReturnCancellOrdersInList(Arg.Is<OrderingEntity>(o => o.Id == 4));
        await AssertCompleted(1);
    }

    [Fact]
    public async Task ReturnCancelledOrders_missing_returns_400_text_and_does_not_save()
    {
        Order.ReturnCancellOrdersInList(Arg.Any<OrderingEntity>()).Returns(false);

        var response = await Send(HttpMethod.Put, "ReturnCancelledOrders", TestData.Order(4));

        await Contract.AssertText(response, HttpStatusCode.BadRequest, "Orders is not exist!");
        await AssertCompleted(0);
    }
}
