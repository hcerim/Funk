using System;
using Funk.Exceptions;

namespace Funk
{
    public static partial class Prelude
    {
        public static EnumerableException exception(Exception exception) => exception.ToException();
    }
}
