// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="EmptyDescriptionGenerator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using FluentMigrator.Expressions;

namespace FluentMigrator.Runner.Generators
{
    /// <summary>
    /// Class EmptyDescriptionGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.IDescriptionGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.IDescriptionGenerator" />
    public class EmptyDescriptionGenerator : IDescriptionGenerator
    {
        /// <summary>
        /// Generates the description statements.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GenerateDescriptionStatements(CreateTableExpression expression)
        {
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public string GenerateDescriptionStatement(AlterTableExpression expression)
        {
            return string.Empty;
        }

        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public string GenerateDescriptionStatement(CreateColumnExpression expression)
        {
            return string.Empty;
        }

        /// <summary>
        /// Generates the description statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public string GenerateDescriptionStatement(AlterColumnExpression expression)
        {
            return string.Empty;
        }
    }
}
