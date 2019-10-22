// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Postgres
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Postgres92Processor.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2018, Fluent Migrator Project
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

using System.Collections.Generic;

using FluentMigrator.Runner.Generators.Postgres92;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.Postgres;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.Postgres92
{
    /// <summary>
    /// Class Postgres92Processor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.Postgres.PostgresProcessor" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.Postgres.PostgresProcessor" />
    public class Postgres92Processor : PostgresProcessor
    {
        /// <summary>
        /// Gets the database type
        /// </summary>
        /// <value>The type of the database.</value>
        public override string DatabaseType => "Postgres92";

        /// <summary>
        /// Gets the database type aliases
        /// </summary>
        /// <value>The database type aliases.</value>
        public override IList<string> DatabaseTypeAliases { get; } = new List<string> { "PostgreSQL92" };

        /// <summary>
        /// Initializes a new instance of the <see cref="Postgres92Processor"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        /// <param name="pgOptions">The pg options.</param>
        public Postgres92Processor(
            [NotNull] PostgresDbFactory factory,
            [NotNull] Postgres92Generator generator,
            [NotNull] ILogger<PostgresProcessor> logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor,
            [NotNull] PostgresOptions pgOptions)
            : base(factory, generator, logger, options, connectionStringAccessor, pgOptions)
        {
        }
    }
}
