using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core
{
    public class Interpreter
    {
        private readonly IParser _parser;

        public Interpreter(IParser parser)
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
