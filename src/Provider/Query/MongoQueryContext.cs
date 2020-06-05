using Microsoft.EntityFrameworkCore.Mongo.Storage;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class MongoQueryContext : QueryContext
    {
        public MongoQueryContext(
            QueryContextDependencies dependencies,
            IMongoConnection cosmosClient)
            : base(dependencies)
        {
            MongoClient = cosmosClient;
        }

        public virtual IMongoConnection MongoClient { get; }
    }
}
