using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Linq.Expressions;
using System.Threading;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class MongoQueryCompiler : IQueryCompiler
    {
        public Func<QueryContext, TResult> CreateCompiledAsyncQuery<TResult>(Expression query)
        {
            throw new NotImplementedException();
        }

        public Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression query)
        {
            throw new NotImplementedException();
        }

        public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
