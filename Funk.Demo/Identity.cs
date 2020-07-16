namespace Funk.Demo
{
    public sealed class Identity
    {
        public Maybe<string> Token { get; }

        public Identity(string token)
        {
            Token = token.AsNotEmptyString();
        }
    }
}
