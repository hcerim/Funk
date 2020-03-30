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
        public static Maybe<bool> IsTrue(this bool item)
        {
            if (item)
            {
                return true;
            }
            return empty;
        }

        /// <summary>
        /// If false or null, Maybe will be empty.
        /// </summary>
        public static Maybe<bool> IsTrue(this bool? item)
        {
            return item is null ? empty : IsTrue(item.Value);
        }
    }
}
