using System.Diagnostics.CodeAnalysis;

namespace Haskuldr.Abstractions.Types;

public readonly record struct Option<T>
{
    private readonly T? _value;

    public bool HasValue { get; }

    private Option(T value)
    {
        _value = value;
        
        HasValue = true;
    }

    public static Option<T> FromValue(T value) => new(value);
    
    public static Option<T> None() => default;
    
    public bool TryPickValue([NotNullWhen(true)] out T? value)
    {
        value = HasValue ? _value : default;
        
        return HasValue;
    }
    
    public static implicit operator Option<T>(T value) => FromValue(value);
}