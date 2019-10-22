// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ExecuteSqlScriptExpressionTests.cs" company="FluentMigrator Project">
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
using System.IO;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Tests.Helpers;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Expressions
{
    /// <summary>
    /// Defines test class ExecuteSqlScriptExpressionTests.
    /// </summary>
    [TestFixture]
    public class ExecuteSqlScriptExpressionTests
    {
        /// <summary>
        /// The test SQL script
        /// </summary>
        private string testSqlScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testscript.sql");
        /// <summary>
        /// The script contents
        /// </summary>
        private string scriptContents = "TEST SCRIPT";

        /// <summary>
        /// Defines the test method ErrorIsReturnWhenSqlScriptIsNullOrEmpty.
        /// </summary>
        [Test]
        public void ErrorIsReturnWhenSqlScriptIsNullOrEmpty()
        {
            var expression = new ExecuteSqlScriptExpression { SqlScript = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.SqlScriptCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ExecutesTheStatementWithParameters.
        /// </summary>
        [Test]
        public void ExecutesTheStatementWithParameters()
        {
            const string scriptContentsWithParameters = "TEST SCRIPT ParameterValue $(escaped_parameter) $(missing_parameter)";
            var expression = new ExecuteSqlScriptExpression()
            {
                SqlScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestScriptWithParameters.sql"),
                Parameters = new Dictionary<string, string> { { "parameter", "ParameterValue" } }
            };

            var processor = new Mock<IMigrationProcessor>();
            processor.Setup(x => x.Execute(scriptContentsWithParameters)).Verifiable();

            expression.ExecuteWith(processor.Object);
            processor.Verify();
        }

        /// <summary>
        /// Defines the test method ExecutesTheStatement.
        /// </summary>
        [Test]
        public void ExecutesTheStatement()
        {
            var expression = new ExecuteSqlScriptExpression { SqlScript = testSqlScript };

            var processor = new Mock<IMigrationProcessor>();
            processor.Setup(x => x.Execute(scriptContents)).Verifiable();

            expression.ExecuteWith(processor.Object);
            processor.Verify();
        }

        /// <summary>
        /// Defines the test method ToStringIsDescriptive.
        /// </summary>
        [Test]
        public void ToStringIsDescriptive()
        {
            var expression = new ExecuteSqlScriptExpression { SqlScript = testSqlScript };
            expression.ToString().ShouldBe($"ExecuteSqlScript {testSqlScript}");
        }

        /// <summary>
        /// Defines the test method CanUseScriptsOnAnotherDriveToWorkingDirectory.
        /// </summary>
        [Test]
        [Category("NotWorkingOnMono")]
        public void CanUseScriptsOnAnotherDriveToWorkingDirectory()
        {
            var scriptOnAnotherDrive = "z" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar + testSqlScript;
            var expression = new ExecuteSqlScriptExpression { SqlScript = scriptOnAnotherDrive };

            var defaultRootPath = "c" + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar + "code";
            var conventionSet = ConventionSets.CreateNoSchemaName(defaultRootPath);
            var processed = expression.Apply(conventionSet);

            processed.SqlScript.ShouldBe(scriptOnAnotherDrive);
        }
    }
}
