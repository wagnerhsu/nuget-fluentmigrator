// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Jet
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="JetColumn.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Data;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;

namespace FluentMigrator.Runner.Generators.Jet
{
    /// <summary>
    /// Class JetColumn.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.ColumnBase" />
    internal class JetColumn : ColumnBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JetColumn"/> class.
        /// </summary>
        public JetColumn()
            : base(new JetTypeMap(), new JetQuoter())
        {

        }

        /// <inheritdoc />
        protected override string FormatType(ColumnDefinition column)
        {
            if (column.IsIdentity)
            {
                // In Jet an identity column always of type COUNTER which is a integer type
                if (column.Type != DbType.Int32)
                {
                    throw new ArgumentException("Jet Engine only allows identity columns on integer columns");
                }

                return "COUNTER";
            }
            return base.FormatType(column);
        }

        /// <inheritdoc />
        protected override string FormatIdentity(ColumnDefinition column)
        {
            //Identity type is handled by FormatType
            return string.Empty;
        }
    }
}
