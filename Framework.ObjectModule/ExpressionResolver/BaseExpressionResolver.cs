using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.ObjectModule
{
    public abstract class BaseExpressionResolver
    {
        public BaseExpressionResolver()
        {
            this.SQL = string.Empty;
        }

        public string SQL;

        protected string GetColumns(Expression expression)
        {
            var right = expression as LambdaExpression;
            if (right.Body is NewExpression)
            {
                var newExpression = right.Body as NewExpression;
                return string.Join(",", newExpression.Arguments.Select(m => $"[{(m as MemberExpression).Member.Name}]"));
            }
            else if (right.Body is MemberInitExpression)
            {
                var memberInitExpression = right.Body as MemberInitExpression;
                if (memberInitExpression != null && memberInitExpression.Bindings != null && memberInitExpression.Bindings.Count > 0)
                {
                    return string.Join(",", memberInitExpression.Bindings.Select(b => $"[{b.Member.Name}]").ToArray());
                }
            }
            else if (right.Body.NodeType == ExpressionType.Parameter)
            {
                return string.Join(",", right.Body.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(f => $"[{f.Name}]").ToArray());
            }
            else
            {
                return $"[{(right.Body as MemberExpression).Member.Name}]";
            }

            return string.Empty;
        }

        public abstract void Resolve(Expression expression);
    }
}
