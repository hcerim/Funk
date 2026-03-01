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
        [Pure]
        public bool IsNull() => t is null;

        /// <summary>
        /// Returns true if the specified value is not null.
        /// </summary>
        [Pure]
        public bool IsNotNull() => !t.IsNull();
    }

    /// <summary>
    /// Returns the item if not null. Otherwise, creates and returns a new instance.
    /// </summary>
    public static T Initialize<T>(this T item) where T : new() => item.IsNull() ? new T() : item;
}