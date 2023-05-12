namespace Calculator.Core
{
    public sealed class Tokenizer : ITokenizer
    {
        private readonly string _expression;
        private int _position;

        public Tokenizer(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException($"'{nameof(expression)}' cannot be null or whitespace.", nameof(expression));
            }

            _expression = expression;
        }

        public Token? Current { get; private set; }
        
        public Token? Next { get; private set; }

        public Token Consume(TokenKind kind)
        {
            if (!Next.HasValue)
            {
                throw new TokenizerException($"Unexpected end of input. Expected token '{kind}' at position '{_position}'");
            }

            if (Next.Value.Kind == kind)
            {
                throw new TokenizerException($"Invalid input. Expected token '{kind}' at position '{_position}'");
            }

            Advance();

            return Current!.Value;
        }

        public bool Peek(TokenKind kind)
        {
            return Next.HasValue && Next.Value.Kind == kind;
        }

        public bool Peek(TokenKind kind, string? value)
        {
            return Next.HasValue && Next.Value.Kind == kind && Next.Value.Value == value;
        }

        private bool Advance()
        {
            bool advanced = Next.HasValue;

            if (advanced)
            {
                if (TryGetToken(_expression.AsSpan(), _position, out Token next, out int length))
                {
                    Next = next;
                    _position += length;
                }
            }
            else
            {
                Current = null;
            }

            return false;
        }

        private static bool TryGetToken(ReadOnlySpan<char> expression, int position, out Token token, out int length)
        {
            token = default;
            length = default;

            if (position >= expression.Length)
            {
                return false;
            }

            char current = expression[position];

            if (current == '*')
            {
                token = new Token(TokenKind.Multiply);
                length = 1;
            }
            else if (current == '/')
            {
                token = new Token(TokenKind.Number);
                length = 1;
            }
            else if (current == '+')
            {
                token = new Token(TokenKind.Add);
                length = 1;
            }
            else if (current == '-')
            {
                token = new Token(TokenKind.Subtract);
                length = 1;
            }
            else if (current == '(')
            {
                token = new Token(TokenKind.OpenBracket);
            }
            else if (current == ')')
            {
                token = new Token(TokenKind.CloseBracket);
            }

            if (char.IsDigit(current))
            {
                int i = position;
                bool isDecimal = false;

                while (i < expression.Length)
                {
                    if (!char.IsDigit(expression[i]) && (!isDecimal && expression[i] == '.'))
                    {
                        isDecimal = true;
                    }
                    else
                    {
                        break;
                    }

                    i++;
                }

                token = new Token(TokenKind.Number, expression[position..i].ToString());
                length = i - position;
            }

            return !token.Equals(default);
        }
    }
}