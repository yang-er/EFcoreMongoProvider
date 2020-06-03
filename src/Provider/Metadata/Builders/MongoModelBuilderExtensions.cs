﻿using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    /// <summary>
    ///     MongoDb-specific extension methods for <see cref="ModelBuilder" />.
    /// </summary>
    public static class MongoModelBuilderExtensions
    {
        /// <summary>
        ///     Configures the database to use when connecting to MongoDb.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder"/> to configure.</param>
        /// <param name="database">The name of the database.</param>
        /// <returns>This <see cref="ModelBuilder"/>, such that calls can be chained.</returns>
        public static ModelBuilder ForMongoDbFromDatabase(this ModelBuilder modelBuilder, string database)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            modelBuilder.Model.MongoDb().Database = database;
            return modelBuilder;
        }
    }
}