using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    public class MongoCollectionAttributeConvention : EntityTypeAttributeConventionBase<TableAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="MongoCollectionAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public MongoCollectionAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessEntityTypeAdded(
            IConventionEntityTypeBuilder entityTypeBuilder,
            TableAttribute attribute,
            IConventionContext<IConventionEntityTypeBuilder> context)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NotNull(attribute, nameof(attribute));
            entityTypeBuilder.MongoDb().CollectionName = attribute.Name;
        }
    }
}
