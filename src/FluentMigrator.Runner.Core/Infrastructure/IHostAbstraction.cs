// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="IHostAbstraction.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

using JetBrains.Annotations;

namespace FluentMigrator.Runner.Infrastructure
{
    /// <summary>
    /// Interface IHostAbstraction
    /// </summary>
    public interface IHostAbstraction
    {
        /// <summary>
        /// Gets the base directory.
        /// </summary>
        /// <value>The base directory.</value>
        string BaseDirectory { get; }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>System.Object.</returns>
        [NotNull]
        object CreateInstance([CanBeNull] IServiceProvider serviceProvider, [NotNull] string assemblyName, [NotNull] string typeName);

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        [NotNull, ItemNotNull]
        IEnumerable<Assembly> GetLoadedAssemblies();
    }
}
