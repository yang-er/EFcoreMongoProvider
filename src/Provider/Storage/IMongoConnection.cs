using MongoDB.Driver;

namespace Microsoft.EntityFrameworkCore.Mongo.Storage
{
    /// <summary>
    /// The connection interface exposed to DatabaseCreator and DatabaseWrapper.
    /// </summary>
    public interface IMongoConnection
    {
        /// <summary>
        /// The client to link to MongoDB clusters.
        /// </summary>
        IMongoClient Client { get; }

        /// <summary>
        /// The database for this DbContext.
        /// </summary>
        IMongoDatabase Database { get; }

        /// <summary>
        /// The database name in MongoDB.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the corresponding entity collection.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <returns>The collection for this Entity Type.</returns>
        IMongoCollection<TEntity> GetCollection<TEntity>();
    }
}
