// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="INetConfigManager.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#if NETFRAMEWORK
using System.Configuration;

using JetBrains.Annotations;

namespace FluentMigrator.Runner.Initialization.NetFramework
{
    /// <summary>
    /// Understand .NET config mechanism and provides access to Configuration sections
    /// </summary>
    public interface INetConfigManager
    {
        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Configuration.</returns>
        [NotNull]
        Configuration LoadFromFile(string path);

        /// <summary>
        /// Loads from machine configuration.
        /// </summary>
        /// <returns>Configuration.</returns>
        [NotNull]
        Configuration LoadFromMachineConfiguration();
    }
}
#endif
