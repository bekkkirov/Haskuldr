using System.Diagnostics.CodeAnalysis;

namespace Haskuldr.Abstractions.Types;

/// <summary>
/// Represents a result object that encapsulates either a successful value of type <typeparamref name="TValue"/>
/// or an error of type <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value in case of success.</typeparam>
/// <typeparam name="TError">The type of the error in case of failure.</typeparam>
public readonly record struct Result<TValue, TError> 
    where TValue : notnull
    where TError: notnull
{
    private readonly TValue? _value;
    private readonly TError? _error;

    /// <summary>
    /// Gets a value indicating whether the result represents a success case.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(_error))]
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents an error case.
    /// </summary>
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

    /// <summary>
    /// Attempts to retrieve the success value of this <see cref="Result{TValue,TError}"/>.
    /// </summary>
    public bool TryPickValue(
        [NotNullWhen(true)] out TValue? value,
        [NotNullWhen(false)] out TError? error)
    {
        value = IsSuccess ? _value : default;
        error = default;

        return IsSuccess;
    }

    /// <summary>
    /// Attempts to retrieve the error value of this <see cref="Result{TValue,TError}"/>
    /// </summary>
    public bool TryPickError(
        [NotNullWhen(true)] out TError? error,
        [NotNullWhen(false)] out TValue? value)
    {
        error = IsError ? _error : default;
        value = default;

        return IsError;
    }

    /// <summary>
    /// Retrieves the success value of the result if the result represents a success, otherwise throws an exception.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the result represents an error.</exception>
    public TValue GetValue()
    {
        return IsSuccess 
            ? _value 
            : throw new InvalidOperationException("Cannot access value because result is error");   
    }

    /// <summary>
    /// Retrieves the success value if the result represents a success, or the default value of the success type if the result represents a failure.
    /// </summary>
    public TValue? GetValueOrDefault()
    {
        return IsSuccess 
            ? _value 
            : default;
    }

    /// <summary>
    /// Retrieves the error value of the result if the result represents an error, otherwise throws an exception.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result represents a success.</exception>
    public TError GetError()
    {
        return IsError 
            ? _error 
            : throw new InvalidOperationException("Cannot access error because result is success");  
    }

    /// <summary>
    /// Retrieves the error value if the result represents a failure. If the result is a success, returns the default value for <typeparamref name="TError"/>.
    /// </summary>
    public TError? GetErrorOrDefault()
    {
        return IsError 
            ? _error 
            : default;
    }

    /// <summary>
    /// Maps the value contained in this <see cref="Result{TValue,TError}"/> to a result of type <typeparamref name="TResult"/> using the provided mapping functions.
    /// </summary>
    public TResult Map<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onError)
    {
        return IsSuccess ? onSuccess(_value) : onError(_error!);
    }

    /// <summary>
    /// Creates a success result containing the specified value.
    /// </summary>
    public static Result<TValue, TError> FromValue(TValue value)
    {
        return new Result<TValue, TError>(value);
    }

    /// <summary>
    /// Creates an error result containing the specified error.
    /// </summary>
    public static Result<TValue, TError> FromError(TError error)
    {
        return new Result<TValue, TError>(error);
    }

    public static implicit operator Result<TValue, TError>(TValue value) => FromValue(value);

    public static implicit operator Result<TValue, TError>(TError error) => FromError(error);
}