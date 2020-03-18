using System.Diagnostics.Contracts;

namespace Funk
{
    public static class NullCheckExtensions
    {
        [Pure]
        public static bool IsNull<T>(this T t) where T : class => t.SafeEquals(null);

        [Pure]
        public static bool IsNull<T>(this T? t) where T : struct => t.SafeEquals(null);

        [Pure]
        public static bool IsNotNull<T>(this T t) where T : class => !t.IsNull();

        [Pure]
        public static bool IsNotNull<T>(this T? t) where T : struct => !t.IsNull();
    }
}
