// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Db2
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2ISeriesProcessorFactory.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.DB2.iSeries;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.DB2.iSeries
{
    /// <summary>
    /// Class Db2ISeriesProcessorFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    [Obsolete]
    public class Db2ISeriesProcessorFactory : MigrationProcessorFactory
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ISeriesProcessorFactory"/> class.
        /// </summary>
        [Obsolete]
        public Db2ISeriesProcessorFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ISeriesProcessorFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public Db2ISeriesProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <returns>IMigrationProcessor.</returns>
        [Obsolete]
        public override IMigrationProcessor Create(string connectionString, IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new Db2ISeriesDbFactory(_serviceProvider);
            var quoter = new Db2ISeriesQuoter();
            var connection = factory.CreateConnection(connectionString);
            var generatorOptions = new OptionsWrapper<GeneratorOptions>(new GeneratorOptions());
            return new Db2ISeriesProcessor(connection, new Db2ISeriesGenerator(quoter, generatorOptions), announcer, options, factory);
        }
    }
}
