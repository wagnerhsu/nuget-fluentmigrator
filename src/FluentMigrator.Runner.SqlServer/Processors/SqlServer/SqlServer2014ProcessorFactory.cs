// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2014ProcessorFactory.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

using FluentMigrator.Runner.Generators.SqlServer;

namespace FluentMigrator.Runner.Processors.SqlServer
{
    /// <summary>
    /// Class SqlServer2014ProcessorFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    [Obsolete]
    public class SqlServer2014ProcessorFactory : MigrationProcessorFactory
    {
        /// <summary>
        /// The database types
        /// </summary>
        private static readonly string[] _dbTypes = {"SqlServer2014", "SqlServer"};

        /// <summary>
        /// Creates the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="announcer">The announcer.</param>
        /// <param name="options">The options.</param>
        /// <returns>IMigrationProcessor.</returns>
        [Obsolete]
        public override IMigrationProcessor Create(string connectionString, IAnnouncer announcer, IMigrationProcessorOptions options)
        {
            var factory = new SqlServerDbFactory();
            var connection = factory.CreateConnection(connectionString);
            return new SqlServerProcessor(_dbTypes, connection, new SqlServer2014Generator(new SqlServer2008Quoter()), announcer, options, factory);
        }
    }
}
