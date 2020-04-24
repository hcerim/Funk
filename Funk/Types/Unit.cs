using System;
using System.Diagnostics.Contracts;

namespace Funk
{
    /// <summary>
    /// Type that represents an empty value.
    /// Should be used to represent the absence of data.
    /// It does not contain any information and has only one possible value, itself.
    /// </summary>
    public readonly struct Unit : IEquatable<Unit>
    {
        [Pure]
        public static Unit Value => new Unit();

        /// <summary>
        /// Maps Unit to the result of the selector.
        /// </summary>
        public T Match<T>(Func<Unit, T> selector) => selector(Value);

        /// <summary>
        /// Executes operation provided with the Unit.
        /// </summary>
        public void Match(Action<Unit> operation) => operation(Value);

        public static bool operator ==(Unit unit, Unit other) => true;

        public static bool operator !=(Unit unit, Unit other) => false;

        public override string ToString() => "empty";

        public bool Equals(Unit other) => true;

        public override bool Equals(object obj) => obj is Unit;

        public override int GetHashCode() => 0;
    }
}
