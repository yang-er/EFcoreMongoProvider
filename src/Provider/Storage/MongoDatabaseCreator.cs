using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Mongo.Storage
{
    public class MongoDatabaseCreator : IDatabaseCreator
    {
        public IMongoConnection Connection { get; }

        public IModel Model { get; }

        public MongoDatabaseCreator(IMongoConnection connection, IModel model)
        {
            Connection = Check.NotNull(connection, nameof(connection));
            Model = Check.NotNull(model, nameof(model));
        }

        public bool CanConnect()
        {
            try { Connection.Client.ListDatabaseNames().Dispose(); return true; }
            catch { return false; }
        }

        public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
        {
            try { (await Connection.Client.ListDatabaseNamesAsync(cancellationToken)).Dispose(); return true; }
            catch { return false; }
        }

        public bool EnsureCreated()
        {
            try { Connection.Database.ListCollectionNames().Dispose(); return true; }
            catch { return false; }
        }

        public async Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            try { (await Connection.Database.ListCollectionNamesAsync(null, cancellationToken)).Dispose(); return true; }
            catch { return false; }
        }

        public bool EnsureDeleted()
        {
            Connection.Client.DropDatabase(Connection.Name);
            return true;
        }

        public async Task<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            await Connection.Client.DropDatabaseAsync(Connection.Name, cancellationToken);
            return true;
        }
    }
}
