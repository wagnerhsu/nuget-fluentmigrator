// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseSequenceTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace FluentMigrator.Tests.Integration.Processors
{
    /// <summary>
    /// Class BaseSequenceTests.
    /// </summary>
    public abstract class BaseSequenceTests
    {
        /// <summary>
        /// Callings the sequence exists returns false if sequence does not exist.
        /// </summary>
        public abstract void CallingSequenceExistsReturnsFalseIfSequenceDoesNotExist();
        /// <summary>
        /// Callings the sequence exists returns false if sequence does not exist with schema.
        /// </summary>
        public abstract void CallingSequenceExistsReturnsFalseIfSequenceDoesNotExistWithSchema();
        /// <summary>
        /// Callings the sequence exists returns true if sequence exists.
        /// </summary>
        public abstract void CallingSequenceExistsReturnsTrueIfSequenceExists();
        /// <summary>
        /// Callings the sequence exists returns true if sequence exists with schema.
        /// </summary>
        public abstract void CallingSequenceExistsReturnsTrueIfSequenceExistsWithSchema();
    }
}