using System;
using System.Diagnostics.Contracts;
using static Funk.Prelude;

namespace Funk
{
    public static class BoolExt
    {
        public static T Match<T>(this bool item, Func<Unit, T> ifFalse, Func<Unit, T> ifTrue) => item.Match(false, _ => ifFalse(Unit.Value), true, _ => ifTrue(Unit.Value));

        public static void Match(this bool item, Action<Unit> ifFalse = null, Action<Unit> ifTrue = null) => item.Match(false, _ => ifFalse?.Invoke(Unit.Value), true, _ => ifTrue?.Invoke(Unit.Value));

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
    }
}
