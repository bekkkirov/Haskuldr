namespace Haskuldr.EventSystem;

[AttributeUsage(AttributeTargets.Class)]
public sealed class OrderAttribute(int order) : Attribute
{
    public int Order { get; } = order;
}