// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServerCe
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerCeColumn.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Model;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServerCeColumn.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000Column" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000Column" />
    internal class SqlServerCeColumn : SqlServer2000Column
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCeColumn"/> class.
        /// </summary>
        /// <param name="typeMap">The type map.</param>
        /// <param name="quoter">The quoter.</param>
        public SqlServerCeColumn(ITypeMap typeMap, IQuoter quoter)
            : base(typeMap, quoter)
        {
        }

        /// <inheritdoc />
        protected override string FormatNullable(ColumnDefinition column)
        {
            if (column.IsNullable.GetValueOrDefault())
                return column.ModificationType == ColumnModificationType.Alter ? "NULL" : string.Empty;

            return "NOT NULL";
        }
    }
}
