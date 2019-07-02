using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class ExpressionResolverManager<T>
    {
        public ExpressionResolverManager(MethodCallExpression methodCall)
        {
            this._methodCall = methodCall;
            this._selectResolver = new SelectExpressionResolver<T>();
            this._orderByResolver = new OrderByExpressionResolver<T>();
            this._groupByResolver = new GroupByExpressionResolver<T>();
            this._whereResolver = new WhereExpressionResolver<T>();
        }
        private MethodCallExpression _methodCall;
        private SelectExpressionResolver<T> _selectResolver;
        private OrderByExpressionResolver<T> _orderByResolver;
        private GroupByExpressionResolver<T> _groupByResolver;
        private WhereExpressionResolver<T> _whereResolver;

        public SelectExpressionResolver<T> SelectResolver { get { return this._selectResolver; } }
        public OrderByExpressionResolver<T> OrderByResolver { get { return this._orderByResolver; } }
        public GroupByExpressionResolver<T> GroupByResolver { get { return this._groupByResolver; } }
        public WhereExpressionResolver<T> WhereResolver { get { return this._whereResolver; } }

        public ExpressionResolverManager<T> Resolve()
        {
            while (this._methodCall != null)
            {
                var method = this._methodCall.Arguments[0];
                var lambda = this._methodCall.Arguments[1];
                var right = (lambda as UnaryExpression).Operand as LambdaExpression;

                switch (this._methodCall.Method.Name)
                {
                    case "Select":
                        this._selectResolver.Resolve(this._methodCall);
                        break;
                    case "GroupBy":
                        this._groupByResolver.Resolve(right);
                        break;
                    case "OrderBy":
                    case "ThenBy":
                    case "OrderByDescending":
                    case "ThenByDescending":
                        this._orderByResolver.Resolve(this._methodCall);
                        break;
                    case "Where":
                        this._whereResolver.Resolve(right);
                        break;
                }

                this._methodCall = method as MethodCallExpression;
            }

            return this;
        }
    }
}
