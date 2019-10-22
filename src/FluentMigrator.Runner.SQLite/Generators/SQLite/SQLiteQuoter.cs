// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SQLite
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SQLiteQuoter.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Runner.Generators.Generic;

namespace FluentMigrator.Runner.Generators.SQLite
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Class SQLiteQuoter.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Generic.GenericQuoter" />
    public class SQLiteQuoter : GenericQuoter
    {
        /// <summary>
        /// Formats the system methods.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public override string FormatSystemMethods(SystemMethods value)
        {
            switch (value)
            {
                case SystemMethods.CurrentUTCDateTime:
                    return "CURRENT_TIMESTAMP";
                case SystemMethods.CurrentDateTime:
                    return "(datetime('now','localtime'))";
            }

            return base.FormatSystemMethods(value);
        }

        /// <summary>
        /// Quotes the name of the schema.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>System.String.</returns>
        /// <inheritdoc />
        public override string QuoteSchemaName(string schemaName)
        {
            return string.Empty;
        }

        /// <summary>
        /// Formats the byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string FormatByteArray(byte[] value)
        {
            var hex = new System.Text.StringBuilder((value.Length * 2) + 3);
            hex.Append("X'");
            foreach (var b in value)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            hex.Append("'");

            return hex.ToString();
        }
    }
}
