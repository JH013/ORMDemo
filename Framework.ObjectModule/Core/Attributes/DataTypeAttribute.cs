using System;
using System.Data;

namespace Framework.ObjectModule
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataTypeAttribute : Attribute
    {
        public DataTypeAttribute(SqlDbType dbType)
        {
            this.DbType = dbType;
        }

        public SqlDbType DbType { get; set; }
    }
}
