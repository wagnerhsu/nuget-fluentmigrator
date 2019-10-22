// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="NetFrameworkHost.cs" company="FluentMigrator Project">
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

#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Reflection;

namespace FluentMigrator.Runner.Infrastructure.Hosts
{
    /// <summary>
    /// Class NetFrameworkHost.
    /// Implements the <see cref="FluentMigrator.Runner.Infrastructure.IHostAbstraction" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Infrastructure.IHostAbstraction" />
    internal class NetFrameworkHost : IHostAbstraction
    {
        /// <summary>
        /// Gets the base directory.
        /// </summary>
        /// <value>The base directory.</value>
        public string BaseDirectory
            => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>System.Object.</returns>
        public object CreateInstance(IServiceProvider serviceProvider, string assemblyName, string typeName)
        {
            if (serviceProvider != null)
            {
                try
                {
                    var asm = AppDomain.CurrentDomain.Load(new AssemblyName(assemblyName));
                    var type = asm.GetType(typeName, true);
                    var result = serviceProvider.GetService(type);
                    if (result != null)
                        return result;
                }
                catch
                {
                    // Ignore, fall back to legacy method
                }
            }

            return AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, typeName);
        }

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        public IEnumerable<Assembly> GetLoadedAssemblies()
            => AppDomain.CurrentDomain.GetAssemblies();
    }
}
#endif
