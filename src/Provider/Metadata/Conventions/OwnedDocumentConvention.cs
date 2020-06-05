using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using System;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    /// <inheritdoc cref="IEntityTypeBaseTypeChangedConvention"/>
    /// <inheritdoc cref="IEntityTypeAddedConvention"/>
    /// <inheritdoc cref="IForeignKeyAddedConvention"/>
    /// <inheritdoc cref="IKeyAddedConvention"/>
    /// <inheritdoc cref="IKeyRemovedConvention"/>
    /// <inheritdoc cref="IModelFinalizedConvention"/>
    public class OwnedDocumentConvention :
        IEntityTypeBaseTypeChangedConvention,
        IEntityTypeAddedConvention,
        IForeignKeyAddedConvention,
        IKeyAddedConvention,
        IKeyRemovedConvention,
        IModelFinalizedConvention
    {
        private void Process(IConventionEntityTypeBuilder entityTypeBuilder)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            var entityType = entityTypeBuilder.Metadata;
            var primaryKey = entityType.FindPrimaryKey();

            bool isComplexType = primaryKey == null || primaryKey.Document().IsOwnershipKey;
            entityTypeBuilder.Document().IsComplexType = isComplexType;
        }

        /// <inheritdoc />
        public void ProcessForeignKeyAdded(
            IConventionRelationshipBuilder relationshipBuilder,
            IConventionContext<IConventionRelationshipBuilder> _)
        {
            Check.NotNull(relationshipBuilder, nameof(relationshipBuilder));
            var principalEntityType = relationshipBuilder.Metadata.PrincipalEntityType;

            bool principalIsOwned = principalEntityType.IsOwned() || principalEntityType.MongoDb().IsComplexType;
            if (principalIsOwned) relationshipBuilder.IsOwnership(true);
        }

        /// <inheritdoc />
        public void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            IConventionContext<IConventionEntityTypeBuilder> _)
            => Process(entityTypeBuilder);

        /// <inheritdoc />
        public void ProcessEntityTypeBaseTypeChanged(
            IConventionEntityTypeBuilder entityTypeBuilder,
            IConventionEntityType newBaseType,
            IConventionEntityType oldBaseType,
            IConventionContext<IConventionEntityType> _)
            => Process(entityTypeBuilder);

        /// <inheritdoc />
        public void ProcessKeyAdded(
            IConventionKeyBuilder keyBuilder,
            IConventionContext<IConventionKeyBuilder> _)
            => Process(keyBuilder.Metadata.DeclaringEntityType.Builder);

        /// <inheritdoc />
        public void ProcessKeyRemoved(
            IConventionEntityTypeBuilder entityTypeBuilder,
            IConventionKey key,
            IConventionContext<IConventionKey> _)
            => Process(entityTypeBuilder);

        public void ProcessModelFinalized(
            IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> _)
        {
            var model = Check.NotNull(modelBuilder, nameof(modelBuilder)).Metadata;
            
            var ownedEntityTypes = model
                .GetEntityTypes()
                .Where(entityType => entityType.IsOwned())
                .Select(entityType => entityType.AsEntityType())
                .ToList();

            foreach (var ownedEntityType in ownedEntityTypes)
            {
                bool isOwnedType = ownedEntityType.HasClrType()
                    ? model.IsOwned(ownedEntityType.ClrType)
                    : throw new NotSupportedException();

                var key = ownedEntityType.FindPrimaryKey();

                if (key != null)
                {
                    ownedEntityType.Builder.HasNoKey(key, ConfigurationSource.Explicit);
                    ownedEntityType.Builder.RemoveUnusedShadowProperties(key.Properties, ConfigurationSource.Explicit);
                }

                if (ownedEntityType.FindPrimaryKey() != null)
                    throw new NotImplementedException("Why this fucking unexpected key goes back?");

                var invalidProps = ownedEntityType.GetProperties()
                    .Where(p => p.IsShadowProperty()
                        && p.ValueGenerated != ValueGenerated.Never
                        && p.GetValueGeneratorFactory() == null)
                    .ToList();
                if (invalidProps.Count > 0)
                    throw new NotImplementedException("Why this fucking shadow property goes back?");
            }
        }
    }
}
