using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.ObjectModule
{
    public abstract class BaseExpressionResolver<T>
    {
        public BaseExpressionResolver()
        {
            this.SQL = string.Empty;
        }

        public string SQL;

        public abstract void Resolve(Expression expression);
    }
}
