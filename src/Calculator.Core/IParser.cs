using System.Linq.Expressions;

namespace Calculator.Core
{
    public interface IParser
    {
        Expression Parse(string expression);
    }
}
