// ***********************************************************************
// Assembly         : FluentMigrator.Runner.MySql
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MySql5ProcessorFactory.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, FluentMigrator Project
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

using FluentMigrator.Runner.Generators.MySql;

namespace FluentMigrator.Runner.Processors.MySql
{
    /// <summary>
    /// Class MySql5ProcessorFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    [Obsolete]
    public class MySql5ProcessorFactory : MigrationProcessorFactory
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySql5ProcessorFactory"/> class.
        /// </summary>
        [Obsolete]
        public MySql5ProcessorFactory()
            : this(serviceProvider: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySql5ProcessorFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public MySql5ProcessorFactory(IServiceProvider serviceProvider)
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
            var factory = new MySqlDbFactory(_serviceProvider);
            var connection = factory.CreateConnection(connectionString);
            return new MySqlProcessor(connection, new MySql5Generator(new MySqlQuoter()), announcer, options, factory);
        }
    }
}
