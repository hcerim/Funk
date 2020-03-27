using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static Maybe<T> may<T>(T item) => item.AsMaybe();

        [Pure]
        public static Maybe<T> may<T>(T? item) where T : struct => item.AsMaybe();
    }
}
