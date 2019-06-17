using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class SelectExpressionResolver : BaseExpressionResolver
    {
        public SelectExpressionResolver() : base()
        {
            this.SQL = "*";
        }

        public LambdaExpression Expression { get; set; }

        public override void Resolve(Expression expression)
        {
            MethodCallExpression methodCall = expression as MethodCallExpression;
            var method = methodCall.Arguments[0];
            var lambda = methodCall.Arguments[1];
            var right = this.Expression = (lambda as UnaryExpression).Operand as LambdaExpression;

            this.SQL = base.GetColumns(right);
            if (string.IsNullOrEmpty(this.SQL))
            {
                this.SQL = "*";
            }
        }
    }
}
