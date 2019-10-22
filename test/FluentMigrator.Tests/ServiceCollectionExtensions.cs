// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ServiceCollectionExtensions.cs" company="FluentMigrator Project">
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

using System.Reflection;
using System.Runtime.CompilerServices;

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Tests.Logging;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace FluentMigrator.Tests
{
    /// <summary>
    /// Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Creates the services.
        /// </summary>
        /// <param name="addAssemblySource">if set to <c>true</c> [add assembly source].</param>
        /// <returns>IServiceCollection.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IServiceCollection CreateServices(bool addAssemblySource = true)
        {
            var services = new ServiceCollection()
                .AddFluentMigratorCore();

            if (addAssemblySource)
            {
                services
                    .AddSingleton<IAssemblySourceItem>(new AssemblySourceItem(Assembly.GetExecutingAssembly()));
            }

            services
                .AddSingleton<ILoggerProvider, TestLoggerProvider>()
                .Configure<RunnerOptions>(opt => opt.AllowBreakingChange = true);

            return services;
        }

        /// <summary>
        /// Withes the migrations in.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns>IServiceCollection.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IServiceCollection WithMigrationsIn(
            [NotNull] this IServiceCollection services,
            [CanBeNull] string @namespace,
            bool recursive = false)
        {
            return services
                .Configure<TypeFilterOptions>(
                    opt =>
                    {
                        opt.Namespace = @namespace;
                        opt.NestedNamespaces = recursive;
                    })
                .ConfigureRunner(builder => builder.WithMigrationsIn(Assembly.GetExecutingAssembly()));
        }

        /// <summary>
        /// Withes the processor.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="processor">The processor.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection WithProcessor(
            [NotNull] this IServiceCollection services,
            [NotNull] IMock<IMigrationProcessor> processor)
        {
            return services
                .AddScoped<IProcessorAccessor>(_ => new PassThroughProcessorAccessor(processor.Object));
        }

        /// <summary>
        /// Class PassThroughProcessorAccessor.
        /// Implements the <see cref="FluentMigrator.Runner.Processors.IProcessorAccessor" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Runner.Processors.IProcessorAccessor" />
        private class PassThroughProcessorAccessor : IProcessorAccessor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PassThroughProcessorAccessor"/> class.
            /// </summary>
            /// <param name="processor">The processor.</param>
            public PassThroughProcessorAccessor(IMigrationProcessor processor)
            {
                Processor = processor;
            }

            /// <inheritdoc />
            public IMigrationProcessor Processor { get; }
        }
    }
}
