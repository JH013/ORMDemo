using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class QuerySet<T> : IQueryable<T>, IOrderedQueryable<T>, IList<T>
    {

        #region Construction

        public QuerySet()
        {
            this._expression = Expression.Constant(this);
            this._provider = new DbQueryProvider<T>();

            this._tempDatas = new List<T>();
        }

        public QuerySet(Expression expression, IQueryProvider provider)
        {
            this._expression = expression;
            this._provider = provider;

            this._tempDatas = new List<T>();
        }

        #endregion

        #region Private Field

        // Linq Expression
        private Expression _expression;
        private IQueryProvider _provider;

        // 临时数据，用户Insert
        private List<T> _tempDatas;

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

        #region IList

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            this._tempDatas.Add(item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Method

        public void Merge()
        {
            if (this._tempDatas.Any())
            {
                if(this._tempDatas.Count == 1)
                {
                    var data = this._tempDatas.First();

                }
            }
        }

        #endregion
    }
}
