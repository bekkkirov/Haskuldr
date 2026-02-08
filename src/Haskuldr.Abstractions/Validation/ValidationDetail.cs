namespace Haskuldr.Abstractions.Validation;

public sealed record ValidationDetail(
    string FieldName,
    string Description);