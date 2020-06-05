using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    /// <inheritdoc />
    public class MongoConventionSetBuilder : ProviderConventionSetBuilder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MongoConventionSetBuilder" /> class.
        /// </summary>
        /// <param name="dependencies">Parameter object containing dependencies for this service.</param>
        public MongoConventionSetBuilder(
            ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        public override ConventionSet CreateConventionSet()
        {
            var conventionSet = base.CreateConventionSet();
            Check.NotNull(conventionSet, nameof(conventionSet));

            var ownedDocumentConvention = new OwnedDocumentConvention();
            /*
            DatabaseGeneratedAttributeConvention databaseGeneratedAttributeConvention
                = new MongoDatabaseGeneratedAttributeConvention(Dependencies);

            KeyAttributeConvention keyAttributeConvention =
                new MongoKeyAttributeConvention(Dependencies);

            var bsonRequiredAttributeConvention
                = new BsonRequiredAttributeConvention(Dependencies);

            conventionSet.EntityTypeAddedConventions
                .With(ownedDocumentConvention)
                .With(new MongoCollectionAttributeConvention(Dependencies))
                .With(new BsonDiscriminatorAttributeConvention(Dependencies))
                .With(new BsonIgnoreAttributeConvention())
                .With(new BsonKnownTypesAttributeConvention(Dependencies));
            
            conventionSet.EntityTypeBaseTypeChangedConventions
                .With(ownedDocumentConvention);

            conventionSet.KeyAddedConventions
                .With(ownedDocumentConvention);

            conventionSet.KeyRemovedConventions
                .With(ownedDocumentConvention);

            conventionSet.ForeignKeyAddedConventions
                .With(ownedDocumentConvention);
            */
            conventionSet.ForeignKeyOwnershipChangedConventions
                .Without(item => item is NavigationEagerLoadingConvention);
            /*
            conventionSet.PropertyAddedConventions
                .Replace(databaseGeneratedAttributeConvention)
                .Replace(keyAttributeConvention)
                .With(bsonRequiredAttributeConvention);

            conventionSet.PropertyFieldChangedConventions
                .Replace(databaseGeneratedAttributeConvention)
                .Replace(keyAttributeConvention)
                .With(bsonRequiredAttributeConvention);
            */
            conventionSet.ModelFinalizedConventions
                //.Replace(keyAttributeConvention)
                .Insert(0, ownedDocumentConvention);

            return conventionSet;
        }
    }
}