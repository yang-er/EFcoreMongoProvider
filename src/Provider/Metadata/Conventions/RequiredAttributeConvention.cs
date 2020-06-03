using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class BsonRequiredAttributeConvention : PropertyAttributeConventionBase<BsonRequiredAttribute>
    {
        /// <summary>
        ///     Creates a new instance of <see cref="BsonRequiredAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public BsonRequiredAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessPropertyAdded(
            IConventionPropertyBuilder propertyBuilder,
            BsonRequiredAttribute attribute,
            MemberInfo clrMember,
            IConventionContext context)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NotNull(attribute, nameof(attribute));
            Check.NotNull(clrMember, nameof(clrMember));

            propertyBuilder.IsRequired(true, fromDataAnnotation: true);
        }
    }
}