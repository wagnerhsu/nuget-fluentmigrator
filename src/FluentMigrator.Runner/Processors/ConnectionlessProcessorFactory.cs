// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ConnectionlessProcessorFactory.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, FluentMigrator Project
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

using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Processors
{
    /// <summary>
    /// A processor factory to create SQL statements only (without executing them)
    /// </summary>
    [Obsolete]
    public class ConnectionlessProcessorFactory : IMigrationProcessorFactory
    {
        /// <summary>
        /// The generator
        /// </summary>
        [NotNull]
        private readonly IMigrationGenerator _generator;

        /// <summary>
        /// The database identifier
        /// </summary>
        [NotNull]
        private readonly string _databaseId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionlessProcessorFactory" /> class.
        /// </summary>
        /// <param name="generatorAccessor">The accessor to get the migration generator to use</param>
        /// <param name="runnerContext">The runner context</param>
        [Obsolete]
        public ConnectionlessProcessorFactory(
            [NotNull] IGeneratorAccessor generatorAccessor,
            [NotNull] IRunnerContext runnerContext)
        {
            _generator = generatorAccessor.Generator;
            _databaseId = runnerContext.Database;
            Name = _generator.GetName();
        }

        /// <inheritdoc />
        [Obsolete]
        public IMigrationProcessor Create(string connectionString, IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var processorOptions = options.GetProcessorOptions(connectionString);
            return new ConnectionlessProcessor(
                new PassThroughGeneratorAccessor(_generator),
                new AnnouncerFluentMigratorLogger(announcer),
                new ProcessorOptionsSnapshot(processorOptions),
                new OptionsWrapper<SelectingProcessorAccessorOptions>(
                    new SelectingProcessorAccessorOptions()
                    {
                        ProcessorId = _databaseId,
                    }));
        }

        /// <inheritdoc />
        public bool IsForProvider(string provider) => true;

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Class PassThroughGeneratorAccessor.
        /// Implements the <see cref="FluentMigrator.Runner.Generators.IGeneratorAccessor" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.Generators.IGeneratorAccessor" />
        private class PassThroughGeneratorAccessor : IGeneratorAccessor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PassThroughGeneratorAccessor"/> class.
            /// </summary>
            /// <param name="generator">The generator.</param>
            public PassThroughGeneratorAccessor(IMigrationGenerator generator)
            {
                Generator = generator;
            }

            /// <inheritdoc />
            public IMigrationGenerator Generator { get; }
        }

        /// <summary>
        /// Class ProcessorOptionsSnapshot.
        /// Implements the <see cref="Microsoft.Extensions.Options.IOptionsSnapshot{FluentMigrator.Runner.Processors.ProcessorOptions}" />
        /// </summary>
        /// <seealso cref="Microsoft.Extensions.Options.IOptionsSnapshot{FluentMigrator.Runner.Processors.ProcessorOptions}" />
        private class ProcessorOptionsSnapshot : IOptionsSnapshot<ProcessorOptions>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProcessorOptionsSnapshot"/> class.
            /// </summary>
            /// <param name="options">The options.</param>
            public ProcessorOptionsSnapshot(ProcessorOptions options)
            {
                Value = options;
            }

            /// <summary>
            /// The default configured TOptions instance, equivalent to Get(string.Empty).
            /// </summary>
            /// <value>The value.</value>
            public ProcessorOptions Value { get; }

            /// <summary>
            /// Gets the specified name.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns>ProcessorOptions.</returns>
            public ProcessorOptions Get(string name)
            {
                return Value;
            }
        }
    }
}
