namespace Calculator.Core
{
    public record Token
    {
        public required TokenKind Kind { get; init; }

        public string? Text { get; init; }

        public object? Value { get; init; }

        public required int Position { get; init; }

        public required int Length { get; init; }
    }
}