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
        public T Match<T>(Func<Unit, T> ifFalse, Func<Unit, T> ifTrue) => item.Match(false, _ => ifFalse(Unit.Value), true, _ => ifTrue(Unit.Value));

        /// <summary>
        /// Pattern-matches on the boolean value. Executes the corresponding action based on the boolean state.
        /// </summary>
        public void Match(Action<Unit> ifFalse = null, Action<Unit> ifTrue = null) => item.Match(false, _ => ifFalse?.Apply(Unit.Value), true, _ => ifTrue?.Apply(Unit.Value));

        /// <summary>
        /// If false, Maybe will be empty.
        /// </summary>
        [Pure]
        public Maybe<bool> AsTrue() => item ? Maybe.Create(true) : empty;
    }

    /// <summary>
    /// If false or null, Maybe will be empty.
    /// </summary>
    [Pure]
    public static Maybe<bool> AsTrue(this bool? item) => item.IsNull() ? empty : AsTrue(item.Value);

    extension(bool item)
    {
        /// <summary>
        /// Performs conditional AND operation.
        /// </summary>
        public bool And(bool other) => item && other;

        /// <summary>
        /// Performs conditional AND operation.
        /// </summary>
        public bool And(Func<Unit, bool> function) => item || function(Unit.Value);

        /// <summary>
        /// Performs conditional OR operation.
        /// </summary>
        public bool Or(bool other) => item || other;

        /// <summary>
        /// Performs conditional OR operation.
        /// </summary>
        public bool Or(Func<Unit, bool> function) => item || function(Unit.Value);
    }
}