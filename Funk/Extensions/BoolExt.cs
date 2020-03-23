using System;
using System.Diagnostics.Contracts;

namespace Funk
{
    public static class BoolExt
    {
        [Pure]
        public static T Match<T>(this bool item, Func<Unit, T> ifFalse, Func<Unit, T> ifTrue) => item is false ? ifFalse(Unit.Value) : ifTrue(Unit.Value);

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
    }
}
