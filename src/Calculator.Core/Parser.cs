using System.Linq.Expressions;

namespace Calculator.Core
{
    public sealed partial class Parser : IParser
    {
        private readonly ITokenizer _tokenizer;

        public Parser(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        }

        public Expression Parse(string expression)
        {
            ParserImpl impl = new ParserImpl(_tokenizer, expression);

            return impl.Parse();
        }
    }
}
