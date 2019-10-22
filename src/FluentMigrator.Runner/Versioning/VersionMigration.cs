// ***********************************************************************
// Assembly         : FluentMigrator.Runner
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="VersionMigration.cs" company="FluentMigrator Project">
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

using FluentMigrator.Runner.VersionTableInfo;

namespace FluentMigrator.Runner.Versioning
{
    /// <summary>
    /// Class VersionMigration.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    public class VersionMigration : Migration
    {
        /// <summary>
        /// The version table meta data
        /// </summary>
        private readonly IVersionTableMetaData _versionTableMetaData;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionMigration"/> class.
        /// </summary>
        /// <param name="versionTableMetaData">The version table meta data.</param>
        public VersionMigration(IVersionTableMetaData versionTableMetaData)
        {
            _versionTableMetaData = versionTableMetaData;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Table(_versionTableMetaData.TableName)
                .InSchema(_versionTableMetaData.SchemaName)
                .WithColumn(_versionTableMetaData.ColumnName).AsInt64().NotNullable();
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            Delete.Table(_versionTableMetaData.TableName).InSchema(_versionTableMetaData.SchemaName);
        }
    }

    /// <summary>
    /// Class VersionSchemaMigration.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    public class VersionSchemaMigration : Migration
    {
        /// <summary>
        /// The version table meta data
        /// </summary>
        private IVersionTableMetaData _versionTableMetaData;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionSchemaMigration"/> class.
        /// </summary>
        /// <param name="versionTableMetaData">The version table meta data.</param>
        public VersionSchemaMigration(IVersionTableMetaData versionTableMetaData)
        {
            _versionTableMetaData = versionTableMetaData;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!string.IsNullOrEmpty(_versionTableMetaData.SchemaName))
                Create.Schema(_versionTableMetaData.SchemaName);
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            if (!string.IsNullOrEmpty(_versionTableMetaData.SchemaName))
                Delete.Schema(_versionTableMetaData.SchemaName);
        }
    }

    /// <summary>
    /// Class VersionUniqueMigration.
    /// Implements the <see cref="FluentMigrator.ForwardOnlyMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.ForwardOnlyMigration" />
    public class VersionUniqueMigration : ForwardOnlyMigration
    {
        /// <summary>
        /// The version table meta
        /// </summary>
        private readonly IVersionTableMetaData _versionTableMeta;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionUniqueMigration"/> class.
        /// </summary>
        /// <param name="versionTableMeta">The version table meta.</param>
        public VersionUniqueMigration(IVersionTableMetaData versionTableMeta)
        {
            _versionTableMeta = versionTableMeta;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Index(_versionTableMeta.UniqueIndexName)
                .OnTable(_versionTableMeta.TableName)
                .InSchema(_versionTableMeta.SchemaName)
                .WithOptions().Unique()
                .WithOptions().Clustered()
                .OnColumn(_versionTableMeta.ColumnName);

            Alter.Table(_versionTableMeta.TableName).InSchema(_versionTableMeta.SchemaName).AddColumn(_versionTableMeta.AppliedOnColumnName).AsDateTime().Nullable();
        }

    }

    /// <summary>
    /// Class VersionDescriptionMigration.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    public class VersionDescriptionMigration : Migration
    {
        /// <summary>
        /// The version table meta
        /// </summary>
        private readonly IVersionTableMetaData _versionTableMeta;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionDescriptionMigration"/> class.
        /// </summary>
        /// <param name="versionTableMeta">The version table meta.</param>
        public VersionDescriptionMigration(IVersionTableMetaData versionTableMeta)
        {
            _versionTableMeta = versionTableMeta;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Alter.Table(_versionTableMeta.TableName).InSchema(_versionTableMeta.SchemaName)
                .AddColumn(_versionTableMeta.DescriptionColumnName).AsString(1024).Nullable();
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down()
        {
            Delete.Column(_versionTableMeta.DescriptionColumnName)
                  .FromTable(_versionTableMeta.TableName).InSchema(_versionTableMeta.SchemaName);
        }
    }
}
