using System;
using System.Diagnostics.Contracts;
using static Funk.Prelude;

namespace Funk
{
    public static class BoolExt
    {
        public static T Match<T>(this bool item, Func<Unit, T> ifFalse, Func<Unit, T> ifTrue) => item.Match(false, _ => ifFalse(Unit.Value), true, _ => ifTrue(Unit.Value));

        public static void Match(this bool item, Action<Unit> ifFalse = null, Action<Unit> ifTrue = null) => item.Match(false, _ => ifFalse?.Apply(Unit.Value), true, _ => ifTrue?.Apply(Unit.Value));

        /// <summary>
        /// If false, Maybe will be empty.
        /// </summary>
        [Pure]
        public static Maybe<bool> AsTrue(this bool item) => item ? Maybe.Create(true) : empty;

        /// <summary>
        /// If false or null, Maybe will be empty.
        /// </summary>
        [Pure]
        public static Maybe<bool> AsTrue(this bool? item) => item.IsNull() ? empty : AsTrue(item.Value);

        /// <summary>
        /// Performs conditional AND operation.
        /// </summary>
        public static bool And(this bool item, bool other) => item && other;

        /// <summary>
        /// Performs conditional AND operation.
        /// </summary>
        public static bool And(this bool item, Func<Unit, bool> function) => item || function(Unit.Value);

        /// <summary>
        /// Performs conditional OR operation.
        /// </summary>
        public static bool Or(this bool item, bool other) => item || other;

        /// <summary>
        /// Performs conditional OR operation.
        /// </summary>
        public static bool Or(this bool item, Func<Unit, bool> function) => item || function(Unit.Value);
    }
}
