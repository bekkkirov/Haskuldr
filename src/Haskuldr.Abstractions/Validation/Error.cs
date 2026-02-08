namespace Haskuldr.Abstractions.Validation;

public sealed record Error
{
    public ErrorType Type { get; }
    
    public string Resource { get; }

    public string? Description { get; }

    private Error(
        ErrorType type,
        string resource,
        string? description)
    {
        Type = type;
        Resource = resource;
        Description = description;
    }

    public static Error NotFound(string resource, string? description = null) =>
        new(ErrorType.NotFound, resource, description);

    public static Error Unauthorized(string resource, string? description = null) =>
        new(ErrorType.Unauthorized, resource, description);

    public static Error Forbidden(string resource, string? description = null) =>
        new(ErrorType.Forbidden, resource, description);

    public static Error Conflict(string resource, string? description = null) =>
        new(ErrorType.Conflict, resource, description);
    
    public static Error Internal(string resource, string? description = null) =>
        new(ErrorType.Internal, resource, description);
}