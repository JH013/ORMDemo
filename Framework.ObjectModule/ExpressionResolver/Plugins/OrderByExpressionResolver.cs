using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class OrderByExpressionResolver<T> : BaseExpressionResolver<T>
    {
        public OrderByExpressionResolver() : base()
        {
            this._order = string.Empty;
            this._columns = new List<string>();
        }

        private string _order;
        private List<string> _columns;

        public override void Resolve(Expression expression)
        {
            MethodCallExpression methodCall = expression as MethodCallExpression;

            string order = string.Empty;
            string columnName = methodCall.Method.ColumnName();
            if (string.Equals(columnName, "OrderBy") || string.Equals(columnName, "ThenBy"))
            {
                order = "ASC";
            }
            else if (string.Equals(columnName, "OrderByDescending") || string.Equals(columnName, "ThenByDescending"))
            {
                order = "DESC";
            }

            if (string.IsNullOrEmpty(this._order))
            {
                this._order = order;
            }

            if(!string.Equals(this._order, order))
            {
                throw new Exception("Illegal order way.");
            }


            var lambda = methodCall.Arguments[1];
            var right = (lambda as UnaryExpression).Operand as LambdaExpression;
            var member = (right.Body as MemberExpression).Member.ColumnName();
            string column = $"[{member}]";
            if (!this._columns.Contains(column))
            {
                this._columns.Add(column);
            }

            this._columns.Reverse();
            this.SQL = $"ORDER BY {string.Join(",", this._columns)} {order}";
        }
    }
}
