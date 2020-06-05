// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class ObjectAccessExpression : Expression, IPrintableExpression, IAccessExpression
    {
        public ObjectAccessExpression(INavigation navigation, Expression accessExpression)
        {
            Name = navigation.GetTargetType().GetContainingPropertyName();
            if (Name == null)
            {
                throw new InvalidOperationException(
                    $"Navigation '{navigation.DeclaringEntityType.DisplayName()}.{navigation.Name}' doesn't point to an embedded entity.");
            }

            Navigation = navigation;
            AccessExpression = accessExpression;
        }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => Navigation.ClrType;

        public virtual string Name { get; }

        public virtual INavigation Navigation { get; }

        public virtual Expression AccessExpression { get; }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var outerExpression = visitor.Visit(AccessExpression);

            return Update(outerExpression);
        }

        public virtual ObjectAccessExpression Update(Expression outerExpression)
            => outerExpression != AccessExpression
                ? new ObjectAccessExpression(Navigation, outerExpression)
                : this;

        public virtual void Print(ExpressionPrinter expressionPrinter) => expressionPrinter.Append(ToString());

        public override string ToString() => $"{AccessExpression}[\"{Name}\"]";

        public override bool Equals(object obj)
            => obj != null
               && (ReferenceEquals(this, obj)
                   || obj is ObjectAccessExpression objectAccessExpression
                   && Equals(objectAccessExpression));

        private bool Equals(ObjectAccessExpression objectAccessExpression)
            => Navigation == objectAccessExpression.Navigation
               && AccessExpression.Equals(objectAccessExpression.AccessExpression);

        public override int GetHashCode() => HashCode.Combine(Navigation, AccessExpression);
    }
}
