using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    /// <summary>
    ///     Provides a set of MongoDB-specific extension methods for <see cref="EntityTypeBuilder"/>.
    /// </summary>
    public static class MongoEntityTypeBuilderExtensions
    {
        /// <summary>
        ///     Sets the name of the MongoDB collection used to store the <see cref="IEntityType"/> being built.
        /// </summary>
        /// <param name="entityTypeBuilder">The <see cref="EntityTypeBuilder"/> to annotate.</param>
        /// <param name="collectionName">The name of the MongoDB collection.</param>
        /// <returns>The <paramref name="entityTypeBuilder"/>, such that calls be chained.</returns>
        public static EntityTypeBuilder ToCollection(
            this EntityTypeBuilder entityTypeBuilder,
            string collectionName)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NotEmpty(collectionName, nameof(collectionName));

            entityTypeBuilder.MongoDb().CollectionName = collectionName;
            return entityTypeBuilder;
        }

        /// <summary>
        ///     Sets the discriminator used to query instances of the <see cref="IEntityType"/> being built.
        /// </summary>
        /// <param name="entityTypeBuilder">The <see cref="EntityTypeBuilder"/> to annotate.</param>
        /// <param name="discriminator">The discriminator for the <see cref="IEntityType"/>.</param>
        /// <returns>The <paramref name="entityTypeBuilder"/>, such that calls be chained.</returns>
        public static EntityTypeBuilder HasDiscriminator(
            this EntityTypeBuilder entityTypeBuilder,
            string discriminator)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NotEmpty(discriminator, nameof(discriminator));
            entityTypeBuilder.MongoDb().Discriminator = discriminator;
            return entityTypeBuilder;
        }

        /// <summary>
        ///     Sets the whether or not a discriminator is required to query instances of the <see cref="IEntityType"/> being built.
        /// </summary>
        /// <param name="entityTypeBuilder">The <see cref="EntityTypeBuilder"/> to annotate.</param>
        /// <param name="discriminatorIsRequired">
        ///     <code>true</code> if a discriminator is required to query instances of the entity; otherwise <code>false</code>.
        /// </param>
        /// <returns>The <paramref name="entityTypeBuilder"/>, such that calls be chained.</returns>
        public static EntityTypeBuilder IsDiscriminatorRequired(
            this EntityTypeBuilder entityTypeBuilder,
            bool discriminatorIsRequired = true)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.MongoDb().DiscriminatorIsRequired = discriminatorIsRequired;
            return entityTypeBuilder;
        }

        /// <summary>
        ///     Sets whether the <see cref="IEntityType"/> being built is a root type of a polymorphic hierarchy.
        /// </summary>
        /// <param name="entityTypeBuilder">The <see cref="EntityTypeBuilder"/> to annotate.</param>
        /// <param name="isRootType">
        ///     <code>true</code> if the <see cref="IEntityType"/> is the root entity type; otherwise <code>false</code>.
        /// </param>
        /// <returns>The <paramref name="entityTypeBuilder"/>, such that calls be chained.</returns>
        public static EntityTypeBuilder IsRootType(
            this EntityTypeBuilder entityTypeBuilder,
            bool isRootType = true)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.MongoDb().IsRootType = isRootType;
            return entityTypeBuilder;
        }

        /// <summary>
        ///     Returns the name of the parent property to which the entity type is mapped.
        /// </summary>
        /// <param name="entityType"> The entity type to get the containing property name for. </param>
        /// <returns> The name of the parent property to which the entity type is mapped. </returns>
        public static string? GetContainingPropertyName(this IEntityType entityType) =>
            entityType[MongoAnnotationNames.NavigationName] as string
            ?? GetDefaultContainingPropertyName(entityType);

        private static string? GetDefaultContainingPropertyName(IEntityType entityType)
            => entityType.FindOwnership()?.PrincipalToDependent.Name;

        /// <summary>
        ///     Sets whether the identity of the <see cref="IEntityType"/> being built should be assigned by MongoDb on insert.
        /// </summary>
        /// <param name="entityTypeBuilder">The <see cref="EntityTypeBuilder"/> to annotate.</param>
        /// <param name="assignIdOnInsert">
        ///     <code>true</code> if the identity of the <see cref="IEntityType"/> is assigned on insert;
        ///     otherwise <code>false</code>.
        /// </param>
        /// <returns>The <paramref name="entityTypeBuilder"/>, such that calls be chained.</returns>
        public static EntityTypeBuilder AssignIdOnInsert(
            this EntityTypeBuilder entityTypeBuilder,
            bool assignIdOnInsert = true)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            entityTypeBuilder.MongoDb().AssignIdOnInsert = assignIdOnInsert;
            return entityTypeBuilder;
        }
    }
}