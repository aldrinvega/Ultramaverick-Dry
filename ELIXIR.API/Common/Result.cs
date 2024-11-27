using System;

namespace ELIXIR.API.Common;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        switch (isSuccess)
        {
            case true when error != Error.None:
                throw new InvalidOperationException();
            case false when error == Error.None:
                throw new InvalidOperationException();
            default:
                IsSuccess = isSuccess;
                Error = error;
                break;
        }
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue data) => new(data, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static Result Create(bool condition) => condition ? Success() : Failure(Error.ConditionNotMet);

    public static Result<TValue> Create<TValue>(TValue? data) => data is not null ? Success(data) : Failure<TValue>(Error.NullValue);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? data, bool isSuccess, Error error)
        : base(isSuccess, error) =>
        _value = data;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator Result<TValue>(TValue? data) => Create(data);
}
