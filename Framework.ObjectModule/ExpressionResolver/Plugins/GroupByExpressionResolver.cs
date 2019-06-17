using System.Linq.Expressions;

namespace Framework.ObjectModule
{
    public class GroupByExpressionResolver : BaseExpressionResolver
    {
        public GroupByExpressionResolver() : base()
        {
        }


        public override void Resolve(Expression expression)
        {
            this.SQL = base.GetColumns(expression);
            if (!string.IsNullOrEmpty(this.SQL))
            {
                this.SQL = $"GROUP BY {this.SQL}";
            }
        }
    }
}
