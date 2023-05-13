namespace Calculator.Core
{
    public sealed class InvalidSyntaxException : ParsingException
    {
        public InvalidSyntaxException(string error) : base(error)
        {
            Errors = new[] { error };
        }

        public InvalidSyntaxException(IEnumerable<string> errors) : base("One or more invalid syntax errors")
        {
            Errors = errors ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Errors { get; }
    }
}
