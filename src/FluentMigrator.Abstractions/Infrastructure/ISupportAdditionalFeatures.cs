// ***********************************************************************
// Assembly         : FluentMigrator.Abstractions
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ISupportAdditionalFeatures.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;

namespace FluentMigrator.Infrastructure
{
    /// <summary>
    /// Provides a dictionary to store values for non-standard additional features
    /// </summary>
    public interface ISupportAdditionalFeatures
    {
        /// <summary>
        /// Gets the dictionary to store the values for additional features
        /// </summary>
        /// <value>The additional features.</value>
        IDictionary<string, object> AdditionalFeatures { get; }
    }
}
