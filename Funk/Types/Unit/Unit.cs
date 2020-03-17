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

        [Pure]
        public override string ToString() => string.Empty;

        [Pure]
        public bool Equals(Unit other) => true;
    }
}
