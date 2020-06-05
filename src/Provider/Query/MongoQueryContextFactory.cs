using Microsoft.EntityFrameworkCore.Mongo.Storage;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class MongoQueryContextFactory : IQueryContextFactory
    {
        private readonly QueryContextDependencies _dependencies;
        private readonly IMongoConnection _mongoClient;

        public MongoQueryContextFactory(
            QueryContextDependencies dependencies,
            IMongoConnection cosmosClient)
        {
            _dependencies = dependencies;
            _mongoClient = cosmosClient;
        }

        public virtual QueryContext Create()
            => new MongoQueryContext(_dependencies, _mongoClient);
    }
}
