using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Mongo.DependencyInjection;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    /// <inheritdoc />
    public class MongoDatabaseConvention : IModelInitializedConvention
    {
        private readonly DbContext _dbContext;
        private readonly MongoProvider _provider;

        public MongoDatabaseConvention(DbContext dbContext)
        {
            _dbContext = Check.NotNull(dbContext, nameof(dbContext));
            _provider = _dbContext
                .GetService<IDbContextServices>()
                .ContextOptions
                .FindExtension<MongoProvider>();
        }

        /// <inheritdoc />
        public void ProcessModelInitialized(
            IConventionModelBuilder modelBuilder,
            IConventionContext<IConventionModelBuilder> context)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            modelBuilder.MongoDb().Database = _provider.DatabaseName!;
        }
    }
}