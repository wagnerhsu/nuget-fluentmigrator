// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseIndexTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace FluentMigrator.Tests.Integration.Processors
{
    /// <summary>
    /// Class BaseIndexTests.
    /// </summary>
    public abstract class BaseIndexTests
    {
        /// <summary>
        /// Callings the index exists can accept index name with single quote.
        /// </summary>
        public abstract void CallingIndexExistsCanAcceptIndexNameWithSingleQuote();
        /// <summary>
        /// Callings the index exists can accept table name with single quote.
        /// </summary>
        public abstract void CallingIndexExistsCanAcceptTableNameWithSingleQuote();
        /// <summary>
        /// Callings the index exists returns false if index does not exist.
        /// </summary>
        public abstract void CallingIndexExistsReturnsFalseIfIndexDoesNotExist();
        /// <summary>
        /// Callings the index exists returns false if index does not exist with schema.
        /// </summary>
        public abstract void CallingIndexExistsReturnsFalseIfIndexDoesNotExistWithSchema();
        /// <summary>
        /// Callings the index exists returns false if table does not exist.
        /// </summary>
        public abstract void CallingIndexExistsReturnsFalseIfTableDoesNotExist();
        /// <summary>
        /// Callings the index exists returns false if table does not exist with schema.
        /// </summary>
        public abstract void CallingIndexExistsReturnsFalseIfTableDoesNotExistWithSchema();
        /// <summary>
        /// Callings the index exists returns true if index exists.
        /// </summary>
        public abstract void CallingIndexExistsReturnsTrueIfIndexExists();
        /// <summary>
        /// Callings the index exists returns true if index exists with schema.
        /// </summary>
        public abstract void CallingIndexExistsReturnsTrueIfIndexExistsWithSchema();
    }
}