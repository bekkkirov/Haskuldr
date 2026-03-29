namespace Haskuldr.Abstractions.Validation;

public record Error(
    ErrorType Type,
    string Resource,
    string? Description);