using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Mongo.Storage
{
    public class MongoContextTransactionManager : IDbContextTransactionManager
    {
        private IDbContextTransaction? innerTransaction;

        public IMongoConnection Connection { get; }

        public MongoContextTransactionManager(IMongoConnection connection)
        {
            Connection = connection;
        }

        public IDbContextTransaction? CurrentTransaction => innerTransaction;

        public IDbContextTransaction BeginTransaction()
        {
            if (CurrentTransaction != null)
                throw new InvalidOperationException("Transaction already started.");
            var session = Connection.Client.StartSession();
            session.StartTransaction();
            if (Interlocked.Exchange(ref innerTransaction, new MongoContextTransaction(session)) == null)
                throw new InvalidOperationException("Concurrency error occurred.");
            return innerTransaction!;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(BeginTransaction());
        }

        public void CommitTransaction()
        {
            if (CurrentTransaction == null)
                throw new InvalidOperationException("Transaction not started.");
            CurrentTransaction.Commit();
        }

        public void ResetState()
        {
            if (CurrentTransaction == null)
                throw new InvalidOperationException("Transaction not started.");
            CurrentTransaction.Dispose();
            innerTransaction = null;
        }

        public async Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentTransaction == null)
                throw new InvalidOperationException("Transaction not started.");
            await CurrentTransaction.DisposeAsync();
            innerTransaction = null;
        }

        public void RollbackTransaction()
        {
            if (CurrentTransaction == null)
                throw new InvalidOperationException("Transaction not started.");
            CurrentTransaction.Rollback();
        }
    }
}
