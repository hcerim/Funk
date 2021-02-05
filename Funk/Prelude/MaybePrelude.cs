using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        /// <summary>
        /// Creates a Maybe of object.
        /// </summary>
        [Pure]
        public static Maybe<T> may<T>(T item) => item.AsMaybe();

        /// <summary>
        /// Creates a Maybe of nullable value object.
        /// </summary>
        [Pure]
        public static Maybe<T> may<T>(T? item) where T : struct => item.AsMaybe();
    }
}
