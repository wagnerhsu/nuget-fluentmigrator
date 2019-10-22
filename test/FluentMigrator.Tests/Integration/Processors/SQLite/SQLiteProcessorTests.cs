// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SQLiteProcessorTests.cs" company="FluentMigrator Project">
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
using System.IO;

using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.Processors.SQLite;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.SQLite
{
    /// <summary>
    /// Defines test class SQLiteProcessorTests.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    [Category("SQLite")]
    // ReSharper disable once InconsistentNaming
    public class SQLiteProcessorTests
    {
        /// <summary>
        /// The column
        /// </summary>
        private Mock<ColumnDefinition> _column;
        /// <summary>
        /// The column name
        /// </summary>
        private string _columnName;
        /// <summary>
        /// The table name
        /// </summary>
        private string _tableName;
        /// <summary>
        /// The table name that must be escaped
        /// </summary>
        private string _tableNameThatMustBeEscaped;

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <value>The service provider.</value>
        private ServiceProvider ServiceProvider { get; set; }
        /// <summary>
        /// Gets or sets the service scope.
        /// </summary>
        /// <value>The service scope.</value>
        private IServiceScope ServiceScope { get; set; }
        /// <summary>
        /// Gets or sets the processor.
        /// </summary>
        /// <value>The processor.</value>
        private SQLiteProcessor Processor { get; set; }

        /// <summary>
        /// Defines the test method CanDefaultAutoIncrementColumnTypeToInteger.
        /// </summary>
        [Test]
        public void CanDefaultAutoIncrementColumnTypeToInteger()
        {
            var column = new ColumnDefinition
            {
                Name = "Id",
                IsIdentity = true,
                IsPrimaryKey = true,
                Type = DbType.Int64,
                IsNullable = false
            };

            var expression = new CreateTableExpression { TableName = _tableName };
            expression.Columns.Add(column);

            Processor.Process(expression);
            Processor.TableExists(null, _tableName).ShouldBeTrue();
            Processor.ColumnExists(null, _tableName, "Id").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CanCreateTableExpression.
        /// </summary>
        [Test]
        public void CanCreateTableExpression()
        {
            var expression = new CreateTableExpression { TableName = _tableName };
            expression.Columns.Add(_column.Object);

            Processor.Process(expression);
            Processor.TableExists(null, _tableName).ShouldBeTrue();
            Processor.ColumnExists(null, _tableName, _columnName).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method IsEscapingTableNameCorrectlyOnTableCreate.
        /// </summary>
        [Test]
        public void IsEscapingTableNameCorrectlyOnTableCreate()
        {
            var expression = new CreateTableExpression { TableName = _tableNameThatMustBeEscaped };
            expression.Columns.Add(_column.Object);

            Processor.Process(expression);
        }

        /// <summary>
        /// Defines the test method IsEscapingTableNameCorrectlyOnReadTableData.
        /// </summary>
        [Test]
        public void IsEscapingTableNameCorrectlyOnReadTableData()
        {
            var expression = new CreateTableExpression { TableName = _tableNameThatMustBeEscaped };
            expression.Columns.Add(_column.Object);
            Processor.Process(expression);
            Processor.ReadTableData(null, _tableNameThatMustBeEscaped).Tables.Count.ShouldBe(1);
        }

        /// <summary>
        /// Defines the test method IsEscapingTableNameCorrectlyOnTableExists.
        /// </summary>
        [Test]
        public void IsEscapingTableNameCorrectlyOnTableExists()
        {
            var expression = new CreateTableExpression { TableName = _tableNameThatMustBeEscaped };
            expression.Columns.Add(_column.Object);
            Processor.Process(expression);
            Processor.TableExists(null, _tableNameThatMustBeEscaped).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method IsEscapingTableNameCorrectlyOnColumnExists.
        /// </summary>
        [Test]
        public void IsEscapingTableNameCorrectlyOnColumnExists()
        {
            const string columnName = "123ColumnName";

            var expression = new CreateTableExpression { TableName = _tableNameThatMustBeEscaped };
            expression.Columns.Add(new ColumnDefinition() { Name = "123ColumnName", Type = DbType.AnsiString, IsNullable = true });

            Processor.Process(expression);
            Processor.ColumnExists(null, _tableNameThatMustBeEscaped, columnName).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingProcessWithPerformDBOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges.
        /// </summary>
        [Test]
        public void CallingProcessWithPerformDBOperationExpressionWhenInPreviewOnlyModeWillNotMakeDbChanges()
        {
            var output = new StringWriter();

            var serviceProvider = CreateProcessorServices(
                services => services
                    .AddSingleton<ILoggerProvider>(new SqlScriptFluentMigratorLoggerProvider(output))
                    .ConfigureRunner(r => r.AsGlobalPreview()));
            using (serviceProvider)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<IMigrationProcessor>();

                    bool tableExists;

                    try
                    {
                        var expression =
                            new PerformDBOperationExpression
                            {
                                Operation = (con, trans) =>
                                {
                                    var command = con.CreateCommand();
                                    command.CommandText = "CREATE TABLE ProcessTestTable (test int NULL) ";
                                    command.Transaction = trans;

                                    command.ExecuteNonQuery();
                                }
                            };

                        processor.Process(expression);

                        tableExists = processor.TableExists("", "ProcessTestTable");
                    }
                    finally
                    {
                        processor.RollbackTransaction();
                    }

                    tableExists.ShouldBeFalse();
                }
            }

            Assert.That(output.ToString(), Does.Contain(@"/* Performing DB Operation */"));
        }

        /// <summary>
        /// Creates the processor services.
        /// </summary>
        /// <param name="initAction">The initialize action.</param>
        /// <returns>ServiceProvider.</returns>
        private ServiceProvider CreateProcessorServices([CanBeNull] Action<IServiceCollection> initAction)
        {
            if (!IntegrationTestOptions.SQLite.IsEnabled)
                Assert.Ignore();

            var serivces = ServiceCollectionExtensions.CreateServices()
                .ConfigureRunner(r => r.AddSQLite())
                .AddScoped<IConnectionStringReader>(
                    _ => new PassThroughConnectionStringReader(IntegrationTestOptions.SQLite.ConnectionString));

            initAction?.Invoke(serivces);

            return serivces.BuildServiceProvider();
        }

        /// <summary>
        /// Classes the set up.
        /// </summary>
        [OneTimeSetUp]
        public void ClassSetUp()
        {
            ServiceProvider = CreateProcessorServices(initAction: null);

            _column = new Mock<ColumnDefinition>();
            _tableName = "NewTable";
            _tableNameThatMustBeEscaped = "123NewTable";
            _columnName = "ColumnName";
            _column.SetupGet(c => c.Name).Returns(_columnName);
            _column.SetupGet(c => c.IsNullable).Returns(true);
            _column.SetupGet(c => c.Type).Returns(DbType.Int32);
        }

        /// <summary>
        /// Classes the tear down.
        /// </summary>
        [OneTimeTearDown]
        public void ClassTearDown()
        {
            ServiceProvider?.Dispose();
        }

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            ServiceScope = ServiceProvider.CreateScope();
            Processor = ServiceScope.ServiceProvider.GetRequiredService<SQLiteProcessor>();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ServiceScope?.Dispose();
        }
    }
}
