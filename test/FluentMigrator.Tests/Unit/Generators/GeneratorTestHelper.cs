// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="GeneratorTestHelper.cs" company="FluentMigrator Project">
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

using System;
using System.Collections.Generic;
using System.Data;

using FluentMigrator.Builders.Create.Column;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using FluentMigrator.SqlServer;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace FluentMigrator.Tests.Unit.Generators
{
    /// <summary>
    /// Class GeneratorTestHelper.
    /// </summary>
    public static class GeneratorTestHelper
    {
        /// <summary>
        /// The test table name1
        /// </summary>
        public static string TestTableName1 = "TestTable1";
        /// <summary>
        /// The test table name2
        /// </summary>
        public static string TestTableName2 = "TestTable2";
        /// <summary>
        /// The test column name1
        /// </summary>
        public static string TestColumnName1 = "TestColumn1";
        /// <summary>
        /// The test column name2
        /// </summary>
        public static string TestColumnName2 = "TestColumn2";
        /// <summary>
        /// The test column name3
        /// </summary>
        public static string TestColumnName3 = "TestColumn3";
        /// <summary>
        /// The test index name
        /// </summary>
        public static string TestIndexName = "TestIndex";
        /// <summary>
        /// The test table description
        /// </summary>
        public static string TestTableDescription = "TestDescription";
        /// <summary>
        /// The test column1 description
        /// </summary>
        public static string TestColumn1Description = "TestColumn1Description";
        /// <summary>
        /// The test column2 description
        /// </summary>
        public static string TestColumn2Description = "TestColumn2Description";
        /// <summary>
        /// The test column collation name
        /// </summary>
        public static string TestColumnCollationName = "Latin1_General_CS_AS";
        /// <summary>
        /// The test unique identifier
        /// </summary>
        public static Guid TestGuid = Guid.NewGuid();

        /// <summary>
        /// Gets the create table expression.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableExpression()
        {
            CreateTableExpression expression = new CreateTableExpression() { TableName = TestTableName1, };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName2, Type = DbType.Int32 });
            return expression;
        }

        /// <summary>
        /// Gets the create table with default value.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithDefaultValue()
        {
            CreateTableExpression expression = new CreateTableExpression() { TableName = TestTableName1, };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, Type = DbType.String, DefaultValue = "Default", TableName = TestTableName1 });
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName2, Type = DbType.Int32, DefaultValue = 0, TableName = TestTableName1 });
            return expression;
        }

        /// <summary>
        /// Gets the create table with primary key expression.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithPrimaryKeyExpression()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, IsPrimaryKey = true, Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName2, Type = DbType.Int32 });
            return expression;
        }

        /// <summary>
        /// Gets the create table with named primary key expression.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithNamedPrimaryKeyExpression()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, IsPrimaryKey = true, PrimaryKeyName = "TestKey", Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName2, Type = DbType.Int32 });
            return expression;
        }

        /// <summary>
        /// Gets the create table with named multi column primary key expression.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithNamedMultiColumnPrimaryKeyExpression()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, IsPrimaryKey = true, PrimaryKeyName = "TestKey", Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName2, Type = DbType.Int32, IsPrimaryKey = true });
            return expression;
        }

        /// <summary>
        /// Gets the create table with automatic increment expression.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithAutoIncrementExpression()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, IsIdentity = true, Type = DbType.Int32 });
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName2, Type = DbType.Int32 });
            return expression;
        }

        /// <summary>
        /// Gets the create table with multi column primary key expression.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithMultiColumnPrimaryKeyExpression()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, IsPrimaryKey = true, Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName2, IsPrimaryKey = true, Type = DbType.Int32 });
            return expression;

        }

        /// <summary>
        /// Gets the create table with nullable column.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithNullableColumn()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, IsNullable = true, Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName2, Type = DbType.Int32 });
            return expression;
        }

        /// <summary>
        /// Gets the create table with table description.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithTableDescription()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1, TableDescription = TestTableDescription };

            return expression;
        }

        /// <summary>
        /// Gets the create table with table description and column descriptions.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithTableDescriptionAndColumnDescriptions()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1, TableDescription = TestTableDescription };
            expression.Columns.Add(new ColumnDefinition
            {
                Name = TestColumnName1,
                IsNullable = true,
                Type = DbType.String,
                ColumnDescription = TestColumn1Description
            });
            expression.Columns.Add(new ColumnDefinition
            {
                Name = TestColumnName2,
                Type = DbType.Int32,
                ColumnDescription = TestColumn2Description
            });

            return expression;
        }

        /// <summary>
        /// Gets the create table with foreign key.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithForeignKey()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition
            {
                Name = TestColumnName2,
                Type = DbType.Int32,
                IsForeignKey = true,
                ForeignKey = new ForeignKeyDefinition()
                {
                    PrimaryTable = TestTableName2,
                    ForeignTable = TestTableName1,
                    PrimaryColumns = new[] { TestColumnName2 },
                    ForeignColumns = new[] { TestColumnName1 }
                }
            });
            return expression;
        }

        /// <summary>
        /// Gets the create table with multi column foreign key.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithMultiColumnForeignKey()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition
            {
                Name = TestColumnName2,
                Type = DbType.Int32,
                IsForeignKey = true,
                ForeignKey = new ForeignKeyDefinition()
                {
                    PrimaryTable = TestTableName2,
                    ForeignTable = TestTableName1,
                    PrimaryColumns = new[] { TestColumnName2, "TestColumn4" },
                    ForeignColumns = new[] { TestColumnName1, "TestColumn3" }
                }
            });
            return expression;
        }

        /// <summary>
        /// Gets the create table with name foreign key.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithNameForeignKey()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition
            {
                Name = TestColumnName2,
                Type = DbType.Int32,
                IsForeignKey = true,
                ForeignKey = new ForeignKeyDefinition()
                {
                    Name = "FK_Test",
                    PrimaryTable = TestTableName2,
                    ForeignTable = TestTableName1,
                    PrimaryColumns = new[] { TestColumnName2 },
                    ForeignColumns = new[] { TestColumnName1 }
                }
            });
            return expression;
        }

        /// <summary>
        /// Gets the create table with name multi column foreign key.
        /// </summary>
        /// <returns>CreateTableExpression.</returns>
        public static CreateTableExpression GetCreateTableWithNameMultiColumnForeignKey()
        {
            var expression = new CreateTableExpression { TableName = TestTableName1 };
            expression.Columns.Add(new ColumnDefinition { Name = TestColumnName1, Type = DbType.String });
            expression.Columns.Add(new ColumnDefinition
            {
                Name = TestColumnName2,
                Type = DbType.Int32,
                IsForeignKey = true,
                ForeignKey = new ForeignKeyDefinition()
                {
                    Name = "FK_Test",
                    PrimaryTable = TestTableName2,
                    ForeignTable = TestTableName1,
                    PrimaryColumns = new[] { TestColumnName2, "TestColumn4" },
                    ForeignColumns = new[] { TestColumnName1, "TestColumn3" }
                }
            });
            return expression;
        }

        /// <summary>
        /// Gets the create index expression.
        /// </summary>
        /// <returns>CreateIndexExpression.</returns>
        public static CreateIndexExpression GetCreateIndexExpression()
        {
            var expression = new CreateIndexExpression();
            expression.Index.Name = TestIndexName;
            expression.Index.TableName = TestTableName1;
            expression.Index.IsUnique = false;
            expression.Index.Columns.Add(new IndexColumnDefinition { Direction = Direction.Ascending, Name = TestColumnName1 });
            return expression;
        }

        /// <summary>
        /// Gets the create schema expression.
        /// </summary>
        /// <returns>CreateSchemaExpression.</returns>
        public static CreateSchemaExpression GetCreateSchemaExpression()
        {
            return new CreateSchemaExpression { SchemaName = "TestSchema" };
        }

        /// <summary>
        /// Gets the create sequence expression.
        /// </summary>
        /// <returns>CreateSequenceExpression.</returns>
        public static CreateSequenceExpression GetCreateSequenceExpression()
        {
            return new CreateSequenceExpression
            {
                Sequence =
                {
                    Cache = 10,
                    Cycle = true,
                    Increment = 2,
                    MaxValue = 100,
                    MinValue = 0,
                    Name = "Sequence",
                    StartWith = 2
                }
            };
        }

        /// <summary>
        /// Gets the create multi column create index expression.
        /// </summary>
        /// <returns>CreateIndexExpression.</returns>
        public static CreateIndexExpression GetCreateMultiColumnCreateIndexExpression()
        {

            var expression = new CreateIndexExpression();
            expression.Index.Name = TestIndexName;
            expression.Index.TableName = TestTableName1;
            expression.Index.IsUnique = false;
            expression.Index.Columns.Add(new IndexColumnDefinition { Direction = Direction.Ascending, Name = TestColumnName1 });
            expression.Index.Columns.Add(new IndexColumnDefinition { Direction = Direction.Descending, Name = TestColumnName2 });
            return expression;
        }

        /// <summary>
        /// Gets the create unique index expression.
        /// </summary>
        /// <returns>CreateIndexExpression.</returns>
        public static CreateIndexExpression GetCreateUniqueIndexExpression()
        {
            var expression = new CreateIndexExpression();
            expression.Index.Name = TestIndexName;
            expression.Index.TableName = TestTableName1;
            expression.Index.IsUnique = true;
            expression.Index.Columns.Add(new IndexColumnDefinition { Direction = Direction.Ascending, Name = TestColumnName1 });
            return expression;
        }

        /// <summary>
        /// Gets the create unique multi column index expression.
        /// </summary>
        /// <returns>CreateIndexExpression.</returns>
        public static CreateIndexExpression GetCreateUniqueMultiColumnIndexExpression()
        {
            var expression = new CreateIndexExpression();
            expression.Index.Name = TestIndexName;
            expression.Index.TableName = TestTableName1;
            expression.Index.IsUnique = true;
            expression.Index.Columns.Add(new IndexColumnDefinition { Direction = Direction.Ascending, Name = TestColumnName1 });
            expression.Index.Columns.Add(new IndexColumnDefinition { Direction = Direction.Descending, Name = TestColumnName2 });
            return expression;
        }

        /// <summary>
        /// Gets the create include index expression.
        /// </summary>
        /// <returns>CreateIndexExpression.</returns>
        public static CreateIndexExpression GetCreateIncludeIndexExpression()
        {
            var expression = new CreateIndexExpression();
            expression.Index.Name = TestIndexName;
            expression.Index.TableName = TestTableName1;
            expression.Index.IsUnique = false;
            expression.Index.Columns.Add(new IndexColumnDefinition { Direction = Direction.Ascending, Name = TestColumnName1 });
            var includes = expression.Index.GetAdditionalFeature(SqlServerExtensions.IncludesList, () => new List<IndexIncludeDefinition>());
            includes.Add(new IndexIncludeDefinition { Name = TestColumnName2 });
            return expression;
        }

        /// <summary>
        /// Gets the create multi include index expression.
        /// </summary>
        /// <returns>CreateIndexExpression.</returns>
        public static CreateIndexExpression GetCreateMultiIncludeIndexExpression()
        {
            var expression = new CreateIndexExpression();
            expression.Index.Name = TestIndexName;
            expression.Index.TableName = TestTableName1;
            expression.Index.IsUnique = false;
            expression.Index.Columns.Add(new IndexColumnDefinition { Direction = Direction.Ascending, Name = TestColumnName1 });

            var includes = expression.Index.GetAdditionalFeature(SqlServerExtensions.IncludesList, () => new List<IndexIncludeDefinition>());
            includes.Add(new IndexIncludeDefinition { Name = TestColumnName2 });
            includes.Add(new IndexIncludeDefinition { Name = TestColumnName3 });
            return expression;
        }

        /// <summary>
        /// Gets the insert data expression.
        /// </summary>
        /// <returns>InsertDataExpression.</returns>
        public static InsertDataExpression GetInsertDataExpression()
        {
            var expression = new InsertDataExpression();
            expression.TableName = TestTableName1;
            expression.Rows.Add(new InsertionDataDefinition
                                    {
                                        new KeyValuePair<string, object>("Id", 1),
                                        new KeyValuePair<string, object>("Name", "Just'in"),
                                        new KeyValuePair<string, object>("Website", "codethinked.com")
                                    });
            expression.Rows.Add(new InsertionDataDefinition
                                    {
                                        new KeyValuePair<string, object>("Id", 2),
                                        new KeyValuePair<string, object>("Name", @"Na\te"),
                                        new KeyValuePair<string, object>("Website", "kohari.org")
                                    });

            return expression;
        }

        /// <summary>
        /// Gets the update data expression.
        /// </summary>
        /// <returns>UpdateDataExpression.</returns>
        public static UpdateDataExpression GetUpdateDataExpression()
        {
            var expression = new UpdateDataExpression();
            expression.TableName = TestTableName1;

            expression.Set = new List<KeyValuePair<string, object>>
                                 {
                                     new KeyValuePair<string, object>("Name", "Just'in"),
                                     new KeyValuePair<string, object>("Age", 25)
                                 };

            expression.Where = new List<KeyValuePair<string, object>>
                                   {
                                       new KeyValuePair<string, object>("Id", 9),
                                       new KeyValuePair<string, object>("Homepage", null)
                                   };
            return expression;
        }

        /// <summary>
        /// Gets the update data expression with database null value.
        /// </summary>
        /// <returns>UpdateDataExpression.</returns>
        public static UpdateDataExpression GetUpdateDataExpressionWithDbNullValue()
        {
            var expression = new UpdateDataExpression();
            expression.TableName = TestTableName1;

            expression.Set = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Name", "Just'in"),
                new KeyValuePair<string, object>("Age", 25)
            };

            expression.Where = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("Id", 9),
                new KeyValuePair<string, object>("Homepage", DBNull.Value)
            };
            return expression;
        }

        /// <summary>
        /// Gets the update data expression with all rows.
        /// </summary>
        /// <returns>UpdateDataExpression.</returns>
        public static UpdateDataExpression GetUpdateDataExpressionWithAllRows()
        {
            var expression = new UpdateDataExpression();
            expression.TableName = TestTableName1;

            expression.Set = new List<KeyValuePair<string, object>>
                                 {
                                     new KeyValuePair<string, object>("Name", "Just'in"),
                                     new KeyValuePair<string, object>("Age", 25)
                                 };

            expression.IsAllRows = true;

            return expression;
        }

        /// <summary>
        /// Gets the insert unique identifier expression.
        /// </summary>
        /// <returns>InsertDataExpression.</returns>
        public static InsertDataExpression GetInsertGUIDExpression()
        {
            return GetInsertGUIDExpression(TestGuid);
        }

        /// <summary>
        /// Gets the insert unique identifier expression.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>InsertDataExpression.</returns>
        public static InsertDataExpression GetInsertGUIDExpression(Guid guid)
        {
            var expression = new InsertDataExpression {TableName = TestTableName1};
            expression.Rows.Add(new InsertionDataDefinition {new KeyValuePair<string, object>("guid", guid)});

            return expression;
        }

        /// <summary>
        /// Gets the delete data expression.
        /// </summary>
        /// <returns>DeleteDataExpression.</returns>
        public static DeleteDataExpression GetDeleteDataExpression()
        {
            var expression = new DeleteDataExpression();
            expression.TableName = TestTableName1;
            expression.Rows.Add(new DeletionDataDefinition
                                    {
                                        new KeyValuePair<string, object>("Name", "Just'in"),
                                        new KeyValuePair<string, object>("Website", null)
                                    });

            return expression;
        }

        /// <summary>
        /// Gets the delete data expression with database null value.
        /// </summary>
        /// <returns>DeleteDataExpression.</returns>
        public static DeleteDataExpression GetDeleteDataExpressionWithDbNullValue()
        {
            var expression = new DeleteDataExpression();
            expression.TableName = TestTableName1;
            expression.Rows.Add(new DeletionDataDefinition
            {
                new KeyValuePair<string, object>("Name", "Just'in"),
                new KeyValuePair<string, object>("Website", DBNull.Value)
            });

            return expression;
        }

        /// <summary>
        /// Gets the delete data multiple rows expression.
        /// </summary>
        /// <returns>DeleteDataExpression.</returns>
        public static DeleteDataExpression GetDeleteDataMultipleRowsExpression()
        {
            var expression = new DeleteDataExpression();
            expression.TableName = TestTableName1;
            expression.Rows.Add(new DeletionDataDefinition
                                    {
                                        new KeyValuePair<string, object>("Name", "Just'in"),
                                        new KeyValuePair<string, object>("Website", null)
                                    });
            expression.Rows.Add(new DeletionDataDefinition
                                    {
                                        new KeyValuePair<string, object>("Website", "github.com")
                                    });

            return expression;
        }

        /// <summary>
        /// Gets the delete data all rows expression.
        /// </summary>
        /// <returns>DeleteDataExpression.</returns>
        public static DeleteDataExpression GetDeleteDataAllRowsExpression()
        {
            var expression = new DeleteDataExpression();
            expression.TableName = TestTableName1;
            expression.IsAllRows = true;
            return expression;
        }

        /// <summary>
        /// Gets the rename column expression.
        /// </summary>
        /// <returns>RenameColumnExpression.</returns>
        public static RenameColumnExpression GetRenameColumnExpression()
        {
            return new RenameColumnExpression { OldName = TestColumnName1, NewName = TestColumnName2, TableName = TestTableName1 };
        }

        /// <summary>
        /// Gets the create decimal column expression.
        /// </summary>
        /// <returns>CreateColumnExpression.</returns>
        public static CreateColumnExpression GetCreateDecimalColumnExpression()
        {
            ColumnDefinition column = new ColumnDefinition { Name = TestColumnName1, Type = DbType.Decimal, Size = 19, Precision = 2 };
            return new CreateColumnExpression { TableName = TestTableName1, Column = column };
        }

        /// <summary>
        /// Gets the create currency column expression.
        /// </summary>
        /// <returns>CreateColumnExpression.</returns>
        public static CreateColumnExpression GetCreateCurrencyColumnExpression()
        {
            ColumnDefinition column = new ColumnDefinition { Name = TestColumnName1, Type = DbType.Currency };
            return new CreateColumnExpression { TableName = TestTableName1, Column = column };
        }

        /// <summary>
        /// Gets the create column expression.
        /// </summary>
        /// <returns>CreateColumnExpression.</returns>
        public static CreateColumnExpression GetCreateColumnExpression()
        {
            ColumnDefinition column = new ColumnDefinition { Name = TestColumnName1, Type = DbType.String, Size = 5 };
            return new CreateColumnExpression { TableName = TestTableName1, Column = column };
        }

        /// <summary>
        /// Gets the create column with system method expression.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>ICollection&lt;IMigrationExpression&gt;.</returns>
        public static ICollection<IMigrationExpression> GetCreateColumnWithSystemMethodExpression(string schemaName = null)
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var querySchema = new Mock<IQuerySchema>();
            var context = new MigrationContext(querySchema.Object, serviceProvider, null, null);
            var expr = new CreateColumnExpression
            {
                TableName = TestTableName1,
                SchemaName = schemaName,
                Column = new ColumnDefinition { Name = TestColumnName1, Type = DbType.DateTime }
            };
            context.Expressions.Add(expr);
            var builder = new CreateColumnExpressionBuilder(expr, context);
            builder.SetExistingRowsTo(SystemMethods.CurrentDateTime);
            return context.Expressions;
        }

        /// <summary>
        /// Gets the create column expression with description.
        /// </summary>
        /// <returns>CreateColumnExpression.</returns>
        public static CreateColumnExpression GetCreateColumnExpressionWithDescription()
        {
            CreateColumnExpression columnExpression = GetCreateColumnExpression();
            columnExpression.Column.ColumnDescription = TestColumn1Description;
            return columnExpression;
        }

        /// <summary>
        /// Gets the create column expression with collation.
        /// </summary>
        /// <returns>CreateColumnExpression.</returns>
        public static CreateColumnExpression GetCreateColumnExpressionWithCollation()
        {
            CreateColumnExpression columnExpression = GetCreateColumnExpression();
            columnExpression.Column.CollationName = TestColumnCollationName;
            return columnExpression;
        }

        /// <summary>
        /// Gets the alter table automatic increment column expression.
        /// </summary>
        /// <returns>CreateColumnExpression.</returns>
        public static CreateColumnExpression GetAlterTableAutoIncrementColumnExpression()
        {
            ColumnDefinition column = new ColumnDefinition { Name = TestColumnName1, IsIdentity = true, Type = DbType.Int32 };
            return new CreateColumnExpression { TableName = TestTableName1, Column = column };
        }

        /// <summary>
        /// Gets the alter table with description expression.
        /// </summary>
        /// <returns>AlterTableExpression.</returns>
        public static AlterTableExpression GetAlterTableWithDescriptionExpression()
        {
            return new AlterTableExpression() { TableName = TestTableName1, TableDescription = TestTableDescription };
        }

        /// <summary>
        /// Gets the alter table.
        /// </summary>
        /// <returns>AlterTableExpression.</returns>
        public static AlterTableExpression GetAlterTable()
        {
            return new AlterTableExpression() {TableName = TestTableName1 };
        }

        /// <summary>
        /// Gets the rename table expression.
        /// </summary>
        /// <returns>RenameTableExpression.</returns>
        public static RenameTableExpression GetRenameTableExpression()
        {
            var expression = new RenameTableExpression();
            expression.OldName = TestTableName1;
            expression.NewName = TestTableName2;
            return expression;
        }

        /// <summary>
        /// Gets the alter column add automatic increment expression.
        /// </summary>
        /// <returns>AlterColumnExpression.</returns>
        public static AlterColumnExpression GetAlterColumnAddAutoIncrementExpression()
        {
            ColumnDefinition column = new ColumnDefinition { Name = TestColumnName1, IsIdentity = true, IsPrimaryKey = true, Type = DbType.Int32 };
            return new AlterColumnExpression { TableName = TestTableName1, Column = column };
        }

        /// <summary>
        /// Gets the alter column expression.
        /// </summary>
        /// <returns>AlterColumnExpression.</returns>
        public static AlterColumnExpression GetAlterColumnExpression()
        {
            var expression = new AlterColumnExpression();
            expression.TableName = TestTableName1;

            expression.Column = new ColumnDefinition();
            expression.Column.Name = TestColumnName1;
            expression.Column.Type = DbType.String;
            expression.Column.Size = 20;
            expression.Column.IsNullable = false;
            expression.Column.ModificationType = ColumnModificationType.Alter;

            return expression;
        }

        /// <summary>
        /// Gets the type of the create column expression with nullable custom.
        /// </summary>
        /// <returns>CreateColumnExpression.</returns>
        public static CreateColumnExpression GetCreateColumnExpressionWithNullableCustomType()
        {
            var expression = new CreateColumnExpression();
            expression.TableName = TestTableName1;

            expression.Column = new ColumnDefinition();
            expression.Column.Name = TestColumnName1;
            expression.Column.IsNullable = true;
            expression.Column.CustomType = "MyDomainType";
            expression.Column.ModificationType = ColumnModificationType.Create;

            return expression;
        }

        /// <summary>
        /// Gets the alter column expression with description.
        /// </summary>
        /// <returns>AlterColumnExpression.</returns>
        public static AlterColumnExpression GetAlterColumnExpressionWithDescription()
        {
            var columnExpression = GetAlterColumnExpression();
            columnExpression.Column.ColumnDescription = TestColumn1Description;
            return columnExpression;
        }

        /// <summary>
        /// Gets the alter column expression with collation.
        /// </summary>
        /// <returns>AlterColumnExpression.</returns>
        public static AlterColumnExpression GetAlterColumnExpressionWithCollation()
        {
            var columnExpression = GetAlterColumnExpression();
            columnExpression.Column.CollationName = TestColumnCollationName;
            return columnExpression;
        }

        /// <summary>
        /// Gets the alter schema expression.
        /// </summary>
        /// <returns>AlterSchemaExpression.</returns>
        public static AlterSchemaExpression GetAlterSchemaExpression()
        {
            return new AlterSchemaExpression { DestinationSchemaName = "TestSchema2", SourceSchemaName = "TestSchema1", TableName = "TestTable" };
        }

        /// <summary>
        /// Gets the create foreign key expression.
        /// </summary>
        /// <returns>CreateForeignKeyExpression.</returns>
        public static CreateForeignKeyExpression GetCreateForeignKeyExpression()
        {
            var expression = new CreateForeignKeyExpression
            {
                ForeignKey =
                {
                    PrimaryTable = TestTableName2,
                    ForeignTable = TestTableName1,
                    PrimaryColumns = new[] {TestColumnName2},
                    ForeignColumns = new[] {TestColumnName1}
                }
            };

            var processed = expression.Apply(ConventionSets.NoSchemaName);

            return processed;
        }

        /// <summary>
        /// Gets the create multi column foreign key expression.
        /// </summary>
        /// <returns>CreateForeignKeyExpression.</returns>
        public static CreateForeignKeyExpression GetCreateMultiColumnForeignKeyExpression()
        {
            var expression = new CreateForeignKeyExpression
            {
                ForeignKey =
                {
                    PrimaryTable = TestTableName2,
                    ForeignTable = TestTableName1,
                    PrimaryColumns = new[] {TestColumnName2, "TestColumn4"},
                    ForeignColumns = new[] {TestColumnName1, "TestColumn3"}
                }
            };

            var processed = expression.Apply(ConventionSets.NoSchemaName);

            return processed;
        }

        /// <summary>
        /// Gets the create named foreign key expression.
        /// </summary>
        /// <returns>CreateForeignKeyExpression.</returns>
        public static CreateForeignKeyExpression GetCreateNamedForeignKeyExpression()
        {
            var expression = new CreateForeignKeyExpression();
            expression.ForeignKey.Name = "FK_Test";
            expression.ForeignKey.PrimaryTable = TestTableName2;
            expression.ForeignKey.ForeignTable = TestTableName1;
            expression.ForeignKey.PrimaryColumns = new[] { TestColumnName2 };
            expression.ForeignKey.ForeignColumns = new[] { TestColumnName1 };

            return expression;
        }

        /// <summary>
        /// Gets the create named multi column foreign key expression.
        /// </summary>
        /// <returns>CreateForeignKeyExpression.</returns>
        public static CreateForeignKeyExpression GetCreateNamedMultiColumnForeignKeyExpression()
        {
            var expression = new CreateForeignKeyExpression();
            expression.ForeignKey.Name = "FK_Test";
            expression.ForeignKey.PrimaryTable = TestTableName2;
            expression.ForeignKey.ForeignTable = TestTableName1;
            expression.ForeignKey.PrimaryColumns = new[] { TestColumnName2, "TestColumn4" };
            expression.ForeignKey.ForeignColumns = new[] { TestColumnName1, "TestColumn3" };

            return expression;
        }

        /// <summary>
        /// Gets the delete table expression.
        /// </summary>
        /// <returns>DeleteTableExpression.</returns>
        public static DeleteTableExpression GetDeleteTableExpression()
        {
            return new DeleteTableExpression { TableName = TestTableName1 };
        }

        /// <summary>
        /// Gets the delete column expression.
        /// </summary>
        /// <returns>DeleteColumnExpression.</returns>
        public static DeleteColumnExpression GetDeleteColumnExpression()
        {
            return GetDeleteColumnExpression(new[] { TestColumnName1 });
        }

        /// <summary>
        /// Gets the delete column expression.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>DeleteColumnExpression.</returns>
        public static DeleteColumnExpression GetDeleteColumnExpression(string[] columns)
        {
            return new DeleteColumnExpression { TableName = TestTableName1, ColumnNames = columns };
        }

        /// <summary>
        /// Gets the delete index expression.
        /// </summary>
        /// <returns>DeleteIndexExpression.</returns>
        public static DeleteIndexExpression GetDeleteIndexExpression()
        {
            IndexDefinition indexDefinition = new IndexDefinition { Name = TestIndexName, TableName = TestTableName1 };
            return new DeleteIndexExpression { Index = indexDefinition };
        }

        /// <summary>
        /// Gets the delete foreign key expression.
        /// </summary>
        /// <returns>DeleteForeignKeyExpression.</returns>
        public static DeleteForeignKeyExpression GetDeleteForeignKeyExpression()
        {
            var expression = new DeleteForeignKeyExpression();
            expression.ForeignKey.Name = "FK_Test";
            expression.ForeignKey.ForeignTable = TestTableName1;
            return expression;
        }

        /// <summary>
        /// Gets the delete primary key expression.
        /// </summary>
        /// <returns>DeleteConstraintExpression.</returns>
        public static DeleteConstraintExpression GetDeletePrimaryKeyExpression()
        {
            var expression = new DeleteConstraintExpression(ConstraintType.PrimaryKey);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.ConstraintName = "TESTPRIMARYKEY";
            return expression;
        }

        /// <summary>
        /// Gets the delete schema expression.
        /// </summary>
        /// <returns>DeleteSchemaExpression.</returns>
        public static DeleteSchemaExpression GetDeleteSchemaExpression()
        {
            return new DeleteSchemaExpression { SchemaName = "TestSchema" };
        }

        /// <summary>
        /// Gets the delete sequence expression.
        /// </summary>
        /// <returns>DeleteSequenceExpression.</returns>
        public static DeleteSequenceExpression GetDeleteSequenceExpression()
        {
            return new DeleteSequenceExpression { SequenceName = "Sequence" };
        }

        /// <summary>
        /// Gets the delete unique constraint expression.
        /// </summary>
        /// <returns>DeleteConstraintExpression.</returns>
        public static DeleteConstraintExpression GetDeleteUniqueConstraintExpression()
        {
            var expression = new DeleteConstraintExpression(ConstraintType.Unique);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.ConstraintName = "TESTUNIQUECONSTRAINT";
            return expression;
        }


        /// <summary>
        /// Gets the create primary key expression.
        /// </summary>
        /// <returns>CreateConstraintExpression.</returns>
        public static CreateConstraintExpression GetCreatePrimaryKeyExpression()
        {
            var expression = new CreateConstraintExpression(ConstraintType.PrimaryKey);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.Columns.Add(TestColumnName1);
            var processed = expression.Apply(ConventionSets.NoSchemaName);
            return processed;
        }

        /// <summary>
        /// Gets the create named primary key expression.
        /// </summary>
        /// <returns>CreateConstraintExpression.</returns>
        public static CreateConstraintExpression GetCreateNamedPrimaryKeyExpression()
        {
            var expression = new CreateConstraintExpression(ConstraintType.PrimaryKey);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.Columns.Add(TestColumnName1);
            expression.Constraint.ConstraintName = "TESTPRIMARYKEY";
            return expression;
        }

        /// <summary>
        /// Gets the create multi column primary key expression.
        /// </summary>
        /// <returns>CreateConstraintExpression.</returns>
        public static CreateConstraintExpression GetCreateMultiColumnPrimaryKeyExpression()
        {
            var expression = new CreateConstraintExpression(ConstraintType.PrimaryKey);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.Columns.Add(TestColumnName1);
            expression.Constraint.Columns.Add(TestColumnName2);
            var processed = expression.Apply(ConventionSets.NoSchemaName);
            return processed;
        }

        /// <summary>
        /// Gets the create named multi column primary key expression.
        /// </summary>
        /// <returns>CreateConstraintExpression.</returns>
        public static CreateConstraintExpression GetCreateNamedMultiColumnPrimaryKeyExpression()
        {
            var expression = new CreateConstraintExpression(ConstraintType.PrimaryKey);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.Columns.Add(TestColumnName1);
            expression.Constraint.Columns.Add(TestColumnName2);
            expression.Constraint.ConstraintName = "TESTPRIMARYKEY";
            return expression;
        }

        /// <summary>
        /// Gets the create unique constraint expression.
        /// </summary>
        /// <returns>CreateConstraintExpression.</returns>
        public static CreateConstraintExpression GetCreateUniqueConstraintExpression()
        {
            var expression = new CreateConstraintExpression(ConstraintType.Unique);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.Columns.Add(TestColumnName1);
            var processed = expression.Apply(ConventionSets.NoSchemaName);
            return processed;
        }

        /// <summary>
        /// Gets the create named unique constraint expression.
        /// </summary>
        /// <returns>CreateConstraintExpression.</returns>
        public static CreateConstraintExpression GetCreateNamedUniqueConstraintExpression()
        {
            var expression = new CreateConstraintExpression(ConstraintType.Unique);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.Columns.Add(TestColumnName1);
            expression.Constraint.ConstraintName = "TESTUNIQUECONSTRAINT";
            return expression;
        }

        /// <summary>
        /// Gets the create multi column unique constraint expression.
        /// </summary>
        /// <returns>CreateConstraintExpression.</returns>
        public static CreateConstraintExpression GetCreateMultiColumnUniqueConstraintExpression()
        {
            var expression = new CreateConstraintExpression(ConstraintType.Unique);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.Columns.Add(TestColumnName1);
            expression.Constraint.Columns.Add(TestColumnName2);
            var processed = expression.Apply(ConventionSets.NoSchemaName);
            return processed;
        }

        /// <summary>
        /// Gets the create named multi column unique constraint expression.
        /// </summary>
        /// <returns>CreateConstraintExpression.</returns>
        public static CreateConstraintExpression GetCreateNamedMultiColumnUniqueConstraintExpression()
        {
            var expression = new CreateConstraintExpression(ConstraintType.Unique);
            expression.Constraint.TableName = TestTableName1;
            expression.Constraint.Columns.Add(TestColumnName1);
            expression.Constraint.Columns.Add(TestColumnName2);
            expression.Constraint.ConstraintName = "TESTUNIQUECONSTRAINT";
            return expression;
        }

        /// <summary>
        /// Gets the alter default constraint expression.
        /// </summary>
        /// <returns>AlterDefaultConstraintExpression.</returns>
        public static AlterDefaultConstraintExpression GetAlterDefaultConstraintExpression()
        {
            var expression = new AlterDefaultConstraintExpression
                                 {
                                     ColumnName = TestColumnName1,
                                     DefaultValue = 1,
                                     TableName = TestTableName1
                                 };
            return expression;
        }

        /// <summary>
        /// Gets the delete default constraint expression.
        /// </summary>
        /// <returns>DeleteDefaultConstraintExpression.</returns>
        public static DeleteDefaultConstraintExpression GetDeleteDefaultConstraintExpression()
        {
            var expression = new DeleteDefaultConstraintExpression
            {
                ColumnName = TestColumnName1,
                TableName = TestTableName1
            };
            return expression;
        }
    }
}
