using System;

namespace Funk.Exceptions
{
    public class FunkException : Exception
    {
        public FunkException(string message)
            : base(message)
        {
            Type = FunkExceptionType.Undefined;
        }

        public FunkException(FunkExceptionType type, string message)
            : base(message)
        {
            Type = type;
        }

        public FunkExceptionType Type { get; }
    }
}
