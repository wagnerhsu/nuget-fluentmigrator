// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DefaultMigrationConventionsTests.cs" company="FluentMigrator Project">
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
using System.Linq;
using System.Reflection;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Infrastructure;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.SqlServer;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Unit
{
    /// <summary>
    /// Defines test class DefaultMigrationConventionsTests.
    /// </summary>
    [TestFixture]
    public class DefaultMigrationConventionsTests
    {
        /// <summary>
        /// The default
        /// </summary>
        private static readonly IMigrationRunnerConventions _default = DefaultMigrationRunnerConventions.Instance;

        /// <summary>
        /// Defines the test method GetPrimaryKeyNamePrefixesTableNameWithPKAndUnderscore.
        /// </summary>
        [Test]
        public void GetPrimaryKeyNamePrefixesTableNameWithPKAndUnderscore()
        {
            var expr = new CreateColumnExpression()
            {
                Column =
                {
                    TableName = "Foo",
                    IsPrimaryKey = true,
                }
            };

            var processed = expr.Apply(ConventionSets.NoSchemaName);
            processed.Column.PrimaryKeyName.ShouldBe("PK_Foo");
        }

        /// <summary>
        /// Defines the test method GetForeignKeyNameReturnsValidForeignKeyNameForSimpleForeignKey.
        /// </summary>
        [Test]
        public void GetForeignKeyNameReturnsValidForeignKeyNameForSimpleForeignKey()
        {
            var expr = new CreateForeignKeyExpression()
            {
                ForeignKey =
                {
                    ForeignTable = "Users",
                    ForeignColumns = new[] { "GroupId" },
                    PrimaryTable = "Groups",
                    PrimaryColumns = new[] { "Id" }
                }
            };

            var processed = expr.Apply(ConventionSets.NoSchemaName);

            processed.ForeignKey.Name.ShouldBe("FK_Users_GroupId_Groups_Id");
        }

        /// <summary>
        /// Defines the test method GetForeignKeyNameReturnsValidForeignKeyNameForComplexForeignKey.
        /// </summary>
        [Test]
        public void GetForeignKeyNameReturnsValidForeignKeyNameForComplexForeignKey()
        {
            var expr = new CreateForeignKeyExpression()
            {
                ForeignKey =
                {
                    ForeignTable = "Users",
                    ForeignColumns = new[] { "ColumnA", "ColumnB" },
                    PrimaryTable = "Groups",
                    PrimaryColumns = new[] { "ColumnC", "ColumnD" }
                }
            };

            var processed = expr.Apply(ConventionSets.NoSchemaName);

            processed.ForeignKey.Name.ShouldBe("FK_Users_ColumnA_ColumnB_Groups_ColumnC_ColumnD");
        }

        /// <summary>
        /// Defines the test method GetIndexNameReturnsValidIndexNameForSimpleIndex.
        /// </summary>
        [Test]
        public void GetIndexNameReturnsValidIndexNameForSimpleIndex()
        {
            var expr = new CreateIndexExpression()
            {
                Index =
                {
                    TableName = "Bacon",
                    Columns =
                    {
                        new IndexColumnDefinition { Name = "BaconName", Direction = Direction.Ascending }
                    }
                }
            };

            var processed = expr.Apply(ConventionSets.NoSchemaName);

            processed.Index.Name.ShouldBe("IX_Bacon_BaconName");
        }

        /// <summary>
        /// Defines the test method GetIndexNameReturnsValidIndexNameForComplexIndex.
        /// </summary>
        [Test]
        public void GetIndexNameReturnsValidIndexNameForComplexIndex()
        {
            var expr = new CreateIndexExpression()
            {
                Index =
                {
                    TableName = "Bacon",
                    Columns =
                    {
                        new IndexColumnDefinition { Name = "BaconName", Direction = Direction.Ascending },
                        new IndexColumnDefinition { Name = "BaconSpice", Direction = Direction.Descending }
                    }
                }
            };

            var processed = expr.Apply(ConventionSets.NoSchemaName);

            processed.Index.Name.ShouldBe("IX_Bacon_BaconName_BaconSpice");
        }

        /// <summary>
        /// Defines the test method TypeIsMigrationReturnsTrueIfTypeExtendsMigrationAndHasMigrationAttribute.
        /// </summary>
        [Test]
        public void TypeIsMigrationReturnsTrueIfTypeExtendsMigrationAndHasMigrationAttribute()
        {
            _default.TypeIsMigration(typeof(DefaultConventionMigrationFake))
                .ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method TypeIsMigrationReturnsFalseIfTypeDoesNotExtendMigration.
        /// </summary>
        [Test]
        public void TypeIsMigrationReturnsFalseIfTypeDoesNotExtendMigration()
        {
            _default.TypeIsMigration(typeof(object))
                .ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method TypeIsMigrationReturnsFalseIfTypeDoesNotHaveMigrationAttribute.
        /// </summary>
        [Test]
        public void TypeIsMigrationReturnsFalseIfTypeDoesNotHaveMigrationAttribute()
        {
            _default.TypeIsMigration(typeof(MigrationWithoutAttributeFake))
                .ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method GetMaintenanceStageReturnsCorrectStage.
        /// </summary>
        [Test]
        public void GetMaintenanceStageReturnsCorrectStage()
        {
            _default.GetMaintenanceStage(typeof(MaintenanceAfterEach))
                .ShouldBe(MigrationStage.AfterEach);
        }

        /// <summary>
        /// Defines the test method MigrationInfoShouldRetainMigration.
        /// </summary>
        [Test]
        public void MigrationInfoShouldRetainMigration()
        {
            var migration = new DefaultConventionMigrationFake();
            var migrationinfo = _default.GetMigrationInfoForMigration(migration);
            migrationinfo.Migration.GetType().ShouldBeSameAs(migration.GetType());
        }

        /// <summary>
        /// Defines the test method MigrationInfoShouldExtractVersion.
        /// </summary>
        [Test]
        public void MigrationInfoShouldExtractVersion()
        {
            var migration = new DefaultConventionMigrationFake();
            var migrationinfo = _default.GetMigrationInfoForMigration(migration);
            migrationinfo.Version.ShouldBe(123);
        }

        /// <summary>
        /// Defines the test method MigrationInfoShouldExtractTransactionBehavior.
        /// </summary>
        [Test]
        public void MigrationInfoShouldExtractTransactionBehavior()
        {
            var migration = new DefaultConventionMigrationFake();
            var migrationinfo = _default.GetMigrationInfoForMigration(migration);
            migrationinfo.TransactionBehavior.ShouldBe(TransactionBehavior.None);
        }

        /// <summary>
        /// Defines the test method MigrationInfoShouldExtractTraits.
        /// </summary>
        [Test]
        public void MigrationInfoShouldExtractTraits()
        {
            var migration = new DefaultConventionMigrationFake();
            var migrationinfo = _default.GetMigrationInfoForMigration(migration);
            migrationinfo.Trait("key").ShouldBe("test");
        }

        /// <summary>
        /// Defines the test method ObsoleteMigrationInfoShouldRetainMigration.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteMigrationInfoShouldRetainMigration()
        {
            var migrationType = typeof(DefaultConventionMigrationFake);
            var migrationinfo = _default.GetMigrationInfo(migrationType);
            migrationinfo.Migration.GetType().ShouldBeSameAs(migrationType);
        }

        /// <summary>
        /// Defines the test method ObsoleteMigrationInfoShouldExtractVersion.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteMigrationInfoShouldExtractVersion()
        {
            var migrationType = typeof(DefaultConventionMigrationFake);
            var migrationinfo = _default.GetMigrationInfo(migrationType);
            migrationinfo.Version.ShouldBe(123);
        }

        /// <summary>
        /// Defines the test method ObsoleteMigrationInfoShouldExtractTransactionBehavior.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteMigrationInfoShouldExtractTransactionBehavior()
        {
            var migrationType = typeof(DefaultConventionMigrationFake);
            var migrationinfo = _default.GetMigrationInfo(migrationType);
            migrationinfo.TransactionBehavior.ShouldBe(TransactionBehavior.None);
        }

        /// <summary>
        /// Defines the test method ObsoleteMigrationInfoShouldExtractTraits.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteMigrationInfoShouldExtractTraits()
        {
            var migrationType = typeof(DefaultConventionMigrationFake);
            var migrationinfo = _default.GetMigrationInfo(migrationType);
            migrationinfo.Trait("key").ShouldBe("test");
        }

        /// <summary>
        /// Defines the test method DefaultSchemaConventionDefaultsToNull.
        /// </summary>
        [Test]
        public void DefaultSchemaConventionDefaultsToNull()
        {
            var expr = new ConventionsTestClass();
            var processed = ConventionSets.NoSchemaName.SchemaConvention.Apply(expr);
            processed.SchemaName.ShouldBeNull();
        }

        /// <summary>
        /// Defines the test method TypeHasTagsReturnTrueIfTypeHasTagsAttribute.
        /// </summary>
        [Test]
        public void TypeHasTagsReturnTrueIfTypeHasTagsAttribute()
        {
            _default.TypeHasTags(typeof(TaggedWithUk))
                .ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method TypeHasTagsReturnTrueIfInheritedTypeHasTagsAttribute.
        /// </summary>
        [Test]
        public void TypeHasTagsReturnTrueIfInheritedTypeHasTagsAttribute()
        {
            _default.TypeHasTags(typeof(InheritedFromTaggedWithUk))
                .ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method TypeHasTagsReturnFalseIfTypeDoesNotHaveTagsAttribute.
        /// </summary>
        [Test]
        public void TypeHasTagsReturnFalseIfTypeDoesNotHaveTagsAttribute()
        {
            _default.TypeHasTags(typeof(HasNoTagsFake))
                .ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method TypeHasTagsReturnTrueIfBaseTypeDoesHaveTagsAttribute.
        /// </summary>
        [Test]
        public void TypeHasTagsReturnTrueIfBaseTypeDoesHaveTagsAttribute()
        {
            _default.TypeHasTags(typeof(ConcreteHasTagAttribute))
                .ShouldBeTrue();
        }

        /// <summary>
        /// Class TypeHasMatchingTags.
        /// </summary>
        public class TypeHasMatchingTags
        {
            /// <summary>
            /// Defines the test method WhenTypeHasTagAttributeButNoTagsPassedInReturnsFalse.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasTagAttributeButNoTagsPassedInReturnsFalse()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithUk), new string[] { })
                    .ShouldBeFalse();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasTagAttributeWithNoTagNamesReturnsFalse.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasTagAttributeWithNoTagNamesReturnsFalse()
            {
                _default.TypeHasMatchingTags(typeof(HasTagAttributeWithNoTagNames), new string[] { })
                    .ShouldBeFalse();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasOneTagThatDoesNotMatchSingleThenTagReturnsFalse.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasOneTagThatDoesNotMatchSingleThenTagReturnsFalse()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithUk), new[] { "IE" })
                    .ShouldBeFalse();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasOneTagThatDoesMatchSingleTagThenReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasOneTagThatDoesMatchSingleTagThenReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithUk), new[] { "UK" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasOneTagThatPartiallyMatchesTagThenReturnsFalse.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasOneTagThatPartiallyMatchesTagThenReturnsFalse()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithUk), new[] { "UK2" })
                    .ShouldBeFalse();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasOneTagThatDoesMatchMultipleTagsThenReturnsFalse.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasOneTagThatDoesMatchMultipleTagsThenReturnsFalse()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithUk), new[] { "UK", "Production" })
                    .ShouldBeFalse();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasTagsInTwoAttributeThatDoesMatchSingleTagThenReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasTagsInTwoAttributeThatDoesMatchSingleTagThenReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndProductionAndStagingInTwoTagsAttributes), new[] { "UK" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasTagsInTwoAttributesThatDoesMatchMultipleTagsThenReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasTagsInTwoAttributesThatDoesMatchMultipleTagsThenReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndProductionAndStagingInTwoTagsAttributes), new[] { "UK", "Production" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasTagsInOneAttributeThatDoesMatchMultipleTagsThenReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasTagsInOneAttributeThatDoesMatchMultipleTagsThenReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndProductionAndStagingInOneTagsAttribute), new[] { "UK", "Production" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasTagsInTwoAttributesThatDontNotMatchMultipleTagsThenReturnsFalse.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasTagsInTwoAttributesThatDontNotMatchMultipleTagsThenReturnsFalse()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndProductionAndStagingInTwoTagsAttributes), new[] { "UK", "IE" })
                    .ShouldBeFalse();
            }

            /// <summary>
            /// Defines the test method WhenBaseTypeHasTagsThenConcreteTypeReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenBaseTypeHasTagsThenConcreteTypeReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(ConcreteHasTagAttribute), new[] { "UK" })
                    .ShouldBeTrue();
            }


            //new
            /// <summary>
            /// Defines the test method WhenTypeHasSingleTagWithSingleTagNameAndBehaviorOfAnyAndHasMatchingTagNamesThenReturnTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasSingleTagWithSingleTagNameAndBehaviorOfAnyAndHasMatchingTagNamesThenReturnTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithUkAndAnyBehavior), new[] { "UK", "IE" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasSingleTagWithSingleTagNameAndBehaviorOfAnyButNoMatchingTagNamesThenReturnFalse.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasSingleTagWithSingleTagNameAndBehaviorOfAnyButNoMatchingTagNamesThenReturnFalse()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithUkAndAnyBehavior), new[] { "Chrome", "IE" })
                    .ShouldBeFalse();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasSingleTagWithMultipleTagNamesAndBehaviorOfAnyWithSomeMatchingTagNamesThenReturnTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasSingleTagWithMultipleTagNamesAndBehaviorOfAnyWithSomeMatchingTagNamesThenReturnTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndProductionAndStagingAndAnyBehaviorInOneTagsAttribute), new[] { "UK", "Staging", "IE" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasSingleTagWithMultipleTagNamesAndBehaviorOfAnyWithNoMatchingTagNamesThenReturnFalse.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasSingleTagWithMultipleTagNamesAndBehaviorOfAnyWithNoMatchingTagNamesThenReturnFalse()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndProductionAndStagingAndAnyBehaviorInOneTagsAttribute), new[] { "IE", "Chrome" })
                    .ShouldBeFalse();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasMultipleTagsWithMultipleTagNamesAndAllTagsHaveBehaviorOfAnyWithAllHavingAMatchingTagNameThenReturnTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasMultipleTagsWithMultipleTagNamesAndAllTagsHaveBehaviorOfAnyWithAllHavingAMatchingTagNameThenReturnTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndProductionAndStagingInTwoTagsAttributesWithAnyBehaviorOnBoth), new[] { "UK", "Staging" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasMultipleTagsWithMultipleTagNamesAndAllTagsHaveBehaviorOfAnyWithOneTagNotHavingAMatchingTagNameThenReturnTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasMultipleTagsWithMultipleTagNamesAndAllTagsHaveBehaviorOfAnyWithOneTagNotHavingAMatchingTagNameThenReturnTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndProductionAndStagingInTwoTagsAttributesWithAnyBehaviorOnBoth), new[] { "UK", "IE" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasMultipleTagsWithMultipleTagNamesAndOneHasBehaviorOfAnyAndOtherHasBehaviorOfAllWithAllTagNamesMatchingThenReturnTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasMultipleTagsWithMultipleTagNamesAndOneHasBehaviorOfAnyAndOtherHasBehaviorOfAllWithAllTagNamesMatchingThenReturnTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndAllBehaviorAndProductionAndStagingAndAnyBehaviorInTwoTagsAttributes), new[] { "UK", "Staging" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasMultipleTagsWithMultipleTagNamesAndOneHasBehaviorOfAnyAndOtherHasBehaviorOfAllWithoutAllTagNamesMatchingThenReturnTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasMultipleTagsWithMultipleTagNamesAndOneHasBehaviorOfAnyAndOtherHasBehaviorOfAllWithoutAllTagNamesMatchingThenReturnTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndAllBehaviorAndProductionAndStagingAndAnyBehaviorInTwoTagsAttributes), new[] { "UK", "Staging", "IE" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenTypeHasMultipleTagsWithMultipleTagNamesAndOneHasBehaviorOfAnyWithoutAnyMatchingTagNamesAndOtherHasBehaviorOfAllWithTagNamesMatchingThenReturnTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenTypeHasMultipleTagsWithMultipleTagNamesAndOneHasBehaviorOfAnyWithoutAnyMatchingTagNamesAndOtherHasBehaviorOfAllWithTagNamesMatchingThenReturnTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedWithBeAndUkAndAllBehaviorAndProductionAndStagingAndAnyBehaviorInTwoTagsAttributes), new[] { "BE", "UK" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenBaseInterfaceHasTagsThenConcreteTypeReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenBaseInterfaceHasTagsThenConcreteTypeReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedMigrationBySingleInterfaceTaggedWithUk), new[] { "UK" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenBaseInterfacesHaveTagsThenConcreteTypeReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenBaseInterfacesHaveTagsThenConcreteTypeReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedMigrationByMultipleInterfacesTaggedWithUsAndNy), new[] { "US" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByMultipleInterfacesTaggedWithUsAndNy), new[] { "NY" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByMultipleInterfacesTaggedWithUsAndNy), new[] { "US", "NY" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenBaseInterfaceInheritsTagsThenConcreteTypeReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenBaseInterfaceInheritsTagsThenConcreteTypeReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeInterfaceTaggedWithDev), new[] { "DEV" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenBaseTypesAndInterfacesHaveTagsThenConcreteTypeReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenBaseTypesAndInterfacesHaveTagsThenConcreteTypeReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeInheritanceTaggedWithBetaAndQa), new[] { "Beta" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeInheritanceTaggedWithBetaAndQa), new[] { "QA" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeInheritanceTaggedWithBetaAndQa), new[] { "Beta", "QA" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenAttributionAndBaseTypesAndInterfacesHaveTagsThenConcreteTypeReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenAttributionAndBaseTypesAndInterfacesHaveTagsThenConcreteTypeReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedMigrationByAttributionAndCompositeInheritanceTaggedWithStagingAndBetaAndDev), new[] { "Staging" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByAttributionAndCompositeInheritanceTaggedWithStagingAndBetaAndDev), new[] { "Beta" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByAttributionAndCompositeInheritanceTaggedWithStagingAndBetaAndDev), new[] { "DEV" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByAttributionAndCompositeInheritanceTaggedWithStagingAndBetaAndDev), new[] { "Staging", "Beta" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByAttributionAndCompositeInheritanceTaggedWithStagingAndBetaAndDev), new[] { "Staging", "DEV" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByAttributionAndCompositeInheritanceTaggedWithStagingAndBetaAndDev), new[] { "Beta", "DEV" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByAttributionAndCompositeInheritanceTaggedWithStagingAndBetaAndDev), new[] { "Staging", "Beta", "DEV" })
                    .ShouldBeTrue();
            }

            /// <summary>
            /// Defines the test method WhenBaseInterfacesHaveOverlappingTagsThenConcreteTypeReturnsTrue.
            /// </summary>
            [Test]
            [Category("Tagging")]
            public void WhenBaseInterfacesHaveOverlappingTagsThenConcreteTypeReturnsTrue()
            {
                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeOverlappingTagsTaggedWithCaAndNvOnceAndTxTwice), new[] { "CA" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeOverlappingTagsTaggedWithCaAndNvOnceAndTxTwice), new[] { "NV" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeOverlappingTagsTaggedWithCaAndNvOnceAndTxTwice), new[] { "TX" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeOverlappingTagsTaggedWithCaAndNvOnceAndTxTwice), new[] { "CA", "NV" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeOverlappingTagsTaggedWithCaAndNvOnceAndTxTwice), new[] { "CA", "TX" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeOverlappingTagsTaggedWithCaAndNvOnceAndTxTwice), new[] { "NV", "TX" })
                    .ShouldBeTrue();

                _default.TypeHasMatchingTags(typeof(TaggedMigrationByCompositeOverlappingTagsTaggedWithCaAndNvOnceAndTxTwice), new[] { "CA", "NV", "TX" })
                    .ShouldBeTrue();
            }
        }

        /// <summary>
        /// Defines the test method GetAutoScriptUpName.
        /// </summary>
        [Test]
        public void GetAutoScriptUpName()
        {
            var processor = new Mock<IMigrationProcessor>();
            processor.SetupGet(p => p.DatabaseType).Returns("SqlServer2016");
            processor.SetupGet(p => p.DatabaseTypeAliases).Returns(new[] { "SqlServer" });
            var serviceProvider = ServiceCollectionExtensions.CreateServices()
                .WithProcessor(processor)
                .AddScoped<IConnectionStringReader>(_ => new PassThroughConnectionStringReader("No connection"))
                .BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<IMigrationContext>();
            var expr = new AutoScriptMigrationFake();
            expr.GetUpExpressions(context);

            var expression = context.Expressions.Single();
            var processed = (IAutoNameExpression)expression.Apply(ConventionSets.NoSchemaName);
            processed.AutoNames.ShouldNotBeNull();
            CollectionAssert.AreEqual(
                new[]
                {
                    "Scripts.Up.20130508175300_AutoScriptMigrationFake_SqlServer2016.sql",
                    "Scripts.Up.20130508175300_AutoScriptMigrationFake_SqlServer.sql",
                    "Scripts.Up.20130508175300_AutoScriptMigrationFake_Generic.sql",
                },
                processed.AutoNames);
        }

        /// <summary>
        /// Defines the test method GetAutoScriptDownName.
        /// </summary>
        [Test]
        public void GetAutoScriptDownName()
        {
            var processor = new Mock<IMigrationProcessor>();
            processor.SetupGet(p => p.DatabaseType).Returns("SqlServer2016");
            processor.SetupGet(p => p.DatabaseTypeAliases).Returns(new[] { "SqlServer" });
            var serviceProvider = ServiceCollectionExtensions.CreateServices()
                .WithProcessor(processor)
                .AddScoped<IConnectionStringReader>(_ => new PassThroughConnectionStringReader("No connection"))
                .BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<IMigrationContext>();
            var expr = new AutoScriptMigrationFake();
            expr.GetDownExpressions(context);

            var expression = context.Expressions.Single();
            var processed = (IAutoNameExpression)expression.Apply(ConventionSets.NoSchemaName);

            processed.AutoNames.ShouldNotBeNull();
            CollectionAssert.AreEqual(
                new[]
                {
                    "Scripts.Down.20130508175300_AutoScriptMigrationFake_SqlServer2016.sql",
                    "Scripts.Down.20130508175300_AutoScriptMigrationFake_SqlServer.sql",
                    "Scripts.Down.20130508175300_AutoScriptMigrationFake_Generic.sql",
                },
                processed.AutoNames);
        }

        /// <summary>
        /// Defines the test method ObsoleteGetAutoScriptUpName.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteGetAutoScriptUpName()
        {
            var querySchema = new SqlServerProcessor(
                new[] { "SqlServer2016", "SqlServer" },
                null,
                null,
                null,
                new ProcessorOptions(),
                null);
            var assemblyCollection = new Mock<IAssemblyCollection>();
            assemblyCollection.SetupGet(c => c.Assemblies).Returns(new Assembly[0]);
            var context = new MigrationContext(querySchema, assemblyCollection.Object, null, null);
            var expr = new ObsoleteAutoScriptMigrationFake();
            expr.GetUpExpressions(context);

            var expression = context.Expressions.Single();
            var processed = (IAutoNameExpression)expression.Apply(ConventionSets.NoSchemaName);
            processed.AutoNames.ShouldNotBeNull();
            CollectionAssert.AreEqual(
                new[]
                {
                    "Scripts.Up.20130508175300_ObsoleteAutoScriptMigrationFake_SqlServer2016.sql",
                    "Scripts.Up.20130508175300_ObsoleteAutoScriptMigrationFake_SqlServer.sql",
                    "Scripts.Up.20130508175300_ObsoleteAutoScriptMigrationFake_Generic.sql",
                },
                processed.AutoNames);
        }

        /// <summary>
        /// Defines the test method ObsoleteGetAutoScriptDownName.
        /// </summary>
        [Test]
        [Obsolete]
        public void ObsoleteGetAutoScriptDownName()
        {
            var querySchema = new SqlServerProcessor(
                new[] { "SqlServer2016", "SqlServer" },
                null,
                null,
                null,
                new ProcessorOptions(),
                null);
            var assemblyCollection = new Mock<IAssemblyCollection>();
            assemblyCollection.SetupGet(c => c.Assemblies).Returns(new Assembly[0]);
            var context = new MigrationContext(querySchema, assemblyCollection.Object, null, null);
            var expr = new ObsoleteAutoScriptMigrationFake();
            expr.GetDownExpressions(context);

            var expression = context.Expressions.Single();
            var processed = (IAutoNameExpression)expression.Apply(ConventionSets.NoSchemaName);

            processed.AutoNames.ShouldNotBeNull();
            CollectionAssert.AreEqual(
                new[]
                {
                    "Scripts.Down.20130508175300_ObsoleteAutoScriptMigrationFake_SqlServer2016.sql",
                    "Scripts.Down.20130508175300_ObsoleteAutoScriptMigrationFake_SqlServer.sql",
                    "Scripts.Down.20130508175300_ObsoleteAutoScriptMigrationFake_Generic.sql",
                },
                processed.AutoNames);
        }

        /// <summary>
        /// Class ConventionsTestClass.
        /// Implements the <see cref="FluentMigrator.Expressions.ISchemaExpression" />
        /// Implements the <see cref="FluentMigrator.Expressions.IFileSystemExpression" />
        /// </summary>
        /// <seealso cref="FluentMigrator.Expressions.ISchemaExpression" />
        /// <seealso cref="FluentMigrator.Expressions.IFileSystemExpression" />
        private class ConventionsTestClass : ISchemaExpression, IFileSystemExpression
        {
            /// <summary>
            /// Gets or sets the schema name
            /// </summary>
            /// <value>The name of the schema.</value>
            public string SchemaName { get; set; }
            /// <summary>
            /// Gets or sets the root path (working directory)
            /// </summary>
            /// <value>The root path.</value>
            public string RootPath { get; set; }
        }
    }

    /// <summary>
    /// Class ObsoleteAutoScriptMigrationFake.
    /// Implements the <see cref="FluentMigrator.AutoScriptMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.AutoScriptMigration" />
    [Migration(20130508175300)]
    [Obsolete]
    class ObsoleteAutoScriptMigrationFake : AutoScriptMigration
    {
    }

    /// <summary>
    /// Class AutoScriptMigrationFake.
    /// Implements the <see cref="FluentMigrator.AutoScriptMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.AutoScriptMigration" />
    [Migration(20130508175300)]
    class AutoScriptMigrationFake : AutoScriptMigration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoScriptMigrationFake"/> class.
        /// </summary>
        public AutoScriptMigrationFake()
            : base(new[] { new DefaultEmbeddedResourceProvider() })
        {
        }
    }

    /// <summary>
    /// Class TaggedWithBeAndUkAndProductionAndStagingInOneTagsAttribute.
    /// </summary>
    [Tags("BE", "UK", "Staging", "Production")]
    public class TaggedWithBeAndUkAndProductionAndStagingInOneTagsAttribute
    {
    }

    /// <summary>
    /// Class TaggedWithBeAndUkAndProductionAndStagingAndAnyBehaviorInOneTagsAttribute.
    /// </summary>
    [Tags(TagBehavior.RequireAny, "BE", "UK", "Staging", "Production")]
    public class TaggedWithBeAndUkAndProductionAndStagingAndAnyBehaviorInOneTagsAttribute
    {
    }

    /// <summary>
    /// Class TaggedWithBeAndUkAndProductionAndStagingInTwoTagsAttributes.
    /// </summary>
    [Tags("BE", "UK")]
    [Tags("Staging", "Production")]
    public class TaggedWithBeAndUkAndProductionAndStagingInTwoTagsAttributes
    {
    }

    /// <summary>
    /// Class TaggedWithBeAndUkAndProductionAndStagingInTwoTagsAttributesWithAnyBehaviorOnBoth.
    /// </summary>
    [Tags(TagBehavior.RequireAny, "BE", "UK")]
    [Tags(TagBehavior.RequireAny, "Staging", "Production")]
    public class TaggedWithBeAndUkAndProductionAndStagingInTwoTagsAttributesWithAnyBehaviorOnBoth
    {
    }

    /// <summary>
    /// Class TaggedWithBeAndUkAndAllBehaviorAndProductionAndStagingAndAnyBehaviorInTwoTagsAttributes.
    /// </summary>
    [Tags(TagBehavior.RequireAll, "BE", "UK", "Staging")]
    [Tags(TagBehavior.RequireAny, "Staging", "Production")]
    public class TaggedWithBeAndUkAndAllBehaviorAndProductionAndStagingAndAnyBehaviorInTwoTagsAttributes
    {
    }

    /// <summary>
    /// Class TaggedWithUk.
    /// </summary>
    [Tags("UK")]
    public class TaggedWithUk
    {
    }

    /// <summary>
    /// Class InheritedFromTaggedWithUk.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.TaggedWithUk" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.TaggedWithUk" />
    public class InheritedFromTaggedWithUk : TaggedWithUk
    {
    }

    /// <summary>
    /// Class TaggedWithUkAndAnyBehavior.
    /// </summary>
    [Tags(TagBehavior.RequireAny, "UK")]
    public class TaggedWithUkAndAnyBehavior
    {
    }

    /// <summary>
    /// Class HasTagAttributeWithNoTagNames.
    /// </summary>
    [Tags]
    public class HasTagAttributeWithNoTagNames
    {
    }

    /// <summary>
    /// Class HasNoTagsFake.
    /// </summary>
    public class HasNoTagsFake
    {
    }

    /// <summary>
    /// Class BaseHasTagAttribute.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Tags("UK")]
    public abstract class BaseHasTagAttribute : Migration
    { }

    /// <summary>
    /// Class ConcreteHasTagAttribute.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.BaseHasTagAttribute" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.BaseHasTagAttribute" />
    public class ConcreteHasTagAttribute : BaseHasTagAttribute
    {
        /// <summary>
        /// Ups this instance.
        /// </summary>
        public override void Up() { }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down() { }
    }

    /// <summary>
    /// Class DefaultConventionMigrationFake.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Migration(123, TransactionBehavior.None)]
    [MigrationTrait("key", "test")]
    internal class DefaultConventionMigrationFake : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up() { }
        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down() { }
    }

    /// <summary>
    /// Class MigrationWithoutAttributeFake.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class MigrationWithoutAttributeFake : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up() { }
        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down() { }
    }

    /// <summary>
    /// Class MaintenanceAfterEach.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Maintenance(MigrationStage.AfterEach)]
    internal class MaintenanceAfterEach : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up() { }
        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down() { }
    }

    // tagged interfaces for multiple inheritance of tags
    /// <summary>
    /// Interface ITaggedMigrationAppliesToCountries
    /// </summary>
    [Tags("UK", "US")]
    internal interface ITaggedMigrationAppliesToCountries { }

    /// <summary>
    /// Interface ITaggedMigrationAppliesToUatEnvironment
    /// </summary>
    [Tags("UAT")]
    internal interface ITaggedMigrationAppliesToUatEnvironment { }

    /// <summary>
    /// Interface ITaggedMigrationAppliesToLowerEnvironments
    /// </summary>
    [Tags("DEV", "QA")]
    internal interface ITaggedMigrationAppliesToLowerEnvironments { }

    /// <summary>
    /// Interface ITaggedMigrationAppliesToNonProductionEnvironments
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToLowerEnvironments" />
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToUatEnvironment" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToLowerEnvironments" />
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToUatEnvironment" />
    internal interface ITaggedMigrationAppliesToNonProductionEnvironments : ITaggedMigrationAppliesToLowerEnvironments, ITaggedMigrationAppliesToUatEnvironment
    {
    }

    /// <summary>
    /// Interface ITaggedMigrationAppliesToFeature1
    /// </summary>
    [Tags("CA", "NY")]
    internal interface ITaggedMigrationAppliesToFeature1 { }

    /// <summary>
    /// Interface ITaggedMigrationAppliesToFeature2
    /// </summary>
    [Tags("NV", "TX")]
    internal interface ITaggedMigrationAppliesToFeature2 { }

    /// <summary>
    /// Interface ITaggedMigrationAppliesToFeature3
    /// </summary>
    [Tags("CA", "TX")]
    internal interface ITaggedMigrationAppliesToFeature3 { }

    // migrations by inheritance
    /// <summary>
    /// Class UntaggedConcreteMigration.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    internal class UntaggedConcreteMigration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up() { }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down() { }
    }

    /// <summary>
    /// Class TaggedMigrationAppliesToBetaEnvironment.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    [Tags("Beta")]
    internal class TaggedMigrationAppliesToBetaEnvironment : UntaggedConcreteMigration { }

    /// <summary>
    /// Class TaggedMigrationBySingleInterfaceTaggedWithUk.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToCountries" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToCountries" />
    internal class TaggedMigrationBySingleInterfaceTaggedWithUk : UntaggedConcreteMigration, ITaggedMigrationAppliesToCountries { }

    /// <summary>
    /// Class TaggedMigrationByMultipleInterfacesTaggedWithUsAndNy.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToCountries" />
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToFeature1" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToCountries" />
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToFeature1" />
    internal class TaggedMigrationByMultipleInterfacesTaggedWithUsAndNy : UntaggedConcreteMigration, ITaggedMigrationAppliesToCountries, ITaggedMigrationAppliesToFeature1 { }

    /// <summary>
    /// Class TaggedMigrationByCompositeInterfaceTaggedWithDev.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToNonProductionEnvironments" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToNonProductionEnvironments" />
    internal class TaggedMigrationByCompositeInterfaceTaggedWithDev : UntaggedConcreteMigration, ITaggedMigrationAppliesToNonProductionEnvironments { }

    /// <summary>
    /// Class TaggedMigrationByCompositeInheritanceTaggedWithBetaAndQa.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.TaggedMigrationAppliesToBetaEnvironment" />
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToNonProductionEnvironments" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.TaggedMigrationAppliesToBetaEnvironment" />
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToNonProductionEnvironments" />
    internal class TaggedMigrationByCompositeInheritanceTaggedWithBetaAndQa : TaggedMigrationAppliesToBetaEnvironment, ITaggedMigrationAppliesToNonProductionEnvironments { }

    /// <summary>
    /// Class TaggedMigrationByAttributionAndCompositeInheritanceTaggedWithStagingAndBetaAndDev.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.TaggedMigrationAppliesToBetaEnvironment" />
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToNonProductionEnvironments" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.TaggedMigrationAppliesToBetaEnvironment" />
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToNonProductionEnvironments" />
    [Tags("Staging")]
    internal class TaggedMigrationByAttributionAndCompositeInheritanceTaggedWithStagingAndBetaAndDev : TaggedMigrationAppliesToBetaEnvironment, ITaggedMigrationAppliesToNonProductionEnvironments { }

    /// <summary>
    /// Class TaggedMigrationByCompositeOverlappingTagsTaggedWithCaAndNvOnceAndTxTwice.
    /// Implements the <see cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToFeature2" />
    /// Implements the <see cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToFeature3" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Unit.UntaggedConcreteMigration" />
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToFeature2" />
    /// <seealso cref="FluentMigrator.Tests.Unit.ITaggedMigrationAppliesToFeature3" />
    internal class TaggedMigrationByCompositeOverlappingTagsTaggedWithCaAndNvOnceAndTxTwice : UntaggedConcreteMigration, ITaggedMigrationAppliesToFeature2, ITaggedMigrationAppliesToFeature3 { }
}
