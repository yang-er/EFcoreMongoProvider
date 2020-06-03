using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Mongo.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Mongo.Infrastructure
{
    /// <inheritdoc />
    /// <summary>
    ///     A validator that enforces rules for all MongoDb provider.
    /// </summary>
    public class MongoModelValidator : ModelValidator
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoModelValidator"/> class.
        /// </summary>
        /// <param name="modelValidatorDependencies">Parameter object containing dependencies for this service.</param>
        public MongoModelValidator(
            ModelValidatorDependencies modelValidatorDependencies)
            : base(Check.NotNull(modelValidatorDependencies, nameof(modelValidatorDependencies)))
        {
        }

        public override void Validate(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            base.Validate(Check.NotNull(model, nameof(model)), logger);

            EnsureDistinctCollectionNames(model);
            ValidateDerivedTypes(model);
        }

        /// <inheritdoc />
        protected override void ValidateNoShadowKeys(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            var nonComplexEntityTypes = model
                .GetEntityTypes()
                .Where(entityType => entityType.ClrType != null && !entityType.IsOwned());

            foreach (var entityType in nonComplexEntityTypes)
            {
                foreach (var key in entityType.GetDeclaredKeys())
                {
                    if (key.Properties.Any(p => p.IsShadowProperty())
                        && key is Key concreteKey
                        && ConfigurationSource.Convention.Overrides(concreteKey.GetConfigurationSource())
                        && !key.IsPrimaryKey())
                    {
                        var referencingFk = key.GetReferencingForeignKeys().FirstOrDefault();

                        if (referencingFk != null)
                        {
                            throw new InvalidOperationException(
                                CoreStrings.ReferencedShadowKey(
                                    referencingFk.DeclaringEntityType.DisplayName() +
                                    (referencingFk.DependentToPrincipal == null
                                        ? ""
                                        : "." + referencingFk.DependentToPrincipal.Name),
                                    entityType.DisplayName() +
                                    (referencingFk.PrincipalToDependent == null
                                        ? ""
                                        : "." + referencingFk.PrincipalToDependent.Name),
                                    Property.Format(referencingFk.Properties.Select(a => a.Name)),
                                    Property.Format(entityType.FindPrimaryKey().Properties.Select(a => a.Name))));
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void ValidateOwnership(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            Check.NotNull(model, nameof(model));

            var ownedEntityTypes = model
                .GetEntityTypes()
                .Where(entityType => model.AsModel().IsOwned(entityType.ClrType))
                .ToList();

            foreach (var entityType in ownedEntityTypes)
            {
                var ownerships = entityType
                    .GetForeignKeys()
                    .Where(fk => fk.IsOwnership)
                    .ToList();

                foreach (var ownership in ownerships)
                {
                    var principalToDependentForeignKey = entityType
                        .GetDeclaredForeignKeys()
                        .FirstOrDefault(foreignKey => !foreignKey.IsOwnership
                                                      && foreignKey.PrincipalToDependent != null);

                    if (principalToDependentForeignKey != null)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.InverseToOwnedType(
                                principalToDependentForeignKey.PrincipalEntityType.DisplayName(),
                                principalToDependentForeignKey.PrincipalToDependent.Name,
                                entityType.DisplayName(),
                                ownership.PrincipalEntityType.DisplayName()));
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that each <see cref="EntityType"/> in the given <paramref name="model"/> has a unique collection name.
        /// </summary>
        /// <param name="model">The <see cref="Model"/> to validate.</param>
        protected virtual void EnsureDistinctCollectionNames(IModel model)
        {
            Check.NotNull(model, nameof(model));
            var tables = new HashSet<string>();
            var duplicateCollectionNames = model
                .GetEntityTypes()
                .Where(et => et.BaseType == null)
                .Select(entityType => new
                {
                    new MongoEntityTypeAnnotations(entityType).CollectionName,
                    DisplayName = entityType.DisplayName()
                })
                .Where(tuple => !tables.Add(tuple.CollectionName));

            foreach (var tuple in duplicateCollectionNames)
            {
                throw new InvalidOperationException($"Duplicate collection name \"{tuple.CollectionName}\" defined on entity type \"{tuple.DisplayName}\".");
            }
        }

        protected override void ValidateTypeMappings(
            IModel model,
            IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            base.ValidateTypeMappings(model, logger);
            var modelBuilder = model.AsModel().Builder;

            foreach (var entityType in model.GetEntityTypes())
            {
                var unmappedProperty = entityType.GetProperties().FirstOrDefault(p =>
                    (!ConfigurationSource.Convention.Overrides(p.AsProperty().GetConfigurationSource()) || !p.IsShadowProperty())
                    && Dependencies.TypeMappingSource.FindMapping(p) == null);
                
                if (unmappedProperty != null)
                    throw new InvalidOperationException(
                        CoreStrings.PropertyNotMapped(
                            entityType.DisplayName(), unmappedProperty.Name, unmappedProperty.ClrType.ShortDisplayName()));

                if (!entityType.HasClrType())
                    continue;

                var clrProperties = new HashSet<string>();

                clrProperties.UnionWith(
                    entityType.GetRuntimeProperties().Values
                        .Where(pi => pi.IsCandidateProperty())
                        .Select(pi => pi.Name));

                clrProperties.ExceptWith(entityType.GetProperties().Select(p => p.Name));
                clrProperties.ExceptWith(entityType.GetNavigations().Select(p => p.Name));
                clrProperties.ExceptWith(entityType.GetServiceProperties().Select(p => p.Name));
                clrProperties.RemoveWhere(p => entityType.AsEntityType().Builder.IsIgnored(p, ConfigurationSource.Convention));

                if (clrProperties.Count <= 0)
                {
                    continue;
                }

                foreach (var clrProperty in clrProperties)
                {
                    var actualProperty = entityType.GetRuntimeProperties()[clrProperty];
                    var propertyType = actualProperty.PropertyType;
                    var targetSequenceType = propertyType.TryGetSequenceType();

                    if (modelBuilder.IsIgnored(model.AsModel().GetDisplayName(propertyType),
                        ConfigurationSource.Convention)
                        || (targetSequenceType != null
                            && modelBuilder.IsIgnored(modelBuilder.Metadata.GetDisplayName(targetSequenceType),
                                ConfigurationSource.Convention)))
                    {
                        continue;
                    }

                    var targetType = Dependencies.MemberClassifier.FindCandidateNavigationPropertyType(actualProperty);

                    var isTargetWeakOrOwned
                        = targetType != null
                          && (model.HasEntityTypeWithDefiningNavigation(targetType)
                              || modelBuilder.Metadata.IsOwned(targetType));

                    if (targetType != null
                        && targetType.IsValidEntityType()
                        && (isTargetWeakOrOwned
                            || model.FindEntityType(targetType) != null
                            || targetType.GetRuntimeProperties().Any(p => p.IsCandidateProperty())))
                    {
                        if ((!isTargetWeakOrOwned
                             || !targetType.GetTypeInfo().Equals(entityType.ClrType.GetTypeInfo()))
                            && entityType.GetDerivedTypes().All(
                                dt => dt.FindDeclaredNavigation(actualProperty.Name) == null)
                            && !entityType.IsInDefinitionPath(targetType))
                            throw new InvalidOperationException(
                                CoreStrings.NavigationNotAdded(
                                    entityType.DisplayName(), actualProperty.Name, propertyType.ShortDisplayName()));
                    }
                    else if (targetSequenceType == null && propertyType.GetTypeInfo().IsInterface
                        || targetSequenceType != null && targetSequenceType.GetTypeInfo().IsInterface)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.InterfacePropertyNotAdded(
                                entityType.DisplayName(), actualProperty.Name, propertyType.ShortDisplayName()));
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            CoreStrings.PropertyNotAdded(
                                entityType.DisplayName(), actualProperty.Name, propertyType.ShortDisplayName()));
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that all entities in the given <paramref name="model"/> have unique discriminators.
        /// </summary>
        /// <param name="model">The <see cref="Model"/> to validate.</param>
        protected virtual void ValidateDerivedTypes(IModel model)
        {
            var derivedTypes = Check.NotNull(model, nameof(model))
                .GetEntityTypes()
                .Where(entityType => entityType.BaseType != null && entityType.ClrType.IsInstantiable());
            var discriminatorSet = new HashSet<Tuple<IEntityType, string>>();
            foreach (var entityType in derivedTypes)
            {
                ValidateDiscriminator(entityType, discriminatorSet);
            }
        }

        private void ValidateDiscriminator(IEntityType entityType, ISet<Tuple<IEntityType,string>> discriminatorSet)
        {
            var annotations = new MongoEntityTypeAnnotations(entityType);
            if (string.IsNullOrWhiteSpace(annotations.Discriminator))
            {
                throw new InvalidOperationException($"Missing discriminator value for entity type {entityType.DisplayName()}.");
            }
            if (!discriminatorSet.Add(Tuple.Create(entityType.GetRootType(), annotations.Discriminator)))
            {
                throw new InvalidOperationException($"Duplicate discriminator value {annotations.Discriminator} for root entity type {entityType.GetRootType().DisplayName()} (defined on {entityType.DisplayName()}).");
            }
        }
    }
}