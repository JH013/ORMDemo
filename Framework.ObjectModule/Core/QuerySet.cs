using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public partial class QuerySet<T> : IQueryable<T>, IOrderedQueryable<T>
    {

        #region 构造方法

        public QuerySet()
        {
            this._expression = Expression.Constant(this);
            this._provider = new DbQueryProvider<T>();

            this._tempDatas_Insert = new List<T>();
            this._tempDatas_Remove = new List<IQueryable<T>>();
            this._tempDatas_Update = new List<TempEntry<T>>();
            this._realType = typeof(T);
        }

        public QuerySet(Expression expression, IQueryProvider provider)
        {
            this._expression = expression;
            this._provider = provider;

            this._tempDatas_Insert = new List<T>();
            this._tempDatas_Remove = new List<IQueryable<T>>();
            this._tempDatas_Update = new List<TempEntry<T>>();
            this._realType = typeof(T);
        }

        #endregion

        #region 私有字段

        // Linq Expression
        private Expression _expression;
        private IQueryProvider _provider;

        // 临时数据，插入操作使用
        private List<T> _tempDatas_Insert;

        // 临时数据，删除操作使用
        private List<IQueryable<T>> _tempDatas_Remove;

        private List<TempEntry<T>> _tempDatas_Update;

        // 真实数据类型，插入、删除操作使用
        private Type _realType;
        #endregion

        #region IQueryable, IOrderedQueryable  

        public Expression Expression => this._expression;

        public Type ElementType => typeof(QuerySet<T>);

        public IQueryProvider Provider => this._provider;

        public IEnumerator<T> GetEnumerator()
        {
            var result = _provider.Execute<object>(_expression);
            if (result == null)
                yield break;

            var realObj = result as List<object>;
            if (realObj != null)
            {
                foreach (var item in realObj)
                {
                    yield return (T)item;
                }
            }
            else
            {
                var realT = result as List<T>;
                foreach (var item in realT)
                {
                    yield return (T)item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 插入

        public void Add(T item)
        {
            this._tempDatas_Insert.Add(item);
        }

        public void AddRange(List<T> items)
        {
            this._tempDatas_Insert.AddRange(items);
        }

        #endregion

        #region 删除

        //public void Remove(T item)
        //{
        //    this._tempDatas_Remove.Add(item);
        //}

        public void Remove(IQueryable<T> item)
        {
            this._tempDatas_Remove.Add(item);
        }

        #endregion

        #region 更新

        public TempEntry<T> UpdateEntry(T item)
        {
            var en = new TempEntry<T>();
            en.Attach(item);
            this._tempDatas_Update.Add(en);
            return en;
        }

        public void UpdateByPrimary(T item)
        {
            var en = new TempEntry<T>();
            en.Attach(item, true);
            this._tempDatas_Update.Add(en);
        }

        #endregion
    }
}
