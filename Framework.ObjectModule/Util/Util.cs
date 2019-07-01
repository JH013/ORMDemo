using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public static class Util
    {
        public static bool IsAnonymousType(this Type type)
        {
            Boolean hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            Boolean nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            Boolean isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }

        public static dynamic SetDynamicAttr(dynamic obj, string key, object value)
        {
            var dict = (obj as IDictionary<string, object>);
            if (dict.Keys.Contains(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }

            return obj;
        }

        //public static IEnumerable<T> Select<T>(this IEnumerable source, string selector, params object[] values)
        //{
        //    return Select(source, selector, values).Cast<T>();
        //}
        //public static IEnumerable Select(this IEnumerable source, string selector, params object[] values)
        //{
        //    if (source == null) throw new ArgumentNullException("source");
        //    if (selector == null) throw new ArgumentNullException("selector");
        //    LambdaExpression lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(source.AsQueryable().ElementType, null, selector, values);
        //    return source.AsQueryable().Provider.CreateQuery(
        //        Expression.Call(
        //            typeof(Queryable), "Select",
        //            new Type[] { source.AsQueryable().ElementType, lambda.Body.Type },
        //            source.AsQueryable().Expression, Expression.Quote(lambda)));
        //}

        //public static IDictionary<string, object> AddProperty(this object obj, string name, object value)
        //{
        //    var dictionary = obj.ToDictionary();
        //    dictionary.Add(name, value);
        //    return dictionary;
        //}

        //// helper
        //public static IDictionary<string, object> ToDictionary(this object obj)
        //{
        //    IDictionary<string, object> result = new Dictionary<string, object>();
        //    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
        //    foreach (PropertyDescriptor property in properties)
        //    {
        //        result.Add(property.Name, property.GetValue(obj));
        //    }
        //    return result;
        //}

        public static SqlDbType GetSqlDataType(PropertyInfo property)
        {
            switch (property.PropertyType.FullName)
            {
                case "System.String":
                    return SqlDbType.NChar;
                case "System.Byte":
                    return SqlDbType.TinyInt;
                case "System.Int16":
                    return SqlDbType.SmallInt;
                case "System.Int32":
                    return SqlDbType.Int;
                case "System.Int64":
                    return SqlDbType.BigInt;
                case "System.Single":
                    return SqlDbType.Real;
                case "System.Double":
                    return SqlDbType.Float;
                case "System.Boolean":
                    return SqlDbType.Bit;
                case "System.DateTime":
                    return SqlDbType.DateTime;
                case "System.Guid":
                    return SqlDbType.UniqueIdentifier;
                case "System.Object":
                    return SqlDbType.Variant;
                default:
                    return SqlDbType.NChar;
            }
        }

        public static SqlDbType GetSqlDataType(MemberInfo member)
        {
            switch (member.ReflectedType.FullName)
            {
                case "System.String":
                    return SqlDbType.NChar;
                case "System.Byte":
                    return SqlDbType.TinyInt;
                case "System.Int16":
                    return SqlDbType.SmallInt;
                case "System.Int32":
                    return SqlDbType.Int;
                case "System.Int64":
                    return SqlDbType.BigInt;
                case "System.Single":
                    return SqlDbType.Real;
                case "System.Double":
                    return SqlDbType.Float;
                case "System.Boolean":
                    return SqlDbType.Bit;
                case "System.DateTime":
                    return SqlDbType.DateTime;
                case "System.Guid":
                    return SqlDbType.UniqueIdentifier;
                case "System.Object":
                    return SqlDbType.Variant;
                default:
                    return SqlDbType.NChar;
            }
        }
    }
}
