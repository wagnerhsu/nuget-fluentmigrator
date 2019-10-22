// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ObsoleteExecuteEmbeddedSqlExpressionTests.cs" company="FluentMigrator Project">
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

using NUnit.Framework;
using FluentMigrator.Expressions;
using FluentMigrator.Tests.Helpers;
using FluentMigrator.Infrastructure;
using Moq;

using System;
using System.Collections.Generic;
using System.Reflection;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Expressions
{
    /// <summary>
    /// Defines test class ObsoleteExecuteEmbeddedSqlScriptExpressionTests.
    /// </summary>
    [TestFixture]
    [Obsolete]
    public class ObsoleteExecuteEmbeddedSqlScriptExpressionTests
    {
        /// <summary>
        /// The test SQL script
        /// </summary>
        private const string TestSqlScript = "embeddedtestscript.sql";
        /// <summary>
        /// The script contents
        /// </summary>
        private const string ScriptContents = "TEST SCRIPT";

        /// <summary>
        /// Defines the test method ErrorIsReturnWhenSqlScriptIsNullOrEmpty.
        /// </summary>
        [Test]
        public void ErrorIsReturnWhenSqlScriptIsNullOrEmpty()
        {
            var expression = new ExecuteEmbeddedSqlScriptExpression { SqlScript = null };
            var errors = ValidationHelper.CollectErrors(expression);
            errors.ShouldContain(ErrorMessages.SqlScriptCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ExecutesTheStatement.
        /// </summary>
        [Test]
        public void ExecutesTheStatement()
        {
            var expression = new ExecuteEmbeddedSqlScriptExpression { SqlScript = TestSqlScript, MigrationAssemblies = new SingleAssembly(Assembly.GetExecutingAssembly()) };

            var processor = new Mock<IMigrationProcessor>();
            processor.Setup(x => x.Execute(ScriptContents)).Verifiable();

            expression.ExecuteWith(processor.Object);
            processor.Verify();
        }

        /// <summary>
        /// Defines the test method ExecutesTheStatementWithParameters.
        /// </summary>
        [Test]
        public void ExecutesTheStatementWithParameters()
        {
            const string scriptContentsWithParameters = "TEST SCRIPT ParameterValue $(escaped_parameter) $(missing_parameter)";
            var expression = new ExecuteEmbeddedSqlScriptExpression
            {
                SqlScript = "EmbeddedTestScriptWithParameters.sql",
                MigrationAssemblies = new SingleAssembly(Assembly.GetExecutingAssembly()),
                Parameters = new Dictionary<string, string> { { "parameter", "ParameterValue" } }
            };

            var processor = new Mock<IMigrationProcessor>();
            processor.Setup(x => x.Execute(scriptContentsWithParameters)).Verifiable();

            expression.ExecuteWith(processor.Object);
            processor.Verify();
        }

        /// <summary>
        /// Defines the test method ResourceFinderIsCaseInsensitive.
        /// </summary>
        [Test]
        public void ResourceFinderIsCaseInsensitive()
        {
            var expression = new ExecuteEmbeddedSqlScriptExpression { SqlScript = TestSqlScript.ToUpper(), MigrationAssemblies = new SingleAssembly(Assembly.GetExecutingAssembly()) };
            var processor = new Mock<IMigrationProcessor>();
            processor.Setup(x => x.Execute(ScriptContents)).Verifiable();

            expression.ExecuteWith(processor.Object);
            processor.Verify();
        }

        /// <summary>
        /// Defines the test method ResourceFinderFindFileWithFullName.
        /// </summary>
        [Test]
        public void ResourceFinderFindFileWithFullName()
        {
            var expression = new ExecuteEmbeddedSqlScriptExpression { SqlScript = "InitialSchema.sql", MigrationAssemblies = new SingleAssembly(Assembly.GetExecutingAssembly()) };
            var processor = new Mock<IMigrationProcessor>();
            processor.Setup(x => x.Execute("InitialSchema")).Verifiable();

            expression.ExecuteWith(processor.Object);
            processor.Verify();
        }

        /// <summary>
        /// Defines the test method ResourceFinderFindFileWithFullNameAndNamespace.
        /// </summary>
        [Test]
        public void ResourceFinderFindFileWithFullNameAndNamespace()
        {
            var expression = new ExecuteEmbeddedSqlScriptExpression { SqlScript = "FluentMigrator.Tests.EmbeddedResources.InitialSchema.sql", MigrationAssemblies = new SingleAssembly(Assembly.GetExecutingAssembly()) };
            var processor = new Mock<IMigrationProcessor>();
            processor.Setup(x => x.Execute("InitialSchema")).Verifiable();

            expression.ExecuteWith(processor.Object);
            processor.Verify();
        }

        /// <summary>
        /// Defines the test method ResourceFinderFindThrowsExceptionIfFoundMoreThenOneResource.
        /// </summary>
        [Test]
        public void ResourceFinderFindThrowsExceptionIfFoundMoreThenOneResource()
        {
            var expression = new ExecuteEmbeddedSqlScriptExpression { SqlScript = "NotUniqueResource.sql", MigrationAssemblies = new SingleAssembly(Assembly.GetExecutingAssembly()) };
            var processor = new Mock<IMigrationProcessor>();

            Assert.Throws<InvalidOperationException>(() => expression.ExecuteWith(processor.Object));
            processor.Verify(x => x.Execute("NotUniqueResource"), Times.Never());
        }

        /// <summary>
        /// Defines the test method ToStringIsDescriptive.
        /// </summary>
        [Test]
        public void ToStringIsDescriptive()
        {
            var expression = new ExecuteSqlScriptExpression { SqlScript = TestSqlScript };
            expression.ToString().ShouldBe("ExecuteSqlScript embeddedtestscript.sql");
        }
    }
}
