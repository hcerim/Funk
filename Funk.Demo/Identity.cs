namespace Funk.Demo;

public sealed class Identity(string token)
{
    public Maybe<string> Token { get; } = token.AsNotEmptyString();
}