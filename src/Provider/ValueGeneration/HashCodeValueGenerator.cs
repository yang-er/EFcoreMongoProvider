using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Microsoft.EntityFrameworkCore.Mongo.ValueGeneration
{
    /// <inheritdoc />
    public class HashCodeValueGenerator : ValueGenerator<int?>
    {
        /// <inheritdoc />
        /// <summary>
        ///     Generates a new <see cref="int?"/> value.
        /// </summary>
        /// <param name="entry">The <see cref="EntityEntry"/> whose value is to be generated.</param>
        /// <returns>A new <see cref="int?"/> for <see cref="EntityEntry"/>.</returns>
        public override int? Next(EntityEntry entry)
            => entry.Entity?.GetHashCode();

        /// <inheritdoc />
        /// <summary>
        ///     <code>true</code> if this <see cref="HashCodeValueGenerator"/> generates temporary values;
        ///     otherwise <code>false</code>.
        /// </summary>
        /// <remarks>Always returns <c>true</c>.</remarks>
        public override bool GeneratesTemporaryValues => false;
    }
}