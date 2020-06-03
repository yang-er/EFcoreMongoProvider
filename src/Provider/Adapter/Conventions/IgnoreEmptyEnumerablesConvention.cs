﻿using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Microsoft.EntityFrameworkCore.Mongo.Adapter.Conventions
{
    /// <summary>
    /// A convention that ignores empty <see cref="IEnumerable"/> instances when serializing Bson documents. 
    /// </summary>
    public class IgnoreEmptyEnumerablesConvention : ConventionBase, IMemberMapConvention
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreEmptyEnumerablesConvention"/> class.
        /// </summary>
        public IgnoreEmptyEnumerablesConvention()
            : base(Regex.Replace(nameof(IgnoreEmptyEnumerablesConvention), "Convention$", ""))
        {
        }

        /// <summary>
        /// Applies the Ignore Empty Enumerables convention to the given <paramref name="memberMap"/>.
        /// </summary>
        /// <param name="memberMap">The <see cref="BsonMemberMap" /> to which the convention will be applied.</param>
        public virtual void Apply(BsonMemberMap memberMap)
        {
            Check.NotNull(memberMap, nameof(memberMap));
            if (memberMap.MemberType.TryGetSequenceType() != null)
            {
                memberMap.SetShouldSerializeMethod(@object =>
                    {
                        object value = memberMap.Getter(@object);
                        return (value as IEnumerable)?.GetEnumerator().MoveNext() ?? false;
                    });
            }
        }
    }
}