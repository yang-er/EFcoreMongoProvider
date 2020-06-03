using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    /// <inheritdoc />
    public class MongoKeyAttributeConvention : KeyAttributeConvention
    {
        /// <summary>
        ///     Creates a new instance of <see cref="MongoKeyAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public MongoKeyAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        private static readonly KeyAttribute KeyAttribute = new KeyAttribute();

        /// <inheritdoc />
        public override void ProcessPropertyAdded(
            IConventionPropertyBuilder propertyBuilder,
            IConventionContext<IConventionPropertyBuilder> context)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var memberInfo = propertyBuilder.Metadata.GetIdentifyingMemberInfo();
            if (memberInfo?.IsDefined(typeof(BsonIdAttribute), true) ?? false)
                ProcessPropertyAdded(propertyBuilder, KeyAttribute, memberInfo, context);
        }

        /// <inheritdoc />
        public override void ProcessModelFinalized(
            IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
        {
            var entityTypes = modelBuilder.Metadata
                .GetEntityTypes()
                .Where(entityType => entityType.MongoDb().IsDerivedType);

            foreach (var entityType in entityTypes)
                foreach (var declaredProperty in entityType.GetDeclaredProperties())
                    if (declaredProperty.GetIdentifyingMemberInfo()?.IsDefined(typeof(BsonIdAttribute), true) ?? false)
                        throw new InvalidOperationException(
                            CoreStrings.KeyAttributeOnDerivedEntity(entityType.DisplayName(), declaredProperty.Name));

            base.ProcessModelFinalized(modelBuilder, context);
        }
    }
}