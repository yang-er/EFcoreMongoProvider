using Microsoft.EntityFrameworkCore.Mongo.Adapter.Conventions;
using MongoDB.Bson.Serialization.Conventions;
using System;

namespace Microsoft.EntityFrameworkCore.Mongo.Adapter
{
    /// <inheritdoc />
    /// <summary>
    /// Provides a set of conventions that configures the MongoDb C# Driver to work appropriately with the EntityFrameworkCore.
    /// </summary>
    public class EntityFrameworkCoreConventionPack : ConventionPack
    {
        /// <summary>
        /// Registers the <see cref="EntityFrameworkCoreConventionPack"/>.
        /// </summary>
        /// <param name="typeFilter"></param>
        public static void Register(Func<Type, bool> typeFilter)
        {
            ConventionRegistry.Register(
                "Microsoft.EntityFrameworkCore.Mongo.Conventions",
                Instance,
                typeFilter);
        }

        /// <summary>
        /// The singleton instance of <see cref="EntityFrameworkCoreConventionPack"/>.
        /// </summary>
        public static EntityFrameworkCoreConventionPack Instance { get; } = new EntityFrameworkCoreConventionPack();

        private EntityFrameworkCoreConventionPack()
        {
            AddRange(new IConvention[]
            {
                new AbstractBaseClassConvention(),
                new KeyAttributeConvention(),
                new NotMappedAttributeConvention()
            });
        }
    }
}