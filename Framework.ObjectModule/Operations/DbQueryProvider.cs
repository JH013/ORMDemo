using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public class DbQueryProvider<T> : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            IQueryable<TElement> query = new QuerySet<TElement>(expression, this);
            return query;
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            // Select表达式解析器
            SelectExpressionResolver<T> selectResolver = new SelectExpressionResolver<T>();

            // OrderBy表达式解析器
            OrderByExpressionResolver<T> orderByResolver = new OrderByExpressionResolver<T>();

            // GroupBy表达式解析器
            GroupByExpressionResolver<T> groupByResolver = new GroupByExpressionResolver<T>();

            // Where表达式解析器
            WhereExpressionResolver<T> whereResolver = new WhereExpressionResolver<T>();

            MethodCallExpression methodCall = expression as MethodCallExpression;
            while (methodCall != null)
            {
                var method = methodCall.Arguments[0];
                var lambda = methodCall.Arguments[1];
                var right = (lambda as UnaryExpression).Operand as LambdaExpression;

                switch (methodCall.Method.Name)
                {
                    case "Select":
                        selectResolver.Resolve(methodCall);
                        break;
                    case "GroupBy":
                        groupByResolver.Resolve(right);
                        break;
                    case "OrderBy":
                    case "ThenBy":
                    case "OrderByDescending":
                    case "ThenByDescending":
                        orderByResolver.Resolve(methodCall);
                        break;
                    case "Where":
                        whereResolver.Resolve(right);
                        break;

                }

                methodCall = method as MethodCallExpression;
            }

            string sql = $"SELECT {selectResolver.SQL} FROM {typeof(T).TableName()} WHERE {whereResolver.SQL} {groupByResolver.SQL} {orderByResolver.SQL}";
            var data = DatabaseOperator.ExecuteReader<T>(DatabaseOperator.connection, sql, whereResolver.SqlParameters);

            // 处理Select语句返回匿名对象
            if (selectResolver.Expression != null)
            {
                var selctQuery = data.AsQueryable().Provider.CreateQuery(
                    Expression.Call(typeof(Queryable),
                    "Select",
                    new Type[] {
                        data.AsQueryable().ElementType,
                        selectResolver.Expression.Body.Type },
                        data.AsQueryable().Expression,
                        Expression.Quote(selectResolver.Expression)));


                List<object> list = new List<object>();
                foreach (var item in selctQuery)
                {
                    list.Add(Convert.ChangeType(item, selectResolver.Expression.ReturnType));
                }

                return (TResult)(object)list;
            }

            return (TResult)(object)data;
        }
    }
}
