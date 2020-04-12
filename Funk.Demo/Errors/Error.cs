using System;

namespace Funk.Demo
{
    public class Error : Exception
    {
        public Error(string message)
            : base(message)
        {
        }
    }

    public class InvalidRequestError : Error
    {
        public InvalidRequestError(string message)
            : base(message)
        {
        }
    }

    public class UnauthorizedError : Error
    {
        public UnauthorizedError(string message)
            : base(message)
        {
        }
    }

    public class ForbiddenError : Error
    {
        public ForbiddenError(string message)
            : base(message)
        {
        }
    }

    public class JsonError : Error
    {
        public JsonError(string message)
            : base(message)
        {
        }
    }
}
