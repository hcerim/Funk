using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static Maybe<T> maybe<T>(T item) => item.AsMaybe();
    }
}
