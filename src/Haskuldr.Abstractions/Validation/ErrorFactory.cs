namespace Haskuldr.Abstractions.Validation;

public class ErrorFactory(string Resource)
{
    public Error NotFound(string? description = null) =>
        new(ErrorType.NotFound, Resource, description);

    public Error Unauthorized(string? description = null) =>
        new(ErrorType.Unauthorized, Resource, description);

    public Error Forbidden(string? description = null) =>
        new(ErrorType.Forbidden, Resource, description);

    public Error Conflict(string? description = null) =>
        new(ErrorType.Conflict, Resource, description);
    
    public Error Internal(string? description = null) =>
        new(ErrorType.Internal, Resource, description);
}