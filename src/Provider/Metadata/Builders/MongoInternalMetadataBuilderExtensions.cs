using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using System.Diagnostics;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    public static class MongoInternalMetadataBuilderExtensions
    {
        [DebuggerStepThrough]
        public static MongoEntityTypeAnnotations MongoDb(this IConventionEntityTypeBuilder entityTypeBuilder)
            => MongoDb(Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder)).Metadata);

        [DebuggerStepThrough]
        public static MongoEntityTypeAnnotations MongoDb(this EntityTypeBuilder entityTypeBuilder)
            => MongoDb(Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder)).Metadata);

        [DebuggerStepThrough]
        public static MongoEntityTypeAnnotations MongoDb(this IEntityType entityType)
            => MongoDb(Check.Is<IMutableEntityType>(entityType, nameof(entityType)));

        [DebuggerStepThrough]
        public static MongoEntityTypeAnnotations MongoDb(this IMutableEntityType mutableEntityType)
            => new MongoEntityTypeAnnotations(Check.NotNull(mutableEntityType, nameof(mutableEntityType)));

        [DebuggerStepThrough]
        public static MongoModelAnnotations MongoDb(this IConventionModelBuilder modelBuilder)
            => MongoDb(Check.NotNull(modelBuilder, nameof(modelBuilder)).Metadata);

        [DebuggerStepThrough]
        public static MongoModelAnnotations MongoDb(this ModelBuilder modelBuilder)
            => MongoDb(Check.NotNull(modelBuilder, nameof(modelBuilder)).Model);

        [DebuggerStepThrough]
        public static MongoModelAnnotations MongoDb(this IModel model)
            => MongoDb(Check.Is<IMutableModel>(model, nameof(model)));

        [DebuggerStepThrough]
        public static MongoModelAnnotations MongoDb(this IMutableModel mutableModel)
            => new MongoModelAnnotations(Check.NotNull(mutableModel, nameof(mutableModel)));
    }
}