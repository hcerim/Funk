using System;

namespace Funk.Exceptions
{
    public abstract class FunkException : Exception
    {
        protected FunkException(FunkExceptionType type, string message)
            : base(message)
        {
            Type = type;
        }

        public FunkExceptionType Type { get; }
    }
}
