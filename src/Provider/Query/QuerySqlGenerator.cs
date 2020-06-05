using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class QuerySqlGenerator : SqlExpressionVisitor
    {
        protected override Expression VisitEntityProjection(EntityProjectionExpression entityProjectionExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitIn(InExpression inExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitKeyAccess(KeyAccessExpression keyAccessExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitObjectAccess(ObjectAccessExpression objectAccessExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitObjectArrayProjection(ObjectArrayProjectionExpression objectArrayProjectionExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitOrdering(OrderingExpression orderingExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitProjection(ProjectionExpression projectionExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitRootReference(RootReferenceExpression rootReferenceExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSelect(SelectExpression selectExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSqlBinary(SqlBinaryExpression sqlBinaryExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSqlConditional(SqlConditionalExpression caseExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSqlConstant(SqlConstantExpression sqlConstantExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSqlParameter(SqlParameterExpression sqlParameterExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSqlUnary(SqlUnaryExpression sqlUnaryExpression)
        {
            throw new NotImplementedException();
        }
    }
}
