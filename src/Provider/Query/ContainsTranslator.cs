// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class ContainsTranslator : IMethodCallTranslator
    {
        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public ContainsTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public virtual SqlExpression? Translate(SqlExpression instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        {
            if (method.IsGenericMethod
                && method.GetGenericMethodDefinition().Equals(EnumerableMethods.Contains))
            {
                return _sqlExpressionFactory.In(arguments[1], arguments[0], false);
            }

            if ((method.DeclaringType.GetInterfaces().Contains(typeof(IList))
                 || method.DeclaringType.IsGenericType
                 && method.DeclaringType.GetGenericTypeDefinition() == typeof(ICollection<>))
                && string.Equals(method.Name, nameof(IList.Contains)))
            {
                return _sqlExpressionFactory.In(arguments[0], instance, false);
            }

            return null;
        }
    }
}
