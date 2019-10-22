// ***********************************************************************
// Assembly         : FluentMigrator.Runner.MySql
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="MySql5TypeMap.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, FluentMigrator Project
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

using System.Data;

namespace FluentMigrator.Runner.Generators.MySql
{
    /// <summary>
    /// Class MySql5TypeMap.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.MySql.MySql4TypeMap" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.MySql.MySql4TypeMap" />
    internal class MySql5TypeMap : MySql4TypeMap
    {
        /// <summary>
        /// The decimal capacity
        /// </summary>
        public new const int DecimalCapacity = 65;

        /// <summary>
        /// Setups the type maps.
        /// </summary>
        protected override void SetupTypeMaps()
        {
            base.SetupTypeMaps();

            SetTypeMap(DbType.Decimal, "DECIMAL($size,$precision)", DecimalCapacity);

            SetTypeMap(DbType.StringFixedLength, "NCHAR(255)");
            SetTypeMap(DbType.StringFixedLength, "NCHAR($size)", StringCapacity);
            SetTypeMap(DbType.StringFixedLength, "TEXT CHARACTER SET utf8", TextCapacity);
            SetTypeMap(DbType.StringFixedLength, "MEDIUMTEXT CHARACTER SET utf8", MediumTextCapacity);
            SetTypeMap(DbType.StringFixedLength, "LONGTEXT CHARACTER SET utf8", LongTextCapacity);
            SetTypeMap(DbType.String, "NVARCHAR(255)");
            SetTypeMap(DbType.String, "NVARCHAR($size)", VarcharCapacity);
            SetTypeMap(DbType.String, "TEXT CHARACTER SET utf8", TextCapacity);
            SetTypeMap(DbType.String, "MEDIUMTEXT CHARACTER SET utf8", MediumTextCapacity);
            SetTypeMap(DbType.String, "LONGTEXT CHARACTER SET utf8", LongTextCapacity);
        }
    }
}
