// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IMigrationScopeManager.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace FluentMigrator.Runner
{
    /// <summary>
    /// Handler for <see cref="IMigrationScope" />
    /// </summary>
    public interface IMigrationScopeManager
    {
        /// <summary>
        /// Gets migration scope for the runner
        /// </summary>
        /// <value>The current scope.</value>
        IMigrationScope CurrentScope { get; }

        /// <summary>
        /// Creates new migration scope
        /// </summary>
        /// <returns>Newly created scope</returns>
        IMigrationScope BeginScope();

        /// <summary>
        /// Creates new migrations scope or reuses existing one
        /// </summary>
        /// <param name="transactional">Defines if transactions should be used</param>
        /// <returns>Migration scope</returns>
        IMigrationScope CreateOrWrapMigrationScope(bool transactional = true);
    }
}
