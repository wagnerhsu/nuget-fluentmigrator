// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Firebird
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdProcessorFactory.cs" company="FluentMigrator Project">
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

using System;

using FluentMigrator.Runner.Generators.Firebird;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors.Firebird
{
    /// <summary>
    /// Class FirebirdProcessorFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    [Obsolete]
    public class FirebirdProcessorFactory : MigrationProcessorFactory
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdProcessorFactory"/> class.
        /// </summary>
        [Obsolete]
        public FirebirdProcessorFactory() : this(FirebirdOptions.AutoCommitBehaviour()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdProcessorFactory"/> class.
        /// </summary>
        /// <param name="fbOptions">The fb options.</param>
        [Obsolete]
        public FirebirdProcessorFactory(FirebirdOptions fbOptions)
            : this(serviceProvider: null, new OptionsWrapper<FirebirdOptions>(fbOptions))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdProcessorFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="fbOptions">The fb options.</param>
        public FirebirdProcessorFactory(IServiceProvider serviceProvider, IOptions<FirebirdOptions> fbOptions = null)
        {
            _serviceProvider = serviceProvider;
            FbOptions = fbOptions?.Value ?? FirebirdOptions.AutoCommitBehaviour();
        }

        /// <summary>
        /// Gets or sets the fb options.
        /// </summary>
        /// <value>The fb options.</value>
        public FirebirdOptions FbOptions { get; set; }

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
            var fbOpt = ((FirebirdOptions) FbOptions.Clone())
                .ApplyProviderSwitches(options.ProviderSwitches);
            var factory = new FirebirdDbFactory(_serviceProvider);
            var connection = factory.CreateConnection(connectionString);
            return new FirebirdProcessor(connection, new FirebirdGenerator(FbOptions), announcer, options, factory, fbOpt);
        }
    }
}
