using System;

namespace Funk
{
    /// <summary>
    /// Type with only one value -> itself.
    /// Replacement for empty tuple.
    /// Represents a type that contains no information (empty value).
    /// </summary>
    public struct Unit
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
    }
}
