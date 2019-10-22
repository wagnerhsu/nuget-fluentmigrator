// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Jet
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="JetGenerator.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Generic;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Jet
{
    /// <summary>
    /// Class JetGenerator.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericGenerator" />
    public class JetGenerator : GenericGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JetGenerator"/> class.
        /// </summary>
        public JetGenerator()
            : this(new JetQuoter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JetGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        public JetGenerator(
            [NotNull] JetQuoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JetGenerator"/> class.
        /// </summary>
        /// <param name="quoter">The quoter.</param>
        /// <param name="generatorOptions">The generator options.</param>
        public JetGenerator(
            [NotNull] JetQuoter quoter,
            [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new JetColumn(), quoter, new EmptyDescriptionGenerator(), generatorOptions)
        {
        }

        /// <summary>
        /// Gets the index of the drop.
        /// </summary>
        /// <value>The index of the drop.</value>
        public override string DropIndex { get { return "DROP INDEX {0} ON {1}"; } }

        /// <summary>
        /// Generates an SQL statement to rename a table
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameTableExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Renaming of tables is not supported for Jet");
        }

        /// <summary>
        /// Generates an SQL statement to rename a column
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(RenameColumnExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Renaming of columns is not supported for Jet");
        }

        /// <summary>
        /// Generates an SQL statement to alter a DEFAULT constraint
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(AlterDefaultConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Altering of default constraints is not supported for Jet");
        }

        /// <summary>
        /// Generates a <c>CREATE SEQUENCE</c> SQL statement
        /// </summary>
        /// <param name="expression">The expression to create the SQL for</param>
        /// <returns>The generated SQL</returns>
        public override string Generate(CreateSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported for Jet");
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(DeleteSequenceExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Sequences are not supported for Jet");
        }

        /// <summary>
        /// Generates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>System.String.</returns>
        public override string Generate(DeleteDefaultConstraintExpression expression)
        {
            return CompatibilityMode.HandleCompatibilty("Default constraints are not supported");
        }
    }
}
