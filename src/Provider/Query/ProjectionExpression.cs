// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class ProjectionExpression : Expression, IPrintableExpression
    {
        public ProjectionExpression(Expression expression, string alias)
        {
            Expression = expression;
            Alias = alias;
        }

        public virtual string Alias { get; }

        public virtual Expression Expression { get; }

        public virtual string Name
            => (Expression as IAccessExpression)?.Name;

        public override Type Type => Expression.Type;

        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Expression));

        public virtual ProjectionExpression Update(Expression expression)
            => expression != Expression
                ? new ProjectionExpression(expression, Alias)
                : this;

        public virtual void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Visit(Expression);
            if (!string.Equals(string.Empty, Alias)
                && !string.Equals(Alias, Name))
            {
                expressionPrinter.Append(" AS " + Alias);
            }
        }

        public override bool Equals(object obj)
            => obj != null
               && (ReferenceEquals(this, obj)
                   || obj is ProjectionExpression projectionExpression
                   && Equals(projectionExpression));

        private bool Equals(ProjectionExpression projectionExpression)
            => string.Equals(Alias, projectionExpression.Alias)
               && Expression.Equals(projectionExpression.Expression);

        public override int GetHashCode() => HashCode.Combine(Alias, Expression);
    }
}
