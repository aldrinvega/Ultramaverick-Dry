using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ELIXIR.API.Http
{
    /// <summary>
    /// Per-request <see cref="ActionContext"/> shared by <see cref="MvcRequestBinder"/> and <see cref="MvcResult"/>,
    /// so binding errors and result execution see the same model state.
    /// </summary>
    internal static class MvcRequestContext
    {
        private static readonly object Key = new();

        public static ActionContext Get(HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue(Key, out var existing) && existing is ActionContext context)
                return context;

            context = new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());
            httpContext.Items[Key] = context;
            return context;
        }
    }

    /// <summary>
    /// Binds Minimal API request values with MVC's own model binding (value providers, model binders, Newtonsoft
    /// input formatter, validation), and turns failures into the same responses the [ApiController] filters produce:
    /// unsupported content type → 415 ProblemDetails, invalid model state → <c>InvalidModelStateResponseFactory</c>.
    /// Bind every parameter (in controller parameter order) before checking <see cref="Failure"/>, as MVC does.
    /// </summary>
    public sealed class MvcRequestBinder
    {
        private readonly ActionContext _actionContext;
        private readonly IValueProvider _valueProvider;
        private readonly ParameterBinder _parameterBinder;
        private readonly IModelBinderFactory _modelBinderFactory;
        private readonly IModelMetadataProvider _metadataProvider;

        private MvcRequestBinder(ActionContext actionContext, IValueProvider valueProvider)
        {
            _actionContext = actionContext;
            _valueProvider = valueProvider;
            var services = actionContext.HttpContext.RequestServices;
            _parameterBinder = services.GetRequiredService<ParameterBinder>();
            _modelBinderFactory = services.GetRequiredService<IModelBinderFactory>();
            _metadataProvider = services.GetRequiredService<IModelMetadataProvider>();
        }

        public static async Task<MvcRequestBinder> CreateAsync(HttpContext httpContext)
        {
            var actionContext = MvcRequestContext.Get(httpContext);
            var mvcOptions = httpContext.RequestServices.GetRequiredService<IOptions<MvcOptions>>().Value;
            var valueProvider = await CompositeValueProvider.CreateAsync(actionContext, mvcOptions.ValueProviderFactories);
            return new MvcRequestBinder(actionContext, valueProvider);
        }

        public async Task<T> BindAsync<T>(MvcParameter<T> parameter)
        {
            var bindingInfo = new BindingInfo { BindingSource = parameter.Source };
            var descriptor = new ParameterDescriptor
            {
                Name = parameter.Name,
                ParameterType = parameter.Type,
                BindingInfo = bindingInfo
            };
            var metadata = _metadataProvider.GetMetadataForType(parameter.Type);
            var modelBinder = _modelBinderFactory.CreateBinder(new ModelBinderFactoryContext
            {
                Metadata = metadata,
                BindingInfo = bindingInfo,
                CacheToken = parameter
            });

            var result = await _parameterBinder.BindModelAsync(
                _actionContext, modelBinder, _valueProvider, descriptor, metadata, value: null, container: null);

            return result.IsModelSet ? (T)result.Model! : default!;
        }

        /// <summary>The response MVC would have sent for the bound values, or null when binding succeeded.</summary>
        public IResult Failure()
        {
            var modelState = _actionContext.ModelState;
            var services = _actionContext.HttpContext.RequestServices;

            // Same order as the [ApiController] filters: UnsupportedContentTypeFilter, then ModelStateInvalidFilter.
            var unsupportedContentType = modelState.Values
                .SelectMany(entry => entry.Errors)
                .Any(error => error.Exception is UnsupportedContentTypeException);
            if (unsupportedContentType)
            {
                var clientError = services.GetRequiredService<IClientErrorFactory>()
                    .GetClientError(_actionContext, new UnsupportedMediaTypeResult());
                return new MvcResult(clientError ?? new UnsupportedMediaTypeResult());
            }

            if (!modelState.IsValid)
            {
                var apiBehavior = services.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value;
                return new MvcResult(apiBehavior.InvalidModelStateResponseFactory(_actionContext));
            }

            return null;
        }
    }
}
