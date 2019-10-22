// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ProfileAttribute.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace FluentMigrator
{
    /// <summary>
    /// Defines a profile for a migration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ProfileAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileAttribute" /> class.
        /// </summary>
        /// <param name="profileName">The profile name</param>
        public ProfileAttribute(string profileName)
        {
            ProfileName = profileName;
        }

        /// <summary>
        /// Gets the profile name
        /// </summary>
        /// <value>The name of the profile.</value>
        public string ProfileName { get; }
    }
}
