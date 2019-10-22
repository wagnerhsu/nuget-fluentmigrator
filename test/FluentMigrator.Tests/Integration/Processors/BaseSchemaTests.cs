// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseSchemaTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace FluentMigrator.Tests.Integration.Processors
{
    /// <summary>
    /// Class BaseSchemaTests.
    /// </summary>
    public abstract class BaseSchemaTests
    {
        /// <summary>
        /// Callings the schema exists returns false if schema does not exist.
        /// </summary>
        public abstract void CallingSchemaExistsReturnsFalseIfSchemaDoesNotExist();
        /// <summary>
        /// Callings the schema exists returns true if schema exists.
        /// </summary>
        public abstract void CallingSchemaExistsReturnsTrueIfSchemaExists();
    }
}