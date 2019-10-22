// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="Migration_v200_AlterTableUsersModifyColumnName.cs" company="FluentMigrator Project">
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

using System;

namespace FluentMigrator.Tests.Integration.Migrations.Firebird.Issue817
{
    /// <summary>
    /// Class Migration_v200_AlterTableUsersModifyColumnName.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Migration(200)]
    // ReSharper disable once InconsistentNaming
    public class Migration_v200_AlterTableUsersModifyColumnName : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Alter.Table("Users")
                .AlterColumn("Name")
                .AsString(80);
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
