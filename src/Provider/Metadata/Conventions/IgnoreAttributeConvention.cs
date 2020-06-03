using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    public class BsonIgnoreAttributeConvention : IEntityTypeAddedConvention
    {
        /// <inheritdoc />
        public void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            IConventionContext<IConventionEntityTypeBuilder> context)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            Type clrType = entityTypeBuilder.Metadata.ClrType;
            if (clrType == null)
            {
                return;
            }

            IEnumerable<MemberInfo> members = clrType.GetRuntimeProperties()
                .Cast<MemberInfo>()
                .Concat(clrType.GetRuntimeFields())
                .Where(memberInfo => memberInfo.IsDefined(typeof(BsonIgnoreAttribute), true));

            foreach (MemberInfo member in members)
            {
                entityTypeBuilder.Ignore(member.Name, fromDataAnnotation: true);
            }
        }
    }
}