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
            int affectLines = 0;
            foreach (var entry in entries)
            {
                if (entry.UpdateByPrimaryKey)
                {
                    affectLines += UpdateByPrimaryKey(entry);
                }
                else
                {
                    affectLines += UpdateByExpression(entry);
                }
            }

            return affectLines;
        }

        private static int UpdateByPrimaryKey<T>(TempEntry<T> entry)
        {
            var primaryKeyProp = entry.Data.PrimaryKey();
            if (primaryKeyProp == null)
            {
                throw new Exception("No primary key.");
            }

            List<string> properties = typeof(T).GetProperties().Where(p => !string.Equals(p.Name, primaryKeyProp.Name)).Select(p => p.Name).ToList();
            if (properties == null || !properties.Any())
            {
                throw new Exception("No update fields.");
            }

            List<string> columnList = new List<string>();
            List<SqlParameter> paramList = new List<SqlParameter>();
            FormatSqlParmeter<T>(entry.Data, properties, ref columnList, ref paramList, "");
            string primaryKeyColumn = primaryKeyProp.ColumnName();
            string sql = $"UPDATE {typeof(T).TableName()} SET {string.Join(",", columnList.Select(c => $"{c} = @{c}"))} WHERE {primaryKeyColumn} = @{primaryKeyColumn}";
            SqlParameter parameter = new SqlParameter($"{primaryKeyColumn}", primaryKeyProp.GetValue(entry.Data) ?? DBNull.Value);
            var dataTypeAttr = primaryKeyProp.Attr<DataTypeAttribute>();
            parameter.SqlDbType = dataTypeAttr != null ? dataTypeAttr.DbType : Util.GetSqlDataType(primaryKeyProp);
            var stringLengthAttr = primaryKeyProp.Attr<StringLengthAttribute>();
            if (stringLengthAttr != null)
            {
                parameter.Size = stringLengthAttr.Length;
            }

            paramList.Add(parameter);
            return DatabaseOperator.ExecuteNonQuery(DatabaseOperator.connection, sql, paramList.ToArray());
        }

        private static int UpdateByExpression<T>(TempEntry<T> entry)
        {
            string paramPrefix = "p_";
            List<string> properties = entry.ModifiedProperties.Any() ? entry.ModifiedProperties : typeof(T).GetProperties().Select(p => p.Name).ToList();
            if (properties == null || !properties.Any())
            {
                throw new Exception("No update fields.");
            }

            List<string> columnList = new List<string>();
            List<SqlParameter> paramList = new List<SqlParameter>();
            FormatSqlParmeter<T>(entry.Data, properties, ref columnList, ref paramList, paramPrefix);

            // Where表达式解析器
            WhereExpressionResolver<T> whereResolver = new WhereExpressionResolver<T>();
            whereResolver.Resolve(entry.UpdateCondition as LambdaExpression);

            string sql = $"UPDATE {typeof(T).TableName()} SET {string.Join(",", columnList.Select(c => $"{c} = @{paramPrefix}{c}"))} WHERE {whereResolver.SQL}";
            var parameters = paramList.Concat(whereResolver.SqlParameters.ToList());
            return DatabaseOperator.ExecuteNonQuery(DatabaseOperator.connection, sql, parameters.ToArray());
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
