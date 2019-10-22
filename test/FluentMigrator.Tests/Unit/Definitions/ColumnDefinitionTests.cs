// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ColumnDefinitionTests.cs" company="FluentMigrator Project">
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

using System.Data;

using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Tests.Helpers;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit.Definitions
{
    /// <summary>
    /// Defines test class ColumnDefinitionTests.
    /// </summary>
    [TestFixture]
    public class ColumnDefinitionTests
    {
        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenColumnNameIsNull.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenColumnNameIsNull()
        {
            var column = new ColumnDefinition { Name = null };
            var errors = ValidationHelper.CollectErrors(column);
            errors.ShouldContain(ErrorMessages.ColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenColumnNameIsEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenColumnNameIsEmptyString()
        {
            var column = new ColumnDefinition { Name = string.Empty };
            var errors = ValidationHelper.CollectErrors(column);
            errors.ShouldContain(ErrorMessages.ColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenColumnNameIsNotNullOrEmptyString.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenColumnNameIsNotNullOrEmptyString()
        {
            var column = new ColumnDefinition { Name = "Bacon" };
            var errors = ValidationHelper.CollectErrors(column);
            errors.ShouldNotContain(ErrorMessages.ColumnNameCannotBeNullOrEmpty);
        }

        /// <summary>
        /// Defines the test method ErrorIsReturnedWhenColumnTypeIsNotSet.
        /// </summary>
        [Test]
        public void ErrorIsReturnedWhenColumnTypeIsNotSet()
        {
            var column = new ColumnDefinition { Name = "Column", Type = null };
            var errors = ValidationHelper.CollectErrors(column);
            errors.ShouldContain(ErrorMessages.ColumnTypeMustBeDefined);
        }

        /// <summary>
        /// Defines the test method ErrorIsNotReturnedWhenColumnTypeIsSet.
        /// </summary>
        [Test]
        public void ErrorIsNotReturnedWhenColumnTypeIsSet()
        {
            var column = new ColumnDefinition { Name = "Column", Type = DbType.String };
            var errors = ValidationHelper.CollectErrors(column);
            errors.ShouldNotContain(ErrorMessages.ColumnTypeMustBeDefined);
        }
    }
}
