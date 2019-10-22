// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Issue817.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2018, FluentMigrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using FluentMigrator.Tests.Integration.Migrations.Firebird.Issue817;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd
{
    /// <summary>
    /// Defines test class Issue817.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd.FbEndToEndFixture" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.Firebird.EndToEnd.FbEndToEndFixture" />
    [TestFixture]
    [Category("Integration")]
    [Category("Firebird")]
    public class Issue817 : FbEndToEndFixture
    {
        /// <summary>
        /// Defines the test method TestIssue817.
        /// </summary>
        [Test]
        public void TestIssue817()
        {
            var executor = MakeTask(
                "migrate",
                typeof(Migration_v100_AddUsersTable).Namespace,
                ctxt => { ctxt.PreviewOnly = true; });
            executor.Execute();
        }
    }
}
