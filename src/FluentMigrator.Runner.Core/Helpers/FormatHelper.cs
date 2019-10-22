// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FormatHelper.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace FluentMigrator.Runner.Helpers
{
    /// <summary>
    /// Class FormatHelper.
    /// </summary>
    public class FormatHelper
    {
        /// <summary>
        /// Formats the SQL escape.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>System.String.</returns>
        public static string FormatSqlEscape(string sql)
        {
            return sql.Replace("'", "''");
        }
    }
}
