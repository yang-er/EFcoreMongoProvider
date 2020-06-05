// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class InExpression : SqlExpression
    {
        public InExpression(SqlExpression item, bool negated, SqlExpression values, CoreTypeMapping? typeMapping)
            : base(typeof(bool), typeMapping)
        {
            Item = item;
            IsNegated = negated;
            Values = values;
        }

        public virtual SqlExpression Item { get; }

        public virtual bool IsNegated { get; }

        public virtual SqlExpression Values { get; }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newItem = (SqlExpression)visitor.Visit(Item);
            var values = (SqlExpression)visitor.Visit(Values);

            return Update(newItem, values);
        }

        public virtual InExpression Negate() => new InExpression(Item, !IsNegated, Values, TypeMapping);

        public virtual InExpression Update(SqlExpression item, SqlExpression values)
            => item != Item || values != Values
                ? new InExpression(item, IsNegated, values, TypeMapping)
                : this;

        public override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Visit(Item);
            expressionPrinter.Append(IsNegated ? " NOT IN " : " IN ");
            expressionPrinter.Append("(");
            expressionPrinter.Visit(Values);
            expressionPrinter.Append(")");
        }

        public override bool Equals(object obj)
            => obj != null
               && (ReferenceEquals(this, obj)
                   || obj is InExpression inExpression
                   && Equals(inExpression));

        private bool Equals(InExpression inExpression)
            => base.Equals(inExpression)
               && Item.Equals(inExpression.Item)
               && IsNegated.Equals(inExpression.IsNegated)
               && (Values == null ? inExpression.Values == null : Values.Equals(inExpression.Values));

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Item, IsNegated, Values);
    }
}
