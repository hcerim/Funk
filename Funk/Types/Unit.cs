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
        /// <summary>
        /// Returns newly created Unit.
        /// </summary>
        [Pure]
        public static Unit Value => new Unit();

        /// <summary>
        /// Executes function and returns its result.
        /// </summary>
        public T Match<T>(Func<Unit, T> selector) => selector(Value);

        /// <summary>
        /// Executes non-returning function.
        /// </summary>
        public void Match(Action<Unit> operation) => operation(Value);

        /// <summary>
        /// Compares two Unit objects.
        /// Always returns true as two Unit objects are always equal.
        /// </summary>
        [Pure]
        public static bool operator ==(Unit unit, Unit other) => true;

        /// <summary>
        /// Compares two Unit objects.
        /// Always returns false as two Unit objects are always equal.
        /// </summary>
        [Pure]
        public static bool operator !=(Unit unit, Unit other) => false;

        /// <summary>
        /// Returns "empty" as the Unit object represents an empty value.
        /// </summary>
        [Pure]
        public override string ToString() => "empty";

        /// <summary>
        /// Compares two Unit objects.
        /// Always returns true as two Unit objects are always equal.
        /// </summary>
        [Pure]
        public bool Equals(Unit other) => true;

        /// <summary>
        /// Compares two Unit objects.
        /// Returns true if the other object is Unit. Otherwise returns false.
        /// </summary>
        [Pure]
        public override bool Equals(object obj) => obj is Unit;

        /// <summary>
        /// Returns 0.
        /// </summary>
        [Pure]
        public override int GetHashCode() => 0;
    }
}
