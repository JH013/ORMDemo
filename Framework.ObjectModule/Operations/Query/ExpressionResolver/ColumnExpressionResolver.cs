﻿using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.ObjectModule
{
    public class ColumnExpressionResolver<T> : BaseExpressionResolver<T>
    {
        public ColumnExpressionResolver() : base()
        {
            this.SQL = string.Empty;
        }

        public override void Resolve(Expression expression)
        {
        }

        protected string GetColumns(Expression expression)
        {
            var right = expression as LambdaExpression;
            if (right.Body is NewExpression)
            {
                var newExpression = right.Body as NewExpression;
                return string.Join(",", newExpression.Arguments.Select(m => $"[{(m as MemberExpression).Member.Name}]"));
            }
            else if (right.Body is MemberInitExpression)
            {
                var memberInitExpression = right.Body as MemberInitExpression;
                if (memberInitExpression != null && memberInitExpression.Bindings != null && memberInitExpression.Bindings.Count > 0)
                {
                    return string.Join(",", memberInitExpression.Bindings.Select(b => $"[{b.Member.Name}]").ToArray());
                }
            }
            else if (right.Body.NodeType == ExpressionType.Parameter)
            {
                return string.Join(",", typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(f => $"[{f.Name}]").ToArray());
            }
            else
            {
                return $"[{(right.Body as MemberExpression).Member.Name}]";
            }

            return string.Empty;
        }
    }
}