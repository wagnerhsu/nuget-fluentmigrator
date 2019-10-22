// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseColumnTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors
{
    /// <summary>
    /// Class BaseColumnTests.
    /// </summary>
    [Category("Integration")]
    [Category("Column")]
    public abstract class BaseColumnTests
    {
        /// <summary>
        /// Callings the column exists can accept column name with single quote.
        /// </summary>
        public abstract void CallingColumnExistsCanAcceptColumnNameWithSingleQuote();
        /// <summary>
        /// Callings the column exists can accept table name with single quote.
        /// </summary>
        public abstract void CallingColumnExistsCanAcceptTableNameWithSingleQuote();
        /// <summary>
        /// Callings the column exists returns false if column does not exist.
        /// </summary>
        public abstract void CallingColumnExistsReturnsFalseIfColumnDoesNotExist();
        /// <summary>
        /// Callings the column exists returns false if column does not exist with schema.
        /// </summary>
        public abstract void CallingColumnExistsReturnsFalseIfColumnDoesNotExistWithSchema();
        /// <summary>
        /// Callings the column exists returns false if table does not exist.
        /// </summary>
        public abstract void CallingColumnExistsReturnsFalseIfTableDoesNotExist();
        /// <summary>
        /// Callings the column exists returns false if table does not exist with schema.
        /// </summary>
        public abstract void CallingColumnExistsReturnsFalseIfTableDoesNotExistWithSchema();
        /// <summary>
        /// Callings the column exists returns true if column exists.
        /// </summary>
        public abstract void CallingColumnExistsReturnsTrueIfColumnExists();
        /// <summary>
        /// Callings the column exists returns true if column exists with schema.
        /// </summary>
        public abstract void CallingColumnExistsReturnsTrueIfColumnExistsWithSchema();
    }
}
