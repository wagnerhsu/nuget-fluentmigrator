// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Db2
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Db2ISeriesDbFactory.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Processors.DB2.iSeries
{
    /// <summary>
    /// Class Db2ISeriesDbFactory.
    /// Implements the <see cref="FluentMigrator.Runner.Processors.ReflectionBasedDbFactory" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Processors.ReflectionBasedDbFactory" />
    public class Db2ISeriesDbFactory : ReflectionBasedDbFactory
    {
        /// <summary>
        /// The test entries
        /// </summary>
        private static readonly TestEntry[] _testEntries =
        {
            new TestEntry("IBM.Data.DB2.iSeries", "IBM.Data.DB2.iSeries.iDB2Factory"),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ISeriesDbFactory"/> class.
        /// </summary>
        [Obsolete]
        public Db2ISeriesDbFactory()
            : base(_testEntries)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db2ISeriesDbFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public Db2ISeriesDbFactory(IServiceProvider serviceProvider)
            : base(serviceProvider, _testEntries)
        {
        }
    }
}
