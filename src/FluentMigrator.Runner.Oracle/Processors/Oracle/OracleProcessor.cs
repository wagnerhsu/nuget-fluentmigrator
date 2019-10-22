// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Oracle
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleProcessor.cs" company="FluentMigrator Project">
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
using System.Data;

using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace FluentMigrator.Runner.Processors.Oracle
{
    /// <summary>
    /// Class OracleProcessor.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.Oracle.OracleProcessorBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.Oracle.OracleProcessorBase" />
    public class OracleProcessor : OracleProcessorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleProcessor"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete]
        public OracleProcessor(
            IDbConnection connection,
            IMigrationGenerator generator,
            IAnnouncer announcer,
            IMigrationProcessorOptions options,
            IDbFactory factory)
            : base("Oracle", connection, generator, announcer, options, factory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleProcessor"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="generator">The generator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <param name="connectionStringAccessor">The connection string accessor.</param>
        public OracleProcessor(
            [NotNull] OracleDbFactory factory,
            [NotNull] IOracleGenerator generator,
            [NotNull] ILogger<OracleProcessor> logger,
            [NotNull] IOptionsSnapshot<ProcessorOptions> options,
            [NotNull] IConnectionStringAccessor connectionStringAccessor)
            : base("Oracle", factory, generator, logger, options, connectionStringAccessor)
        {
        }
    }
}
