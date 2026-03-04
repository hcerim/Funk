using System.Diagnostics.Contracts;

namespace Funk;

public static partial class Prelude
{
    /// <summary>
    /// Creates a Maybe of object.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="item">The value to wrap.</param>
    /// <returns>A Maybe containing the value if not null, or an empty Maybe.</returns>
    [Pure]
    public static Maybe<T> may<T>(T item) => item.AsMaybe();

    /// <summary>
    /// Creates a Maybe of nullable value object.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="item">The nullable value to wrap.</param>
    /// <returns>A Maybe containing the value if not null, or an empty Maybe.</returns>
    [Pure]
    public static Maybe<T> may<T>(T? item) where T : struct => item.AsMaybe();
}