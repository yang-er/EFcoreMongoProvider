using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Mongo.Storage
{
    public class MongoContextTransactionManager2 : IDbContextTransactionManager
    {
        public IDbContextTransaction? CurrentTransaction => null;

        public IDbContextTransaction BeginTransaction()
        {
            throw new NotSupportedException();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromException<IDbContextTransaction>(new NotSupportedException());
        }

        public void CommitTransaction()
        {
            throw new NotSupportedException();
        }

        public void ResetState()
        {
            throw new NotSupportedException();
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromException(new NotSupportedException());
        }

        public void RollbackTransaction()
        {
            throw new NotSupportedException();
        }
    }
}
