using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public class DbInsertProvider
    {
        //public static int Insert<T>(T data)
        //{
        //    Type type = typeof(T);
        //    var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
        //    foreach(var p in properties)
        //    {
        //        p.GetValue(data);
        //    }



        //    EntityBase entity = EntityManager.GetEntity(condition.ConnectionName, condition.TableName);
        //    if (entity == null)
        //    {
        //        throw new Exception("Invalid connection or table.");
        //    }
        //    List<string> attrList = new List<string>();
        //    List<string> paramList = new List<string>();
        //    foreach (var attr in entity.Attributes.Keys)
        //    {
        //        attrList.Add(attr);
        //        paramList.Add(string.Format("@{0}", attr));
        //    }
        //    string sqlTemplate = "INSERT INTO {0}({1})VALUES({2})";
        //    string sqlText = string.Format(sqlTemplate, type.Name, string.Join(",", attrList), string.Join(",", paramList));
        //    List<SqlParameter> paras = new List<SqlParameter> { };

        //    foreach (var attr in entity.Attributes.Keys)
        //    {
        //        paras.Add(this.Convert<T>(entity.Attributes[attr], data));
        //    }

        //    return DatabaseOperator.ExecuteNonQuery(DatabaseOperator.connection, sqlText, paras.ToArray());
        //}

        //public int Insert<T>(SqlCondition condition, List<T> datas)
        //{
        //    DataTable dt = new DataTable();
        //    EntityBase entity = EntityManager.GetEntity(condition.ConnectionName, condition.TableName);
        //    if (entity == null)
        //    {
        //        throw new Exception("Invalid connection or table.");
        //    }
        //    Hashtable mappings = new Hashtable();
        //    foreach (var attr in entity.Attributes.Keys)
        //    {
        //        dt.Columns.Add(attr, typeof(T).GetProperty(attr).PropertyType);
        //        mappings.Add(attr, attr);
        //    }
        //    foreach (var data in datas)
        //    {
        //        DataRow dr = dt.NewRow();
        //        foreach (var attr in entity.Attributes.Keys)
        //        {
        //            dr[attr] = data.GetType().GetProperty(attr).GetValue(data);
        //        }
        //        dt.Rows.Add(dr);
        //    }
        //    string connStr = SqlServerConnectionProvider.Instance.GetConnectionString(condition.ConnectionName);
        //    return SqlServerBaseOrder.ExecuteBulkCopy(connStr, condition.TableName, dt, mappings);
        //}
    }
}
