// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseTableTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace FluentMigrator.Tests.Integration.Processors
{
    /// <summary>
    /// Class BaseTableTests.
    /// </summary>
    public abstract class BaseTableTests
    {
        /// <summary>
        /// Callings the table exists can accept table name with single quote.
        /// </summary>
        public abstract void CallingTableExistsCanAcceptTableNameWithSingleQuote();
        /// <summary>
        /// Callings the table exists returns false if table does not exist.
        /// </summary>
        public abstract void CallingTableExistsReturnsFalseIfTableDoesNotExist();
        /// <summary>
        /// Callings the table exists returns false if table does not exist with schema.
        /// </summary>
        public abstract void CallingTableExistsReturnsFalseIfTableDoesNotExistWithSchema();
        /// <summary>
        /// Callings the table exists returns true if table exists.
        /// </summary>
        public abstract void CallingTableExistsReturnsTrueIfTableExists();
        /// <summary>
        /// Callings the table exists returns true if table exists with schema.
        /// </summary>
        public abstract void CallingTableExistsReturnsTrueIfTableExistsWithSchema();
    }
}