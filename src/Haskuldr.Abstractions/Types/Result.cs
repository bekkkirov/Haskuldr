using System.Diagnostics.CodeAnalysis;

namespace Haskuldr.Abstractions.Types;

public readonly record struct Result<TValue, TError>
{
    private readonly TValue? _value;

    private readonly TError? _error;

    public bool IsSuccess { get; }

    public bool IsError => !IsSuccess;

    private Result(TValue value)
    {
        _value = value;

        IsSuccess = true;
    }

    private Result(TError error)
    {
        _error = error;

        IsSuccess = false;
    }

    public bool TryPickValue([NotNullWhen(true)] out TValue? value)
    {
        value = IsSuccess ? _value : default;

        return IsSuccess;
    }

    public bool TryPickError([NotNullWhen(true)] out TError? error)
    {
        error = IsError ? _error : default;

        return IsError;
    }

    public static Result<TValue, TError> FromValue(TValue value)
    {
        return new Result<TValue, TError>(value);
    }

    public static Result<TValue, TError> FromError(TError error)
    {
        return new Result<TValue, TError>(error);
    }

    public static implicit operator Result<TValue, TError>(TValue value) => FromValue(value);

    public static implicit operator Result<TValue, TError>(TError error) => FromError(error);
}