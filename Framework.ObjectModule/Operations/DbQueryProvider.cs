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
        #region 接口实现

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
            MethodCallExpression methodCall = expression as MethodCallExpression;
            if (string.Equals(methodCall.Method.Name, "First", StringComparison.OrdinalIgnoreCase))
            {
                return this.ExecuteFirst<TResult>(methodCall);
            }
            else if (string.Equals(methodCall.Method.Name, "Count", StringComparison.OrdinalIgnoreCase))
            {
                return this.ExecuteCount<TResult>(methodCall);
            }
            else
            {
                return this.ExecuteCoreQuery<TResult>(methodCall);
            }
        }

        #endregion

        #region 私有方法

        private TResult ExecuteFirst<TResult>(MethodCallExpression methodCall)
        {
            List<T> data = null;
            if (methodCall.Arguments.Count == 2)
            {
                var lambda = (methodCall.Arguments[1] as UnaryExpression).Operand as LambdaExpression;
                SelectExpressionResolver<T> selectResolver = new SelectExpressionResolver<T>();
                WhereExpressionResolver<T> whereResolver = new WhereExpressionResolver<T>();
                whereResolver.Resolve(lambda);

                if (methodCall.Arguments[0] is MethodCallExpression)
                {
                    ExpressionResolverManager<T> manager = new ExpressionResolverManager<T>(methodCall.Arguments[0] as MethodCallExpression).Resolve();
                    var sql = $"SELECT TOP 1 {selectResolver.SQL} FROM {typeof(T).TableName()} WHERE {$"{whereResolver.SQL} AND {manager.WhereResolver.SQL}"} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL}";
                    var parameters = whereResolver.SqlParameters.Concat(manager.WhereResolver.SqlParameters);
                    data = DatabaseOperator.ExecuteReader<T>(DatabaseOperator.connection, sql, parameters.ToArray());
                }
                else if (methodCall.Arguments[0] is ConstantExpression)
                {
                    var sql = $"SELECT TOP 1 {selectResolver.SQL} FROM {typeof(T).TableName()} WHERE {whereResolver.SQL}";
                    data = DatabaseOperator.ExecuteReader<T>(DatabaseOperator.connection, sql, whereResolver.SqlParameters);
                }
            }
            else if (methodCall.Arguments.Count == 1)
            {
                var manager = new ExpressionResolverManager<T>(methodCall.Arguments[0] as MethodCallExpression).Resolve();
                var sql = $"SELECT TOP 1 {manager.SelectResolver.SQL} FROM {typeof(T).TableName()} WHERE {manager.WhereResolver.SQL} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL}";
                data = DatabaseOperator.ExecuteReader<T>(DatabaseOperator.connection, sql, manager.WhereResolver.SqlParameters);
            }

            var ret = Convert.ChangeType((dynamic)data[0], typeof(T));
            return (TResult)(object)ret;
        }

        private TResult ExecuteCount<TResult>(MethodCallExpression methodCall)
        {
            int count = 0;
            if (methodCall.Arguments.Count == 2)
            {
                var lambda = (methodCall.Arguments[1] as UnaryExpression).Operand as LambdaExpression;
                WhereExpressionResolver<T> whereResolver = new WhereExpressionResolver<T>();
                whereResolver.Resolve(lambda);

                if (methodCall.Arguments[0] is MethodCallExpression)
                {
                    ExpressionResolverManager<T> manager = new ExpressionResolverManager<T>(methodCall.Arguments[0] as MethodCallExpression).Resolve();
                    var sql = $"SELECT Count(*) FROM {typeof(T).TableName()} WHERE {$"{whereResolver.SQL} AND {manager.WhereResolver.SQL}"} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL}";
                    var parameters = whereResolver.SqlParameters.Concat(manager.WhereResolver.SqlParameters);
                    count = (int)DatabaseOperator.ExecuteScalar(DatabaseOperator.connection, sql, parameters.ToArray());
                }
                else if (methodCall.Arguments[0] is ConstantExpression)
                {
                    var sql = $"SELECT Count(*) FROM {typeof(T).TableName()} WHERE {whereResolver.SQL}";
                    count = (int)DatabaseOperator.ExecuteScalar(DatabaseOperator.connection, sql, whereResolver.SqlParameters);
                }
            }
            else if (methodCall.Arguments.Count == 1)
            {
                var manager = new ExpressionResolverManager<T>(methodCall.Arguments[0] as MethodCallExpression).Resolve();
                var sql = $"SELECT Count(*) FROM {typeof(T).TableName()} WHERE {manager.WhereResolver.SQL} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL}";
                count = (int)DatabaseOperator.ExecuteScalar(DatabaseOperator.connection, sql, manager.WhereResolver.SqlParameters);
            }

            return (TResult)(object)count;
        }

        private TResult ExecuteCoreQuery<TResult>(MethodCallExpression methodCall)
        {
            var manager = new ExpressionResolverManager<T>(methodCall).Resolve();
            string sql = $"SELECT {manager.SelectResolver.SQL} FROM {typeof(T).TableName()} WHERE {manager.WhereResolver.SQL} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL}";
            var data = DatabaseOperator.ExecuteReader<T>(DatabaseOperator.connection, sql, manager.WhereResolver.SqlParameters);

            // 处理Select语句返回匿名对象
            if (manager.SelectResolver.Expression != null)
            {
                var selctQuery = data.AsQueryable().Provider.CreateQuery(
                    Expression.Call(typeof(Queryable),
                    "Select",
                    new Type[] {
                        data.AsQueryable().ElementType,
                        manager.SelectResolver.Expression.Body.Type },
                        data.AsQueryable().Expression,
                        Expression.Quote(manager.SelectResolver.Expression)));


                List<object> list = new List<object>();
                foreach (var item in selctQuery)
                {
                    list.Add(Convert.ChangeType(item, manager.SelectResolver.Expression.ReturnType));
                }

                return (TResult)(object)list;
            }

            return (TResult)(object)data;
        }

        #endregion
    }
}
