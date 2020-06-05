// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class ObjectArrayProjectionExpression : Expression, IPrintableExpression, IAccessExpression
    {
        public ObjectArrayProjectionExpression(
            INavigation navigation, Expression accessExpression, EntityProjectionExpression innerProjection = null)
        {
            var targetType = navigation.GetTargetType();
            Type = typeof(IEnumerable<>).MakeGenericType(targetType.ClrType);

            Name = targetType.GetContainingPropertyName();
            if (Name == null)
            {
                throw new InvalidOperationException(
                    $"Navigation '{navigation.DeclaringEntityType.DisplayName()}.{navigation.Name}' doesn't point to an embedded entity.");
            }

            Navigation = navigation;
            AccessExpression = accessExpression;
            InnerProjection = innerProjection ?? new EntityProjectionExpression(
                                  targetType,
                                  new RootReferenceExpression(targetType, ""));
        }

        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type { get; }

        public virtual string Name { get; }

        public virtual INavigation Navigation { get; }

        public virtual Expression AccessExpression { get; }

        public virtual EntityProjectionExpression InnerProjection { get; }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var accessExpression = visitor.Visit(AccessExpression);
            var innerProjection = visitor.Visit(InnerProjection);

            return Update(accessExpression, (EntityProjectionExpression)innerProjection);
        }

        public virtual ObjectArrayProjectionExpression Update(Expression accessExpression, EntityProjectionExpression innerProjection)
            => accessExpression != AccessExpression || innerProjection != InnerProjection
                ? new ObjectArrayProjectionExpression(Navigation, accessExpression, innerProjection)
                : this;

        public virtual void Print(ExpressionPrinter expressionPrinter)
            => expressionPrinter.Append(ToString());

        public override string ToString() => $"{AccessExpression}[\"{Name}\"]";

        public override bool Equals(object obj)
            => obj != null
               && (ReferenceEquals(this, obj)
                   || obj is ObjectArrayProjectionExpression arrayProjectionExpression
                   && Equals(arrayProjectionExpression));

        private bool Equals(ObjectArrayProjectionExpression objectArrayProjectionExpression)
            => AccessExpression.Equals(objectArrayProjectionExpression.AccessExpression)
               && InnerProjection.Equals(objectArrayProjectionExpression.InnerProjection);

        public override int GetHashCode() => HashCode.Combine(AccessExpression, InnerProjection);
    }
}
