// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SQLite
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SQLiteTypeMap.cs" company="FluentMigrator Project">
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

namespace FluentMigrator.Runner.Generators.SQLite
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Class SQLiteTypeMap.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.Base.TypeMapBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.Base.TypeMapBase" />
    internal class SQLiteTypeMap : TypeMapBase
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
        public const int DecimalCapacity = 19;
        /// <summary>
        /// The XML capacity
        /// </summary>
        public const int XmlCapacity = 1073741823;

        /// <summary>
        /// Setups the type maps.
        /// </summary>
        protected override void SetupTypeMaps()
        {
            SetTypeMap(DbType.Binary, "BLOB");
            SetTypeMap(DbType.Byte, "INTEGER");
            SetTypeMap(DbType.Int16, "INTEGER");
            SetTypeMap(DbType.Int32, "INTEGER");
            SetTypeMap(DbType.Int64, "INTEGER");
            SetTypeMap(DbType.SByte, "INTEGER");
            SetTypeMap(DbType.UInt16, "INTEGER");
            SetTypeMap(DbType.UInt32, "INTEGER");
            SetTypeMap(DbType.UInt64, "INTEGER");
            SetTypeMap(DbType.Currency, "NUMERIC");
            SetTypeMap(DbType.Decimal, "NUMERIC");
            SetTypeMap(DbType.Double, "NUMERIC");
            SetTypeMap(DbType.Single, "NUMERIC");
            SetTypeMap(DbType.VarNumeric, "NUMERIC");
            SetTypeMap(DbType.AnsiString, "TEXT");
            SetTypeMap(DbType.String, "TEXT");
            SetTypeMap(DbType.AnsiStringFixedLength, "TEXT");
            SetTypeMap(DbType.StringFixedLength, "TEXT");

            SetTypeMap(DbType.Date, "DATETIME");
            SetTypeMap(DbType.DateTime, "DATETIME");
            SetTypeMap(DbType.DateTime2, "DATETIME");
            SetTypeMap(DbType.Time, "DATETIME");
            SetTypeMap(DbType.Boolean, "INTEGER");
            SetTypeMap(DbType.Guid, "UNIQUEIDENTIFIER");
        }

        /// <summary>
        /// Gets the type map.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="size">The size.</param>
        /// <param name="precision">The precision.</param>
        /// <returns>System.String.</returns>
        public override string GetTypeMap(DbType type, int? size, int? precision)
        {
            return base.GetTypeMap(type, size: null, precision: null);
        }
    }
}
