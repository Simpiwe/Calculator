using System.Linq.Expressions;

namespace Calculator.Core
{
    /*
    Grammar:
        additive        : term ((+ | -) additive)*

        term            : LBracket additive RBracket
                        : atom ((* | /) term)*

        atom            : (+ | - | ) <number>
    */
    public sealed partial class Parser
    {
        class ParserImpl
        {
            private readonly IReadOnlyList<Token> _tokens;
            private int _position;

            public ParserImpl(ITokenizer tokenizer, string expression)
            {
                _tokens = tokenizer.GetTokens(expression).ToList();
            }

            public Expression Parse()
            {
                IEnumerable<Token> unknownTokens = _tokens.Where(x => x.Kind == TokenKind.Unknown);
                
                if (unknownTokens.Any())
                {
                    IEnumerable<string> errors = unknownTokens.Select(x => $"Invalid syntax. Unknown token '{x.Text}' at position '{x.Position}'");

                    throw new InvalidSyntaxException(errors);
                }

                return Additive(_tokens, ref _position);
            }

            private static Token? GetToken(IReadOnlyList<Token> tokens, int position)
            {
                if (position < 0 || position >= tokens.Count)
                {
                    return null;
                }

                return tokens[position];
            }

            private static bool IsTokenKind(IReadOnlyList<Token> tokens, int position, TokenKind kind)
            {
                Token? token = GetToken(tokens, position);

                return token?.Kind == kind;
            }

            private static Expression Additive(IReadOnlyList<Token> tokens, ref int position)
            {
                Expression left = Term(tokens, ref position);

                if (IsTokenKind(tokens, position, TokenKind.Add))
                {
                    position++;
                    left = Expression.Add(left, Additive(tokens, ref position));
                }
                else if (IsTokenKind(tokens, position, TokenKind.Subtract))
                {
                    position++;
                    left = Expression.Subtract(left, Additive(tokens, ref position));
                }

                return left;
            }

            private static Expression Term(IReadOnlyList<Token> tokens, ref int position)
            {
                Expression left;

                if (IsTokenKind(tokens, position, TokenKind.OpenBracket))
                {
                    position++;
                    left = Additive(tokens, ref position);

                    if (!IsTokenKind(tokens, position, TokenKind.CloseBracket))
                    {
                        throw new InvalidSyntaxException($"Invalid syntax. Expected ')' at position '{position}'");
                    }

                    position++;
                }
                else
                {
                    left = Atom(tokens, ref position);

                    if (IsTokenKind(tokens, position, TokenKind.Multiply))
                    {
                        position++;
                        left = Expression.Multiply(left, Term(tokens, ref position));
                    }
                    else if (IsTokenKind(tokens, position, TokenKind.Divide))
                    {
                        position++;
                        left = Expression.Divide(left, Term(tokens, ref position));
                    }
                }

                return left;
            }

            private static Expression Atom(IReadOnlyList<Token> tokens, ref int position)
            {
                bool negate = IsTokenKind(tokens, position, TokenKind.Subtract);

                if (negate || IsTokenKind(tokens, position, TokenKind.Add))
                {
                    position++;
                }

                Token current = GetToken(tokens, position) ?? throw new InvalidSyntaxException("Unexpected end of input. Expected a +, - or a number.");

                if (current.Kind != TokenKind.Number)
                {
                    throw new InvalidSyntaxException($"Invalid token <{current.Kind}> at position '{current.Position}'. Expected a number.");
                }

                Expression number = Expression.Constant(current.Value);
                position++;

                return negate ? Expression.Negate(number) : number;
            }
        }
    }
}
