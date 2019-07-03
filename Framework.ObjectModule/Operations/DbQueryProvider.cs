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
            if (methodCall == null && expression is ConstantExpression)
            {
                return this.ExecuteNoneFunc<TResult>();
            }

            switch (methodCall.Method.Name)
            {
                case "First":
                    return this.ExecuteFirstFunc<TResult>(methodCall);
                case "Count":
                    return this.ExecuteCountFunc<TResult>(methodCall);
                case "Any":
                    var count = this.ExecuteCountFunc<int>(methodCall);
                    return (TResult)(object)(count > 0);
                case "All":
                    return this.ExecuteAllFunc<TResult>(methodCall);
                default:
                    return this.ExecuteCoreQuery<TResult>(methodCall);
            }
        }

        #endregion

        #region 私有方法

        private TResult ExecuteNoneFunc<TResult>()
        {
            SelectExpressionResolver<T> selectResolver = new SelectExpressionResolver<T>();
            string sql = $"SELECT {selectResolver.SQL} FROM {typeof(T).TableName()}";
            var data = DatabaseOperator.ExecuteReader<T>(DatabaseOperator.connection, sql);
            return (TResult)(object)data;
        }

        private TResult ExecuteFirstFunc<TResult>(MethodCallExpression methodCall)
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

        private TResult ExecuteCountFunc<TResult>(MethodCallExpression methodCall)
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

        private TResult ExecuteAllFunc<TResult>(MethodCallExpression methodCall)
        {
            int countOrigin = 0;
            int countAll = 0;
            var lambda = (methodCall.Arguments[1] as UnaryExpression).Operand as LambdaExpression;
            WhereExpressionResolver<T> whereResolver = new WhereExpressionResolver<T>();
            whereResolver.Resolve(lambda);

            if (methodCall.Arguments[0] is MethodCallExpression)
            {
                ExpressionResolverManager<T> manager = new ExpressionResolverManager<T>(methodCall.Arguments[0] as MethodCallExpression).Resolve();
                var sqlAll = $"SELECT Count(*) FROM {typeof(T).TableName()} WHERE {$"{whereResolver.SQL} AND {manager.WhereResolver.SQL}"} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL}";
                var parameters = whereResolver.SqlParameters.Concat(manager.WhereResolver.SqlParameters);
                countAll = (int)DatabaseOperator.ExecuteScalar(DatabaseOperator.connection, sqlAll, parameters.ToArray());

                var sqlOrigin = $"SELECT Count(*) FROM {typeof(T).TableName()} WHERE {manager.WhereResolver.SQL} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL}";
                countOrigin = (int)DatabaseOperator.ExecuteScalar(DatabaseOperator.connection, sqlOrigin, manager.WhereResolver.SqlParameters);
            }
            else if (methodCall.Arguments[0] is ConstantExpression)
            {
                var sqlAll = $"SELECT Count(*) FROM {typeof(T).TableName()} WHERE {whereResolver.SQL}";
                countAll = (int)DatabaseOperator.ExecuteScalar(DatabaseOperator.connection, sqlAll, whereResolver.SqlParameters);

                var sqlOrigin = $"SELECT Count(*) FROM {typeof(T).TableName()}";
                countOrigin = (int)DatabaseOperator.ExecuteScalar(DatabaseOperator.connection, sqlOrigin, whereResolver.SqlParameters);
            }

            return (TResult)(object)(countAll == countOrigin);
        }

        private TResult ExecuteCoreQuery<TResult>(MethodCallExpression methodCall)
        {
            string sql = string.Empty;
            var manager = new ExpressionResolverManager<T>(methodCall).Resolve();
            if (manager.SkipResolver.Skip > 0 && manager.TakeResolver.Take > 0)
            {
                sql = $"SELECT {manager.SelectResolver.SQL} FROM {typeof(T).TableName()} WHERE {manager.WhereResolver.SQL} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL} OFFSET {manager.SkipResolver.Skip} ROWS FETCH NEXT {manager.TakeResolver.Take} ROWS ONLY";
            }
            else if (manager.SkipResolver.Skip > 0 && manager.TakeResolver.Take == 0)
            {
                sql = $"SELECT {manager.SelectResolver.SQL} FROM {typeof(T).TableName()} WHERE {manager.WhereResolver.SQL} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL} {manager.SkipResolver.SQL}";
            }
            else if (manager.SkipResolver.Skip == 0 && manager.TakeResolver.Take > 0)
            {
                sql = $"SELECT {manager.TakeResolver.SQL} {manager.SelectResolver.SQL} FROM {typeof(T).TableName()} WHERE {manager.WhereResolver.SQL} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL}";
            }
            else
            {
                sql = $"SELECT {manager.SelectResolver.SQL} FROM {typeof(T).TableName()} WHERE {manager.WhereResolver.SQL} {manager.GroupByResolver.SQL} {manager.OrderByResolver.SQL}";
            }

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
