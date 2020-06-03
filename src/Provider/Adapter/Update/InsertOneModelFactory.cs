using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using MongoDB.Driver;

namespace Microsoft.EntityFrameworkCore.Mongo.Adapter.Update
{
    /// <summary>
    /// Creates <see cref="InsertOneModel{TEntity}" /> from a given <see cref="IUpdateEntry" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being added.</typeparam>
    public class InsertOneModelFactory<TEntity> : WriteModelFactory<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteModelFactory{TEntity}" /> class.
        /// </summary>
        /// <param name="valueGeneratorSelector">The <see cref="IValueGeneratorSelector" /> to use for populating concurrency tokens.</param>
        /// <param name="entityType">The <see cref="IEntityType" /> for which this <see cref="WriteModelFactory{TEntity}" /> will be used.</param>
        public InsertOneModelFactory(
            IValueGeneratorSelector valueGeneratorSelector,
            IEntityType entityType)
            : base(
                  Check.NotNull(valueGeneratorSelector, nameof(valueGeneratorSelector)),
                  Check.NotNull(entityType, nameof(entityType)))
        {
        }

        /// <summary>
        /// Creates an <see cref="InsertOneModel{TEntity}" /> that maps the given <paramref name="updateEntry" />.
        /// </summary>
        /// <param name="updateEntry">The <see cref="IUpdateEntry" /> to map.</param>
        /// <returns>A new <see cref="InsertOneModel{TEntity}" /> containing the inserted values represented
        /// by <paramref name="updateEntry" />.</returns>
        public override WriteModel<TEntity> CreateWriteModel(IUpdateEntry updateEntry)
        {
            InternalEntityEntry internalEntityEntry = Check.Is<InternalEntityEntry>(updateEntry, nameof(updateEntry));
            UpdateDbGeneratedProperties(internalEntityEntry);
            return new InsertOneModel<TEntity>((TEntity)internalEntityEntry.Entity);
        }
    }
}