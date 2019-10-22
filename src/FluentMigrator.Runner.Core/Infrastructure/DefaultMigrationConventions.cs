// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DefaultMigrationConventions.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.VersionTableInfo;

namespace FluentMigrator.Runner.Infrastructure
{
    /// <summary>
    /// Class DefaultMigrationRunnerConventions.
    /// Implements the <see cref="FluentMigrator.Runner.IMigrationRunnerConventions" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.IMigrationRunnerConventions" />
    public class DefaultMigrationRunnerConventions : IMigrationRunnerConventions
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="DefaultMigrationRunnerConventions"/> class from being created.
        /// </summary>
        private DefaultMigrationRunnerConventions()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static DefaultMigrationRunnerConventions Instance { get; } = new DefaultMigrationRunnerConventions();

        /// <summary>
        /// Gets the type is migration.
        /// </summary>
        /// <value>The type is migration.</value>
        public Func<Type, bool> TypeIsMigration => TypeIsMigrationImpl;
        /// <summary>
        /// Gets the type is profile.
        /// </summary>
        /// <value>The type is profile.</value>
        public Func<Type, bool> TypeIsProfile => TypeIsProfileImpl;
        /// <summary>
        /// Gets the get maintenance stage.
        /// </summary>
        /// <value>The get maintenance stage.</value>
        public Func<Type, MigrationStage?> GetMaintenanceStage => GetMaintenanceStageImpl;
        /// <summary>
        /// Gets the type is version table meta data.
        /// </summary>
        /// <value>The type is version table meta data.</value>
        public Func<Type, bool> TypeIsVersionTableMetaData => TypeIsVersionTableMetaDataImpl;

        /// <summary>
        /// Gets the get migration information.
        /// </summary>
        /// <value>The get migration information.</value>
        [Obsolete]
        public Func<Type, IMigrationInfo> GetMigrationInfo => GetMigrationInfoForImpl;

        /// <inheritdoc />
        public Func<IMigration, IMigrationInfo> GetMigrationInfoForMigration => GetMigrationInfoForMigrationImpl;

        /// <summary>
        /// Gets the type has tags.
        /// </summary>
        /// <value>The type has tags.</value>
        public Func<Type, bool> TypeHasTags => TypeHasTagsImpl;
        /// <summary>
        /// Gets the type has matching tags.
        /// </summary>
        /// <value>The type has matching tags.</value>
        public Func<Type, IEnumerable<string>, bool> TypeHasMatchingTags => TypeHasMatchingTagsImpl;

        /// <summary>
        /// Types the is migration implementation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TypeIsMigrationImpl(Type type)
        {
            return typeof(IMigration).IsAssignableFrom(type) && type.GetCustomAttributes<MigrationAttribute>().Any();
        }

        /// <summary>
        /// Gets the maintenance stage implementation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Nullable&lt;MigrationStage&gt;.</returns>
        private static MigrationStage? GetMaintenanceStageImpl(Type type)
        {
            if (!typeof(IMigration).IsAssignableFrom(type))
                return null;

            var attribute = type.GetCustomAttribute<MaintenanceAttribute>();
            return attribute?.Stage;
        }

        /// <summary>
        /// Types the is profile implementation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TypeIsProfileImpl(Type type)
        {
            return typeof(IMigration).IsAssignableFrom(type) && type.GetCustomAttributes<ProfileAttribute>().Any();
        }

        /// <summary>
        /// Types the is version table meta data implementation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TypeIsVersionTableMetaDataImpl(Type type)
        {
            return typeof(IVersionTableMetaData).IsAssignableFrom(type) && type.GetCustomAttributes<VersionTableMetaDataAttribute>().Any();
        }

        /// <summary>
        /// Gets the migration information for migration implementation.
        /// </summary>
        /// <param name="migration">The migration.</param>
        /// <returns>IMigrationInfo.</returns>
        private static IMigrationInfo GetMigrationInfoForMigrationImpl(IMigration migration)
        {
            var migrationType = migration.GetType();
            var migrationAttribute = migrationType.GetCustomAttribute<MigrationAttribute>();
            var migrationInfo = new MigrationInfo(migrationAttribute.Version, migrationAttribute.Description, migrationAttribute.TransactionBehavior, migrationAttribute.BreakingChange, () => migration);

            foreach (var traitAttribute in migrationType.GetCustomAttributes<MigrationTraitAttribute>(true))
                migrationInfo.AddTrait(traitAttribute.Name, traitAttribute.Value);

            return migrationInfo;
        }

        /// <summary>
        /// Gets the migration information for implementation.
        /// </summary>
        /// <param name="migrationType">Type of the migration.</param>
        /// <returns>IMigrationInfo.</returns>
        private IMigrationInfo GetMigrationInfoForImpl(Type migrationType)
        {
            var migration = (IMigration) Activator.CreateInstance(migrationType);
            return GetMigrationInfoForMigration(migration);
        }

        /// <summary>
        /// Types the has tags implementation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TypeHasTagsImpl(Type type)
        {
            return GetInheritedCustomAttributes<TagsAttribute>(type).Any();
        }

        /// <summary>
        /// Gets the inherited custom attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        private static IEnumerable<T> GetInheritedCustomAttributes<T>(Type type)
        {
            var attributeType = typeof(T);

            return type
                .GetCustomAttributes(attributeType, true)
                .Union(
                    type.GetInterfaces()
                        .SelectMany(interfaceType => interfaceType.GetCustomAttributes(attributeType, true)))
                .Distinct()
                .Cast<T>();
        }

        /// <summary>
        /// Types the has matching tags implementation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="tagsToMatch">The tags to match.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TypeHasMatchingTagsImpl(Type type, IEnumerable<string> tagsToMatch)
        {
            var tags = GetInheritedCustomAttributes<TagsAttribute>(type).ToList();
            var matchTagsList = tagsToMatch.ToList();

            if (tags.Count != 0 && matchTagsList.Count == 0)
                return false;

            var tagNamesForAllBehavior = tags.Where(t => t.Behavior == TagBehavior.RequireAll).SelectMany(t => t.TagNames).ToArray();
            if (tagNamesForAllBehavior.Any() && matchTagsList.All(t => tagNamesForAllBehavior.Any(t.Equals)))
            {
                return true;
            }

            var tagNamesForAnyBehavior = tags.Where(t => t.Behavior == TagBehavior.RequireAny).SelectMany(t => t.TagNames).ToArray();
            if (tagNamesForAnyBehavior.Any() && matchTagsList.Any(t => tagNamesForAnyBehavior.Any(t.Equals)))
            {
                return true;
            }

            return false;
        }
    }
}
