// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="BaseConstraintTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors
{
    /// <summary>
    /// Class BaseConstraintTests.
    /// </summary>
    [Category("Integration")]
    [Category("Constraint")]
    public abstract class BaseConstraintTests
    {
        /// <summary>
        /// Callings the constraint exists can accept constraint name with single quote.
        /// </summary>
        public abstract void CallingConstraintExistsCanAcceptConstraintNameWithSingleQuote();
        /// <summary>
        /// Callings the constraint exists can accept table name with single quote.
        /// </summary>
        public abstract void CallingConstraintExistsCanAcceptTableNameWithSingleQuote();
        /// <summary>
        /// Callings the constraint exists returns false if constraint does not exist.
        /// </summary>
        public abstract void CallingConstraintExistsReturnsFalseIfConstraintDoesNotExist();
        /// <summary>
        /// Callings the constraint exists returns false if constraint does not exist with schema.
        /// </summary>
        public abstract void CallingConstraintExistsReturnsFalseIfConstraintDoesNotExistWithSchema();
        /// <summary>
        /// Callings the constraint exists returns false if table does not exist.
        /// </summary>
        public abstract void CallingConstraintExistsReturnsFalseIfTableDoesNotExist();
        /// <summary>
        /// Callings the constraint exists returns false if table does not exist with schema.
        /// </summary>
        public abstract void CallingConstraintExistsReturnsFalseIfTableDoesNotExistWithSchema();
        /// <summary>
        /// Callings the constraint exists returns true if constraint exists.
        /// </summary>
        public abstract void CallingConstraintExistsReturnsTrueIfConstraintExists();
        /// <summary>
        /// Callings the constraint exists returns true if constraint exists with schema.
        /// </summary>
        public abstract void CallingConstraintExistsReturnsTrueIfConstraintExistsWithSchema();
    }
}
