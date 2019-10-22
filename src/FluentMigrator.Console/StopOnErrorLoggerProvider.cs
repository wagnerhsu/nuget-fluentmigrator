// ***********************************************************************
// Assembly         : Migrate
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="StopOnErrorLoggerProvider.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentMigrator.Console
{
    /// <summary>
    /// Pauses the application after every error
    /// </summary>
    public class StopOnErrorLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// The options
        /// </summary>
        private readonly FluentMigratorLoggerOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopOnErrorLoggerProvider" /> class.
        /// </summary>
        /// <param name="options">The logger options</param>
        public StopOnErrorLoggerProvider(IOptions<FluentMigratorLoggerOptions> options)
        {
            _options = options.Value;
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new StopOnErrorLogger(_options);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Nothing to dispose
        }

        /// <summary>
        /// Class StopOnErrorLogger.
        /// Implements the <see cref="FluentMigrator.Runner.Logging.FluentMigratorConsoleLogger" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.Logging.FluentMigratorConsoleLogger" />
        private class StopOnErrorLogger : FluentMigratorConsoleLogger
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StopOnErrorLogger" /> class.
            /// </summary>
            /// <param name="options">The logger options</param>
            public StopOnErrorLogger(FluentMigratorLoggerOptions options)
                : base(options)
            {
            }

            /// <inheritdoc />
            protected override void WriteError(string message)
            {
                base.WriteError(message);
                Pause();
            }

            /// <inheritdoc />
            protected override void WriteError(Exception exception)
            {
                base.WriteError(exception);
                Pause();
            }

            /// <summary>
            /// Pauses this instance.
            /// </summary>
            private void Pause()
            {
                System.Console.WriteLine(@"Press enter to continue...");
                System.Console.ReadLine();
            }
        }
    }
}
