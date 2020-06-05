using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using System.Diagnostics;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    public static class DocumentInternalMetadataBuilderExtensions
    {
        [DebuggerStepThrough]
        public static DocumentEntityTypeAnnotations Document(this IConventionEntityTypeBuilder internalEntityTypeBuilder)
            => Check.NotNull(internalEntityTypeBuilder, nameof(internalEntityTypeBuilder)).Metadata.Document();

        [DebuggerStepThrough]
        public static DocumentEntityTypeAnnotations Document(this EntityTypeBuilder entityTypeBuilder)
            => Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder)).Metadata.Document();

        [DebuggerStepThrough]
        public static DocumentEntityTypeAnnotations Document(this EntityType entityType)
            => Check.NotNull<IEntityType>(entityType, nameof(entityType)).Document();

        [DebuggerStepThrough]
        public static DocumentEntityTypeAnnotations Document(this IEntityType entityType)
            => new DocumentEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)));

        [DebuggerStepThrough]
        public static DocumentKeyAnnotations Document(this IConventionKeyBuilder keyBuilder)
            => Check.NotNull(keyBuilder, nameof(keyBuilder)).Metadata.Document();

        [DebuggerStepThrough]
        public static DocumentKeyAnnotations Document(this Key key)
            => Check.NotNull<IKey>(key, nameof(key)).Document();

        [DebuggerStepThrough]
        public static DocumentKeyAnnotations Document(this IKey key)
            => new DocumentKeyAnnotations(Check.NotNull(key, nameof(key)));
    }
}