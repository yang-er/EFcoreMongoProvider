using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Driver;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata
{
    /// <inheritdoc />
    public class MongoEntityTypeAnnotations : DocumentEntityTypeAnnotations
    {
        /// <inheritdoc />
        public MongoEntityTypeAnnotations(IEntityType entityType)
            : base(entityType)
        {
        }

        public virtual bool AssignIdOnInsert
        {
            get => CollectionSettings?.AssignIdOnInsert ?? false;
            set => GetOrCreateCollectionSettings().AssignIdOnInsert = value;
        }

        public virtual string CollectionName
        {
            get => GetAnnotation<string>(MongoAnnotationNames.CollectionName)
                   ?? Utilities.Pluralize(Utilities.ToLowerCamelCase(Metadata.ClrType.Name));
            set => SetAnnotation(MongoAnnotationNames.CollectionName, Check.NotEmpty(value, nameof(CollectionName)));
        }

        public virtual MongoCollectionSettings CollectionSettings
        {
            get => GetAnnotation<MongoCollectionSettings>(MongoAnnotationNames.CollectionSettings);
            set => SetAnnotation(MongoAnnotationNames.CollectionSettings, Check.NotNull(value, nameof(CollectionSettings)));
        }

        public virtual string Discriminator
        {
            get => GetAnnotation<string>(MongoAnnotationNames.Discriminator) ?? Metadata.ClrType.Name;
            set => SetAnnotation(MongoAnnotationNames.Discriminator, Check.NotEmpty(value, nameof(Discriminator)));
        }

        public virtual bool DiscriminatorIsRequired
        {
            get => GetAnnotation<bool?>(MongoAnnotationNames.DiscriminatorIsRequired) ?? false;
            set => SetAnnotation(MongoAnnotationNames.DiscriminatorIsRequired, value);
        }

        public virtual bool IsDerivedType
        {
            get => GetAnnotation<bool?>(MongoAnnotationNames.IsDerivedType) ?? false;
            set => SetAnnotation(MongoAnnotationNames.IsDerivedType, value);
        }

        public virtual bool IsRootType
        {
            get => GetAnnotation<bool?>(MongoAnnotationNames.IsRootType) ?? false;
            set => SetAnnotation(MongoAnnotationNames.IsRootType, value);
        }

        private MongoCollectionSettings GetOrCreateCollectionSettings()
            => CollectionSettings ??= new MongoCollectionSettings();
    }
}