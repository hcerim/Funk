using System;
using System.Diagnostics.Contracts;

namespace Funk
{
    /// <summary>
    /// Type with only one value -> itself.
    /// Represents a type that contains no information (empty value).
    /// </summary>
    public struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Value = new Unit();

        /// <summary>
        /// Transforms Unit into a specified object.
        /// </summary>
        [Pure]
        public T TransformTo<T>(T t) => t;

        /// <summary>
        /// Transforms unit into a specified object.
        /// </summary>
        [Pure]
        public T TransformTo<T>(Func<Unit, T> f) => f(Value);

        [Pure]
        public override string ToString() => string.Empty;

        [Pure]
        public bool Equals(Unit other) => true;
    }
}
