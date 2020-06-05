using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class MongoQueryCompilationContextFactory : IQueryCompilationContextFactory
    {
        public MongoQueryCompilationContextFactory(
            QueryCompilationContextDependencies dependencies)
        {
            Dependencies = dependencies;
        }

        protected QueryCompilationContextDependencies Dependencies { get; }

        public QueryCompilationContext Create(bool async)
            => new MongoQueryCompilationContext(Dependencies, async);
    }
}
