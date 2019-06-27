using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.ObjectModule
{
    public class SelectExpressionResolver<T> : BaseExpressionResolver<T>
    {
        public SelectExpressionResolver() : base()
        {
            this.SQL = string.Join(",", typeof(T).GetProperties().Select(p => $"[{p.ColumnName()}] AS {p.Name}"));
        }

        public LambdaExpression Expression { get; set; }

        public override void Resolve(Expression expression)
        {
            MethodCallExpression methodCall = expression as MethodCallExpression;
            var method = methodCall.Arguments[0];
            var lambda = methodCall.Arguments[1];
            var right = this.Expression = (lambda as UnaryExpression).Operand as LambdaExpression;

            this.SQL = this.GetColumns(right);
            if (string.IsNullOrEmpty(this.SQL))
            {
                this.SQL = string.Join(",", typeof(T).GetProperties().Select(p => $"[{p.ColumnName()}] AS {p.Name}"));
            }
        }

        private string GetColumns(Expression expression)
        {
            var right = expression as LambdaExpression;
            if (right.Body is NewExpression)
            {
                var newExpression = right.Body as NewExpression;
                return string.Join(",", newExpression.Arguments.Select(m => $"[{(m as MemberExpression).Member.ColumnName()}] AS {(m as MemberExpression).Member.Name}"));
            }
            else if (right.Body is MemberInitExpression)
            {
                var memberInitExpression = right.Body as MemberInitExpression;
                if (memberInitExpression != null && memberInitExpression.Bindings != null && memberInitExpression.Bindings.Count > 0)
                {
                    return string.Join(",", memberInitExpression.Bindings.Select(b => $"[{b.Member.ColumnName()}] AS {b.Member.Name}").ToArray());
                }
            }
            else if (right.Body.NodeType == ExpressionType.Parameter)
            {
                return string.Join(",", typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(f => $"[{f.ColumnName()}] AS {f.Name}").ToArray());
            }
            else
            {
                return $"[{(right.Body as MemberExpression).Member.ColumnName()}] AS {(right.Body as MemberExpression).Member.Name}";
            }

            return string.Empty;
        }
    }
}
