using Haskuldr.Abstractions.Types;

namespace Haskuldr.Abstractions.Validation;

public interface IValidator<in TInput>
{
    Option<Error> Validate(TInput instance);
}