using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    /// <inheritdoc cref="IEntityTypeBaseTypeChangedConvention"/>
    /// <inheritdoc cref="IEntityTypeAddedConvention"/>
    /// <inheritdoc cref="IForeignKeyAddedConvention"/>
    /// <inheritdoc cref="IKeyAddedConvention"/>
    /// <inheritdoc cref="IKeyRemovedConvention"/>
    public class OwnedDocumentConvention :
        IEntityTypeBaseTypeChangedConvention,
        IEntityTypeAddedConvention,
        IForeignKeyAddedConvention,
        IKeyAddedConvention,
        IKeyRemovedConvention
    {
        /// <inheritdoc />
        public void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            IConventionContext<IConventionEntityTypeBuilder>? _)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            var entityType = entityTypeBuilder.Metadata;
            var primaryKey = entityType.FindPrimaryKey();

            bool isComplexType = primaryKey == null
                                 || primaryKey.Document().IsOwnershipKey;

            entityTypeBuilder.Document().IsComplexType = isComplexType;
        }

        /// <inheritdoc />
        public void ProcessEntityTypeBaseTypeChanged(
            IConventionEntityTypeBuilder entityTypeBuilder,
            IConventionEntityType newBaseType,
            IConventionEntityType oldBaseType,
            IConventionContext<IConventionEntityType> _)
        {
            ProcessEntityTypeAdded(entityTypeBuilder, null);
        }

        /// <inheritdoc />
        public void ProcessForeignKeyAdded(
            IConventionRelationshipBuilder relationshipBuilder,
            IConventionContext<IConventionRelationshipBuilder> _)
        {
            Check.NotNull(relationshipBuilder, nameof(relationshipBuilder));

            var principalEntityType = relationshipBuilder.Metadata.PrincipalEntityType;

            bool principalIsOwned = principalEntityType.IsOwned()
                                 || principalEntityType.MongoDb().IsComplexType;

            if (principalIsOwned)
                relationshipBuilder.IsOwnership(true);
        }

        /// <inheritdoc />
        public void ProcessKeyAdded(
            IConventionKeyBuilder keyBuilder,
            IConventionContext<IConventionKeyBuilder> _)
        {
            ProcessEntityTypeAdded(Check.NotNull(keyBuilder, nameof(keyBuilder)).Metadata.DeclaringEntityType.Builder, null);
        }

        public void ProcessKeyRemoved(
            IConventionEntityTypeBuilder entityTypeBuilder,
            IConventionKey key,
            IConventionContext<IConventionKey> _)
        {
            ProcessEntityTypeAdded(entityTypeBuilder, null);
        }
    }
}
