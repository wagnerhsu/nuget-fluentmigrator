// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Oracle
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleDbFactory.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2018, Fluent Migrator Project
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

namespace FluentMigrator.Runner.Processors.Oracle
{
    /// <summary>
    /// Class OracleDbFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.Oracle.OracleBaseDbFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.Oracle.OracleBaseDbFactory" />
    public class OracleDbFactory : OracleBaseDbFactory
    {
        /// <summary>
        /// The test entries
        /// </summary>
        private static readonly TestEntry[] _testEntries =
        {
            new TestEntry("Oracle.DataAccess", "Oracle.DataAccess.Client.OracleClientFactory"),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDbFactory"/> class.
        /// </summary>
        [Obsolete]
        public OracleDbFactory()
            : this(serviceProvider: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDbFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public OracleDbFactory(IServiceProvider serviceProvider)
            : base(serviceProvider, _testEntries)
        {
        }
    }
}
