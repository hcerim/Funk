using System.Diagnostics.Contracts;

namespace Funk
{
    public static class NullCheckExt
    {
        [Pure]
        public static bool IsNull<T>(this T t) => t == null;

        [Pure]
        public static bool IsNotNull<T>(this T t) => !t.IsNull();

        public static T Initialize<T>(this T item) where T : new() => item.IsNull() ? new T() : item;
    }
}
