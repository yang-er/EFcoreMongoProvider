using Microsoft.EntityFrameworkCore.Query;
using System;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class MongoQueryCompilationContextFactory : IQueryCompilationContextFactory
    {
        public QueryCompilationContext Create(bool async)
        {
            throw new InvalidOperationException();
        }
    }
}
