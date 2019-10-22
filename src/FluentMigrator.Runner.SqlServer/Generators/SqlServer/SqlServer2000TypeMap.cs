// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2000TypeMap.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
//
// Copyright (c) 2007-2018, Fluent Migrator Project
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

using FluentMigrator.Runner.Generators.Base;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2000TypeMap.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.TypeMapBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.TypeMapBase" />
    public class SqlServer2000TypeMap : TypeMapBase
    {
        /// <summary>
        /// The ANSI string capacity
        /// </summary>
        public const int AnsiStringCapacity = 8000;
        /// <summary>
        /// The ANSI text capacity
        /// </summary>
        public const int AnsiTextCapacity = 2147483647;
        /// <summary>
        /// The unicode string capacity
        /// </summary>
        public const int UnicodeStringCapacity = 4000;
        /// <summary>
        /// The unicode text capacity
        /// </summary>
        public const int UnicodeTextCapacity = 1073741823;
        /// <summary>
        /// The image capacity
        /// </summary>
        public const int ImageCapacity = 2147483647;
        /// <summary>
        /// The decimal capacity
        /// </summary>
        public const int DecimalCapacity = 38;

        /// <summary>
        /// Setups the type maps.
        /// </summary>
        protected override void SetupTypeMaps()
        {
            SetTypeMap(DbType.AnsiStringFixedLength, "CHAR(255)");
            SetTypeMap(DbType.AnsiStringFixedLength, "CHAR($size)", AnsiStringCapacity);
            SetTypeMap(DbType.AnsiString, "VARCHAR(255)");
            SetTypeMap(DbType.AnsiString, "VARCHAR($size)", AnsiStringCapacity);
            SetTypeMap(DbType.AnsiString, "TEXT", AnsiTextCapacity);
            SetTypeMap(DbType.Binary, "VARBINARY(8000)");
            SetTypeMap(DbType.Binary, "VARBINARY($size)", AnsiStringCapacity);
            SetTypeMap(DbType.Binary, "IMAGE", ImageCapacity);
            SetTypeMap(DbType.Boolean, "BIT");
            SetTypeMap(DbType.Byte, "TINYINT");
            SetTypeMap(DbType.Currency, "MONEY");
            SetTypeMap(DbType.Date, "DATETIME");
            SetTypeMap(DbType.DateTime, "DATETIME");
            SetTypeMap(DbType.DateTime2, "DATETIME");
            SetTypeMap(DbType.Decimal, "DECIMAL(19,5)");
            SetTypeMap(DbType.Decimal, "DECIMAL($size,$precision)", DecimalCapacity);
            SetTypeMap(DbType.Double, "DOUBLE PRECISION");
            SetTypeMap(DbType.Guid, "UNIQUEIDENTIFIER");
            SetTypeMap(DbType.Int16, "SMALLINT");
            SetTypeMap(DbType.Int32, "INT");
            SetTypeMap(DbType.Int64, "BIGINT");
            SetTypeMap(DbType.Single, "REAL");
            SetTypeMap(DbType.StringFixedLength, "NCHAR(255)");
            SetTypeMap(DbType.StringFixedLength, "NCHAR($size)", UnicodeStringCapacity);
            SetTypeMap(DbType.String, "NVARCHAR(255)");
            SetTypeMap(DbType.String, "NVARCHAR($size)", UnicodeStringCapacity);
            // Officially this is 1073741823 but we will allow the int.MaxValue Convention
            SetTypeMap(DbType.String, "NTEXT", int.MaxValue);
            SetTypeMap(DbType.Time, "DATETIME");
        }
    }
}
