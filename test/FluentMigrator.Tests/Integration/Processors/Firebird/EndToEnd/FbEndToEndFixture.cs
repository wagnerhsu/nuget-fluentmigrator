// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FbEndToEndFixture.cs" company="FluentMigrator Project">
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

using FirebirdSql.Data.FirebirdClient;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Tests.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd
{
    /// <summary>
    /// Class FbEndToEndFixture.
    /// </summary>
    [Category("Integration")]
    [Category("Firebird")]
    public class FbEndToEndFixture
    {
        /// <summary>
        /// The firebird library prober
        /// </summary>
        private static readonly FirebirdLibraryProber _firebirdLibraryProber = new FirebirdLibraryProber();
        /// <summary>
        /// The temporary database
        /// </summary>
        private TemporaryDatabase _temporaryDatabase;

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        protected string ConnectionString => _temporaryDatabase.ConnectionString;

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            if (!IntegrationTestOptions.Firebird.IsEnabled)
                Assert.Ignore();
            _temporaryDatabase = new TemporaryDatabase(
                IntegrationTestOptions.Firebird,
                _firebirdLibraryProber);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            if (_temporaryDatabase == null)
                return;

            FbDatabase.DropDatabase(_temporaryDatabase.ConnectionString);
            _temporaryDatabase.Dispose();
            _temporaryDatabase = null;
        }

        /// <summary>
        /// Migrates the specified migrations namespace.
        /// </summary>
        /// <param name="migrationsNamespace">The migrations namespace.</param>
        protected void Migrate(string migrationsNamespace)
        {
            MakeTask("migrate", migrationsNamespace).Execute();
        }

        /// <summary>
        /// Rollbacks the specified migrations namespace.
        /// </summary>
        /// <param name="migrationsNamespace">The migrations namespace.</param>
        protected void Rollback(string migrationsNamespace)
        {
            MakeTask("rollback", migrationsNamespace).Execute();
        }

        /// <summary>
        /// Makes the task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="migrationsNamespace">The migrations namespace.</param>
        /// <param name="configureOptions">The configure options.</param>
        /// <returns>TaskExecutor.</returns>
        protected TaskExecutor MakeTask(string task, string migrationsNamespace, Action<ProcessorOptions> configureOptions = null)
        {
            var services = new ServiceCollection()
                .AddFluentMigratorCore()
                .AddLogging(lb => lb.AddDebug())
                .AddSingleton<ILoggerProvider, TestLoggerProvider>()
                .ConfigureRunner(builder => builder
                    .AddFirebird())
                .Configure<RunnerOptions>(opt => opt.AllowBreakingChange = true)
                .AddScoped<IConnectionStringReader>(_ => new PassThroughConnectionStringReader(ConnectionString))
                .WithMigrationsIn(migrationsNamespace)
                .Configure<RunnerOptions>(opt => opt.Task = task);

            var serviceBuilder = services.BuildServiceProvider();
            return serviceBuilder.GetRequiredService<TaskExecutor>();
        }

        /// <summary>
        /// Tables the exists.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool TableExists(string candidate)
        {
            return IsInDatabase(cmd =>
                {
                    cmd.CommandText = "select rdb$relation_name from rdb$relations where (rdb$flags is not null) and (rdb$relation_name = @table)";
                    cmd.Parameters.AddWithValue("table", candidate.ToUpper());
                });
        }

        /// <summary>
        /// Columns the exists.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="candidateColumn">The candidate column.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool ColumnExists(string tableName, string candidateColumn)
        {
            return IsInDatabase(cmd =>
                {
                    cmd.CommandText = "select rdb$field_name from rdb$relation_fields where (rdb$relation_name = @table) and (rdb$field_name = @column)";
                    cmd.Parameters.AddWithValue("table", tableName.ToUpper());
                    cmd.Parameters.AddWithValue("column", candidateColumn.ToUpper());
                });
        }

        /// <summary>
        /// Determines whether [is in database] [the specified adjust command].
        /// </summary>
        /// <param name="adjustCommand">The adjust command.</param>
        /// <returns><c>true</c> if [is in database] [the specified adjust command]; otherwise, <c>false</c>.</returns>
        protected bool IsInDatabase(Action<FbCommand> adjustCommand)
        {
            bool result;
            using (var connection = new FbConnection(ConnectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        adjustCommand(cmd);
                        using (var reader = cmd.ExecuteReader())
                        {
                            result = reader.Read();
                        }
                    }

                    tx.Commit();
                }
            }

            return result;
        }
    }
}
