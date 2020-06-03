namespace Microsoft.EntityFrameworkCore.Mongo.Metadata
{
    public static class DocumentAnnotationNames
    {
        private const string Prefix = "Document:";
        public const string IsComplexType = Prefix + nameof(IsComplexType);
        public const string IsOwnershipKey = Prefix + nameof(IsOwnershipKey);
    }
}