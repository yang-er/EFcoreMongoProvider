using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Bson.Serialization.Attributes;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    /// <inheritdoc />
    public class BsonDiscriminatorAttributeConvention : EntityTypeAttributeConventionBase<BsonDiscriminatorAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="BsonDiscriminatorAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public BsonDiscriminatorAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            BsonDiscriminatorAttribute attribute,
            IConventionContext<IConventionEntityTypeBuilder> context)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NotNull(attribute, nameof(attribute));
            MongoEntityTypeAnnotations annotations = entityTypeBuilder.MongoDb();
            if (!string.IsNullOrWhiteSpace(attribute.Discriminator))
            {
                annotations.Discriminator = attribute.Discriminator;
            }

            if (!annotations.DiscriminatorIsRequired)
            {
                annotations.DiscriminatorIsRequired = attribute.Required;
            }

            annotations.IsRootType = attribute.RootClass;
        }
    }
}