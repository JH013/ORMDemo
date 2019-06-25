using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public class DbInsertProvider
    {
        public static int Insert<T>(List<T> datas)
        {
            if (datas.Count == 1)
            {
                return SingleInsert(datas.First());
            }

            return BatchInsert(datas);
        }

        private static int SingleInsert<T>(T data)
        {
            Type type = typeof(T);
            string tableName = type.Name;// 表名（默认取类名）
            var tableAttr = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
            if (tableAttr != null)
            {
                tableName = (tableAttr as TableAttribute).TableName;
            }

            List<string> columnList = new List<string>();// 列名列表            
            List<SqlParameter> paramList = new List<SqlParameter>();// 参数列表
            FormatSqlParmeter<T>(data, ref columnList, ref paramList);
            string sqlText = $"INSERT INTO {tableName}({string.Join(",", columnList.Select(c => $"[{c}]"))})VALUES({string.Join(",", columnList.Select(c => $"@{c}"))})";
            return DatabaseOperator.ExecuteNonQuery(DatabaseOperator.connection, sqlText, paramList.ToArray());
        }

        private static int BatchInsert<T>(List<T> datas)
        {
            DataTable dt = new DataTable();

            Type type = typeof(T);
            string tableName = type.Name;// 表名（默认取类名）
            var tableAttr = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
            if (tableAttr != null)
            {
                tableName = (tableAttr as TableAttribute).TableName;
            }

            var tempMap = new Dictionary<string, string>();

            Hashtable mappings = new Hashtable();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            foreach (var p in properties)
            {
                var columnAttr = p.Attr<ColumnAttribute>();
                var columnName = columnAttr == null ? p.Name : columnAttr.ColumnName;
                mappings.Add(columnName, columnName);
                dt.Columns.Add(columnName, p.PropertyType);
                tempMap.Add(p.Name, columnName);
            }

            foreach (var data in datas)
            {
                DataRow dr = dt.NewRow();
                foreach (var key in tempMap.Keys)
                {
                    dr[tempMap[key]] = data.GetType().GetProperty(key).GetValue(data);
                }

                dt.Rows.Add(dr);
            }
            return DatabaseOperator.ExecuteBulkCopy(DatabaseOperator.connection, tableName, dt, mappings);
        }

        private static void FormatSqlParmeter<T>(T data, ref List<string> columns, ref List<SqlParameter> parameters)
        {
            Type type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            foreach (var p in properties)
            {
                var columnAttr = p.Attr<ColumnAttribute>();
                var columnName = columnAttr == null ? p.Name : columnAttr.ColumnName;
                columns.Add(columnName);

                SqlParameter parameter = new SqlParameter(columnName, p.GetValue(data) ?? DBNull.Value);
                var dataTypeAttr = p.Attr<DataTypeAttribute>();
                if (dataTypeAttr != null)
                {
                    parameter.SqlDbType = dataTypeAttr.DbType;
                }
                else
                {
                    switch (p.PropertyType.FullName)
                    {
                        case "System.String":
                            parameter.SqlDbType = SqlDbType.NChar;
                            break;
                        case "System.Byte":
                            parameter.SqlDbType = SqlDbType.TinyInt;
                            break;
                        case "System.Int16":
                            parameter.SqlDbType = SqlDbType.SmallInt;
                            break;
                        case "System.Int32":
                            parameter.SqlDbType = SqlDbType.Int;
                            break;
                        case "System.Int64":
                            parameter.SqlDbType = SqlDbType.BigInt;
                            break;
                        case "System.Single":
                            parameter.SqlDbType = SqlDbType.Real;
                            break;
                        case "System.Double":
                            parameter.SqlDbType = SqlDbType.Float;
                            break;
                        case "System.Boolean":
                            parameter.SqlDbType = SqlDbType.Bit;
                            break;
                        case "System.DateTime":
                            parameter.SqlDbType = SqlDbType.DateTime;
                            break;
                        case "System.Guid":
                            parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                            break;
                        case "System.Object":
                            parameter.SqlDbType = SqlDbType.Variant;
                            break;
                    }
                }

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
