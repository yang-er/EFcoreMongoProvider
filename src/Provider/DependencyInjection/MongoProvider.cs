using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Microsoft.EntityFrameworkCore.Mongo.DependencyInjection
{
    public class MongoProvider : IDbContextOptionsExtension, IDatabaseProvider
    {
        private MongoProviderInfo? _info;

        public MongoClientSettings Settings { get; }

        public string ConnectString { get; }

        public string DatabaseName { get; }

        public DbContextOptionsExtensionInfo Info => _info ??= new MongoProviderInfo(this);

        public string Name => "MongoDB";

        public MongoProvider(string connect, string databaseName)
        {
            ConnectString = Check.NotNull(connect, nameof(connect));
            Settings = MongoClientSettings.FromConnectionString(ConnectString);
            DatabaseName = databaseName;
        }

        public void ApplyServices(IServiceCollection services)
        {
            var builder = new MongoServiceBuilder(services);
            builder.TryAddCoreServices();
            services.AddSingleton<IDatabaseProvider>(this);
        }

        public void Validate(IDbContextOptions options)
        {
        }

        public bool IsConfigured(IDbContextOptions options)
        {
            return !string.IsNullOrEmpty(ConnectString) && !string.IsNullOrEmpty(DatabaseName);
        }
    }
}
