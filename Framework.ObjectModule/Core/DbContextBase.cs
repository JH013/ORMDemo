using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public class DbContextBase : IDisposable
    {
        #region 构造方法

        public DbContextBase(string db)
        {
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 合并数据
        /// </summary>
        /// <returns>影响行数</returns>
        public int Merge()
        {
            // 影响行数
            int affectLins = 0;
            var properties = this.GetType().GetProperties();
            foreach (var p in properties)
            {
                affectLins += this.Execute<DbInsertProvider>(p, "_tempDatas_Insert");
                affectLins += this.Execute<DbDeleteProvider>(p, "_tempDatas_Remove");
                affectLins += this.Execute<DbUpdateProvider>(p, "_tempDatas_Update");
            }

            return affectLins;
        }

        public void Dispose()
        {
        }

        #endregion

        #region 私有方法

        private int Execute<T>(PropertyInfo property, string tempData)
        {
            /// 影响行数
            int affectLins = 0;

            // QuerySet<T>属性值
            var propValue = property.GetValue(this);

            // 真实数据类型_realType字段值
            var typeValue = propValue.GetType().GetField("_realType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(propValue);

            // 临时数据_tempDatas_Insert字段值
            var field = propValue.GetType().GetField(tempData, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var fieldValue = field.GetValue(propValue);
            if (fieldValue != null)
            {
                // Insert方法调用
                var method = typeof(T).GetMethod("Execute").MakeGenericMethod(typeValue as Type);
                var ret = method.Invoke(null, BindingFlags.Public, null, new object[] { fieldValue }, null);

                // 影响行数累加
                affectLins += (int)ret;

                // 清空临时数据
                field.FieldType.GetMethod("Clear").Invoke(fieldValue, null);
            }

            return affectLins;
        }

        #endregion
    }
}
