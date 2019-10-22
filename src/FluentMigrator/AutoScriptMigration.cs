// ***********************************************************************
// Assembly         : FluentMigrator
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="AutoScriptMigration.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, Fluent Migrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

using JetBrains.Annotations;

namespace FluentMigrator
{
    /// <summary>
    /// Migration that automatically uses embedded SQL scripts depending on the database type name
    /// </summary>
    /// <remarks>The embedded SQL scripts must end in <c>Scripts.{Direction}.{Version}_{DerivedTypeName}_{DatabaseType}.sql</c>.
    /// <para>The <c>{Direction}</c> can be <c>Up</c> or <c>Down</c>.</para><para>The <c>{Version}</c> is the migration version.</para><para>The <c>{DerivedTypeName}</c> is the name of the type derived from <see cref="AutoScriptMigration" />.</para><para>The <c>{DatabaseType}</c> is the database type name. For SQL Server 2016, the variants <c>SqlServer2016</c>,
    /// <c>SqlServer</c>, and <c>Generic</c> will be tested.</para><para>The behavior may be overriden by providing a custom <c>FluentMigrator.Runner.Conventions.IAutoNameConvention</c>.</para></remarks>
    public abstract class AutoScriptMigration : MigrationBase
    {
        /// <summary>
        /// The embedded resource providers
        /// </summary>
        [CanBeNull]
        private readonly IReadOnlyCollection<IEmbeddedResourceProvider> _embeddedResourceProviders;

        /// <summary>
        /// The providers
        /// </summary>
        [CanBeNull]
        private IReadOnlyCollection<IEmbeddedResourceProvider> _providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoScriptMigration"/> class.
        /// </summary>
        [Obsolete]
        protected AutoScriptMigration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoScriptMigration"/> class.
        /// </summary>
        /// <param name="embeddedResourceProviders">The embedded resource providers.</param>
        protected AutoScriptMigration([NotNull] IEnumerable<IEmbeddedResourceProvider> embeddedResourceProviders)
        {
            _embeddedResourceProviders = embeddedResourceProviders.ToList();
        }

        /// <inheritdoc />
        public sealed override void Up()
        {
#pragma warning disable 612
            var expression = new ExecuteEmbeddedAutoSqlScriptExpression(
                GetProviders(),
                GetType(),
                GetDatabaseNames(),
                MigrationDirection.Up)
            {
                MigrationAssemblies = Context.MigrationAssemblies,
#pragma warning restore 612
            };

            Context.Expressions.Add(expression);
        }

        /// <inheritdoc />
        public sealed override void Down()
        {
#pragma warning disable 612
            var expression = new ExecuteEmbeddedAutoSqlScriptExpression(
                GetProviders(),
                GetType(),
                GetDatabaseNames(),
                MigrationDirection.Down)
            {
                MigrationAssemblies = Context.MigrationAssemblies,
#pragma warning restore 612
            };

            Context.Expressions.Add(expression);
        }

        /// <summary>
        /// Gets the database names.
        /// </summary>
        /// <returns>IList&lt;System.String&gt;.</returns>
        private IList<string> GetDatabaseNames()
        {
            var dbNames = new List<string>();
            if (Context.QuerySchema != null)
            {
                dbNames.Add(Context.QuerySchema.DatabaseType);
                dbNames.AddRange(Context.QuerySchema.DatabaseTypeAliases);
            }

            return dbNames;
        }

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;IEmbeddedResourceProvider&gt;.</returns>
        [NotNull]
        private IReadOnlyCollection<IEmbeddedResourceProvider> GetProviders()
        {
            if (_providers != null)
                return _providers;

            if (_embeddedResourceProviders != null)
            {
                return _providers = _embeddedResourceProviders;
            }

            var providers = new List<IEmbeddedResourceProvider>
            {
#pragma warning disable 612
                new DefaultEmbeddedResourceProvider(Context.MigrationAssemblies)
#pragma warning restore 612
            };

            return _providers = providers;
        }

        /// <summary>
        /// Class ExecuteEmbeddedAutoSqlScriptExpression. This class cannot be inherited.
        /// Implements the <see cref="FluentMigrator.Expressions.ExecuteEmbeddedSqlScriptExpressionBase" />
        /// Implements the <see cref="FluentMigrator.Expressions.IAutoNameExpression" />
        /// Implements the <see cref="System.ComponentModel.DataAnnotations.IValidatableObject" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Expressions.ExecuteEmbeddedSqlScriptExpressionBase" />
        /// <seealso cref="FluentMigrator.Expressions.IAutoNameExpression" />
        /// <seealso cref="System.ComponentModel.DataAnnotations.IValidatableObject" />
        private sealed class ExecuteEmbeddedAutoSqlScriptExpression :
            ExecuteEmbeddedSqlScriptExpressionBase,
            IAutoNameExpression,
            IValidatableObject
        {
            /// <summary>
            /// The embedded resource providers
            /// </summary>
            [NotNull]
            private readonly IReadOnlyCollection<IEmbeddedResourceProvider> _embeddedResourceProviders;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExecuteEmbeddedAutoSqlScriptExpression"/> class.
            /// </summary>
            /// <param name="embeddedResourceProviders">The embedded resource providers.</param>
            /// <param name="migrationType">Type of the migration.</param>
            /// <param name="databaseNames">The database names.</param>
            /// <param name="direction">The direction.</param>
            public ExecuteEmbeddedAutoSqlScriptExpression([NotNull] IEnumerable<IEmbeddedResourceProvider> embeddedResourceProviders, Type migrationType, IList<string> databaseNames, MigrationDirection direction)
            {
                _embeddedResourceProviders = embeddedResourceProviders.ToList();
                MigrationType = migrationType;
                DatabaseNames = databaseNames;
                Direction = direction;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ExecuteEmbeddedAutoSqlScriptExpression"/> class.
            /// </summary>
            /// <param name="assemblyCollection">The assembly collection.</param>
            /// <param name="migrationType">Type of the migration.</param>
            /// <param name="databaseNames">The database names.</param>
            /// <param name="direction">The direction.</param>
            [Obsolete]
            // ReSharper disable once UnusedMember.Local
            public ExecuteEmbeddedAutoSqlScriptExpression(IAssemblyCollection assemblyCollection, Type migrationType, IList<string> databaseNames, MigrationDirection direction)
            {
                _embeddedResourceProviders = new[]
                {
                    new DefaultEmbeddedResourceProvider(assemblyCollection),
                };

                MigrationType = migrationType;
                DatabaseNames = databaseNames;
                Direction = direction;
            }

            /// <summary>
            /// Gets or sets the automatically generated names
            /// </summary>
            /// <value>The automatic names.</value>
            public IList<string> AutoNames { get; set; }
            /// <summary>
            /// Gets or sets the context in which the automatically generated name gets used
            /// </summary>
            /// <value>The automatic name context.</value>
            public AutoNameContext AutoNameContext { get; } = AutoNameContext.EmbeddedResource;
            /// <summary>
            /// Gets the type of the migration object
            /// </summary>
            /// <value>The type of the migration.</value>
            public Type MigrationType { get; }
            /// <summary>
            /// Gets the database names
            /// </summary>
            /// <value>The database names.</value>
            public IList<string> DatabaseNames { get; }
            /// <summary>
            /// Gets the direction of the migration
            /// </summary>
            /// <value>The direction.</value>
            public MigrationDirection Direction { get; }

            /// <summary>
            /// Gets or sets the migration assemblies
            /// </summary>
            /// <value>The migration assemblies.</value>
            [Obsolete]
            [CanBeNull]
            public IAssemblyCollection MigrationAssemblies { get; set; }

            /// <inheritdoc />
            public override void ExecuteWith(IMigrationProcessor processor)
            {
                IReadOnlyCollection<(string name, Assembly Assembly)> resourceNames;
#pragma warning disable 612
                if (MigrationAssemblies != null)
                {
                    resourceNames = MigrationAssemblies.GetManifestResourceNames()
                        .Select(item => (name: item.Name, assembly: item.Assembly))
                        .ToList();
#pragma warning restore 612
                }
                else
                {
                    resourceNames = _embeddedResourceProviders
                        .SelectMany(x => x.GetEmbeddedResources())
                        .Distinct()
                        .ToList();
                }

                var embeddedResourceNameWithAssembly = GetQualifiedResourcePath(resourceNames, AutoNames.ToArray());
                string sqlText;

                var stream = embeddedResourceNameWithAssembly
                    .assembly.GetManifestResourceStream(embeddedResourceNameWithAssembly.name);
                if (stream == null)
                {
                    throw new InvalidOperationException(
                        $"The ressource {embeddedResourceNameWithAssembly.name} couldn't be found in {embeddedResourceNameWithAssembly.assembly.FullName}");
                }

                using (stream)
                using (var reader = new StreamReader(stream))
                {
                    sqlText = reader.ReadToEnd();
                }

                Execute(processor, sqlText);
            }

            /// <inheritdoc />
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (AutoNames.Count == 0)
                    yield return new ValidationResult(ErrorMessages.SqlScriptCannotBeNullOrEmpty);
            }
        }
    }
}
