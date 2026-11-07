using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.API.Http
{
    /// <summary>
    /// Runs an MVC <see cref="IActionResult"/> from a Minimal API handler, so the response goes through the
    /// same MVC output formatters (Newtonsoft), content negotiation and null → 204 handling the controllers use.
    /// </summary>
    public sealed class MvcResult : IResult
    {
        public MvcResult(IActionResult actionResult) => ActionResult = actionResult;

        public IActionResult ActionResult { get; }

        public Task ExecuteAsync(HttpContext httpContext)
            => ActionResult.ExecuteResultAsync(MvcRequestContext.Get(httpContext));
    }

    /// <summary>Shorthands matching the <c>ControllerBase</c> helpers the controllers used.</summary>
    public static class MvcResults
    {
        /// <summary><c>Ok(value)</c>: 200 via content negotiation; a null value becomes 204.</summary>
        public static IResult Ok(object value) => new MvcResult(new OkObjectResult(value));

        /// <summary><c>Ok()</c>: 200 with an empty body.</summary>
        public static IResult Ok() => new MvcResult(new OkResult());

        /// <summary><c>BadRequest(value)</c>: 400 via content negotiation.</summary>
        public static IResult BadRequest(object value) => new MvcResult(new BadRequestObjectResult(value));

        /// <summary><c>new JsonResult(value)</c>: 200 application/json, regardless of the Accept header.</summary>
        public static IResult Json(object value) => new MvcResult(new JsonResult(value));
    }
}
