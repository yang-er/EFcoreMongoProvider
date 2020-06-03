using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.EntityFrameworkCore.Mongo
{
    /// <summary>
    /// A set of utilities to assist with MongoDb values.
    /// </summary>
    public static class Utilities
    {
        private static readonly Regex LeadingUppercaseRegex
            = new Regex(pattern: "^(([A-Z](?![a-z]))+|([A-Z](?=[a-z])))", options: RegexOptions.Compiled);
        private static readonly Regex SingularRegex
            = new Regex(pattern: "(ey|.)(?<!s)$", options: RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Converts the first character or group of capital characters of a string to lower camel case.
        /// </summary>
        /// <param name="value">The string to convert to lower camel case.</param>
        /// <returns>A string containing a lower camel case version of the original <paramref name="value"/>.</returns>
        public static string ToLowerCamelCase(string value)
            => LeadingUppercaseRegex.Replace(
                Check.NotNull(value, nameof(value)), match => match.Value.ToLower());

        /// <summary>
        /// Pluralizes a string by replacing any trailing 'y' with 'ies'.
        /// </summary>
        /// <param name="value">The string to pluralize.</param>
        /// <returns>A pluralized version of the original <paramref name="value"/>.</returns>
        public static string Pluralize(string value)
            => SingularRegex.Replace(
                Check.NotNull(value, nameof(value)),
                match => string.Equals(a: "y", b: match.Value, comparisonType: StringComparison.OrdinalIgnoreCase)
                    ? "ies"
                    : $"{match.Value}s");

        /// <summary>
        /// Determines whether or not the current <see cref="IEntityType"/> represents a MongoDB root document.
        /// </summary>
        /// <param name="entityType">The current <see cref="IEntityType"/>.</param>
        /// <returns><c>true</c> if <paramref name="entityType"/> represents a root document; or <c>false</c>.</returns>
        public static bool IsDocumentRootEntityType(this IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));
            while (entityType.MongoDb().IsDerivedType && entityType.BaseType != null)
                entityType = entityType.BaseType;
            return !entityType.IsOwned();
        }

        /// <summary>
        /// Gets the <see cref="IEntityType"/> that represents the least-derived, non-abstract base representation of a given class.
        /// </summary>
        /// <param name="entityType">The <see cref="IEntityType"/> to start from.</param>
        /// <returns>The least-derived, non-abstract root document type for <paramref name="entityType"/>.</returns>
        public static IEntityType GetCollectionEntityType(this IEntityType entityType)
        {
            Check.NotOwned(entityType, nameof(entityType));
            while (entityType.MongoDb().IsDerivedType && entityType.BaseType != null)
                entityType = entityType.BaseType;
            return entityType;
        }
    }
}