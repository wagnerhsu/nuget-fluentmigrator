// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TextWriterLoggerProvider.cs" company="FluentMigrator Project">
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

using System.IO;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;

using Microsoft.Extensions.Logging;

namespace FluentMigrator.Tests.Logging
{
    /// <summary>
    /// Logger provider to capture the log output
    /// </summary>
    public class TextWriterLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// The writer
        /// </summary>
        private readonly TextWriter _writer;
        /// <summary>
        /// The options
        /// </summary>
        private readonly FluentMigratorLoggerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterLoggerProvider"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public TextWriterLoggerProvider(TextWriter writer)
            : this(writer, new FluentMigratorLoggerOptions() { ShowSql = true })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterLoggerProvider"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="options">The options.</param>
        public TextWriterLoggerProvider(TextWriter writer, FluentMigratorLoggerOptions options)
        {
            _writer = writer;
            _options = options;
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new TextWriterLogger(_writer, _options);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _writer?.Dispose();
        }

        /// <summary>
        /// Class TextWriterLogger.
        /// Implements the <see cref="FluentMigrator.Runner.Logging.FluentMigratorRunnerLogger" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.Logging.FluentMigratorRunnerLogger" />
        private class TextWriterLogger : FluentMigratorRunnerLogger
        {
            /// <inheritdoc />
            public TextWriterLogger(TextWriter output, FluentMigratorLoggerOptions options)
                : base(output, output, options)
            {
            }
        }
    }
}
