using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Conventions
{
    /// <inheritdoc />
    public class MongoDatabaseGeneratedAttributeConvention : DatabaseGeneratedAttributeConvention
    {
        /// <summary>
        ///     Creates a new instance of <see cref="MongoDatabaseGeneratedAttributeConvention" />.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this convention. </param>
        public MongoDatabaseGeneratedAttributeConvention(ProviderConventionSetBuilderDependencies dependencies)
            : base(dependencies)
        {
        }

        /// <inheritdoc />
        protected override void ProcessPropertyAdded(
            IConventionPropertyBuilder propertyBuilder,
            DatabaseGeneratedAttribute attribute,
            MemberInfo clrMember,
            IConventionContext context)
        {
            if (attribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
            {
                propertyBuilder.Metadata
                    .DeclaringEntityType
                    .MongoDb()
                    .AssignIdOnInsert = true;
            }

            base.ProcessPropertyAdded(propertyBuilder, attribute, clrMember, context);
        }
    }
}