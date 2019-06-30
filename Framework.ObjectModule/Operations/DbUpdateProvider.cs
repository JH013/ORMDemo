using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public class DbUpdateProvider
    {
        public static int Execute<T>(List<TempEntry<T>> entries)
        {
            string paramPrefix = "p_";
            foreach (var entry in entries)
            {
                List<string> properties = entry.ModifiedProperties.Any() ? entry.ModifiedProperties : typeof(T).GetProperties().Select(p => p.Name).ToList();
                if (properties == null || !properties.Any())
                {
                    throw new Exception("No update fields.");
                }

                List<string> columnList = new List<string>();
                List<SqlParameter> paramList = new List<SqlParameter>();
                FormatSqlParmeter<T>(entry.Data, entry.ModifiedProperties, ref columnList, ref paramList, paramPrefix);

                // Where表达式解析器
                WhereExpressionResolver<T> whereResolver = new WhereExpressionResolver<T>();
                whereResolver.Resolve(entry.UpdateCondition as LambdaExpression);

                string sql = $"UPDATE {typeof(T).TableName()} SET {string.Join(",", columnList.Select(c => $"{c} = @{paramPrefix}{c}"))} WHERE {whereResolver.SQL}";
                var parameters = paramList.Concat(whereResolver.SqlParameters.ToList());
                return DatabaseOperator.ExecuteNonQuery(DatabaseOperator.connection, sql, parameters.ToArray());
            }

            return 0;
        }

        private static void FormatSqlParmeter<T>(T data, List<string> modified, ref List<string> columns, ref List<SqlParameter> parameters, string paramPrefix)
        {
            Type type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            foreach (var p in properties)
            {
                if (modified.Contains(p.Name))
                {
                    var columnName = p.ColumnName();
                    columns.Add(columnName);

                    SqlParameter parameter = new SqlParameter($"{paramPrefix}{columnName}", p.GetValue(data) ?? DBNull.Value);
                    var dataTypeAttr = p.Attr<DataTypeAttribute>();
                    parameter.SqlDbType = dataTypeAttr != null ? dataTypeAttr.DbType : Util.GetSqlDataType(p);
                    var stringLengthAttr = p.Attr<StringLengthAttribute>();
                    if (stringLengthAttr != null)
                    {
                        parameter.Size = stringLengthAttr.Length;
                    }

                    parameters.Add(parameter);
                }
            }
        }
    }
}
