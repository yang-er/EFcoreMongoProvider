using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class MongoShapedQueryCompilingExpressionVisitor :
        ShapedQueryCompilingExpressionVisitor
    {
        public MongoShapedQueryCompilingExpressionVisitor(
            ShapedQueryCompilingExpressionVisitorDependencies dependencies,
            QueryCompilationContext queryCompilationContext)
            : base(dependencies, queryCompilationContext)
        {
        }

        protected override Expression VisitShapedQueryExpression(
            ShapedQueryExpression shapedQueryExpression)
        {
            throw new NotImplementedException();
        }
    }
}
