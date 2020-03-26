namespace Funk.Exceptions
{
    public class UnhandledValueException : FunkException
    {
        public UnhandledValueException(string message)
            : base(FunkExceptionType.UnhandledValue, message)
        {
        }
    }
}
