using System.Diagnostics.CodeAnalysis;

namespace Haskuldr.Abstractions.Types;

public readonly record struct Union<T1, T2>
    where T1 : notnull
    where T2 : notnull
{
    private readonly T1? _value1;
    private readonly T2? _value2;

    [MemberNotNullWhen(true, nameof(_value1))]
    [MemberNotNullWhen(false, nameof(_value2))]
    public bool IsT1 { get; }

    [MemberNotNullWhen(true, nameof(_value2))]
    [MemberNotNullWhen(false, nameof(_value1))]
    public bool IsT2 => !IsT1;

    private Union(T1 value)
    {
        _value1 = value;

        IsT1 = true;
    }

    private Union(T2 value)
    {
        _value2 = value;

        IsT1 = false;
    }

    public bool TryPickT1(
        [NotNullWhen(true)] out T1? value1,
        [NotNullWhen(false)] out T2? value2)
    {
        value1 = IsT1 ? _value1 : default;
        value2 = IsT2 ? _value2 : default;

        return IsT1;
    }

    public bool TryPickT2(
        [NotNullWhen(true)] out T2? value2,
        [NotNullWhen(false)] out T1? value1)
    {
        value2 = IsT2 ? _value2 : default;
        value1 = IsT1 ? _value1 : default;

        return IsT2;
    }

    public T1 GetT1()
    {
        return IsT1
            ? _value1
            : throw new InvalidOperationException($"Cannot access {nameof(T1)} because union contains {nameof(T2)}");
    }

    public T1? GetT1OrDefault()
    {
        return IsT1
            ? _value1
            : default;
    }

    public T2 GetT2()
    {
        return IsT2
            ? _value2
            : throw new InvalidOperationException($"Cannot access {nameof(T2)} because union contains {nameof(T1)}");
    }

    public T2? GetT2OrDefault()
    {
        return IsT2
            ? _value2
            : default;
    }

    public TResult Map<TResult>(Func<T1, TResult> onT1, Func<T2, TResult> onT2)
    {
        return IsT1 ? onT1(_value1) : onT2(_value2!);
    }

    public static Union<T1, T2> FromT1(T1 value)
    {
        return new Union<T1, T2>(value);
    }

    public static Union<T1, T2> FromT2(T2 value)
    {
        return new Union<T1, T2>(value);
    }

    public static implicit operator Union<T1, T2>(T1 value) => FromT1(value);

    public static implicit operator Union<T1, T2>(T2 value) => FromT2(value);
}
