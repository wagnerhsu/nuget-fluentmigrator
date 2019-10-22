// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SystemMethods.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace FluentMigrator
{
    /// <summary>
    /// The well-known system methods
    /// </summary>
    public enum SystemMethods
    {
        /// <summary>
        /// The function to create a new GUID
        /// </summary>
        NewGuid,

        /// <summary>
        /// The function to create a new sequential GUID
        /// </summary>
        NewSequentialId,

        /// <summary>
        /// The function to get the current timestamp
        /// </summary>
        CurrentDateTime,

        /// <summary>
        /// The function to get the current timestamp with time zone
        /// </summary>
        CurrentDateTimeOffset,

        /// <summary>
        /// The function to get the current UTC timestamp
        /// </summary>
        CurrentUTCDateTime,

        /// <summary>
        /// The function to get the current user
        /// </summary>
        CurrentUser,
    }
}
