using System;

namespace Funk
{
    /// <summary>
    /// Type with only one value -> itself.
    /// Replacement for empty tuple.
    /// Represents a type that contains no information (empty value).
    /// </summary>
    public struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Value = new Unit();

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
