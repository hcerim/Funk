namespace Funk.Demo
{
    public sealed class Auth
    {
        public Maybe<string> Token { get; }

        public Auth(string token)
        {
            Token = token.AsNotEmptyString();
        }
    }
}
