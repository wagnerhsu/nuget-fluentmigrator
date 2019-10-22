// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IDescriptionGenerator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using FluentMigrator.Expressions;

namespace FluentMigrator.Runner.Generators
{
    /// <summary>
    /// Generate SQL statements to set descriptions for tables and columns
    /// </summary>
    public interface IDescriptionGenerator
    {
        /// <summary>
        /// Generates the description statements.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GenerateDescriptionStatements(CreateTableExpression expression);
        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        string GenerateDescriptionStatement(AlterTableExpression expression);
        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        string GenerateDescriptionStatement(CreateColumnExpression expression);
        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        string GenerateDescriptionStatement(AlterColumnExpression expression);
    }
}
