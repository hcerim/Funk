using System;
using System.Diagnostics.Contracts;
using static Funk.Prelude;

namespace Funk;

/// <summary>
/// Provides extension methods for boolean operations and pattern matching.
/// </summary>
public static class BoolExt
{
    extension(bool item)
    {
        /// <summary>
        /// Pattern-matches on the boolean value. Returns the result of the corresponding function based on the boolean state.
        /// </summary>
        /// <typeparam name="T">The return type of the match.</typeparam>
        /// <param name="ifFalse">The function to execute when the value is false.</param>
        /// <param name="ifTrue">The function to execute when the value is true.</param>
        /// <returns>The result of the matched function.</returns>
        public T Match<T>(Func<Unit, T> ifFalse, Func<Unit, T> ifTrue) => item.Match(false, _ => ifFalse(Unit.Value), true, _ => ifTrue(Unit.Value));

        /// <summary>
        /// Pattern-matches on the boolean value. Executes the corresponding action based on the boolean state.
        /// </summary>
        /// <param name="ifFalse">The action to execute when the value is false.</param>
        /// <param name="ifTrue">The action to execute when the value is true.</param>
        public void Match(Action<Unit> ifFalse = null, Action<Unit> ifTrue = null) => item.Match(false, _ => ifFalse?.Apply(Unit.Value), true, _ => ifTrue?.Apply(Unit.Value));

        /// <summary>
        /// If false, Maybe will be empty.
        /// </summary>
        /// <returns>A Maybe containing true if the value is true, or an empty Maybe.</returns>
        [Pure]
        public Maybe<bool> AsTrue() => item ? Maybe.Create(true) : empty;
    }

    /// <summary>
    /// If false or null, Maybe will be empty.
    /// </summary>
    /// <param name="item">The nullable boolean value.</param>
    /// <returns>A Maybe containing true if the value is true, or an empty Maybe.</returns>
    [Pure]
    public static Maybe<bool> AsTrue(this bool? item) => item.IsNull() ? empty : AsTrue(item.Value);

    extension(bool item)
    {
        /// <summary>
        /// Performs conditional AND operation.
        /// </summary>
        /// <param name="other">The other boolean value.</param>
        /// <returns>true if both values are true; otherwise, false.</returns>
        public bool And(bool other) => item && other;

        /// <summary>
        /// Performs conditional AND operation.
        /// </summary>
        /// <param name="function">The function to evaluate if the value is true.</param>
        /// <returns>true if both the value and the function result are true; otherwise, false.</returns>
        public bool And(Func<Unit, bool> function) => item && function(Unit.Value);

        /// <summary>
        /// Performs conditional OR operation.
        /// </summary>
        /// <param name="other">The other boolean value.</param>
        /// <returns>true if either value is true; otherwise, false.</returns>
        public bool Or(bool other) => item || other;

        /// <summary>
        /// Performs conditional OR operation.
        /// </summary>
        /// <param name="function">The function to evaluate if the value is false.</param>
        /// <returns>true if either the value or the function result is true; otherwise, false.</returns>
        public bool Or(Func<Unit, bool> function) => item || function(Unit.Value);
    }
}
