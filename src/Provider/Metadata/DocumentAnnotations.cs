using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata
{
    public class DocumentAnnotations<TAnnotatable>
        where TAnnotatable : class, IAnnotatable
    {
        protected DocumentAnnotations(TAnnotatable metadata)
        {
            Annotatable = Check.Is<IMutableAnnotatable>(metadata, nameof(metadata));
            Metadata = Check.NotNull(metadata, nameof(metadata));
        }

        public virtual TAnnotatable Metadata { get; }

        protected virtual IMutableAnnotatable Annotatable { get; }

        public virtual T GetAnnotation<T>(string annotationName)
            => (T)Annotatable[Check.NullButNotEmpty(annotationName, nameof(annotationName))];

        public virtual bool SetAnnotation<T>(string annotationName, T value)
        {
            Check.NotEmpty(annotationName, nameof(annotationName));
            Annotatable[annotationName] = value;
            return true;
        }

        public virtual bool CanSetAnnotation(string annotationName, object value)
            => true;
    }
}