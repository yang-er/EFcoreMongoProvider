using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    /// <inheritdoc />
    public class BsonKnownTypesAttributeConvention : EntityTypeAttributeConventionBase<BsonKnownTypesAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="BsonKnownTypesAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public BsonKnownTypesAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        public override void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            IConventionContext<IConventionEntityTypeBuilder> context)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            var type = entityTypeBuilder.Metadata.ClrType;
            if (type == null
                || !Attribute.IsDefined(type, typeof(BsonKnownTypesAttribute), inherit: false))
            {
                return;
            }

            var attributes = type.GetTypeInfo().GetCustomAttributes<BsonKnownTypesAttribute>(false);

            foreach (var attribute in attributes)
            {
                ProcessEntityTypeAdded(entityTypeBuilder, attribute, context);
                if (((IReadableConventionContext)context).ShouldStopProcessing())
                {
                    return;
                }
            }
        }

        /// <inheritdoc />
        protected override void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            BsonKnownTypesAttribute attribute,
            IConventionContext<IConventionEntityTypeBuilder> context)
        {
            MongoEntityTypeAnnotations annotations = entityTypeBuilder.MongoDb();
            if (!annotations.DiscriminatorIsRequired)
            {
                annotations.DiscriminatorIsRequired = entityTypeBuilder.Metadata.IsAbstract();
            }

            if (attribute.KnownTypes != null)
            {
                IConventionModelBuilder modelBuilder = entityTypeBuilder.ModelBuilder;
                Type baseType = entityTypeBuilder.Metadata.ClrType;

                foreach (Type derivedType in attribute.KnownTypes)
                {
                    if (!baseType.IsAssignableFrom(derivedType))
                    {
                        throw new InvalidOperationException($"Known type {derivedType} declared on base type {baseType} does not inherit from base type.");
                    }

                    modelBuilder
                        .Entity(derivedType, fromDataAnnotation: true)
                        .MongoDb()
                        .IsDerivedType = true;
                }
            }
        }
    }
}
