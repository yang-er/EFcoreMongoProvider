using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders
{
    /// <summary>
    ///     <para>
    ///         Service dependencies parameter class for <see cref="MongoConventionSetBuilder" />
    ///     </para>
    ///     <para>
    ///         This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///         directly from your code. This API may change or be removed in future releases.
    ///     </para>
    ///     <para>
    ///         Do not construct instances of this class directly from either provider or application code as the
    ///         constructor signature may change as new dependencies are added. Instead, use this type in 
    ///         your constructor so that an instance will be created and injected automatically by the 
    ///         dependency injection container. To create an instance with some dependent services replaced, 
    ///         first resolve the object from the dependency injection container, then replace selected 
    ///         services using the 'With...' methods. Do not call the constructor at any point in this process.
    ///     </para>
    /// </summary>
    public class MongoConventionSetBuilderDependencies
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoConventionSetBuilderDependencies"/> class.
        /// </summary>
        /// <param name="currentDbContext">Indirection to the current <see cref="DbContext" /> instance.</param>
        /// <param name="typeMappingSource">Maps .NET types to their corresponding database provider types.</param>
        /// <param name="memberClassifier">Determines the property types for candidate navigation properties.</param>
        /// <param name="modelLogger">Traces the process of building a <see cref="Model"/>.</param>
        public MongoConventionSetBuilderDependencies(
            ICurrentDbContext currentDbContext,
            ITypeMappingSource typeMappingSource,
            IMemberClassifier memberClassifier,
            IDiagnosticsLogger<DbLoggerCategory.Model> modelLogger)
        {
            CurrentDbContext = Check.NotNull(currentDbContext, nameof(currentDbContext));
            MemberClassifier = Check.NotNull(memberClassifier, nameof(memberClassifier));
            ModelLogger = Check.NotNull(modelLogger, nameof(modelLogger));
            TypeMappingSource = Check.NotNull(typeMappingSource, nameof(typeMappingSource));
        }

        /// <summary>
        ///     Indirection to the current <see cref="DbContext" /> instance.
        /// </summary>
        public ICurrentDbContext CurrentDbContext { get; }

        /// <summary>
        /// The <see cref="IMemberClassifier"/> to use to determine the property types for candidate navigation properties.
        /// </summary>
        public IMemberClassifier MemberClassifier { get; set; }

        /// <summary>
        /// The <see cref="IDiagnosticsLogger{TLoggerCategory}"/> used for tracing the process of building a <see cref="Model"/>.
        /// </summary>
        public IDiagnosticsLogger<DbLoggerCategory.Model> ModelLogger { get; set; }

        /// <summary>
        /// Maps .NET types to their corresponding database provider types.
        /// </summary>
        public ITypeMappingSource TypeMappingSource { get; }
    }
}
