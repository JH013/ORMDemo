using System;
using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class TakeExpressionResolver<T> : BaseExpressionResolver<T>
    {
        public TakeExpressionResolver()
        {
            this._take = 0;
        }

        private int _take;

        public int Take { get { return this._take; } }

        public new string SQL
        {
            get
            {
                if (this._take >= 0)
                {
                    return $"TOP({this._take})";
                }

                return string.Empty;
            }
        }

        public override void Resolve(Expression expression)
        {
            var methodCall = expression as MethodCallExpression;
            var constant = methodCall.Arguments[1] as ConstantExpression;
            this._take = (int)constant.Value;
        }
    }
}
