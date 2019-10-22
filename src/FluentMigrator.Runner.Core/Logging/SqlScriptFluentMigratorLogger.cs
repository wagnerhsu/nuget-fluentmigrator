// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlScriptFluentMigratorLogger.cs" company="FluentMigrator Project">
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

using Microsoft.Extensions.Logging;

namespace FluentMigrator.Runner.Logging
{
    /// <summary>
    /// The <see cref="ILogger" /> implementation for writing SQL scripts
    /// </summary>
    internal class SqlScriptFluentMigratorLogger : FluentMigratorLogger
    {
        /// <summary>
        /// The writer
        /// </summary>
        private readonly SqlTextWriter _writer;
        /// <summary>
        /// The options
        /// </summary>
        private readonly SqlScriptFluentMigratorLoggerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScriptFluentMigratorLogger" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="options">The options.</param>
        public SqlScriptFluentMigratorLogger(
            SqlTextWriter writer,
            SqlScriptFluentMigratorLoggerOptions options)
            : base(options)
        {
            _writer = writer;
            _options = options;
        }

        /// <inheritdoc />
        protected override void WriteError(string message)
        {
            _writer.WriteExceptionMessage(message);
        }

        /// <inheritdoc />
        protected override void WriteError(Exception exception)
        {
            _writer.WriteException(exception);
        }

        /// <inheritdoc />
        protected override void WriteHeading(string message)
        {
            _writer.WriteLine($"{message} ".PadRight(75, '='));
            _writer.WriteLineDirect(string.Empty);
        }

        /// <inheritdoc />
        protected override void WriteEmphasize(string message)
        {
            _writer.WriteLine(message);
        }

        /// <inheritdoc />
        protected override void WriteElapsedTime(TimeSpan timeSpan)
        {
            _writer.WriteLine($"=> {timeSpan.TotalSeconds}s");
            _writer.WriteLineDirect(string.Empty);
        }

        /// <inheritdoc />
        protected override void WriteSay(string message)
        {
            _writer.WriteLine(message);
        }

        /// <inheritdoc />
        protected override void WriteSql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                WriteEmptySql();
            }
            else
            {
                _writer.WriteLineDirect(sql);
                if (_options.OutputGoBetweenStatements)
                    _writer.WriteLineDirect("GO");
            }
        }

        /// <inheritdoc />
        protected override void WriteEmptySql()
        {
            _writer.WriteLine("No SQL statement executed.");
        }
    }
}
