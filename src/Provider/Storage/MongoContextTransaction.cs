using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Mongo.Storage
{
    public class MongoContextTransaction : IDbContextTransaction
    {
        public IClientSessionHandle Handle { get; }

        public MongoContextTransaction(IClientSessionHandle handle)
        {
            Handle = handle;
        }

        public Guid TransactionId => throw new NotImplementedException();

        public void Commit()
        {
            Handle.CommitTransaction();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Handle.CommitTransactionAsync(cancellationToken);
        }

        public void Dispose()
        {
            Handle.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return Handle.DisposeAsyncIfAvailable();
        }

        public void Rollback()
        {
            Handle.AbortTransaction();
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return Handle.AbortTransactionAsync(cancellationToken);
        }
    }
}
