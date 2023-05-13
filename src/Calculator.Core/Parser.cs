using System.Linq.Expressions;
using System.Reflection;

namespace Calculator.Core
{
    public sealed class Parser : IParser
    {
        private readonly ITokenizer _tokenizer;

        public Parser(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        }

        public Expression Parse(string expression)
        {
            IReadOnlyList<Token> tokens = _tokenizer.GetTokens(expression).ToList();

            IEnumerable<Token> unknownTokens = tokens.Where(x => x.Kind == TokenKind.Unknown);

            if (unknownTokens.Any())
            {
                IEnumerable<string> errors = unknownTokens.Select(x => $"Invalid syntax. Unknown token '{x.Text}' at position '{x.Position}'");

                throw new InvalidSyntaxException(errors);
            }

            int position = 0;
            
            Expression result = Additive(tokens, ref position);

            Token? token = GetToken(tokens, position);
            if (token != null)
            {
                throw new ParsingException("Failed to fully parse the input text. Please try and rephrase the expression.");
            }

            return result;
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
            Expression left;

            //bool negate = IsTokenKind(tokens, position, TokenKind.Subtract);
            //if (negate || IsTokenKind(tokens, position, TokenKind.Add))
            //{
            //    position++;

            //    left = Additive(tokens, ref position);

            //    return negate ? Expression.Negate(left) : left;
            //}

            left = Term(tokens, ref position);
            //if (negate)
            //{
            //    left = Expression.Negate(left);
            //}

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
            Token current = GetToken(tokens, position) ?? throw new InvalidSyntaxException("Unexpected end of input. Expected a number or an identifier.");

            //if (current.Kind != TokenKind.Number && current.Kind != TokenKind.Identifier)
            //{
            //    throw new InvalidSyntaxException($"Invalid token '{current.Text}' at position '{current.Position}'. Expected a number or an identifier.");
            //}

            if (current.Kind == TokenKind.Identifier)
            {
                string identifier = current.Text ?? throw new InvalidOperationException("Identifier name wasn't provided by the Lexer.");
                int identifierPosition = current.Position;

                position++;
                current = GetToken(tokens, position) ?? throw new InvalidSyntaxException($"Unexpected end of input. Expected a '('.");

                if (current.Kind != TokenKind.OpenBracket)
                {
                    throw new InvalidSyntaxException($"Invalid token '{current.Text}' at position '{current.Position}'. Expected a '('.");
                }

                position++;


                IList<Expression> args = Array.Empty<Expression>();
                if (!IsTokenKind(tokens, position, TokenKind.CloseBracket))
                {
                    args = new List<Expression>();

                    while (position < tokens.Count)
                    {
                        args.Add(Additive(tokens, ref position));

                        if (!IsTokenKind(tokens, position, TokenKind.Comma))
                        {
                            break;
                        }

                        position++;
                    }
                }

                current = GetToken(tokens, position) ?? throw new InvalidSyntaxException($"Unexpected end of input. Expected a ')'.");
                if (current.Kind != TokenKind.CloseBracket)
                {
                    throw new InvalidSyntaxException($"Invalid token '{current.Text}' at position '{current.Position}'. Expected a ')'.");
                }

                position++;

                Type[] types = args?.Select(x => x.Type).ToArray() ?? Array.Empty<Type>();

                MethodInfo method = typeof(Math).GetMethod(identifier, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase, types)
                    ?? throw new ParsingException($"Unknown function '{identifier}' at position '{identifierPosition}'. A function that matches the specified signature does not exist.");

                return Expression.Call(method, args);
            }

            Expression left;

            bool negate = IsTokenKind(tokens, position, TokenKind.Subtract);
            if (negate || IsTokenKind(tokens, position, TokenKind.Add))
            {
                position++;

                left = Atom(tokens, ref position);

                return negate ? Expression.Negate(left) : left;
            }

            if (current.Kind != TokenKind.Number)
            {
                throw new InvalidSyntaxException($"Invalid token '{current.Text}' at position '{current.Position}'. Expected a number.");
            }

            position++;

            return Expression.Constant(current.Value);
        }
    }
}
