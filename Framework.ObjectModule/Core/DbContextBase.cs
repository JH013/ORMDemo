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
        public DbContextBase(string db)
        {
        }

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
                // QuerySet<T>属性值
                var propValue = p.GetValue(this);

                // 临时数据_tempDatas字段值
                var tempDatasField = propValue.GetType().GetField("_tempDatas", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var tempDatas = tempDatasField.GetValue(propValue);

                // 真实数据类型_realType字段值
                var typeValue = propValue.GetType().GetField("_realType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(propValue);
                if (tempDatas != null)
                {
                    // Insert方法调用
                    var method = typeof(DbInsertProvider).GetMethod("Insert").MakeGenericMethod(typeValue as Type);
                    var ret = method.Invoke(null, BindingFlags.Public, null, new object[] { tempDatas }, null);

                    // 影响行数累加
                    affectLins += (int)ret;

                    // 清空临时数据
                    tempDatasField.FieldType.GetMethod("Clear").Invoke(tempDatas, null);
                }
            }

            return affectLins;
        }

        public void Dispose()
        {
        }
    }
}
