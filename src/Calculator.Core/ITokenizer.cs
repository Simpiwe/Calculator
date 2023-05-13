namespace Calculator.Core
{
    public interface ITokenizer
    {
        IEnumerable<Token> GetTokens(string input);
    }
}