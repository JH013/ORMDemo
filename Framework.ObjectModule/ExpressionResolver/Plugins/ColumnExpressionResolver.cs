using System.Linq;
using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class ColumnExpressionResolver : BaseExpressionResolver
    {
        public ColumnExpressionResolver() : base()
        {
            this.SQL = string.Empty;
        }

        public override void Resolve(Expression expression)
        {
            var right = expression as LambdaExpression;
            if(right.Body is NewExpression)
            {
                var newExpression = right.Body as NewExpression;
                this.SQL = string.Join(",", newExpression.Arguments.Select(m => $"[{(m as MemberExpression).Member.Name}]"));
            }
            else if(right.Body is MemberInitExpression)
            {
                var memberInitExpression = right.Body as MemberInitExpression;
                if (memberInitExpression != null && memberInitExpression.Bindings != null && memberInitExpression.Bindings.Count > 0)
                {
                    this.SQL = string.Join(",", memberInitExpression.Bindings.Select(b => $"[{b.Member.Name}]").ToArray());
                }
            }
            else
            {
                this.SQL = $"[{(right.Body as MemberExpression).Member.Name}]";
            }
        }
    }
}
