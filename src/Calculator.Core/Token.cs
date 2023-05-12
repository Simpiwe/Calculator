namespace Calculator.Core
{
    public readonly struct Token
    {
        public Token(TokenKind kind) : this(kind, null)
        {
        }

        public Token(TokenKind kind, string? value)
        {
            Kind = kind;
            Value = value;
        }

        public TokenKind Kind { get; }

        public string? Value { get; }
    }
}