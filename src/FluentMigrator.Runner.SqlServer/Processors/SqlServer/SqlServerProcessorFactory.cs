// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerProcessorFactory.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

using FluentMigrator.Runner.Generators.SqlServer;

namespace FluentMigrator.Runner.Processors.SqlServer
{
    /// <summary>
    /// Class SqlServerProcessorFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.MigrationProcessorFactory" />
    [Obsolete]
    public class SqlServerProcessorFactory : MigrationProcessorFactory
    {
        /// <summary>
        /// The database types
        /// </summary>
        private static readonly string[] _dbTypes = {"SqlServer"};

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProcessorFactory"/> class.
        /// </summary>
        [Obsolete]
        public SqlServerProcessorFactory()
        {
        }

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
            return new SqlServerProcessor(_dbTypes, connection, new SqlServer2016Generator(new SqlServer2008Quoter()), announcer, options, factory);
        }

        /// <summary>
        /// Determines whether [is for provider] [the specified provider].
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns><c>true</c> if [is for provider] [the specified provider]; otherwise, <c>false</c>.</returns>
        [Obsolete]
        public override bool IsForProvider(string provider)
        {
            return provider.ToLower().Contains("sqlclient");
        }
    }
}
