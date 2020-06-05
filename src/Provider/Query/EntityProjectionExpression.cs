// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class EntityProjectionExpression : Expression, IPrintableExpression, IAccessExpression
    {
        private readonly IDictionary<IProperty, IAccessExpression> _propertyExpressionsCache
            = new Dictionary<IProperty, IAccessExpression>();

        private readonly IDictionary<INavigation, IAccessExpression> _navigationExpressionsCache
            = new Dictionary<INavigation, IAccessExpression>();

        public EntityProjectionExpression(IEntityType entityType, Expression accessExpression)
        {
            EntityType = entityType;
            AccessExpression = accessExpression;
            Name = (accessExpression as IAccessExpression)?.Name;
        }

        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => EntityType.ClrType;

        public virtual Expression AccessExpression { get; }

        public virtual IEntityType EntityType { get; }

        public virtual string Name { get; }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var accessExpression = visitor.Visit(AccessExpression);

            return accessExpression != AccessExpression
                ? new EntityProjectionExpression(EntityType, accessExpression)
                : this;
        }

        public virtual Expression BindProperty(IProperty property, bool clientEval)
        {
            if (!EntityType.IsAssignableFrom(property.DeclaringEntityType)
                && !property.DeclaringEntityType.IsAssignableFrom(EntityType))
            {
                throw new InvalidOperationException(
                    $"Called EntityProjectionExpression.GetProperty() with incorrect IProperty. EntityType:{EntityType.DisplayName()}, Property:{property.Name}");
            }

            if (!_propertyExpressionsCache.TryGetValue(property, out var expression))
            {
                expression = new KeyAccessExpression(property, AccessExpression);
                _propertyExpressionsCache[property] = expression;
            }

            if (!clientEval
                && expression.Name.Length == 0)
            {
                // Non-persisted property can't be translated
                return null;
            }

            return (Expression)expression;
        }

        public virtual Expression BindNavigation(INavigation navigation, bool clientEval)
        {
            if (!EntityType.IsAssignableFrom(navigation.DeclaringEntityType)
                && !navigation.DeclaringEntityType.IsAssignableFrom(EntityType))
            {
                throw new InvalidOperationException(
                    $"Called EntityProjectionExpression.GetNavigation() with incorrect INavigation. EntityType:{EntityType.DisplayName()}, Navigation:{navigation.Name}");
            }

            if (!_navigationExpressionsCache.TryGetValue(navigation, out var expression))
            {
                if (navigation.IsCollection())
                {
                    expression = new ObjectArrayProjectionExpression(navigation, AccessExpression);
                }
                else
                {
                    expression = new EntityProjectionExpression(
                        navigation.GetTargetType(),
                        new ObjectAccessExpression(navigation, AccessExpression));
                }

                _navigationExpressionsCache[navigation] = expression;
            }

            if (!clientEval
                && expression.Name.Length == 0)
            {
                // Non-persisted navigation can't be translated
                return null;
            }

            return (Expression)expression;
        }

        public virtual Expression BindMember(string name, Type entityClrType, bool clientEval, out IPropertyBase propertyBase)
            => BindMember(MemberIdentity.Create(name), entityClrType, clientEval, out propertyBase);

        public virtual Expression BindMember(
            MemberInfo memberInfo, Type entityClrType, bool clientEval, out IPropertyBase propertyBase)
            => BindMember(MemberIdentity.Create(memberInfo), entityClrType, clientEval, out propertyBase);

        private Expression BindMember(MemberIdentity member, Type entityClrType, bool clientEval, out IPropertyBase propertyBase)
        {
            var entityType = EntityType;
            if (entityClrType != null
                && !entityClrType.IsAssignableFrom(entityType.ClrType))
            {
                entityType = entityType.GetDerivedTypes().First(e => entityClrType.IsAssignableFrom(e.ClrType));
            }

            var property = member.MemberInfo == null
                ? entityType.FindProperty(member.Name)
                : entityType.FindProperty(member.MemberInfo);
            if (property != null)
            {
                propertyBase = property;
                return BindProperty(property, clientEval);
            }

            var navigation = member.MemberInfo == null
                ? entityType.FindNavigation(member.Name)
                : entityType.FindNavigation(member.MemberInfo);
            if (navigation != null)
            {
                propertyBase = navigation;
                return BindNavigation(navigation, clientEval);
            }

            // Entity member not found
            propertyBase = null;
            return null;
        }

        public virtual void Print(ExpressionPrinter expressionPrinter)
            => expressionPrinter.Visit(AccessExpression);

        public override bool Equals(object obj)
            => obj != null
               && (ReferenceEquals(this, obj)
                   || obj is EntityProjectionExpression entityProjectionExpression
                   && Equals(entityProjectionExpression));

        private bool Equals(EntityProjectionExpression entityProjectionExpression)
            => Equals(EntityType, entityProjectionExpression.EntityType)
               && AccessExpression.Equals(entityProjectionExpression.AccessExpression);
        
        public override int GetHashCode() => HashCode.Combine(EntityType, AccessExpression);
    }
}
