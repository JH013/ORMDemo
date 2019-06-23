using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ObjectModule
{
    public class WhereExpressionResolver<T> : BaseExpressionResolver<T>
    {
        public WhereExpressionResolver() : base()
        {
            this._paramDic = new Dictionary<string, object>();
            this._suffix = 0;
            this._paramArray = this._paramDic.Select(p => new SqlParameter(p.Key, p.Value)).ToArray();
        }

        private const string FN_TEMPLATE = "({0} {1} {2})";

        protected Expression _expression;
        private Dictionary<string, object> _paramDic;
        private int _suffix;
        private SqlParameter[] _paramArray;

        public new string SQL
        {
            get
            {
                return this.GetSQL(this._expression);
            }
        }

        public SqlParameter[] SqlParameters
        {
            get
            {
                if (!this._paramArray.Any())
                {
                    this._paramArray = this._paramDic.Select(p => new SqlParameter(p.Key, p.Value)).ToArray();
                }

                return this._paramArray;
            }
        }

        public override void Resolve(Expression expression)
        {
            var right = expression as LambdaExpression;
            if (this._expression == null)
            {
                this._expression = Expression.Lambda<Func<T, bool>>(right.Body, right.Parameters);
            }
            else
            {

                Expression tempEx = Expression.And(right.Body, (this._expression as LambdaExpression).Body);
                this._expression = Expression.Lambda<Func<T, bool>>(tempEx, (this._expression as LambdaExpression).Parameters);
            }
        }

        #region Private

        private string GetSQL(Expression expression)
        {
            if (expression == null)
            {
                return "1 = 1";
            }

            if (expression is LambdaExpression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                expression = lambda.Body;
                return this.GetSQL(expression);
            }

            if (expression is BinaryExpression)
            {
                BinaryExpression binary = expression as BinaryExpression;
                if (binary.Left is MemberExpression)
                {
                    return this.ResolveToFunc(binary);
                }
            }

            var body = expression as BinaryExpression;
            var left = this.GetSQL(body.Left);
            var right = this.GetSQL(body.Right);
            var oper = GetOperator(body.NodeType);

            return string.Format(FN_TEMPLATE, left, oper, right);
        }

        private object GetValue(Expression expression)
        {
            if (expression is ConstantExpression)
            {
                return (expression as ConstantExpression).Value;
            }

            if (expression is UnaryExpression)
            {
                UnaryExpression unary = expression as UnaryExpression;
                LambdaExpression lambda = Expression.Lambda(unary.Operand);
                Delegate fn = lambda.Compile();
                return fn.DynamicInvoke(null);
            }

            if (expression is MemberExpression)
            {
                MemberExpression member = expression as MemberExpression;
                string name = member.Member.Name;
                var constant = member.Expression as ConstantExpression;
                if (constant == null)
                    throw new Exception("Error occured while resolve expression value." + member);
                return constant.Value.GetType().GetFields().First(x => x.Name == name).GetValue(constant.Value);
            }

            throw new Exception("The expression value can not be resolved." + expression);
        }

        private string ResolveToFunc(BinaryExpression expression)
        {
            string key = (expression.Left as MemberExpression).Member.Name;
            string oper = this.GetOperator(expression.NodeType);
            object value = this.GetValue(expression.Right);
            var param = this.GetParamName(key, value);
            string result = string.Format(FN_TEMPLATE, key, oper, param);
            return result;
        }

        private string GetParamName(string param, object value)
        {
            param = "@" + param;

            while (this._paramDic.ContainsKey(param))
            {
                param = param + this._suffix;
                this._suffix++;
            }

            this._paramDic[param] = value;

            return param;
        }

        private string GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.And:
                    return "and";
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.Or:
                    return "or";
                case ExpressionType.OrElse:
                    return "or";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                default:
                    throw new Exception(string.Format("Not support {0} operator." + expressiontype));
            }
        }

        #endregion
    }
}
