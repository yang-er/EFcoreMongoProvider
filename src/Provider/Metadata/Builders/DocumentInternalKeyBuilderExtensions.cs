using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    public static class DocumentInternalKeyBuilderExtensions
    {
        public static IConventionKeyBuilder IsDocumentOwnershipKey(
            this IConventionKeyBuilder keyBuilder,
            bool isDocumentOwnershipKey)
        {
            var documentKeyAnnotations =
                Check.NotNull(keyBuilder, nameof(keyBuilder)).Document();
            documentKeyAnnotations.IsOwnershipKey = isDocumentOwnershipKey;

            return keyBuilder;
        }   
    }
}
