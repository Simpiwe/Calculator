namespace Calculator.Core
{
    public interface ITokenizer
    {
        Token? Current { get; }
        
        Token? Next { get; }

        bool Peek(TokenKind kind);

        bool Peek(TokenKind kind, string? value);

        Token Consume(TokenKind kind);
    }
}