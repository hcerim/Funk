using System.Diagnostics.Contracts;

namespace Funk
{
    public static partial class Prelude
    {
        [Pure]
        public static Unit empty => Unit.Value;
    }
}
