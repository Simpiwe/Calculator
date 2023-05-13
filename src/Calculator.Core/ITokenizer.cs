namespace Calculator.Core
{
    public interface ITokenizer
    {
        IEnumerable<Token> Tokenize(string input);
    }
}