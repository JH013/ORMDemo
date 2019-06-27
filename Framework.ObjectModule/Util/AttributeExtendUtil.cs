using System.Linq;
using System.Reflection;

namespace Framework.ObjectModule
{
    public static class AttributeExtendUtil
    {
        public static T Attr<T>(this PropertyInfo propertyInfo)
        {
            var attrObj = propertyInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault();
            if (attrObj != null)
            {
                return (T)attrObj;
            }

            return default(T);
        }

        public static T Attr<T>(this MemberInfo memberInfo)
        {
            var attrObj = memberInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault();
            if (attrObj != null)
            {
                return (T)attrObj;
            }

            return default(T);
        }

        public static string ColumnName(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.Attr<ColumnAttribute>();
            return attr == null ? propertyInfo.Name : attr.ColumnName;
        }

        public static string ColumnName(this MemberInfo memberInfo)
        {
            var attr = memberInfo.Attr<ColumnAttribute>();
            return attr == null ? memberInfo.Name : attr.ColumnName;
        }

        public static string TableName(this MemberInfo memberInfo)
        {
            var attr = memberInfo.Attr<TableAttribute>();
            return attr == null ? memberInfo.Name : attr.TableName;
        }
    }
}
