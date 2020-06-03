using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata
{
    public class DocumentKeyAnnotations : DocumentAnnotations<IKey>
    {
        public DocumentKeyAnnotations(IKey key)
            : base(key)
        {
        }

        public virtual bool IsOwnershipKey
        {
            get => GetAnnotation<bool?>(DocumentAnnotationNames.IsOwnershipKey) ?? false;
            set => SetAnnotation(DocumentAnnotationNames.IsOwnershipKey, value);
        }
    }
}