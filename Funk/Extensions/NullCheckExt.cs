using System.Diagnostics.Contracts;

namespace Funk
{
    public static class NullCheckExt
    {
        [Pure]
        public static bool IsNull<T>(this T t) => t is null;

        [Pure]
        public static bool IsNotNull<T>(this T t) => !t.IsNull();
    }
}
