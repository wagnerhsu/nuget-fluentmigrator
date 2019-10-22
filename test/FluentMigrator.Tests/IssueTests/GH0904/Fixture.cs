// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Fixture.cs" company="FluentMigrator Project">
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
using FluentMigrator.Runner.Initialization;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace FluentMigrator.Tests.IssueTests.GH0904
{
    /// <summary>
    /// Defines test class Fixture.
    /// </summary>
    [TestFixture]
    [Category("Issue")]
    [Category("GH-0904")]
    [Category("SQLite")]
    public class Fixture
    {
        /// <summary>
        /// The sqlite database file name
        /// </summary>
        private string _sqliteDbFileName;
        /// <summary>
        /// The service provider
        /// </summary>
        private ServiceProvider _serviceProvider;

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _sqliteDbFileName = Path.GetTempFileName();
            _serviceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(
                    rb => rb
                        .AddSQLite()
                        .WithGlobalConnectionString($"Data Source={_sqliteDbFileName}")
                        .ScanIn(typeof(Fixture).Assembly).For.Migrations())
                .Configure<TypeFilterOptions>(
                    opt =>
                    {
                        opt.Namespace = GetType().Namespace;
                        opt.NestedNamespaces = true;
                    })
                .BuildServiceProvider();
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
            File.Delete(_sqliteDbFileName);
        }

        /// <summary>
        /// Defines the test method ProfileMustNotCauseNullReferenceException.
        /// </summary>
        [Test]
        public void ProfileMustNotCauseNullReferenceException()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }
        }
    }
}
