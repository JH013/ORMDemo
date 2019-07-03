using System;
using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class SkipExpressionResolver<T> : BaseExpressionResolver<T>
    {
        public SkipExpressionResolver()
        {
            this._skip = 0;
        }

        private int _skip;

        public int Skip { get { return this._skip; } }

        public new string SQL
        {
            get
            {
                if (this._skip > 0)
                {
                    return $"OFFSET {this._skip} ROWS";
                }

                return string.Empty;
            }
        }

        public override void Resolve(Expression expression)
        {
            var methodCall = expression as MethodCallExpression;
            var constant = methodCall.Arguments[1] as ConstantExpression;
            this._skip += (int)constant.Value;
        }
    }
}
