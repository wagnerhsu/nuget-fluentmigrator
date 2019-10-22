// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Firebird
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdOptions.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Data.Common;

namespace FluentMigrator.Runner.Processors.Firebird
{
    /// <summary>
    /// Enum FirebirdTransactionModel
    /// </summary>
    public enum FirebirdTransactionModel
    {
        /// <summary>
        /// Automatically starts a new transaction when a virtual lock check fails
        /// </summary>
        AutoCommitOnCheckFail,

        /// <summary>
        /// Automaticaly commits every processed statement
        /// </summary>
        AutoCommit,

        /// <summary>
        /// Don't manage transactions
        /// </summary>
        None
    }

    /// <summary>
    /// Class FirebirdOptions.
    /// Implements the <see cref="System.ICloneable" />
    /// </summary>
    /// <seealso cref="System.ICloneable" />
    public class FirebirdOptions : ICloneable
    {
        /// <summary>
        /// Maximum internal length of names in firebird is 31 characters
        /// </summary>
        public static readonly int MaxNameLength = 31;

        /// <summary>
        /// Firebird only supports constraint, table, column, etc. names up to 31 characters
        /// </summary>
        /// <value><c>true</c> if [truncate long names]; otherwise, <c>false</c>.</value>
        public bool TruncateLongNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pack key names].
        /// </summary>
        /// <value><c>true</c> if [pack key names]; otherwise, <c>false</c>.</value>
        public bool PackKeyNames { get; set; }

        /// <summary>
        /// Virtually lock tables and columns touched by DDL statements in a transaction
        /// </summary>
        /// <value><c>true</c> if [virtual lock]; otherwise, <c>false</c>.</value>
        public bool VirtualLock { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all names should be quoted unconditionally.
        /// </summary>
        /// <value><c>true</c> if [force quote]; otherwise, <c>false</c>.</value>
        public bool ForceQuote { get; set; }

        /// <summary>
        /// Which transaction model to use if any to work around firebird's DDL restrictions
        /// </summary>
        /// <value>The transaction model.</value>
        public FirebirdTransactionModel TransactionModel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdOptions"/> class.
        /// </summary>
        public FirebirdOptions()
        {
            TransactionModel = FirebirdTransactionModel.None;
            TruncateLongNames = false;
            VirtualLock = false;
        }

        /// <summary>
        /// Standards the behaviour.
        /// </summary>
        /// <returns>FirebirdOptions.</returns>
        public static FirebirdOptions StandardBehaviour()
        {
            return new FirebirdOptions()
            {
                TransactionModel = FirebirdTransactionModel.None,
                TruncateLongNames = false,
                VirtualLock = false,
            };
        }

        /// <summary>
        /// Commits the on check fail behaviour.
        /// </summary>
        /// <returns>FirebirdOptions.</returns>
        public static FirebirdOptions CommitOnCheckFailBehaviour()
        {
            return new FirebirdOptions()
            {
                TransactionModel = FirebirdTransactionModel.AutoCommitOnCheckFail,
                TruncateLongNames = true,
                VirtualLock = true,
            };
        }

        /// <summary>
        /// Automatics the commit behaviour.
        /// </summary>
        /// <returns>FirebirdOptions.</returns>
        public static FirebirdOptions AutoCommitBehaviour()
        {
            return new FirebirdOptions()
            {
                TransactionModel = FirebirdTransactionModel.AutoCommit,
                TruncateLongNames = true,
                VirtualLock = false,
            };
        }

        /// <summary>
        /// Applies the provider switches.
        /// </summary>
        /// <param name="providerSwitches">The provider switches.</param>
        /// <returns>FirebirdOptions.</returns>
        public FirebirdOptions ApplyProviderSwitches(string providerSwitches)
        {
            var csb = new DbConnectionStringBuilder {ConnectionString = providerSwitches};
            if (csb.TryGetValue("Force Quote", out var forceQuoteObj))
            {
                ForceQuote = ConvertToBoolean(forceQuoteObj);
            }

            return this;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Converts to boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool ConvertToBoolean(object value)
        {
            if (value is bool b)
                return b;
            if (value is string s)
                return ConvertToBoolean(s);
            return Convert.ToInt32(value) != 0;
        }

        /// <summary>
        /// Converts to boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool ConvertToBoolean(string value)
        {
            if (string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}
