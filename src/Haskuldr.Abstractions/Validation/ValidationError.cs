namespace Haskuldr.Abstractions.Validation;

public sealed record ValidationError(
    string Resource,
    IReadOnlyCollection<ValidationDetail> Details) :
    Error(
        ErrorType.Validation,
        Resource,
        "One or more validation errors occurred");