// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Db2
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2ProcessorFactory.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Generators.DB2;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.DB2
{
    /// <summary>
    /// Class Db2ProcessorFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    [Obsolete]
    public class Db2ProcessorFactory : MigrationProcessorFactory
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ProcessorFactory"/> class.
        /// </summary>
        [Obsolete]
        public Db2ProcessorFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ProcessorFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public Db2ProcessorFactory(IServiceProvider serviceProvider)
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
            var factory = new Db2DbFactory(_serviceProvider);
            var connection = factory.CreateConnection(connectionString);
            var generatorOptions = new OptionsWrapper<GeneratorOptions>(new GeneratorOptions());
            return new Db2Processor(connection, new Db2Generator(new Db2Quoter(), generatorOptions), announcer, options, factory);
        }
    }
}
