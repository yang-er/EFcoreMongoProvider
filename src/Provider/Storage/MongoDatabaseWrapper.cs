using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Mongo.Adapter.Update;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Mongo.Storage
{
    public class MongoDatabaseWrapper : Database
    {
        private static readonly MethodInfo GenericUpdateEntries =
            MethodHelper.GetGenericMethodDefinition(
                (MongoDatabaseWrapper here) => here.UpdateEntries<object>(null!));

        private static readonly MethodInfo GenericUpdateEntriesAsync =
            MethodHelper.GetGenericMethodDefinition(
                (MongoDatabaseWrapper here) => here.UpdateEntriesAsync<object>(null!, CancellationToken.None));

        private readonly IMongoConnection _db;
        private readonly WriteModelFactorySelector _mongoDbWriteModelFactorySelector;

        /// <summary>
        /// Initializes a new instance of hte <see cref="MongoDatabaseWrapper"/> class.
        /// </summary>
        /// <param name="databaseDependencies">Parameter object containing dependencies for this service.</param>
        /// <param name="mongoClient">A <see cref="IMongoClient"/> used to communicate with the MongoDB instance.</param>
        /// <param name="writeModelFactorySelector">The <see cref="WriteModelFactorySelector"/> to use to create
        /// <see cref="WriteModelFactory{TEntity}"/> instances.</param>
        public MongoDatabaseWrapper(
            DatabaseDependencies databaseDependencies,
            IMongoConnection mongoClient,
            WriteModelFactorySelector writeModelFactorySelector)
            : base(Check.NotNull(databaseDependencies, nameof(databaseDependencies)))
        {
            _db = Check.NotNull(mongoClient, nameof(mongoClient));
            _mongoDbWriteModelFactorySelector = Check.NotNull(writeModelFactorySelector, nameof(writeModelFactorySelector));
        }

        /// <summary>
        /// Persists changes from the supplied entries to the database.
        /// </summary>
        /// <param name="entries">A list of entries to be persisted.</param>
        /// <returns>The number of entries that were persisted.</returns>
        public override int SaveChanges(IList<IUpdateEntry> entries)
            => GetDocumentUpdateDefinitions(entries)
                .ToLookup(entry => entry.EntityType.GetCollectionEntityType())
                .Sum(grouping => (int)GenericUpdateEntries.MakeGenericMethod(grouping.Key.ClrType)
                    .Invoke(this, new object[] { grouping }));

        private int UpdateEntries<TEntity>(IEnumerable<IUpdateEntry> entries)
        {
            var writeModels = entries
                .Select(entry => _mongoDbWriteModelFactorySelector.Select<TEntity>(entry).CreateWriteModel(entry))
                .ToList();
            var result = _db.GetCollection<TEntity>()
                .BulkWrite(writeModels);
            return (int)(result.DeletedCount + result.InsertedCount + result.ModifiedCount);
        }

        private ISet<IUpdateEntry> GetDocumentUpdateDefinitions(ICollection<IUpdateEntry> entries)
        {
            Check.NotNull(entries, nameof(entries));
            var rootEntries = new HashSet<IUpdateEntry>();

            foreach (var updateEntry in entries)
            {
                if (updateEntry.EntityType.IsDocumentRootEntityType())
                {
                    rootEntries.Add(updateEntry);
                }
                else if (updateEntry is InternalEntityEntry internalEntityEntry)
                {
                    rootEntries.Add(GetRootDocument(internalEntityEntry));
                }
                else
                {
                    // TBD - throw error
                }
            }

            return rootEntries;
        }

        /// <summary>
        /// Asynchronously persists changes from the supplied entries to the database.
        /// </summary>
        /// <param name="entries">A list of entries to be persisted.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken "/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the state of the operation. The result contains the number
        /// of entries that were persisted to the database.
        /// </returns>
        public override async Task<int> SaveChangesAsync(
            IList<IUpdateEntry> entries,
            CancellationToken cancellationToken = default)
        {
            var tasks = GetDocumentUpdateDefinitions(entries)
                .ToLookup(entry => entry.EntityType.GetCollectionEntityType())
                .Select(async grouping => await InvokeUpdateEntriesAsync(grouping, cancellationToken))
                .ToList();

            int[] totals = await Task.WhenAll(tasks);
            return totals.Sum();
        }

        public override Func<QueryContext, TResult> CompileQuery<TResult>(Expression query, bool async)
        {
            return base.CompileQuery<TResult>(query, async);
        }

        private Task<int> InvokeUpdateEntriesAsync(
            IGrouping<IEntityType, IUpdateEntry> entryGrouping,
            CancellationToken cancellationToken)
        {
            return (Task<int>)GenericUpdateEntriesAsync.MakeGenericMethod(entryGrouping.Key.ClrType)
                .Invoke(this, new object[] { entryGrouping, cancellationToken });
        }

        private async Task<int> UpdateEntriesAsync<TEntity>(
            IEnumerable<IUpdateEntry> entries,
            CancellationToken cancellationToken)
        {
            var writeModels = entries
                .Select(entry => _mongoDbWriteModelFactorySelector.Select<TEntity>(entry).CreateWriteModel(entry))
                .ToList();
            var result = await _db.GetCollection<TEntity>()
                .BulkWriteAsync(writeModels,
                    options: null,
                    cancellationToken: cancellationToken);
            return (int)(result.DeletedCount + result.InsertedCount + result.ModifiedCount);
        }

        private IUpdateEntry GetRootDocument(InternalEntityEntry entry)
        {
            var stateManager = entry.StateManager;

            var owningEntityEntry = stateManager
                .GetDependents(entry, entry.EntityType.FindOwnership())
                .SingleOrDefault(owner => owner != null);

            if (owningEntityEntry == null)
                throw new InvalidOperationException($"Encountered orphaned document of type {entry.EntityType.DisplayName()}.");

            return owningEntityEntry.EntityType.IsDocumentRootEntityType()
                ? owningEntityEntry
                : GetRootDocument(owningEntityEntry);
        }
    }
}
