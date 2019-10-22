// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ColumnModificationType.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace FluentMigrator.Model
{
    /// <summary>
    /// Indicates wheter a column should be created or altered
    /// </summary>
    public enum ColumnModificationType
    {
        /// <summary>
        /// The column in question should be created
        /// </summary>
        Create,

        /// <summary>
        /// The column in question should be altered
        /// </summary>
        Alter
    }
}
