﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.EntityFrameworkCore.Mongo.ValueGeneration
{
    /// <inheritdoc />
    public class IntegerValueGenerator<TValue> : ValueGenerator<TValue>
    {
        private long _currentValue;

        /// <inheritdoc />
        /// <summary>
        ///     Generates a new <see cref="TValue"/> value.
        /// </summary>
        /// <param name="entry">The <see cref="EntityEntry"/> whose value is to be generated.</param>
        /// <returns>A new <see cref="TValue"/> for <see cref="EntityEntry"/>.</returns>
        public override TValue Next(EntityEntry entry)
            => (TValue) Convert.ChangeType(
                Interlocked.Increment(ref _currentValue),
                typeof(TValue),
                CultureInfo.InvariantCulture);

        /// <inheritdoc />
        /// <summary>
        ///     <code>true</code> if this <see cref="IntegerValueGenerator{TValue}"/> generates temporary values;
        ///     otherwise <code>false</code>.
        /// </summary>
        /// <remarks>Always returns <c>false</c>.</remarks>
        public override bool GeneratesTemporaryValues => false;
    }
}