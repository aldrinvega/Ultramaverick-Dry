using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using Xunit;

namespace ELIXIR.Tests.Ordering;

/// <summary>
/// Exact-response assertions for the Ordering API contract (SPEC.md "Endpoint contract").
/// Response kinds: J = JsonResult(string), T = Ok/BadRequest(string) as text/plain,
/// O = Ok(object) via Newtonsoft, E = Ok() empty, P = paginated.
/// </summary>
internal static class Contract
{
    public const string JsonContentType = "application/json; charset=utf-8";
    public const string TextContentType = "text/plain; charset=utf-8";

    /// <summary>The serializer settings MVC uses today (AddNewtonsoftJson defaults + ReferenceLoopHandling.Ignore).</summary>
    public static readonly JsonSerializerSettings MvcSettings = CreateMvcSettings();

    private static JsonSerializerSettings CreateMvcSettings()
    {
        var settings = JsonSerializerSettingsProvider.CreateSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        return settings;
    }

    public static string MvcJson(object? value) => JsonConvert.SerializeObject(value, MvcSettings);

    public static StringContent Body(object value) => RawBody(MvcJson(value));

    public static StringContent RawBody(string json) => new(json, Encoding.UTF8, "application/json");

    public static HttpRequestMessage Request(HttpMethod method, string url, HttpContent? content = null)
        => new(method, url) { Content = content };

    /// <summary>T: <c>Ok("msg")</c> / <c>BadRequest("msg")</c> → text/plain, unquoted.</summary>
    public static async Task AssertText(HttpResponseMessage response, HttpStatusCode status, string text)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal(TextContentType, response.Content.Headers.ContentType?.ToString());
        Assert.Equal(text, await response.Content.ReadAsStringAsync());
    }

    /// <summary>J: <c>new JsonResult("msg")</c> → 200 application/json, quoted.</summary>
    public static async Task AssertJsonString(HttpResponseMessage response, string message)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(JsonContentType, response.Content.Headers.ContentType?.ToString());
        Assert.Equal(JsonConvert.SerializeObject(message), await response.Content.ReadAsStringAsync());
    }

    /// <summary>O: <c>Ok(obj)</c> / <c>BadRequest(obj)</c> → application/json serialized by MVC's Newtonsoft settings.</summary>
    public static async Task AssertJson(HttpResponseMessage response, HttpStatusCode status, object expected)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal(JsonContentType, response.Content.Headers.ContentType?.ToString());
        Assert.Equal(MvcJson(expected), await response.Content.ReadAsStringAsync());
    }

    /// <summary>O with a literal expected body (anchors date/decimal formatting independently of the settings).</summary>
    public static async Task AssertJsonLiteral(HttpResponseMessage response, HttpStatusCode status, string expectedJson)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal(JsonContentType, response.Content.Headers.ContentType?.ToString());
        Assert.Equal(expectedJson, await response.Content.ReadAsStringAsync());
    }

    /// <summary>E: <c>Ok()</c> → status only, empty body. Also used for 204 from <c>Ok(null)</c>.</summary>
    public static async Task AssertEmpty(HttpResponseMessage response, HttpStatusCode status)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
    }

    /// <summary>P: exact Pagination header and CORS expose header.</summary>
    public static void AssertPaginationHeader(HttpResponseMessage response, string expectedHeaderJson)
    {
        Assert.Equal(expectedHeaderJson, Assert.Single(response.Headers.GetValues("Pagination")));
        Assert.Equal("Pagination", Assert.Single(response.Headers.GetValues("Access-Control-Expose-Headers")));
    }

    /// <summary>
    /// Header produced for page 3 of 3 (pageSize 1, totalCount 3). AddPaginationHeader is called with
    /// (HasNextPage, HasPreviousPage) into parameters (hasPreviousPage, hasNextPage), so the flags are swapped:
    /// the real HasNextPage=false, HasPreviousPage=true are emitted as hasPrevious=false, hasNext=true.
    /// </summary>
    public const string LastPageHeader =
        "{\"currentPage\":3,\"itemsPerPage\":1,\"totalItems\":3,\"totalPages\":3,\"hasPrevious\":false,\"hasNext\":true}";

    /// <summary>Page 1 of 3: real HasNextPage=true, HasPreviousPage=false, emitted swapped.</summary>
    public const string PageOneHeader =
        "{\"currentPage\":1,\"itemsPerPage\":1,\"totalItems\":3,\"totalPages\":3,\"hasPrevious\":true,\"hasNext\":false}";

    /// <summary>The paginated response body shape: { listName, currentPage, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage }.</summary>
    public static string PagedBody<T>(string listName, ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS.PagedList<T> page)
    {
        var list = MvcJson(page.ToList());
        return "{\"" + listName + "\":" + list +
               ",\"currentPage\":" + page.CurrentPage +
               ",\"pageSize\":" + page.PageSize +
               ",\"totalCount\":" + page.TotalCount +
               ",\"totalPages\":" + page.TotalPages +
               ",\"hasNextPage\":" + (page.HasNextPage ? "true" : "false") +
               ",\"hasPreviousPage\":" + (page.HasPreviousPage ? "true" : "false") + "}";
    }
}
