using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using NSubstitute;
using Xunit;
using OrderingEntity = ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL.Ordering;

namespace ELIXIR.Tests.Ordering;

/// <summary>
/// Error responses for bad input, captured from the MVC controller: model-binding failures go through
/// <c>InvalidModelStateResponseFactory</c> (ApiValidationError), unsupported content types produce a 415
/// ProblemDetails, and unhandled exceptions go through ExceptionMiddleWare.
/// </summary>
[Trait("Group", "E")]
public sealed class GroupE_BindingErrorTests : OrderingContractTestBase
{
    public GroupE_BindingErrorTests(ElixirApiFactory factory) : base(factory) { }

    private static string ValidationError(params string[] errors)
        => "{\"errors\":" + Newtonsoft.Json.JsonConvert.SerializeObject(errors) +
           ",\"statusCode\":400,\"message\":\"You have made a bad reqeust!\"}";

    private static async Task AssertValidationError(HttpResponseMessage response, params string[] errors)
        => await Contract.AssertJsonLiteral(response, HttpStatusCode.BadRequest, ValidationError(errors));

    private static async Task AssertUnsupportedMediaType(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        Assert.Equal("application/problem+json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        Assert.Matches(
            "^" + Regex.Escape("{\"type\":\"https://tools.ietf.org/html/rfc7231#section-6.5.13\",\"title\":\"Unsupported Media Type\",\"status\":415,\"traceId\":\"") +
            "00-[0-9a-f]{32}-[0-9a-f]{16}-00\"}$",
            await response.Content.ReadAsStringAsync());
    }

    // ---- request body ----

    [Fact]
    public async Task Malformed_object_body_returns_every_newtonsoft_error()
    {
        var response = await SendRaw(HttpMethod.Put, "EditOrderQuantity", "{\"id\":");

        await AssertValidationError(response,
            "Unexpected end when setting id's value. Path 'id', line 1, position 6.",
            "Unexpected end when deserializing object. Path 'id', line 1, position 6.");
        await Order.DidNotReceive().EditQuantityOrder(Arg.Any<OrderingEntity>());
    }

    [Fact]
    public async Task Malformed_array_body_returns_every_newtonsoft_error()
    {
        var response = await SendRaw(HttpMethod.Put, "CancelOrders", "[{");

        await AssertValidationError(response,
            "Unexpected end when reading JSON. Path '[0]', line 1, position 2.",
            "Unexpected end when deserializing array. Path '[0]', line 1, position 2.");
        await Order.DidNotReceive().CancelOrders(Arg.Any<OrderingEntity[]>());
    }

    [Fact]
    public async Task Wrong_property_type_returns_conversion_error()
    {
        var response = await SendRaw(HttpMethod.Put, "EditOrderQuantity", "{\"id\":\"abc\"}");

        await AssertValidationError(response, "Could not convert string to integer: abc. Path 'id', line 1, position 11.");
    }

    [Fact]
    public async Task Array_sent_for_object_returns_newtonsoft_error()
    {
        var response = await SendRaw(HttpMethod.Put, "EditOrderQuantity", "[]");

        await AssertValidationError(response,
            "Cannot deserialize the current JSON array (e.g. [1,2,3]) into type 'ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL.Ordering' " +
            "because the type requires a JSON object (e.g. {\"name\":\"value\"}) to deserialize correctly.\n" +
            "To fix this error either change the JSON to a JSON object (e.g. {\"name\":\"value\"}) or change the deserialized type to an array " +
            "or a type that implements a collection interface (e.g. ICollection, IList) like List<T> that can be deserialized from a JSON array. " +
            "JsonArrayAttribute can also be added to the type to force it to deserialize from a JSON array.\nPath '', line 1, position 1.");
    }

    [Fact]
    public async Task Empty_json_body_returns_non_empty_body_required()
    {
        var response = await SendRaw(HttpMethod.Put, "EditOrderQuantity", "");

        await AssertValidationError(response, "A non-empty request body is required.");
    }

    [Fact]
    public async Task Null_literal_body_returns_non_empty_body_required()
    {
        var response = await SendRaw(HttpMethod.Put, "EditOrderQuantity", "null");

        await AssertValidationError(response, "A non-empty request body is required.");
    }

    [Fact]
    public async Task Unknown_properties_are_ignored()
    {
        Order.CancelOrders(Arg.Any<OrderingEntity[]>()).Returns(false);

        var response = await SendRaw(HttpMethod.Put, "CancelOrders", "[{\"bogus\":1}]");

        await Contract.AssertText(response, HttpStatusCode.BadRequest, "Orders is not exist!");
        await Order.Received(1).CancelOrders(Arg.Is<OrderingEntity[]>(a => a.Length == 1));
    }

    [Fact]
    public async Task Body_without_content_type_returns_415_problem_details()
    {
        var response = await SendNoBody(HttpMethod.Put, "EditOrderQuantity");

        await AssertUnsupportedMediaType(response);
    }

    [Fact]
    public async Task Body_with_text_plain_content_type_returns_415_problem_details()
    {
        var response = await Client.SendAsync(Contract.Request(HttpMethod.Put, BasePath + "EditOrderQuantity",
            new StringContent("{}", Encoding.UTF8, "text/plain")));

        await AssertUnsupportedMediaType(response);
    }

    [Fact]
    public async Task Implicit_body_parameter_without_content_type_returns_415_problem_details()
    {
        var response = await SendNoBody(HttpMethod.Put, "ManualAllocation");

        await AssertUnsupportedMediaType(response);
        await Order.DidNotReceive().ManualAllocationForOrders(Arg.Any<List<ManualAllocation>>());
    }

    // ---- query and route values ----

    [Fact]
    public async Task Non_numeric_int_query_returns_not_valid()
    {
        var response = await Get("GetAllOrdersForScheduleApproval?id=abc");

        await AssertValidationError(response, "The value 'abc' is not valid.");
        await Order.DidNotReceive().GetAllOrdersForScheduleApproval(Arg.Any<int>());
    }

    [Fact]
    public async Task Empty_int_query_returns_invalid()
    {
        var response = await Get("GetAllOrdersForScheduleApproval?id=");

        await AssertValidationError(response, "The value '' is invalid.");
    }

    [Fact]
    public async Task Repeated_int_query_uses_first_value()
    {
        Order.GetAllOrdersForScheduleApproval(Arg.Any<int>()).Returns(new List<OrderDto>());

        await Contract.AssertJsonLiteral(await Get("GetAllOrdersForScheduleApproval?id=1&id=2"), HttpStatusCode.OK, "[]");
        await Order.Received(1).GetAllOrdersForScheduleApproval(1);
    }

    [Fact]
    public async Task Invalid_bool_query_returns_not_valid()
    {
        var response = await Get("GetTotalListForMoveOrder?status=maybe");

        await AssertValidationError(response, "The value 'maybe' is not valid.");
    }

    [Fact]
    public async Task Bool_query_is_case_insensitive()
    {
        Order.TotalListForTransactMoveOrder(Arg.Any<bool>()).Returns(new List<OrderDto>());

        await Contract.AssertJsonLiteral(await Get("GetTotalListForMoveOrder?status=TRUE"), HttpStatusCode.OK, "[]");
        await Order.Received(1).TotalListForTransactMoveOrder(true);
    }

    [Fact]
    public async Task Invalid_user_params_property_names_the_property()
    {
        var response = await Get("GetAllListOfOrdersPagination?PageNumber=x");

        await AssertValidationError(response, "The value 'x' is not valid for PageNumber.");
    }

    [Fact]
    public async Task Empty_user_params_property_returns_invalid()
    {
        var response = await Get("GetAllListOfOrdersPagination?PageSize=");

        await AssertValidationError(response, "The value '' is invalid.");
    }

    [Fact]
    public async Task Invalid_list_item_query_returns_not_valid()
    {
        var response = await Get("PrintMultipleMoveOrderSlip?orderIds=1&orderIds=abc");

        await AssertValidationError(response, "The value 'abc' is not valid.");
    }

    [Fact]
    public async Task Invalid_route_id_and_body_errors_are_reported_together()
    {
        var response = await SendRaw(HttpMethod.Put, "CancelOrdersInMoveOrder/abc", "{\"reason\":");

        await AssertValidationError(response,
            "The value 'abc' is not valid.",
            "Unexpected end when setting reason's value. Path 'reason', line 1, position 10.",
            "Unexpected end when deserializing object. Path 'reason', line 1, position 10.");
        await Order.DidNotReceive().CancelControlInMoveOrder(Arg.Any<int>(), Arg.Any<ReasontDTO>());
    }

    // ---- routing and unhandled errors ----

    [Fact]
    public async Task Unhandled_repository_exception_returns_500_developer_exception_page()
    {
        Order.GetAllListForApprovalOfSchedule().Returns<IReadOnlyList<OrderDto>>(_ => throw new InvalidOperationException("boom"));

        var response = await Get("GetAllListForScheduleApproval");

        // UseDeveloperExceptionPage sits inside ExceptionMiddleWare, so it handles the exception first and
        // writes a plain-text page (stack trace frames differ by implementation; only the head is pinned).
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(Contract.TextContentType, response.Content.Headers.ContentType?.ToString());
        Assert.StartsWith("System.InvalidOperationException: boom", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Unknown_action_returns_404()
    {
        var response = await Get("NoSuchAction");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
