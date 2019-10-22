// ***********************************************************************
// Assembly         : Migrate
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-17-2019
// ***********************************************************************
// <copyright file="MigratorConsole.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License

// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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
using System.Collections.Generic;
using System.Linq;

using FluentMigrator.Exceptions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Initialization.NetFramework;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.Processors;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Mono.Options;

using static FluentMigrator.Runner.ConsoleUtilities;

namespace FluentMigrator.Console
{
    /// <summary>
    /// Class MigratorConsole.
    /// </summary>
    public class MigratorConsole
    {
        /// <summary>
        /// The application context
        /// </summary>
        public string ApplicationContext;
        /// <summary>
        /// The connection
        /// </summary>
        public string Connection;
        /// <summary>
        /// The connection string configuration path
        /// </summary>
        public string ConnectionStringConfigPath;
        /// <summary>
        /// The namespace
        /// </summary>
        public string Namespace;
        /// <summary>
        /// The nested namespaces
        /// </summary>
        public bool NestedNamespaces;
        /// <summary>
        /// The output
        /// </summary>
        public bool Output;
        /// <summary>
        /// The output filename
        /// </summary>
        public string OutputFilename;
        /// <summary>
        /// The preview only
        /// </summary>
        public bool PreviewOnly;
        /// <summary>
        /// The processor type
        /// </summary>
        public string ProcessorType;
        /// <summary>
        /// The profile
        /// </summary>
        public string Profile;
        /// <summary>
        /// The show help
        /// </summary>
        public bool ShowHelp;
        /// <summary>
        /// The steps
        /// </summary>
        public int Steps;
        /// <summary>
        /// The tags
        /// </summary>
        public List<string> Tags = new List<string>();
        /// <summary>
        /// The include untagged maintenances
        /// </summary>
        public bool IncludeUntaggedMaintenances;
        /// <summary>
        /// The include untagged migrations
        /// </summary>
        public bool IncludeUntaggedMigrations = true;
        /// <summary>
        /// The target assembly
        /// </summary>
        public string TargetAssembly;
        /// <summary>
        /// The task
        /// </summary>
        public string Task;
        /// <summary>
        /// The timeout
        /// </summary>
        public int? Timeout;
        /// <summary>
        /// The verbose
        /// </summary>
        public bool Verbose;
        /// <summary>
        /// The stop on error
        /// </summary>
        public bool StopOnError;
        /// <summary>
        /// The version
        /// </summary>
        public long Version;
        /// <summary>
        /// The start version
        /// </summary>
        public long StartVersion;
        /// <summary>
        /// The no connection
        /// </summary>
        public bool NoConnection;
        /// <summary>
        /// The working directory
        /// </summary>
        public string WorkingDirectory;
        /// <summary>
        /// The transaction per session
        /// </summary>
        public bool TransactionPerSession;
        /// <summary>
        /// The allow breaking change
        /// </summary>
        public bool AllowBreakingChange;
        /// <summary>
        /// The provider switches
        /// </summary>
        public string ProviderSwitches;
        /// <summary>
        /// The strip comments
        /// </summary>
        public bool StripComments = true;
        /// <summary>
        /// Gets or sets the default name of the schema.
        /// </summary>
        /// <value>The default name of the schema.</value>
        public string DefaultSchemaName { get; set; }

        /// <summary>
        /// Runs the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        public int Run(params string[] args)
        {
            var dbChoicesList = new List<string>();
            string dbChoices;

            var services = CreateCoreServices()
                .AddScoped<IConnectionStringReader>(_ => new PassThroughConnectionStringReader("No connection"));
            using (var sp = services.BuildServiceProvider(validateScopes: false))
            {
                var processors = sp.GetRequiredService<IEnumerable<IMigrationProcessor>>().ToList();
                dbChoicesList.AddRange(processors.Select(p => p.DatabaseType));
            }

            dbChoices = string.Join(
                ", ",
                dbChoicesList
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x, StringComparer.OrdinalIgnoreCase));

            System.Console.Out.WriteHeader();

            try
            {
                var optionSet = new OptionSet
                {
                    {
                        "assembly=|a=|target=",
                        "REQUIRED. The assembly containing the migrations you want to execute.",
                        v => { TargetAssembly = v; }
                    },
                    {
                        "provider=|dbType=|db=",
                        $"REQUIRED. The kind of database you are migrating against. Available choices are: {dbChoices}.",
                        v => { ProcessorType = v; }
                    },
                    {
                        "connectionString=|connection=|conn=|c=",
                        "The name of the connection string (falls back to machine name) or the connection string itself to the server and database you want to execute your migrations against.",
                        v => { Connection = v; }
                    },
                    {
                        "connectionStringConfigPath=|configPath=",
                        string.Format(
                            "The path of the machine.config where the connection string named by connectionString" +
                            " is found. If not specified, it defaults to the machine.config used by the currently running CLR version"),
                        v => { ConnectionStringConfigPath = v; }
                    },
                    {
                        "namespace=|ns=",
                        "The namespace contains the migrations you want to run. Default is all migrations found within the Target Assembly will be run.",
                        v => { Namespace = v; }
                    },
                    {
                        "nested",
                        "Whether migrations in nested namespaces should be included. Used in conjunction with the namespace option.",
                        v => { NestedNamespaces = v != null; }
                    },
                    {
                        "output|out|o",
                        "Output generated SQL to a file. Default is no output. Use outputFilename to control the filename, otherwise [assemblyname].sql is the default.",
                        v => { Output = v != null; }
                    },
                    {
                        "outputFilename=|outfile=|of=",
                        "The name of the file to output the generated SQL to. The output option must be included for output to be saved to the file.",
                        v => { OutputFilename = v; }
                    },
                    {
                        "preview|p",
                        "Only output the SQL generated by the migration - do not execute it. Default is false.",
                        v => { PreviewOnly = v != null; }
                    },
                    {
                        "steps=",
                        "The number of versions to rollback if the task is 'rollback'. Default is 1.",
                        v => { Steps = int.Parse(v); }
                    },
                    {
                        "task=|t=",
                        "The task you want FluentMigrator to perform. Available choices are: migrate:up, migrate (same as migrate:up), migrate:down, rollback, rollback:toversion, rollback:all, validateversionorder, listmigrations. Default is 'migrate'.",
                        v => { Task = v; }
                    },
                    {
                        "version=",
                        "The specific version to migrate. Default is 0, which will run all migrations.",
                        v => { Version = long.Parse(v); }
                    },
                    {
                        "startVersion=",
                        "The specific version to start migrating from. Only used when NoConnection is true. Default is 0",
                        v => { StartVersion = long.Parse(v); }
                    },
                    {
                        "noConnection",
                        "Indicates that migrations will be generated without consulting a target database. Should only be used when generating an output file.",
                        v => { NoConnection = v != null; }
                    },
                    {
                        "verbose=",
                        "Show the SQL statements generated and execution time in the console. Default is false.",
                        v => { Verbose = v != null; }
                    },
                    {
                        "stopOnError=",
                        "Pauses migration execution until the user input if any error occured. Default is false.",
                        v => { StopOnError = v != null; }
                    },
                    {
                        "workingdirectory=|wd=",
                        "The directory to load SQL scripts specified by migrations from.",
                        v => { WorkingDirectory = v; }
                    },
                    {
                        "profile=",
                        "The profile to run after executing migrations.",
                        v => { Profile = v; }
                    },
                    {
                        "context=",
                        "Set ApplicationContext to the given string.",
                        v => { ApplicationContext = v; }
                    },
                    {
                        "timeout=",
                        "Overrides the default SqlCommand timeout of 30 seconds.",
                        v => { Timeout = int.Parse(v); }
                    },
                    {
                        "tag=",
                        "Filters the migrations to be run by tag.",
                        v => { Tags.Add(v); }
                    },
                    {
                        "include-untagged:",
                        "Include untagged migrations and/or maintenance objects.",
                        v =>
                        {
                            if (string.IsNullOrEmpty(v))
                            {
                                IncludeUntaggedMigrations = IncludeUntaggedMaintenances = true;
                            }
                            else
                            {
                                var items = v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => x.ToLowerInvariant().Trim())
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .Select(
                                        x =>
                                        {
                                            var hasOption = x.EndsWith("+") || x.EndsWith("-");
                                            var enable = !x.EndsWith("-");
                                            var name = hasOption ? x.Substring(0, x.Length - 1) : x;
                                            return new
                                            {
                                                FullName = x,
                                                ShortName = name.Substring(Math.Min(2, name.Length)),
                                                Enable = enable,
                                            };
                                        });
                                foreach (var item in items)
                                {
                                    switch (item.ShortName)
                                    {
                                        case "ma":
                                            IncludeUntaggedMaintenances = item.Enable;
                                            break;
                                        case "mi":
                                            IncludeUntaggedMigrations = item.Enable;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException(
                                                $"The argument {item.FullName} is not supported. "
                                              + "Valid values are: ma, maintenance, mi, migrations with an optional '+' or '-' at the end to enable or disable the option. "
                                              + "Multiple values may be given when separated by a comma.");
                                    }
                                }
                            }
                        }
                    },
                    {
                        "providerswitches=",
                        "Provider specific switches",
                        v => { ProviderSwitches = v; }
                    },
                    {
                        "strip|strip-comments",
                        "Strip comments from the SQL scripts. Default is true.",
                        v => { StripComments = v != null; }
                    },
                    {
                        "help|h|?",
                        "Displays this help menu.",
                        v => { ShowHelp = true; }
                    },
                    {
                        "transaction-per-session|tps",
                        "Overrides the transaction behavior of migrations, so that all migrations to be executed will run in one transaction.",
                        v => { TransactionPerSession = v != null; }
                    },
                    {
                        "allow-breaking-changes|abc",
                        "Allows execution of migrations marked as breaking changes.",
                        v => { AllowBreakingChange = v != null; }
                    },
                    {
                        "default-schema-name=",
                        "Set default schema name for the VersionInfo table and the migrations.",
                        v => { DefaultSchemaName = v; }
                    },
                };

                try
                {
                    optionSet.Parse(args);
                }
                catch (OptionException e)
                {
                    AsError(() => System.Console.Error.WriteException(e));
                    System.Console.WriteLine(@"Try 'migrate --help' for more information.");
                    return 2;
                }

                if (string.IsNullOrEmpty(Task))
                {
                    Task = "migrate";
                }

                if (!ValidateArguments(optionSet))
                {
                    return 1;
                }

                if (ShowHelp)
                {
                    DisplayHelp(optionSet);
                    return 0;
                }

                return ExecuteMigrations();
            }
            catch (MissingMigrationsException ex)
            {
                AsError(() => System.Console.Error.WriteException(ex));
                return 6;
            }
            catch (RunnerException ex)
            {
                AsError(() => System.Console.Error.WriteException(ex));
                return 5;
            }
            catch (FluentMigratorException ex)
            {
                AsError(() => System.Console.Error.WriteException(ex));
                return 4;
            }
            catch (Exception ex)
            {
                AsError(() => System.Console.Error.WriteException(ex));
                return 3;
            }
        }

        /// <summary>
        /// Validates the arguments.
        /// </summary>
        /// <param name="optionSet">The option set.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ValidateArguments(OptionSet optionSet)
        {
            if (string.IsNullOrEmpty(TargetAssembly))
            {
                DisplayHelp(optionSet, "Please enter the path of the assembly containing migrations you want to execute.");
                return false;
            }
            if (string.IsNullOrEmpty(ProcessorType))
            {
                DisplayHelp(optionSet, "Please enter the kind of database you are migrating against.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Displays the help.
        /// </summary>
        /// <param name="optionSet">The option set.</param>
        /// <param name="validationErrorMessage">The validation error message.</param>
        private void DisplayHelp(OptionSet optionSet, string validationErrorMessage)
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(validationErrorMessage);
            System.Console.ResetColor();
            DisplayHelp(optionSet);
        }

        /// <summary>
        /// Displays the help.
        /// </summary>
        /// <param name="p">The p.</param>
        private void DisplayHelp(OptionSet p)
        {
            System.Console.WriteLine(@"Usage:");
            System.Console.WriteLine(@"  migrate [OPTIONS]");
            System.Console.WriteLine(@"Example:");
            System.Console.WriteLine(@"  migrate -a bin\debug\MyMigrations.dll -db SqlServer2008 -conn ""SEE_BELOW"" -profile ""Debug""");
            System.Console.Out.WriteHorizontalRuler();
            System.Console.WriteLine(@"Example Connection Strings:");
            System.Console.WriteLine(@"  MySql: Data Source=172.0.0.1;Database=Foo;User Id=USERNAME;Password=BLAH");
            System.Console.WriteLine(@"  Oracle: Server=172.0.0.1;Database=Foo;Uid=USERNAME;Pwd=BLAH");
            System.Console.WriteLine(@"  SqlLite: Data Source=:memory:");
            System.Console.WriteLine(@"  SqlServer: server=127.0.0.1;database=Foo;user id=USERNAME;password=BLAH");
            System.Console.WriteLine(@"             server=.\SQLExpress;database=Foo;trusted_connection=true");
            System.Console.WriteLine(@"   ");
            System.Console.WriteLine(@"OR use a named connection string from the machine.config:");
            System.Console.WriteLine(@"  migrate -a bin\debug\MyMigrations.dll -db SqlServer2008 -conn ""namedConnection"" -profile ""Debug""");
            System.Console.Out.WriteHorizontalRuler();
            System.Console.WriteLine(@"Options:");
            p.WriteOptionDescriptions(System.Console.Out);
        }

        /// <summary>
        /// Gets a value indicating whether [executing against ms SQL].
        /// </summary>
        /// <value><c>true</c> if [executing against ms SQL]; otherwise, <c>false</c>.</value>
        private bool ExecutingAgainstMsSql => ProcessorType.StartsWith("SqlServer", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Executes the migrations.
        /// </summary>
        /// <returns>System.Int32.</returns>
        private int ExecuteMigrations()
        {
            var conventionSet = new DefaultConventionSet(DefaultSchemaName, WorkingDirectory);

            var services = CreateCoreServices()
                .Configure<FluentMigratorLoggerOptions>(
                    opt =>
                    {
                        opt.ShowElapsedTime = Verbose;
                        opt.ShowSql = Verbose;
                    })
                .AddSingleton<IConventionSet>(conventionSet)
                .Configure<SelectingProcessorAccessorOptions>(opt => opt.ProcessorId = ProcessorType)
                .Configure<AssemblySourceOptions>(opt => opt.AssemblyNames = new[] { TargetAssembly })
#pragma warning disable 612
                .Configure<AppConfigConnectionStringAccessorOptions>(
                    opt => opt.ConnectionStringConfigPath = ConnectionStringConfigPath)
#pragma warning restore 612
                .Configure<TypeFilterOptions>(
                    opt =>
                    {
                        opt.Namespace = Namespace;
                        opt.NestedNamespaces = NestedNamespaces;
                    })
                .Configure<RunnerOptions>(
                    opt =>
                    {
                        opt.Task = Task;
                        opt.Version = Version;
                        opt.StartVersion = StartVersion;
                        opt.NoConnection = NoConnection;
                        opt.Steps = Steps;
                        opt.Profile = Profile;
                        opt.Tags = Tags.ToArray();
#pragma warning disable 612
                        opt.ApplicationContext = ApplicationContext;
#pragma warning restore 612
                        opt.TransactionPerSession = TransactionPerSession;
                        opt.AllowBreakingChange = AllowBreakingChange;
                        opt.IncludeUntaggedMaintenances = IncludeUntaggedMaintenances;
                        opt.IncludeUntaggedMigrations = IncludeUntaggedMigrations;
                    })
                .Configure<ProcessorOptions>(
                    opt =>
                    {
                        opt.ConnectionString = Connection;
                        opt.PreviewOnly = PreviewOnly;
                        opt.ProviderSwitches = ProviderSwitches;
                        opt.StripComments = StripComments;
                        opt.Timeout = Timeout == null ? null : (TimeSpan?) TimeSpan.FromSeconds(Timeout.Value);
                    });

            if (StopOnError)
            {
                services
                    .AddSingleton<ILoggerProvider, StopOnErrorLoggerProvider>();
            }
            else
            {
                services
                    .AddSingleton<ILoggerProvider, FluentMigratorConsoleLoggerProvider>();
            }

            if (Output)
            {
                services
                    .Configure<LogFileFluentMigratorLoggerOptions>(
                        opt =>
                        {
                            opt.ShowSql = true;
                            opt.OutputFileName = OutputFilename;
                            opt.OutputGoBetweenStatements = ExecutingAgainstMsSql;
                        })
                    .AddSingleton<ILoggerProvider, LogFileFluentMigratorLoggerProvider>();
            }

            using (var serviceProvider = services.BuildServiceProvider(validateScopes: false))
            {
                var executor = serviceProvider.GetRequiredService<TaskExecutor>();
                executor.Execute();
            }

            return 0;
        }

        /// <summary>
        /// Creates the core services.
        /// </summary>
        /// <returns>IServiceCollection.</returns>
        private static IServiceCollection CreateCoreServices()
        {
            var services = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(
                    builder => builder
                        .AddDb2()
                        .AddDb2ISeries()
                        .AddDotConnectOracle()
                        .AddDotConnectOracle12C()
                        .AddFirebird()
                        .AddHana()
                        .AddMySql4()
                        .AddMySql5()
                        .AddOracle()
                        .AddOracle12C()
                        .AddOracleManaged()
                        .AddOracle12CManaged()
                        .AddPostgres()
                        .AddPostgres92()
                        .AddRedshift()
                        .AddSqlAnywhere()
                        .AddSQLite()
                        .AddSqlServer()
                        .AddSqlServer2000()
                        .AddSqlServer2005()
                        .AddSqlServer2008()
                        .AddSqlServer2012()
                        .AddSqlServer2014()
                        .AddSqlServer2016()
                        .AddSqlServerCe());
            return services;
        }
    }
}
