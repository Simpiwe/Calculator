using System.Linq.Expressions;

namespace Calculator.Core
{
    public class Evaluator
    {
        private readonly IParser _parser;

        public Evaluator(IParser parser)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public T? Evaluate<T>(string expression)
        {
            Expression parsedExpression = _parser.Parse(expression);

            return (T?)(parsedExpression is ConstantExpression e
                ? e.Value
                : Expression.Lambda(parsedExpression).Compile().DynamicInvoke());
        }
    }
}
