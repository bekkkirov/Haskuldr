namespace Haskuldr.Abstractions.Validation;

public record ErrorBase(
    ErrorType Type,
    string Resource,
    string? Description);