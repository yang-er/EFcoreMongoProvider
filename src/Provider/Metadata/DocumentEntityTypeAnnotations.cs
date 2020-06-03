using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata
{
    public class DocumentEntityTypeAnnotations : DocumentAnnotations<IEntityType>
    {
        public DocumentEntityTypeAnnotations(IEntityType entityType)
            : base(entityType)
        {
        }

        public virtual bool IsComplexType
        {
            get => GetAnnotation<bool?>(DocumentAnnotationNames.IsComplexType) ?? false;
            set => SetAnnotation(DocumentAnnotationNames.IsComplexType, value);
        }
    }
}