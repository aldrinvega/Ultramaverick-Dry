using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ELIXIR.API.Http
{
    /// <summary>
    /// Documents the <see cref="MvcParametersMetadata"/> of Minimal API endpoints the way Swagger described the
    /// controller actions: query/route parameters (complex query objects expanded to their properties) and a request
    /// body with every media type the MVC input formatters accept.
    /// </summary>
    public sealed class MvcParameterOperationFilter : IOperationFilter
    {
        private readonly MvcOptions _mvcOptions;

        public MvcParameterOperationFilter(IOptions<MvcOptions> mvcOptions) => _mvcOptions = mvcOptions.Value;

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata?
                .OfType<MvcParametersMetadata>()
                .LastOrDefault();
            if (metadata is null)
                return;

            operation.Parameters ??= new List<OpenApiParameter>();
            foreach (var parameter in metadata.Parameters)
            {
                if (parameter.Source == BindingSource.Body)
                {
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content = BodyMediaTypes(parameter.Type).ToDictionary(
                            mediaType => mediaType,
                            _ => new OpenApiMediaType { Schema = context.SchemaGenerator.GenerateSchema(parameter.Type, context.SchemaRepository) })
                    };
                }
                else if (parameter.Source == BindingSource.Path)
                {
                    Add(operation, context, parameter.Name, ParameterLocation.Path, parameter.Type, required: true);
                }
                else if (IsSimpleOrCollectionOfSimple(parameter.Type))
                {
                    Add(operation, context, parameter.Name, ParameterLocation.Query, parameter.Type, required: false);
                }
                else
                {
                    foreach (var property in parameter.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(p => p.CanRead && p.CanWrite))
                        Add(operation, context, property.Name, ParameterLocation.Query, property.PropertyType, required: false);
                }
            }
        }

        private static void Add(OpenApiOperation operation, OperationFilterContext context, string name,
            ParameterLocation location, Type type, bool required)
        {
            // Minimal API may already list route parameters from the pattern; keep a single, typed entry.
            var existing = operation.Parameters.FirstOrDefault(p => p.Name == name && p.In == location);
            if (existing is not null)
                operation.Parameters.Remove(existing);

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = name,
                In = location,
                Required = required,
                Schema = context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository)
            });
        }

        /// <summary>Same lookup MVC's ApiExplorer uses for action request formats.</summary>
        private IEnumerable<string> BodyMediaTypes(Type type)
            => _mvcOptions.InputFormatters
                .OfType<IApiRequestFormatMetadataProvider>()
                .SelectMany(formatter => formatter.GetSupportedContentTypes(contentType: null!, objectType: type)
                                         ?? (IReadOnlyList<string>)Array.Empty<string>())
                .Distinct(StringComparer.OrdinalIgnoreCase);

        private static bool IsSimpleOrCollectionOfSimple(Type type)
        {
            if (IsSimple(type))
                return true;

            var element = type.GetInterfaces()
                .Concat(new[] { type })
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(i => i.GetGenericArguments()[0])
                .FirstOrDefault();
            return element is not null && IsSimple(element);
        }

        private static bool IsSimple(Type type) => TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
    }
}
