// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Firebird
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdTableSchema.cs" company="FluentMigrator Project">
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

using System;
using System.Collections.Generic;
using System.Linq;

using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Firebird;
using FluentMigrator.Runner.Models;

namespace FluentMigrator.Runner.Processors.Firebird
{
    /// <summary>
    /// Class FirebirdTableSchema. This class cannot be inherited.
    /// </summary>
    internal sealed class FirebirdTableSchema
    {
        /// <summary>
        /// The quoter
        /// </summary>
        private readonly FirebirdQuoter _quoter;
        /// <summary>
        /// Gets the table meta.
        /// </summary>
        /// <value>The table meta.</value>
        public TableInfo TableMeta { get; private set; }
        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<ColumnInfo> Columns { get; private set; }
        /// <summary>
        /// Gets the indexes.
        /// </summary>
        /// <value>The indexes.</value>
        public List<IndexInfo> Indexes { get; private set; }
        /// <summary>
        /// Gets the constraints.
        /// </summary>
        /// <value>The constraints.</value>
        public List<ConstraintInfo> Constraints { get; private set; }
        /// <summary>
        /// Gets the triggers.
        /// </summary>
        /// <value>The triggers.</value>
        public List<TriggerInfo> Triggers { get; private set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="FirebirdTableSchema"/> is exists.
        /// </summary>
        /// <value><c>true</c> if exists; otherwise, <c>false</c>.</value>
        public bool Exists => TableMeta?.Exists ?? false;
        /// <summary>
        /// Gets the processor.
        /// </summary>
        /// <value>The processor.</value>
        public FirebirdProcessor Processor { get; }
        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public FirebirdTableDefinition Definition { get; }
        /// <summary>
        /// Gets a value indicating whether this instance has primary key.
        /// </summary>
        /// <value><c>true</c> if this instance has primary key; otherwise, <c>false</c>.</value>
        public bool HasPrimaryKey { get { return Definition.Columns.Any(x => x.IsPrimaryKey); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebirdTableSchema"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="quoter">The quoter.</param>
        public FirebirdTableSchema(string tableName, FirebirdProcessor processor, FirebirdQuoter quoter)
        {
            _quoter = quoter;
            TableName = tableName;
            Processor = processor;
            Definition = new FirebirdTableDefinition()
            {
                Name = tableName,
                SchemaName = string.Empty
            };
            Load();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            LoadMeta();
            LoadColumns();
            LoadIndexes();
            LoadConstraints();
            LoadTriggers();
        }

        /// <summary>
        /// Loads the meta.
        /// </summary>
        private void LoadMeta()
        {
            TableMeta = TableInfo.Read(Processor, TableName, _quoter);
        }

        /// <summary>
        /// Loads the columns.
        /// </summary>
        private void LoadColumns()
        {
            Columns = ColumnInfo.Read(Processor, TableMeta);
            foreach (ColumnInfo column in Columns)
            {
                ColumnDefinition colDef = new ColumnDefinition()
                {
                    TableName = TableMeta.Name,
                    Name = column.Name,
                    DefaultValue = column.DefaultValue == DBNull.Value ? new ColumnDefinition.UndefinedDefaultValue() : column.DefaultValue,
                    IsNullable = column.IsNullable,
                    Type = column.DBType,
                    Precision = column.Precision,
                    Size = column.CharacterLength
                };
                if (colDef.Type == null)
                    colDef.CustomType = column.CustomType;

                Definition.Columns.Add(colDef);
            }
        }

        /// <summary>
        /// Loads the indexes.
        /// </summary>
        private void LoadIndexes()
        {
            Indexes = IndexInfo.Read(Processor, TableMeta);
            foreach (IndexInfo index in Indexes)
            {
                IndexDefinition indexDef = new IndexDefinition()
                {
                    Name = index.Name,
                    TableName = TableMeta.Name,
                    IsUnique = index.IsUnique
                };
                index.Columns.ForEach(x => indexDef.Columns.Add(
                    new IndexColumnDefinition()
                    {
                        Name = x,
                        Direction = index.IsAscending ? Direction.Ascending : Direction.Descending
                    }));

                Definition.Indexes.Add(indexDef);
            }
        }
        /// <summary>
        /// Removes the index.
        /// </summary>
        /// <param name="indexName">Name of the index.</param>
        private void RemoveIndex(string indexName)
        {
            if (Definition.Indexes.Any(x => x.Name == indexName))
            {
                IndexDefinition indexDef = Definition.Indexes.First(x => x.Name == indexName);
                Definition.Indexes.Remove(indexDef);
            }
        }

        /// <summary>
        /// Loads the constraints.
        /// </summary>
        private void LoadConstraints()
        {
            Constraints = ConstraintInfo.Read(Processor, TableMeta);
            foreach (ConstraintInfo constraint in Constraints)
            {
                List<string> columns = new List<string>();
                if (Indexes.Any(x => x.Name == constraint.IndexName))
                    columns = Indexes.First(x => x.Name == constraint.IndexName).Columns;

                foreach (ColumnDefinition column in Definition.Columns)
                {
                    if (columns.Contains(column.Name))
                    {
                        if (constraint.IsPrimaryKey)
                        {
                            column.IsPrimaryKey = true;
                            column.PrimaryKeyName = constraint.Name;
                            RemoveIndex(constraint.Name);
                        }

                        if (constraint.IsNotNull)
                            column.IsNullable = false;

                        if (constraint.IsUnique)
                            column.IsUnique = true;
                    }
                }

                if (constraint.IsForeignKey)
                {
                    ForeignKeyDefinition fkDef = new ForeignKeyDefinition()
                    {
                        Name = constraint.Name,
                        ForeignTable = TableMeta.Name,
                        ForeignColumns = columns,
                        PrimaryTable = constraint.ForeignIndex.TableName,
                        PrimaryColumns = constraint.ForeignIndex.Columns,
                        OnUpdate = constraint.UpdateRule,
                        OnDelete = constraint.DeleteRule
                    };

                    RemoveIndex(constraint.Name);

                    Definition.ForeignKeys.Add(fkDef);
                }
            }
        }

        /// <summary>
        /// Loads the triggers.
        /// </summary>
        private void LoadTriggers()
        {
            Triggers = TriggerInfo.Read(Processor, TableMeta);
        }

    }
}
