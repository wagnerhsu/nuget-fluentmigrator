// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Db2
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2DbFactory.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Processors.DB2
{
    /// <summary>
    /// Class Db2DbFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.ReflectionBasedDbFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.ReflectionBasedDbFactory" />
    public class Db2DbFactory : ReflectionBasedDbFactory
    {
        /// <summary>
        /// The test entries
        /// </summary>
        private static readonly TestEntry[] _testEntries =
        {
            new TestEntry("IBM.Data.DB2.Core", "IBM.Data.DB2.Core.DB2Factory"),
            new TestEntry("IBM.Data.DB2", "IBM.Data.DB2.DB2Factory"),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2DbFactory"/> class.
        /// </summary>
        [Obsolete]
        public Db2DbFactory()
            : base(_testEntries)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2DbFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public Db2DbFactory(IServiceProvider serviceProvider)
            : base(serviceProvider, _testEntries)
        {
        }
    }
}
