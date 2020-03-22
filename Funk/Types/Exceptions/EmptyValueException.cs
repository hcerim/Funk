namespace Funk.Exceptions
{
    public class EmptyValueException : FunkException
    {
        public EmptyValueException(string message)
            : base(FunkExceptionType.EmptyValue, message)
        {
        }
    }
}
