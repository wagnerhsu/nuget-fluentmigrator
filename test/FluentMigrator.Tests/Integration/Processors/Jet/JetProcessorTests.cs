// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="JetProcessorTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
#region License
// Copyright (c) 2007-2018, Sean Chambers <schambers80@gmail.com>
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

using FluentMigrator.Tests.Helpers;

using NUnit.Framework;

using Shouldly;

namespace FluentMigrator.Tests.Integration.Processors.Jet
{
    /// <summary>
    /// Defines test class JetProcessorTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.Processors.Jet.JetIntegrationTests" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.Processors.Jet.JetIntegrationTests" />
    [TestFixture]
    public class JetProcessorTests : JetIntegrationTests
    {
        /// <summary>
        /// Defines the test method CallingTableExistsReturnsFalseIfTableDoesNotExist.
        /// </summary>
        [Test]
        public void CallingTableExistsReturnsFalseIfTableDoesNotExist()
        {
            Processor.TableExists(null, "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingTableExistsReturnsTrueIfTableExists.
        /// </summary>
        [Test]
        public void CallingTableExistsReturnsTrueIfTableExists()
        {
            using (var table = new JetTestTable(Processor, "id int"))
                Processor.TableExists(null, table.Name).ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsTrueIfColumnExists.
        /// </summary>
        [Test]
        public void CallingColumnExistsReturnsTrueIfColumnExists()
        {
            using (var table = new JetTestTable(Processor, "id int"))
                Processor.ColumnExists(null, table.Name, "id").ShouldBeTrue();
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsFalseIfTableDoesNotExist.
        /// </summary>
        [Test]
        public void CallingColumnExistsReturnsFalseIfTableDoesNotExist()
        {
            Processor.ColumnExists(null, "DoesNotExist", "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CallingColumnExistsReturnsFalseIfColumnDoesNotExist.
        /// </summary>
        [Test]
        public void CallingColumnExistsReturnsFalseIfColumnDoesNotExist()
        {
            using (var table = new JetTestTable(Processor, "id int"))
                Processor.ColumnExists(null, table.Name, "DoesNotExist").ShouldBeFalse();
        }

        /// <summary>
        /// Defines the test method CanReadData.
        /// </summary>
        [Test]
        public void CanReadData()
        {
            using (var table = new JetTestTable(Processor, "id int"))
            {
                AddTestData(table);

                DataSet ds = Processor.Read("SELECT * FROM {0}", table.Name);

                ds.ShouldNotBeNull();
                ds.Tables.Count.ShouldBe(1);
                ds.Tables[0].Rows.Count.ShouldBe(3);
                ds.Tables[0].Rows[2][0].ShouldBe(2);
            }
        }

        /// <summary>
        /// Defines the test method CanReadTableData.
        /// </summary>
        [Test]
        public void CanReadTableData()
        {
            using (var table = new JetTestTable(Processor, "id int"))
            {
                AddTestData(table);

                DataSet ds = Processor.ReadTableData(null, table.Name);

                ds.ShouldNotBeNull();
                ds.Tables.Count.ShouldBe(1);
                ds.Tables[0].Rows.Count.ShouldBe(3);
                ds.Tables[0].Rows[2][0].ShouldBe(2);
            }
        }

        /// <summary>
        /// Adds the test data.
        /// </summary>
        /// <param name="table">The table.</param>
        private void AddTestData(JetTestTable table)
        {
            for (int i = 0; i < 3; i++)
            {
                var cmd = table.Connection.CreateCommand();
                cmd.Transaction = table.Transaction;
                cmd.CommandText = $"INSERT INTO {table.Name} (id) VALUES ({i})";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
