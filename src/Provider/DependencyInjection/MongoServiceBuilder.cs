﻿using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Mongo.Adapter.Update;
using Microsoft.EntityFrameworkCore.Mongo.ChangeTracking;
using Microsoft.EntityFrameworkCore.Mongo.Diagnostics;
using Microsoft.EntityFrameworkCore.Mongo.Infrastructure;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Mongo.Query;
using Microsoft.EntityFrameworkCore.Mongo.Storage;
using Microsoft.EntityFrameworkCore.Mongo.ValueGeneration;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Mongo.DependencyInjection
{
    public class MongoServiceBuilder : EntityFrameworkServicesBuilder
    {
        public MongoServiceBuilder(IServiceCollection services) : base(services) { }

        public override EntityFrameworkServicesBuilder TryAddCoreServices()
        {
            TryAddProviderSpecificServices(map =>
            {
                map.TryAddScoped<IDatabase, MongoDatabaseWrapper>();
                map.TryAddScoped<IDatabaseCreator, MongoDatabaseCreator>();
                map.TryAddScoped<IMongoConnection, MongoConnection>();
                map.TryAddScoped<IQueryCompiler, MongoQueryCompiler>();
                map.TryAddScoped<IQueryCompilationContextFactory, MongoQueryCompilationContextFactory>();
                map.TryAddScoped<IValueGeneratorSelector, MongoValueGeneratorSelector>();
                map.TryAddScoped<WriteModelFactorySelector, WriteModelFactorySelector>();
                map.TryAddScoped<WriteModelFactoryCache, WriteModelFactoryCache>();
                map.TryAddScoped<IProviderConventionSetBuilder, MongoConventionSetBuilder>();
                map.TryAddScoped<MongoConventionSetBuilderDependencies, MongoConventionSetBuilderDependencies>();
                map.TryAddSingleton<ITypeMappingSource, MongoTypeMappingSource>();
                map.TryAddSingleton<IInternalEntityEntryFactory, MongoEntityEntryFactory>();
                map.TryAddSingleton<IModelValidator, MongoModelValidator>();
                map.TryAddSingleton<LoggingDefinitions, MongoLoggingDefinitions>();
            });

            return base.TryAddCoreServices();
        }
    }
}
