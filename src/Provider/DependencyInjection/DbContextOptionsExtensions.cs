using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Mongo;
using Microsoft.EntityFrameworkCore.Mongo.Adapter;
using Microsoft.EntityFrameworkCore.Mongo.DependencyInjection;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Bson;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class MongoDbContextOptionsExtensions
    {
        static MongoDbContextOptionsExtensions()
        {
            if (!typeof(ObjectId).GetTypeInfo().IsDefined(typeof(TypeConverterAttribute)))
                TypeDescriptor.AddAttributes(typeof(ObjectId), new TypeConverterAttribute(typeof(ObjectIdTypeConverter)));
            EntityFrameworkCoreConventionPack.Register(type => true);
        }

        public static DbContextOptionsBuilder UseMongo(this DbContextOptionsBuilder options, string connectUrl, string databaseName)
        {
            Check.NotNull(connectUrl, nameof(connectUrl));
            var mongo = new MongoProvider(connectUrl, databaseName);
            ((IDbContextOptionsBuilderInfrastructure)options).AddOrUpdateExtension(mongo);
            return options;
        }
    }
}
