// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseSchemaExtensionsTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace FluentMigrator.Tests.Integration.Processors
{
    /// <summary>
    /// Class BaseSchemaExtensionsTests.
    /// </summary>
    public abstract class BaseSchemaExtensionsTests
    {
        /// <summary>
        /// Callings the column exists can accept schema name with single quote.
        /// </summary>
        public abstract void CallingColumnExistsCanAcceptSchemaNameWithSingleQuote();
        /// <summary>
        /// Callings the constraint exists can accept schema name with single quote.
        /// </summary>
        public abstract void CallingConstraintExistsCanAcceptSchemaNameWithSingleQuote();
        /// <summary>
        /// Callings the index exists can accept schema name with single quote.
        /// </summary>
        public abstract void CallingIndexExistsCanAcceptSchemaNameWithSingleQuote();
        /// <summary>
        /// Callings the schema exists can accept schema name with single quote.
        /// </summary>
        public abstract void CallingSchemaExistsCanAcceptSchemaNameWithSingleQuote();
        /// <summary>
        /// Callings the table exists can accept schema name with single quote.
        /// </summary>
        public abstract void CallingTableExistsCanAcceptSchemaNameWithSingleQuote();
    }
}