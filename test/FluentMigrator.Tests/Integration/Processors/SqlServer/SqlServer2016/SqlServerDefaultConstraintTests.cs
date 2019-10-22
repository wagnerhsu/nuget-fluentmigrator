// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerDefaultConstraintTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, Fluent Migrator Project
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

using FluentMigrator.Runner.Initialization;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.SqlServer.SqlServer2016
{
    /// <summary>
    /// Defines test class SqlServerDefaultConstraintTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.SqlServer.SqlServer2016.SqlServerIntegrationTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.SqlServer.SqlServer2016.SqlServerIntegrationTests" />
    [TestFixture]
    public class SqlServerDefaultConstraintTests : SqlServerIntegrationTests
    {
        /// <summary>
        /// Defines the test method Issue715.
        /// </summary>
        [Test]
        public void Issue715()
        {
            try
            {
                Execute(
                    services => services
                        .Configure<RunnerOptions>(opt => opt.Task = "migrate")
                        .WithMigrationsIn(typeof(Migrations.SqlServer.Issue715.Migration150).Namespace),
                    sp =>
                    {
                        var task = sp.GetRequiredService<TaskExecutor>();
                        task.Execute();
                    });
            }
            finally
            {
                Execute(
                    services => services.AddScoped<TaskExecutor>()
                        .Configure<RunnerOptions>(opt => opt.Task = "rollback:all")
                        .WithMigrationsIn(typeof(Migrations.SqlServer.Issue715.Migration150).Namespace),
                    sp =>
                    {
                        var task = sp.GetRequiredService<TaskExecutor>();
                        task.Execute();
                    });
            }
        }

        /// <summary>
        /// Executes the specified initialize action.
        /// </summary>
        /// <param name="initAction">The initialize action.</param>
        /// <param name="executeAction">The execute action.</param>
        private void Execute(
            [CanBeNull] Action<IServiceCollection> initAction,
            [NotNull] Action<IServiceProvider> executeAction)
        {
            using (var serviceProvider = CreateProcessorServices(initAction))
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    executeAction(scope.ServiceProvider);
                }
            }
        }
    }
}
