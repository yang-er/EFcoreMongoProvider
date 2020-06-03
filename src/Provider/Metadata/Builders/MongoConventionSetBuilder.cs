using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    /// <inheritdoc />
    public class MongoConventionSetBuilder : ProviderConventionSetBuilder
    {
        private readonly MongoConventionSetBuilderDependencies _mongoDbConventionSetBuilderDependencies;
        private readonly ProviderConventionSetBuilderDependencies _anotherDependencies;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MongoConventionSetBuilder" /> class.
        /// </summary>
        /// <param name="dep1">Parameter object containing dependencies for this service.</param>
        /// <param name="dependencies">Parameter object containing dependencies for this service.</param>
        public MongoConventionSetBuilder(
            MongoConventionSetBuilderDependencies dep1,
            ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
            _mongoDbConventionSetBuilderDependencies
                = Check.NotNull(dep1, nameof(dep1));
            _anotherDependencies
                = Check.NotNull(dependencies, nameof(dependencies));
        }

        public override ConventionSet CreateConventionSet()
        {
            var conventionSet = base.CreateConventionSet();
            Check.NotNull(conventionSet, nameof(conventionSet));

            var ownedDocumentConvention = new OwnedDocumentConvention();

            DatabaseGeneratedAttributeConvention databaseGeneratedAttributeConvention
                = new MongoDatabaseGeneratedAttributeConvention(_anotherDependencies);

            KeyAttributeConvention keyAttributeConvention =
                new MongoKeyAttributeConvention(_anotherDependencies);

            var mongoDatabaseConvention
                = new MongoDatabaseConvention(_mongoDbConventionSetBuilderDependencies.CurrentDbContext.Context);

            var bsonRequiredAttributeConvention
                = new BsonRequiredAttributeConvention(_anotherDependencies);

            conventionSet.ModelInitializedConventions
                .With(mongoDatabaseConvention);

            conventionSet.EntityTypeAddedConventions
                .With(ownedDocumentConvention)
                .With(new MongoCollectionAttributeConvention(_anotherDependencies))
                .With(new BsonDiscriminatorAttributeConvention(_anotherDependencies))
                .With(new BsonIgnoreAttributeConvention())
                .With(new BsonKnownTypesAttributeConvention(_anotherDependencies));

            conventionSet.EntityTypeBaseTypeChangedConventions
                .With(ownedDocumentConvention);

            conventionSet.KeyAddedConventions
                .With(ownedDocumentConvention);

            conventionSet.KeyRemovedConventions
                .With(ownedDocumentConvention);

            conventionSet.ForeignKeyAddedConventions
                .With(ownedDocumentConvention);

            conventionSet.ForeignKeyOwnershipChangedConventions
                .Without(item => item is NavigationEagerLoadingConvention);

            conventionSet.PropertyAddedConventions
                .Replace(databaseGeneratedAttributeConvention)
                .Replace(keyAttributeConvention)
                .With(bsonRequiredAttributeConvention);

            conventionSet.PropertyFieldChangedConventions
                .Replace(databaseGeneratedAttributeConvention)
                .Replace(keyAttributeConvention)
                .With(bsonRequiredAttributeConvention);

            conventionSet.ModelFinalizedConventions
                .Replace(keyAttributeConvention);

            return conventionSet;
        }
    }
}