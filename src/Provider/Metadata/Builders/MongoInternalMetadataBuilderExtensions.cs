using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    public static class MongoInternalMetadataBuilderExtensions
    {
        public static MongoEntityTypeAnnotations MongoDb(this IConventionEntityTypeBuilder entityTypeBuilder)
            => MongoDb(Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder)).Metadata);

        public static MongoEntityTypeAnnotations MongoDb(this EntityTypeBuilder entityTypeBuilder)
            => MongoDb(Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder)).Metadata);

        public static MongoEntityTypeAnnotations MongoDb(this IEntityType entityType)
            => MongoDb(Check.Is<IMutableEntityType>(entityType, nameof(entityType)));

        public static MongoEntityTypeAnnotations MongoDb(this IMutableEntityType mutableEntityType)
            => new MongoEntityTypeAnnotations(Check.NotNull(mutableEntityType, nameof(mutableEntityType)));

        public static MongoModelAnnotations MongoDb(this IConventionModelBuilder modelBuilder)
            => MongoDb(Check.NotNull(modelBuilder, nameof(modelBuilder)).Metadata);

        public static MongoModelAnnotations MongoDb(this ModelBuilder modelBuilder)
            => MongoDb(Check.NotNull(modelBuilder, nameof(modelBuilder)).Model);

        public static MongoModelAnnotations MongoDb(this IModel model)
            => MongoDb(Check.Is<IMutableModel>(model, nameof(model)));

        public static MongoModelAnnotations MongoDb(this IMutableModel mutableModel)
            => new MongoModelAnnotations(Check.NotNull(mutableModel, nameof(mutableModel)));
    }
}