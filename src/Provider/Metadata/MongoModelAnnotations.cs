using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Driver;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata
{
    /// <inheritdoc />
    public class MongoModelAnnotations : DocumentAnnotations<IModel>
    {
        public MongoModelAnnotations(IModel model)
            : base(Check.NotNull(model, nameof(model)))
        {
        }

        public virtual MongoDatabaseSettings DatabaseSettings
        {
            get => GetAnnotation<MongoDatabaseSettings>(MongoAnnotationNames.DatabaseSettings);
            set => SetAnnotation(MongoAnnotationNames.DatabaseSettings, Check.NotNull(value, nameof(DatabaseSettings)));
        }
    }
}