namespace Calculator.Core
{
    public interface ITokenizer
    {
        IEnumerable<Token> GetTokens(ReadOnlySpan<char> input);
    }
}