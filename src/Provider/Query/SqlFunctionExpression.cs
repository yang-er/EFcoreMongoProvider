// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class SqlFunctionExpression : SqlExpression
    {
        public SqlFunctionExpression(
            string name,
            IEnumerable<SqlExpression> arguments,
            Type type,
            CoreTypeMapping typeMapping)
            : base(type, typeMapping)
        {
            Name = name;
            Arguments = (arguments ?? Array.Empty<SqlExpression>()).ToList();
        }

        public virtual string Name { get; }

        public virtual IReadOnlyList<SqlExpression> Arguments { get; }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var changed = false;
            var arguments = new SqlExpression[Arguments.Count];
            for (var i = 0; i < arguments.Length; i++)
            {
                arguments[i] = (SqlExpression)visitor.Visit(Arguments[i]);
                changed |= arguments[i] != Arguments[i];
            }

            return changed
                ? new SqlFunctionExpression(
                    Name,
                    arguments,
                    Type,
                    TypeMapping)
                : this;
        }

        public virtual SqlFunctionExpression ApplyTypeMapping(CoreTypeMapping typeMapping)
            => new SqlFunctionExpression(
                Name,
                Arguments,
                Type,
                typeMapping ?? TypeMapping);

        public virtual SqlFunctionExpression Update(IReadOnlyList<SqlExpression> arguments)
            => !arguments.SequenceEqual(Arguments)
                ? new SqlFunctionExpression(Name, arguments, Type, TypeMapping)
                : this;

        public override void Print(ExpressionPrinter expressionPrinter)
        {
            expressionPrinter.Append(Name);
            expressionPrinter.Append("(");
            expressionPrinter.VisitList(Arguments);
            expressionPrinter.Append(")");
        }

        public override bool Equals(object obj)
            => obj != null
               && (ReferenceEquals(this, obj)
                   || obj is SqlFunctionExpression sqlFunctionExpression
                   && Equals(sqlFunctionExpression));

        private bool Equals(SqlFunctionExpression sqlFunctionExpression)
            => base.Equals(sqlFunctionExpression)
               && string.Equals(Name, sqlFunctionExpression.Name)
               && Arguments.SequenceEqual(sqlFunctionExpression.Arguments);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(base.GetHashCode());
            hash.Add(Name);
            for (var i = 0; i < Arguments.Count; i++)
            {
                hash.Add(Arguments[i]);
            }

            return hash.ToHashCode();
        }
    }
}
