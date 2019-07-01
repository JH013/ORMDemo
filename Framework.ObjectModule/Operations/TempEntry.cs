using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class TempEntry<T>
    {
        #region 构造方法

        public TempEntry()
        {
            this._modifiedProperties = new List<string>();
        }

        #endregion

        #region 私有字段

        private T _data;
        private List<string> _modifiedProperties;
        private Expression<Func<T, bool>> _updateCondition;
        private bool _updateByPrimaryKey;

        #endregion

        #region 公有属性

        public T Data { get { return this._data; } }

        public List<string> ModifiedProperties { get { return this._modifiedProperties; } }

        public Expression<Func<T, bool>> UpdateCondition { get { return this._updateCondition; } }

        public bool UpdateByPrimaryKey { get { return this._updateByPrimaryKey; } }

        #endregion

        public TempEntry<T> Attach(T data, bool byPrimaryKey = false)
        {
            this._data = data;
            this._updateByPrimaryKey = byPrimaryKey;
            return this;
        }

        public TempEntry<T> Modified(Expression<Func<T, object>> predicate)
        {
            string property = string.Empty;
            if (predicate.Body is MemberExpression)
            {
                property = (predicate.Body as MemberExpression).Member.Name;
            }
            else if (predicate.Body is UnaryExpression)
            {
                property = ((predicate.Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            this._modifiedProperties.Add(property);
            return this;
        }

        public void Condition(Expression<Func<T, bool>> predicate)
        {
            this._updateCondition = predicate;
        }
    }
}
