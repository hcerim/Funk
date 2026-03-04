using System.Diagnostics.Contracts;

namespace Funk;

/// <summary>
/// Provides extension methods for null checking operations.
/// </summary>
public static class NullCheckExt
{
    extension<T>(T t)
    {
        /// <summary>
        /// Returns true if the specified value is null.
        /// </summary>
        /// <returns>true if the value is null; otherwise, false.</returns>
        [Pure]
        public bool IsNull() => t is null;

        /// <summary>
        /// Returns true if the specified value is not null.
        /// </summary>
        /// <returns>true if the value is not null; otherwise, false.</returns>
        [Pure]
        public bool IsNotNull() => !t.IsNull();
    }

    /// <summary>
    /// Returns the item if not null. Otherwise, creates and returns a new instance.
    /// </summary>
    /// <param name="item">The value to check for null.</param>
    /// <returns>The item if not null; otherwise, a new instance of T.</returns>
    public static T Initialize<T>(this T item) where T : new() => item.IsNull() ? new T() : item;
}
