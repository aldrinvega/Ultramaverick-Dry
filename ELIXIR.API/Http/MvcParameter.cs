using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ELIXIR.API.Http
{
    /// <summary>
    /// One action parameter, described the way MVC binds it (name, CLR type, binding source). Handlers bind it with
    /// <see cref="MvcRequestBinder"/>; the same instance is attached to the endpoint so Swagger can document it.
    /// </summary>
    public abstract class MvcParameter
    {
        protected MvcParameter(string name, Type type, BindingSource source)
        {
            Name = name;
            Type = type;
            Source = source;
        }

        /// <summary>The controller parameter name (model-state key and query/route name).</summary>
        public string Name { get; }

        public Type Type { get; }

        public BindingSource Source { get; }

        public static MvcParameter<T> Query<T>(string name) => new(name, BindingSource.Query);

        public static MvcParameter<T> Route<T>(string name) => new(name, BindingSource.Path);

        public static MvcParameter<T> Body<T>(string name) => new(name, BindingSource.Body);
    }

    /// <inheritdoc />
    public sealed class MvcParameter<T> : MvcParameter
    {
        internal MvcParameter(string name, BindingSource source) : base(name, typeof(T), source) { }
    }

    /// <summary>Endpoint metadata: the MVC parameters an endpoint binds, in controller parameter order.</summary>
    public sealed class MvcParametersMetadata
    {
        public MvcParametersMetadata(MvcParameter[] parameters) => Parameters = parameters;

        public MvcParameter[] Parameters { get; }
    }

    public static class MvcParameterEndpointExtensions
    {
        /// <summary>Declares the MVC parameters this endpoint binds (used for the Swagger description).</summary>
        public static RouteHandlerBuilder WithMvcParameters(this RouteHandlerBuilder builder, params MvcParameter[] parameters)
            => builder.WithMetadata(new MvcParametersMetadata(parameters));
    }
}
