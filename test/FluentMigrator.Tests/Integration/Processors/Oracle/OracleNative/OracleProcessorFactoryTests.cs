// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="OracleProcessorFactoryTests.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.Processors.Oracle;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Oracle.OracleNative
{
    /// <summary>
    /// Defines test class OracleProcessorFactoryTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.Oracle.OracleProcessorFactoryTestsBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.Oracle.OracleProcessorFactoryTestsBase" />
    [TestFixture]
    [Category("Oracle")]
    [Obsolete]
    public class OracleProcessorFactoryTests : OracleProcessorFactoryTestsBase
    {
        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            base.SetUp(new OracleProcessorFactory(serviceProvider: null));
        }
    }
}
