using System.Linq.Expressions;

namespace Calculator.Core
{
    public sealed record ParseResult
    {
        private ParseResult(Expression expression)
        {
            Status = ParseStatus.Success;
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        private ParseResult(ParseStatus status, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));
            }

            Message = message;
            Status = status;
        }

        public static ParseResult Success(Expression expression)
        {
            return new ParseResult(expression);
        }

        public static ParseResult InvalidSyntax(string message)
        {
            return new ParseResult(ParseStatus.Failed_InvalidSyntax, message);
        }

        public Expression? Expression { get; }

        public ParseStatus Status { get; }

        public string? Message { get; }
    }
}
