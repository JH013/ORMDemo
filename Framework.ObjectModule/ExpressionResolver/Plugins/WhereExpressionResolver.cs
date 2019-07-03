using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.ObjectModule
{
    public class WhereExpressionResolver<T> : BaseExpressionResolver<T>
    {
        public WhereExpressionResolver() : base()
        {
            this._suffix = 0;
            this._paramList = new List<SqlParameter>();
            this._sql = string.Empty;
        }

        protected Expression _expression;
        private int _suffix;
        private List<SqlParameter> _paramList;
        private string _sql;

        public new string SQL
        {
            get
            {
                if (string.IsNullOrEmpty(this._sql))
                {
                    this._sql = this.GetSQL(this._expression);
                }

                return this._sql;
            }
        }

        public SqlParameter[] SqlParameters
        {
            get
            {
                return this._paramList.ToArray();
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

        #region 私有方法

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
                    return this.ResolveBinaryExToSQL(binary);
                }
            }

            if (expression is MethodCallExpression)
            {
                var meCall = expression as MethodCallExpression;
                return this.ResolveMethodCallExToSQL(meCall);
            }

            var body = expression as BinaryExpression;
            var left = this.GetSQL(body.Left);
            var right = this.GetSQL(body.Right);
            var oper = GetBinaryOperator(body.NodeType);

            return $"({left} {oper} {right})";
        }

        #region 解析二元表达式

        private string ResolveBinaryExToSQL(BinaryExpression expression)
        {
            var memberInfo = (expression.Left as MemberExpression).Member;
            string key = memberInfo.ColumnName();
            string oper = this.GetBinaryOperator(expression.NodeType);
            object value = this.GetBinaryValue(expression.Right);
            SqlParameter parameter = this.FormatSqlParameter(memberInfo, value);
            this._paramList.Add(parameter);
            return $"({key} {oper} @{parameter.ParameterName})";
        }

        private object GetBinaryValue(Expression expression)
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

        private string GetBinaryOperator(ExpressionType expressiontype)
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

        #region 解析方法调用表达式

        private string ResolveMethodCallExToSQL(MethodCallExpression expression)
        {
            switch (expression.Method.Name.ToLower())
            {
                case "contains":
                    var memberExpression = expression.Object as MemberExpression;
                    var fieldInfo = memberExpression.Member as FieldInfo;
                    var fieldValue = fieldInfo.GetValue((memberExpression.Expression as ConstantExpression).Value);
                    dynamic tempValue = fieldValue;
                    List<dynamic> tempValues = new List<dynamic>();
                    foreach (var key in tempValue)
                    {
                        tempValues.Add(key);
                    }

                    var column = expression.Arguments[0] as MemberExpression;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    for (int i = 0; i < tempValues.Count; i++)
                    {
                        var parameter = this.FormatSqlParameter(column.Member, tempValues[i]);
                        parameters.Add(parameter);
                    }

                    var sql = $"[{column.Member.ColumnName()}] IN ({string.Join(",", parameters.Select(p => $"@{p.ParameterName}"))})";
                    this._paramList.AddRange(parameters);
                    return sql;
            }

            return string.Empty;
        }

        #endregion

        private SqlParameter FormatSqlParameter(MemberInfo memberInfo, object memberValue)
        {            
            var paramName = $"{memberInfo.Name}_p_{this._suffix}";
            SqlParameter parameter = new SqlParameter(paramName, memberValue);
            var dataTypeAttr = memberInfo.Attr<DataTypeAttribute>();
            parameter.SqlDbType = dataTypeAttr != null ? dataTypeAttr.DbType : Util.GetSqlDataType(memberInfo);
            var stringLengthAttr = memberInfo.Attr<StringLengthAttribute>();
            if (stringLengthAttr != null)
            {
                parameter.Size = stringLengthAttr.Length;
            }

            this._suffix++;
            return parameter;
        }

        #endregion
    }
}
