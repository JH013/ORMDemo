using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.ObjectModule
{
    public class GroupByExpressionResolver<T> : BaseExpressionResolver<T>
    {
        public GroupByExpressionResolver() : base()
        {
        }


        public override void Resolve(Expression expression)
        {
            this.SQL = this.GetColumns(expression);
            if (!string.IsNullOrEmpty(this.SQL))
            {
                this.SQL = $"GROUP BY {this.SQL}";
            }
        }

        private string GetColumns(Expression expression)
        {
            var right = expression as LambdaExpression;
            if (right.Body is NewExpression)
            {
                var newExpression = right.Body as NewExpression;
                return string.Join(",", newExpression.Arguments.Select(m => $"[{(m as MemberExpression).Member.ColumnName()}]"));
            }
            else if (right.Body is MemberInitExpression)
            {
                var memberInitExpression = right.Body as MemberInitExpression;
                if (memberInitExpression != null && memberInitExpression.Bindings != null && memberInitExpression.Bindings.Count > 0)
                {
                    return string.Join(",", memberInitExpression.Bindings.Select(b => $"[{b.Member.ColumnName()}]").ToArray());
                }
            }
            else if (right.Body.NodeType == ExpressionType.Parameter)
            {
                return string.Join(",", typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(f => $"[{f.ColumnName()}]").ToArray());
            }
            else
            {
                return $"[{(right.Body as MemberExpression).Member.ColumnName()}]";
            }

            return string.Empty;
        }
    }
}
