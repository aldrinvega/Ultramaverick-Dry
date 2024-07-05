using System;
using System.Collections.Generic;
using RDF.Arcana.API.Common;

namespace ELIXIR.API.Common
{
    public class Result
    {
        protected internal Result(bool isSuccess, List<Error> errors)
        {
            switch (isSuccess)
            {
                case true when errors.Count > 0:
                    throw new InvalidOperationException();
                case false when errors.Count == 0:
                    throw new InvalidOperationException();
                default:
                    IsSuccess = isSuccess;
                    Errors = errors;
                    break;
            }
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public List<Error> Errors { get; }

        public static Result Success() => new(true, new List<Error>());

        public static Result<TValue> Success<TValue>(TValue data) => new(data, true, new List<Error>());

        public static Result Failure(Error error) => new(false, new List<Error> { error });

        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, new List<Error> { error });

        public static Result Failure(List<Error> errors) => new(false, errors);

        public static Result<TValue> Failure<TValue>(List<Error> errors) => new(default, false, errors);

        public static Result Create(bool condition) => condition ? Success() : Failure(new List<Error> { Error.ConditionNotMet });

        public static Result<TValue> Create<TValue>(TValue? data) => data is not null ? Success(data) : Failure<TValue>(new List<Error> { Error.NullValue });
    }

    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal Result(TValue? data, bool isSuccess, List<Error> errors)
            : base(isSuccess, errors) =>
            _value = data;

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

        public static implicit operator Result<TValue>(TValue? data) => Create(data);
    }
}