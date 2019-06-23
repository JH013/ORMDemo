using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public class DatabaseOperator
    {
        public static string connection = @"Data Source=.; Initial Catalog=TestEF; User Id=sa; Password=1qaz2wsxE;";

        public static int ExecuteNonQuery(string connectionStr, string sqlText, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sqlText, conn))
                {
                    command.Parameters.AddRange(parameters);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static List<T> ExecuteReader<T>(string connectionStr, string sqlText)
        {
            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sqlText, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return DataReaderToList<T>(reader);
                    }
                }
            }
        }

        public static List<T> ExecuteReader<T>(string connectionStr, string sqlText, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionStr))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sqlText, conn))
                {
                    command.Parameters.AddRange(parameters);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return DataReaderToList<T>(reader);
                    }
                }
            }
        }

        private static List<T> DataReaderToList<T>(SqlDataReader rdr)
        {
            List<T> list = new List<T>();

            while (rdr.Read())
            {
                T t = System.Activator.CreateInstance<T>();
                Type obj = t.GetType();
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    object tempValue = null;

                    if (rdr.IsDBNull(i))
                    {

                        string typeFullName = obj.GetProperty(rdr.GetName(i)).PropertyType.FullName;
                        tempValue = GetDBNullValue(typeFullName);

                    }
                    else
                    {
                        tempValue = rdr.GetValue(i);

                    }

                    obj.GetProperty(rdr.GetName(i)).SetValue(t, tempValue, null);

                }

                list.Add(t);

            }
            return list;
        }

        private static object GetDBNullValue(string typeFullName)
        {

            typeFullName = typeFullName.ToLower();

            if (typeFullName == "System.String")
            {
                return String.Empty;
            }
            if (typeFullName == "System.Int32")
            {
                return 0;
            }
            //if (typeFullName == "System.DateTime")
            //{
            //    return Convert.ToDateTime(BaseSet.DateTimeLongNull);
            //}
            if (typeFullName == "System.Boolean")
            {
                return false;
            }
            //if (typeFullName == DataType.Int)
            //{
            //    return 0;
            //}

            return null;
        }
    }
}
