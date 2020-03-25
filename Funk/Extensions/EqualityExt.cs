using System.Diagnostics.Contracts;

namespace Funk
{
    public static class EqualityExt
    {
        [Pure]
        public static bool SafeEquals<T>(this T t, T other) => Equals(t, other);

        [Pure]
        public static bool SafeEquals<T>(this T t, T? other) where T : struct => ((T?)t).SafeEquals(other);

        [Pure]
        public static bool SafeEquals<T>(this T? t, T other) where T : struct => t.SafeEquals((T?)other);

        [Pure]
        public static bool SafeNotEquals<T>(this T t, T other) => !t.SafeEquals(other);

        [Pure]
        public static bool SafeNotEquals<T>(this T t, T? other) where T : struct => !t.SafeEquals(other);

        [Pure]
        public static bool SafeNotEquals<T>(this T? t, T other) where T : struct => !t.SafeEquals(other);
    }
}
