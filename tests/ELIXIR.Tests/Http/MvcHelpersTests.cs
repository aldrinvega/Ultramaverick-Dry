using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.API.Http;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using OrderingEntity = ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL.Ordering;

namespace ELIXIR.Tests.Http;

/// <summary>
/// Unit tests for <see cref="MvcResult"/>/<see cref="MvcResults"/> and <see cref="MvcRequestBinder"/>, run against the
/// app's real MVC services on a bare <see cref="DefaultHttpContext"/> (no routing, no endpoints).
/// </summary>
[Trait("Group", "Helpers")]
public sealed class MvcHelpersTests : IClassFixture<ElixirApiFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    public MvcHelpersTests(ElixirApiFactory factory) => _scope = factory.Services.CreateScope();

    public void Dispose() => _scope.Dispose();

    private DefaultHttpContext Context(string accept = ElixirApiFactory.FrontendAccept, string query = "",
        string? body = null, string? contentType = "application/json")
    {
        var context = new DefaultHttpContext { RequestServices = _scope.ServiceProvider };
        context.Request.Method = body is null ? "GET" : "PUT";
        context.Request.Headers.Accept = accept;
        context.Request.QueryString = new QueryString(query);
        if (body is not null)
        {
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.ContentLength = Encoding.UTF8.GetByteCount(body);
            if (contentType is not null)
                context.Request.ContentType = contentType;
        }
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<(int Status, string? ContentType, string Body)> Run(IResult result, HttpContext context)
    {
        await result.ExecuteAsync(context);
        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        return (context.Response.StatusCode, context.Response.ContentType, body);
    }

    // ---- results ----

    [Fact]
    public async Task Ok_with_null_value_writes_204_and_no_body()
    {
        var (status, _, body) = await Run(MvcResults.Ok(null!), Context());

        Assert.Equal(204, status);
        Assert.Equal(string.Empty, body);
    }

    [Fact]
    public async Task Ok_with_string_and_frontend_accept_writes_plain_text()
    {
        var (status, contentType, body) = await Run(MvcResults.Ok("done"), Context());

        Assert.Equal((200, "text/plain; charset=utf-8", "done"), (status, contentType, body));
    }

    [Fact]
    public async Task Ok_with_string_and_json_only_accept_writes_quoted_json()
    {
        var (status, contentType, body) = await Run(MvcResults.Ok("done"), Context(accept: "application/json"));

        Assert.Equal((200, "application/json; charset=utf-8", "\"done\""), (status, contentType, body));
    }

    [Fact]
    public async Task Ok_with_object_writes_camel_case_newtonsoft_json()
    {
        var (status, contentType, body) = await Run(
            MvcResults.Ok(new { OrderDateTime = new DateTime(2024, 1, 2, 3, 4, 5), Quantity = 12.25m, Missing = (string?)null }),
            Context());

        Assert.Equal((200, "application/json; charset=utf-8"), (status, contentType));
        Assert.Equal("{\"orderDateTime\":\"2024-01-02T03:04:05\",\"quantity\":12.25,\"missing\":null}", body);
    }

    [Fact]
    public async Task Json_writes_quoted_json_regardless_of_accept()
    {
        var (status, contentType, body) = await Run(MvcResults.Json("saved"), Context(accept: "text/plain"));

        Assert.Equal((200, "application/json; charset=utf-8", "\"saved\""), (status, contentType, body));
    }

    [Fact]
    public async Task Ok_without_value_writes_empty_200()
    {
        var (status, _, body) = await Run(MvcResults.Ok(), Context());

        Assert.Equal((200, string.Empty), (status, body));
    }

    [Fact]
    public async Task BadRequest_with_string_writes_400_plain_text()
    {
        var (status, contentType, body) = await Run(MvcResults.BadRequest("nope"), Context());

        Assert.Equal((400, "text/plain; charset=utf-8", "nope"), (status, contentType, body));
    }

    // ---- binding ----

    [Fact]
    public async Task Missing_int_query_binds_default_without_failure()
    {
        var binder = await MvcRequestBinder.CreateAsync(Context());

        var id = await binder.BindAsync(MvcParameter.Query<int>("id"));

        Assert.Equal(0, id);
        Assert.Null(binder.Failure());
    }

    [Fact]
    public async Task Query_values_bind_with_mvc_semantics()
    {
        var binder = await MvcRequestBinder.CreateAsync(Context(query: "?id=7&status=TRUE&farm=Farm%20A&pagenumber=3&PageSize=5000"));

        var id = await binder.BindAsync(MvcParameter.Query<int>("id"));
        var status = await binder.BindAsync(MvcParameter.Query<bool>("status"));
        var farm = await binder.BindAsync(MvcParameter.Query<string>("farm"));
        var paging = await binder.BindAsync(MvcParameter.Query<UserParams>("userParams"));

        Assert.Equal((7, true, "Farm A"), (id, status, farm));
        Assert.Equal((3, 2000), (paging.PageNumber, paging.PageSize));
        Assert.Null(binder.Failure());
    }

    [Fact]
    public async Task Invalid_query_value_fails_with_api_validation_error()
    {
        var context = Context(query: "?id=abc");
        var binder = await MvcRequestBinder.CreateAsync(context);
        await binder.BindAsync(MvcParameter.Query<int>("id"));

        var (status, contentType, body) = await Run(binder.Failure()!, context);

        Assert.Equal((400, "application/json; charset=utf-8"), (status, contentType));
        Assert.Equal("{\"errors\":[\"The value 'abc' is not valid.\"],\"statusCode\":400,\"message\":\"You have made a bad reqeust!\"}", body);
    }

    [Fact]
    public async Task Json_body_binds_leniently_with_newtonsoft()
    {
        var binder = await MvcRequestBinder.CreateAsync(Context(body: "{\"Id\":9,\"quantityOrdered\":\"3.5\",\"orderDate\":\"01/02/2024\"}"));

        var order = await binder.BindAsync(MvcParameter.Body<OrderingEntity>("order"));

        Assert.Equal((9, 3.5m, new DateTime(2024, 1, 2)), (order.Id, order.QuantityOrdered, order.OrderDate));
        Assert.Null(binder.Failure());
    }

    [Fact]
    public async Task Malformed_body_fails_with_every_newtonsoft_error()
    {
        var context = Context(body: "{\"id\":");
        var binder = await MvcRequestBinder.CreateAsync(context);
        await binder.BindAsync(MvcParameter.Body<OrderingEntity>("order"));

        var (status, _, body) = await Run(binder.Failure()!, context);

        Assert.Equal(400, status);
        Assert.Equal("{\"errors\":[\"Unexpected end when setting id's value. Path 'id', line 1, position 6.\"," +
                     "\"Unexpected end when deserializing object. Path 'id', line 1, position 6.\"]," +
                     "\"statusCode\":400,\"message\":\"You have made a bad reqeust!\"}", body);
    }

    [Fact]
    public async Task Body_without_content_type_fails_with_415_problem_details()
    {
        var context = Context(body: "{}", contentType: null);
        var binder = await MvcRequestBinder.CreateAsync(context);
        await binder.BindAsync(MvcParameter.Body<OrderingEntity>("order"));

        var (status, contentType, body) = await Run(binder.Failure()!, context);

        Assert.Equal((415, "application/problem+json; charset=utf-8"), (status, contentType));
        Assert.StartsWith("{\"type\":\"https://tools.ietf.org/html/rfc7231#section-6.5.13\",\"title\":\"Unsupported Media Type\",\"status\":415,\"traceId\":\"", body);
    }

    [Fact]
    public async Task Route_value_binds_from_route_values()
    {
        var context = Context();
        context.Request.RouteValues["id"] = "7";
        var binder = await MvcRequestBinder.CreateAsync(context);

        var id = await binder.BindAsync(MvcParameter.Route<int>("id"));

        Assert.Equal(7, id);
        Assert.Null(binder.Failure());
    }
}
