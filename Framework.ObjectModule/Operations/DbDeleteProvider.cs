using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public class DbDeleteProvider
    {
        public static int Delete<T>(List<IQueryable<T>> queryables)
        {
            foreach (var queryable in queryables)
            {
                var expression = queryable.Expression;

                // Where表达式解析器
                WhereExpressionResolver<T> whereResolver = new WhereExpressionResolver<T>();
                MethodCallExpression methodCall = expression as MethodCallExpression;
                while (methodCall != null)
                {
                    var method = methodCall.Arguments[0];
                    var lambda = methodCall.Arguments[1];
                    var right = (lambda as UnaryExpression).Operand as LambdaExpression;

                    whereResolver.Resolve(right);

                    methodCall = method as MethodCallExpression;
                }

                string sql = $"DELETE FROM {typeof(T).TableName()} WHERE {whereResolver.SQL}";
                return DatabaseOperator.ExecuteNonQuery(DatabaseOperator.connection, sql, whereResolver.SqlParameters);
            }

            return 0;
        }
    }
}
