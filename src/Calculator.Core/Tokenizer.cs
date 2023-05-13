using System.Globalization;

namespace Calculator.Core
{
    public sealed class Tokenizer : ITokenizer
    {
        public IEnumerable<Token> GetTokens(ReadOnlySpan<char> expression)
        {
            List<Token> tokens = new List<Token>();

            for (int i = 0; i < expression.Length; i++)
            {
                Token token;

                //skip whitespaces
                if (char.IsWhiteSpace(expression[i]))
                {
                    continue;
                }

                char current = expression[i];

                if (current == '*')
                {
                    token = new Token()
                    {
                        Kind = TokenKind.Multiply,
                        Text = "*",
                        Length = 1,
                        Position = i
                    };
                }
                else if (current == '/')
                {
                    token = new Token()
                    {
                        Kind = TokenKind.Divide,
                        Text = "/",
                        Length = 1,
                        Position = i
                    };
                }
                else if (current == '+')
                {
                    token = new Token()
                    {
                        Kind = TokenKind.Add,
                        Text = "+",
                        Length = 1,
                        Position = i
                    };
                }
                else if (current == '-')
                {
                    token = new Token()
                    {
                        Kind = TokenKind.Subtract,
                        Text = "-",
                        Length = 1,
                        Position = i
                    };
                }
                else if (current == '(')
                {
                    token = new Token()
                    {
                        Kind = TokenKind.OpenBracket,
                        Text = "(",
                        Length = 1,
                        Position = i
                    };
                }
                else if (current == ')')
                {
                    token = new Token()
                    {
                        Kind = TokenKind.CloseBracket,
                        Text = ")",
                        Length = 1,
                        Position = i
                    };
                }
                else if (char.IsDigit(current))
                {
                    int pos = i;
                    bool isDecimal = false;

                    while (pos < expression.Length)
                    {
                        if (expression[pos] == '.' && !isDecimal)
                        {
                            isDecimal = true;
                        }
                        else if (!char.IsDigit(expression[pos]))
                        {
                            break;
                        }

                        pos++;
                    }

                    ReadOnlySpan<char> text = expression[i..pos];
                    
                    token = new Token
                    {
                        Kind = TokenKind.Number,
                        Length = pos - i,
                        Position = i,
                        Text = text.ToString(),
                        Value = double.Parse(text, CultureInfo.InvariantCulture)
                    };

                    i = pos - 1;
                }
                else
                {
                    int pos = i;

                    while (pos < expression.Length)
                    {
                        if (char.IsWhiteSpace(expression[pos]))
                        {
                            break;
                        }

                        pos++;
                    }

                    token = new Token
                    {
                        Kind = TokenKind.Unknown,
                        Length = pos - i,
                        Position = i,
                        Text = expression[i..pos].ToString()
                    };

                    i = pos - 1;
                }

                tokens.Add(token);
            }

            tokens.Add(new Token
            {
                Kind = TokenKind.EndOfInput,
                Length = 0,
                Position = expression.Length
            });

            return tokens;
        }
    }
}