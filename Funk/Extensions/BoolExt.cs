using System;
using static Funk.Prelude;

namespace Funk
{
    public static class BoolExt
    {
        public static T Match<T>(this bool item, Func<Unit, T> ifFalse, Func<Unit, T> ifTrue) => item ? ifTrue(Unit.Value) : ifFalse(Unit.Value);

        public static void Match(this bool item, Action<Unit> ifFalse, Action<Unit> ifTrue)
        {
            if (item)
            {
                ifTrue(Unit.Value);
            }
            else
            {
                ifFalse(Unit.Value);
            }
        }

        /// <summary>
        /// If false, Maybe will be empty.
        /// </summary>
        public static Maybe<bool> AsTrue(this bool item) => item ? Maybe.Create(true) : empty;

        /// <summary>
        /// If false or null, Maybe will be empty.
        /// </summary>
        public static Maybe<bool> AsTrue(this bool? item) => item.IsNull() ? empty : AsTrue(item.Value);
    }
}
