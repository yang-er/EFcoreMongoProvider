using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class MongoQueryCompilationContext : QueryCompilationContext
    {
        public MongoQueryCompilationContext(
            QueryCompilationContextDependencies dependencies,
            bool async)
            : base(dependencies, async)
        {
        }
    }
}
