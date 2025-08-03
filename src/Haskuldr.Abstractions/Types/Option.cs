using System.Diagnostics.CodeAnalysis;

namespace Haskuldr.Abstractions.Types;

public readonly record struct Option<TValue> 
    where TValue : notnull
{
    private readonly TValue? _value;

    [MemberNotNullWhen(true, nameof(_value))]
    public bool HasValue { get; }

    private Option(TValue value)
    {
        _value = value;
        
        HasValue = true;
    }
    
    public bool TryPickValue([NotNullWhen(true)] out TValue? value)
    {
        value = HasValue ? _value : default;
        
        return HasValue;
    }

    public TValue GetValue()
    {
        return HasValue 
            ? _value 
            : throw new InvalidOperationException("Cannot access value because option is empty");   
    }
    
    public TValue? GetValueOrDefault()
    {
        return HasValue 
            ? _value 
            : default;
    }
    
    public TResult Match<TResult>(Func<TValue, TResult> onValue, Func<TResult> onEmpty)
    {
        return HasValue ? onValue(_value) : onEmpty();
    }
    
    public void Match(Action<TValue> onValue, Action onEmpty)
    {
        if (HasValue)
        {
            onValue(_value);
        }
        else
        {
            onEmpty();
        }
    }

    public static Option<TValue> FromValue(TValue value) => new(value);
    
    public static Option<TValue> None => new ();
    
    public static implicit operator Option<TValue>(TValue value) => FromValue(value);
}