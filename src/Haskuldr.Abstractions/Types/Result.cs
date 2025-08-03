using System.Diagnostics.CodeAnalysis;

namespace Haskuldr.Abstractions.Types;

public readonly record struct Result<TValue, TError> 
    where TValue : notnull
    where TError: notnull
{
    private readonly TValue? _value;
    private readonly TError? _error;

    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(_error))]
    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(_error))]
    [MemberNotNullWhen(false, nameof(_value))]
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

    public bool TryPickValue([NotNullWhen(true)] out TValue? value, out TError? error)
    {
        value = IsSuccess ? _value : default;
        error = default;

        return IsSuccess;
    }

    public bool TryPickError([NotNullWhen(true)] out TError? error, out TValue? value)
    {
        error = IsError ? _error : default;
        value = default;

        return IsError;
    }

    public TValue GetValue()
    {
        return IsSuccess 
            ? _value 
            : throw new InvalidOperationException("Cannot access value because result is error");   
    }
    
    public TValue? GetValueOrDefault()
    {
        return IsSuccess 
            ? _value 
            : default;
    }

    public TError GetError()
    {
        return IsError 
            ? _error 
            : throw new InvalidOperationException("Cannot access error because result is success");  
    }

    public TError? GetErrorOrDefault()
    {
        return IsError 
            ? _error 
            : default;
    }

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onError)
    {
        return IsSuccess ? onSuccess(_value) : onError(_error!);
    }
    
    public void Match(Action<TValue> onSuccess, Action<TError> onError)
    {
        if (IsSuccess)
        {
            onSuccess(_value);
        }
        else
        {
            onError(_error);
        }
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