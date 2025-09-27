using System.Reflection;
using System.Runtime.CompilerServices;

namespace Haskuldr.Shared;

internal static class ThrowHelper
{
    internal static void ThrowIfEmpty(
        Assembly[] assemblies,
        [CallerArgumentExpression(nameof(assemblies))] string? paramName = null)
    {
        if (assemblies.Length == 0)
        {
            throw new ArgumentException(ErrorMessages.NoAssembliesProvided, paramName);
        }
    }
}