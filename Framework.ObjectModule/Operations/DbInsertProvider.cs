using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Framework.ObjectModule
{
    public class DbInsertProvider
    {
        public static int Execute<T>(List<T> datas)
        {
            if (datas.Count == 1)
            {
                return SingleInsert(datas.First());
            }

            return BatchInsert(datas);
        }

        private static int SingleInsert<T>(T data)
        {
            string tableName = typeof(T).TableName();// 表名（默认取类名）
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
            string tableName = type.TableName();// 表名（默认取类名）

            var tempMap = new Dictionary<string, string>();

            Hashtable mappings = new Hashtable();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            foreach (var p in properties)
            {
                var columnName = p.ColumnName();
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
                var columnName = p.ColumnName();
                columns.Add(columnName);

                SqlParameter parameter = new SqlParameter(columnName, p.GetValue(data) ?? DBNull.Value);
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
