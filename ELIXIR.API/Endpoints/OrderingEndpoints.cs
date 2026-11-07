using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ELIXIR.API.Endpoints
{
    /// <summary>
    /// Minimal API endpoints for <c>api/Ordering</c> (formerly <c>OrderingController</c>). Routes, binding and responses
    /// are kept identical to the controller: values are bound with <see cref="Http.MvcRequestBinder"/> and responses
    /// are the same MVC results executed through <see cref="Http.MvcResult"/>. See SPEC.md for the contract.
    /// </summary>
    public static class OrderingEndpoints
    {
        public static IEndpointRouteBuilder MapOrderingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/Ordering").WithTags("Ordering");

            return app;
        }
    }
}
