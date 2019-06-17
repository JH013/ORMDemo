using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class QuerySet<T> : IQueryable<T>, IOrderedQueryable<T>
    {

        #region Construction

        public QuerySet()
        {
            this._expression = Expression.Constant(this);
            this._provider = new SqlProvider<T>();
        }

        public QuerySet(Expression expression, IQueryProvider provider)
        {
            this._expression = expression;
            this._provider = provider;
        }

        #endregion

        #region Private Field

        private Expression _expression;
        private IQueryProvider _provider;

        #endregion

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
    }
}
