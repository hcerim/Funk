﻿using System;
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
        /// Maps Unit to the result of the selector.
        /// </summary>
        [Pure]
        public T Match<T>(Func<Unit, T> selector) => selector(Value);

        /// <summary>
        /// Executes operation provided with the Unit.
        /// </summary>
        public void Match(Action<Unit> operation) => operation(Value);

        [Pure]
        public override string ToString() => string.Empty;

        [Pure]
        public bool Equals(Unit other) => true;
    }
}
