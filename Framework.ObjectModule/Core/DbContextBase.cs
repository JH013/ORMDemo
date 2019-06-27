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
                affectLins += this.ExecuteInsert(p);
                affectLins += this.ExecuteDelete(p);
            }

            return affectLins;
        }

        public void Dispose()
        {
        }

        #endregion

        #region 私有方法

        private int ExecuteInsert(PropertyInfo property)
        {
            /// 影响行数
            int affectLins = 0;

            // QuerySet<T>属性值
            var propValue = property.GetValue(this);

            // 真实数据类型_realType字段值
            var typeValue = propValue.GetType().GetField("_realType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(propValue);

            // 临时数据_tempDatas_Insert字段值
            var field = propValue.GetType().GetField("_tempDatas_Insert", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var fieldValue = field.GetValue(propValue);
            if (fieldValue != null)
            {
                // Insert方法调用
                var method = typeof(DbInsertProvider).GetMethod("Insert").MakeGenericMethod(typeValue as Type);
                var ret = method.Invoke(null, BindingFlags.Public, null, new object[] { fieldValue }, null);

                // 影响行数累加
                affectLins += (int)ret;

                // 清空临时数据
                field.FieldType.GetMethod("Clear").Invoke(fieldValue, null);
            }

            return affectLins;
        }

        private int ExecuteDelete(PropertyInfo property)
        {
            /// 影响行数
            int affectLins = 0;

            // QuerySet<T>属性值
            var propValue = property.GetValue(this);

            // 真实数据类型_realType字段值
            var typeValue = propValue.GetType().GetField("_realType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(propValue);

            // 临时数据_tempDatas_Insert字段值
            var field = propValue.GetType().GetField("_tempDatas_Remove", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var fieldValue = field.GetValue(propValue);
            if (fieldValue != null)
            {
                // Delete方法调用
                var method = typeof(DbDeleteProvider).GetMethod("Delete").MakeGenericMethod(typeValue as Type);
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
