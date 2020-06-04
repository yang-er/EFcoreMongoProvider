using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Mongo.DependencyInjection;
using Microsoft.EntityFrameworkCore.Mongo.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class MongoDatabaseFacadeExtensions
    {
        public static IMongoClient GetMongoClient(this DatabaseFacade databaseFacade)
            => GetService<IMongoConnection>(databaseFacade).Client;

        private static TService GetService<TService>(IInfrastructure<IServiceProvider> databaseFacade)
        {
            Check.NotNull(databaseFacade, nameof(databaseFacade));
            var service = databaseFacade.Instance.GetService<TService>();
            if (service == null)
                throw new InvalidOperationException("MongoDb is not in use.");
            return service;
        }

        /// <summary>
        ///     <para>
        ///         Returns <c>true</c> if the database provider currently in use is the Mongo provider.
        ///     </para>
        ///     <para>
        ///         This method can only be used after the <see cref="DbContext" /> has been configured because
        ///         it is only then that the provider is known. This means that this method cannot be used
        ///         in <see cref="DbContext.OnConfiguring" /> because this is where application code sets the
        ///         provider to use as part of configuring the context.
        ///     </para>
        /// </summary>
        /// <param name="database"> The facade from <see cref="DbContext.Database" />. </param>
        /// <returns> <c>true</c> if the Mongo provider is being used. </returns>
        public static bool IsMongo(this DatabaseFacade database)
            => database.ProviderName.Equals(
                typeof(MongoProvider).GetTypeInfo().Assembly.GetName().Name,
                StringComparison.Ordinal);
    }
}
