namespace Microsoft.EntityFrameworkCore.Mongo.Metadata
{
    public static class MongoAnnotationNames
    {
        private const string Prefix = "MongoDb:";
        public const string CollectionName = Prefix + nameof(CollectionName);
        public const string CollectionSettings = Prefix + nameof(CollectionSettings);
        public const string Database = Prefix + nameof(Database);
        public const string DatabaseSettings = Prefix + nameof(DatabaseSettings);
        public const string Discriminator = Prefix + nameof(Discriminator);
        public const string DiscriminatorIsRequired = Prefix + nameof(DiscriminatorIsRequired);
        public const string IsDerivedType = Prefix + nameof(IsDerivedType);
        public const string IsRootType = Prefix + nameof(IsRootType);
        public const string Namespace = Prefix + nameof(Namespace);
        public const string NavigationName = Prefix + nameof(NavigationName);
    }
}