namespace Haskuldr.Abstractions.Validation;

public abstract class ErrorFactory
{
    protected static Error Validation(
        string resource,
        string code,
        string? description = null) => new(ErrorType.Validation, resource, code, description);

    protected static Error NotFound(
        string resource,
        string code,
        string? description = null) => new(ErrorType.NotFound, resource, code, description);

    protected static Error Unauthorized(
        string resource,
        string code,
        string? description = null) => new(ErrorType.Unauthorized, resource, code, description);

    protected static Error Forbidden(
        string resource,
        string code,
        string? description = null) => new(ErrorType.Forbidden, resource, code, description);

    protected static Error Conflict(
        string resource,
        string code,
        string? description = null) => new(ErrorType.Conflict, resource, code, description);
    
    protected static Error Internal(
        string resource,
        string code,
        string? description = null) => new(ErrorType.Internal, resource, code, description);
}