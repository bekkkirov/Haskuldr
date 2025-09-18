using System.Diagnostics.CodeAnalysis;

namespace Haskuldr.Abstractions.Types;

/// <summary>
/// Represents an optional value that may or may not contain a value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the contained value.</typeparam>
public readonly record struct Option<TValue>
    where TValue : notnull
{
    private readonly TValue? _value;

    /// <summary>
    /// Indicates whether the current <see cref="Option{TValue}"/> instance contains a value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_value))]
    public bool HasValue { get; }

    private Option(TValue value)
    {
        _value = value;

        HasValue = true;
    }

    /// <summary>
    /// Attempts to retrieve the contained value from this <see cref="Option{TValue}"/> if it has one.
    /// </summary>
    public bool TryPickValue([NotNullWhen(true)] out TValue? value)
    {
        value = HasValue ? _value : default;
        
        return HasValue;
    }

    /// <summary>
    /// Retrieves the contained value from this <see cref="Option{TValue}"/> if it has one, otherwise throws an exception.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the <see cref="Option{TValue}"/> does not contain a value.</exception>
    public TValue GetValue()
    {
        return HasValue
            ? _value
            : throw new InvalidOperationException("Cannot access value because option is empty");   
    }

    /// <summary>
    /// Retrieves the contained value from this <see cref="Option{TValue}"/> if it has one,
    /// otherwise returns the default value for type <typeparamref name="TValue"/>.
    /// </summary>
    public TValue? GetValueOrDefault()
    {
        return HasValue 
            ? _value 
            : default;
    }

    /// <summary>
    /// Maps the value contained in this <see cref="Option{TValue}"/> to a result of type <typeparamref name="TResult"/> using the provided mapping functions.
    /// </summary>
    public TResult Map<TResult>(Func<TValue, TResult> onValue, Func<TResult> onEmpty)
    {
        return HasValue ? onValue(_value) : onEmpty();
    }

    /// <summary>
    /// Creates a new <see cref="Option{TValue}"/> containing the specified value.
    /// </summary>
    public static Option<TValue> FromValue(TValue value) => new(value);
    
    /// <summary>
    /// Represents an instance of <see cref="Option{TValue}"/> that does not contain a value.
    /// </summary>
    /// <remarks>
    /// This property provides a predefined, empty <see cref="Option{TValue}"/> instance to indicate the absence of a value.
    /// It is used as a convenient representation for scenarios where no meaningful value is available.
    /// </remarks>
    public static Option<TValue> None => new ();
    
    public static implicit operator Option<TValue>(TValue value) => FromValue(value);
}