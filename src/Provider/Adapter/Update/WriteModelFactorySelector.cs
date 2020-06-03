using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using MongoDB.Driver;
using System;

namespace Microsoft.EntityFrameworkCore.Mongo.Adapter.Update
{
    /// <summary>
    /// Selects an instance of <see cref="WriteModelFactory{TEntity}"/> for a given <see cref="EntityType"/>.
    /// </summary>
    public class WriteModelFactorySelector
    {
        private readonly IValueGeneratorSelector _valueGeneratorSelector;
        private readonly WriteModelFactoryCache _mongoDbWriteModelFactoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteModelFactorySelector"/> class.
        /// </summary>
        /// <param name="valueGeneratorSelector">The <see cref="IValueGeneratorSelector"/> to use for populating concurrency tokens.</param>
        /// <param name="mongoDbWriteModelFactoryCache">A <see cref="WriteModelFactoryCache"/> that can be used to cache the
        /// factory instances returned by this <see cref="WriteModelFactorySelector"/>.</param>
        public WriteModelFactorySelector(
            IValueGeneratorSelector valueGeneratorSelector,
            WriteModelFactoryCache mongoDbWriteModelFactoryCache)
        {
            _valueGeneratorSelector = Check.NotNull(valueGeneratorSelector, nameof(valueGeneratorSelector));
            _mongoDbWriteModelFactoryCache = Check.NotNull(mongoDbWriteModelFactoryCache, nameof(mongoDbWriteModelFactoryCache));
        }

        /// <summary>
        /// Select an <see cref="WriteModelFactory{TEntity}"/> instance for the given <paramref name="updateEntry"/>.
        /// </summary>
        /// <param name="updateEntry">The <see cref="IUpdateEntry"/> that the write model factory will be used to translate.</param>
        /// <typeparam name="TEntity">The type of entity for which to create a <see cref="WriteModelFactory{TEntity}"/> instance.</typeparam>
        /// <returns>An instance of <see cref="WriteModelFactory{TEntity}"/> that can be used to convert <see cref="IUpdateEntry"/>
        /// instances to <see cref="WriteModel{TDocument}"/> instances.</returns>
        public WriteModelFactory<TEntity> Select<TEntity>(IUpdateEntry updateEntry)
            => _mongoDbWriteModelFactoryCache.GetOrAdd(
                Check.NotNull(updateEntry, nameof(updateEntry)).EntityType,
                updateEntry.EntityState,
                Create<TEntity>);

        private WriteModelFactory<TEntity> Create<TEntity>(
            IEntityType entityType,
            EntityState entityState)
        {
            Check.NotNull(entityType, nameof(entityType));
            if (entityState != EntityState.Added &&
                entityState != EntityState.Modified &&
                entityState != EntityState.Unchanged &&
                entityState != EntityState.Deleted)
            {
                throw new InvalidOperationException($"The value provided for entityState must be Added, Modified, Unchanged, or Deleted, but was {entityState}.");
            }

            return entityState switch
            {
                EntityState.Added => new InsertOneModelFactory<TEntity>(_valueGeneratorSelector, entityType),
                EntityState.Deleted => new DeleteOneModelFactory<TEntity>(_valueGeneratorSelector, entityType),
                _ => new ReplaceOneModelFactory<TEntity>(_valueGeneratorSelector, entityType),
            };
        }
    }
}