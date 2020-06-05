using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using System.Diagnostics;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    public static class DocumentInternalKeyBuilderExtensions
    {
        [DebuggerStepThrough]
        public static IConventionKeyBuilder IsDocumentOwnershipKey(
            this IConventionKeyBuilder keyBuilder,
            bool isDocumentOwnershipKey)
        {
            Check.NotNull(keyBuilder, nameof(keyBuilder))
                .Document().IsOwnershipKey = isDocumentOwnershipKey;
            return keyBuilder;
        }   
    }
}
