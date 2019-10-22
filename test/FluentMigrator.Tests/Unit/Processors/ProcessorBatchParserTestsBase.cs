// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="ProcessorBatchParserTestsBase.cs" company="FluentMigrator Project">
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

using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using FluentMigrator.Runner.Initialization;

using Moq;
using Moq.Protected;

using NUnit.Framework;

namespace FluentMigrator.Tests.Unit.Processors
{
    /// <summary>
    /// Class ProcessorBatchParserTestsBase.
    /// </summary>
    [Category("BatchParser")]
    public abstract class ProcessorBatchParserTestsBase
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private const string ConnectionString = "server=this";

        /// <summary>
        /// The connection state
        /// </summary>
        private ConnectionState _connectionState;

        /// <summary>
        /// Gets the mocked connection.
        /// </summary>
        /// <value>The mocked connection.</value>
        protected Mock<DbConnection> MockedConnection { get; private set; }
        /// <summary>
        /// Gets the mocked database provider factory.
        /// </summary>
        /// <value>The mocked database provider factory.</value>
        protected Mock<DbProviderFactory> MockedDbProviderFactory { get; private set; }
        /// <summary>
        /// Gets the mocked connection string accessor.
        /// </summary>
        /// <value>The mocked connection string accessor.</value>
        protected Mock<IConnectionStringAccessor> MockedConnectionStringAccessor { get; private set; }
        /// <summary>
        /// Gets or sets the mocked commands.
        /// </summary>
        /// <value>The mocked commands.</value>
        private List<Mock<DbCommand>> MockedCommands { get; set; }

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _connectionState = ConnectionState.Closed;

            MockedCommands = new List<Mock<DbCommand>>();
            MockedConnection = new Mock<DbConnection>();
            MockedDbProviderFactory = new Mock<DbProviderFactory>();
            MockedConnectionStringAccessor = new Mock<IConnectionStringAccessor>();

            MockedConnection.SetupGet(conn => conn.State).Returns(() => _connectionState);
            MockedConnection.Setup(conn => conn.Open()).Callback(() => _connectionState = ConnectionState.Open);
            MockedConnection.Setup(conn => conn.Close()).Callback(() => _connectionState = ConnectionState.Closed);
            MockedConnection.SetupProperty(conn => conn.ConnectionString);
            MockedConnection.Protected().Setup("Dispose", ItExpr.IsAny<bool>());

            MockedConnectionStringAccessor.SetupGet(a => a.ConnectionString).Returns(ConnectionString);

            MockedDbProviderFactory.Setup(factory => factory.CreateConnection())
                .Returns(MockedConnection.Object);

            MockedDbProviderFactory.Setup(factory => factory.CreateCommand())
                .Returns(
                    () =>
                    {
                        var commandMock = new Mock<DbCommand>()
                            .SetupProperty(cmd => cmd.CommandText);
                        commandMock.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);
                        commandMock.Protected().SetupGet<DbConnection>("DbConnection").Returns(MockedConnection.Object);
                        commandMock.Protected().SetupSet<DbConnection>("DbConnection", ItExpr.Is<DbConnection>(v => v == MockedConnection.Object));
                        commandMock.Protected().Setup("Dispose", ItExpr.IsAny<bool>());
                        MockedCommands.Add(commandMock);
                        return commandMock.Object;
                    });
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            foreach (var mockedCommand in MockedCommands)
            {
                mockedCommand.VerifyNoOtherCalls();
            }

            MockedDbProviderFactory.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Defines the test method TestOneCommandForMultipleLines.
        /// </summary>
        [Test]
        public void TestOneCommandForMultipleLines()
        {
            var command = "SELECT 1\nSELECT 2";
            using (var processor = CreateProcessor())
            {
                processor.Execute(command);
            }

            MockedDbProviderFactory.Verify(factory => factory.CreateConnection());

            foreach (var mockedCommand in MockedCommands)
            {
                MockedDbProviderFactory.Verify(factory => factory.CreateCommand());
                mockedCommand.VerifySet(cmd => cmd.Connection = MockedConnection.Object);
                mockedCommand.VerifySet(cmd => cmd.CommandText = command);
                mockedCommand.Verify(cmd => cmd.ExecuteNonQuery());
                mockedCommand.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
            }

            MockedConnection.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
        }

        /// <summary>
        /// Defines the test method TestThatTwoCommandsGetSeparatedByGo.
        /// </summary>
        [Test]
        public void TestThatTwoCommandsGetSeparatedByGo()
        {
            using (var processor = CreateProcessor())
            {
                processor.Execute("SELECT 1\nGO\nSELECT 2");
            }

            MockedDbProviderFactory.Verify(factory => factory.CreateConnection());

            for (int index = 0; index < MockedCommands.Count; index++)
            {
                var command = $"SELECT {index + 1}";
                var mockedCommand = MockedCommands[index];
                MockedDbProviderFactory.Verify(factory => factory.CreateCommand());
                mockedCommand.VerifySet(cmd => cmd.Connection = MockedConnection.Object);
                mockedCommand.VerifySet(cmd => cmd.CommandText = command);
                mockedCommand.Verify(cmd => cmd.ExecuteNonQuery());
                mockedCommand.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
            }

            MockedConnection.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
        }

        /// <summary>
        /// Defines the test method TestThatTwoCommandsAreNotSeparatedByGoInComment.
        /// </summary>
        [Test]
        public void TestThatTwoCommandsAreNotSeparatedByGoInComment()
        {
            var command = "SELECT 1\n /* GO */\nSELECT 2";

            using (var processor = CreateProcessor())
            {
                processor.Execute(command);
            }

            MockedDbProviderFactory.Verify(factory => factory.CreateConnection());

            foreach (var mockedCommand in MockedCommands)
            {
                MockedDbProviderFactory.Verify(factory => factory.CreateCommand());
                mockedCommand.VerifySet(cmd => cmd.Connection = MockedConnection.Object);
                mockedCommand.VerifySet(cmd => cmd.CommandText = command);
                mockedCommand.Verify(cmd => cmd.ExecuteNonQuery());
                mockedCommand.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
            }

            MockedConnection.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
        }

        /// <summary>
        /// Defines the test method TestThatTwoCommandsAreNotSeparatedByGoInString.
        /// </summary>
        [Test]
        public void TestThatTwoCommandsAreNotSeparatedByGoInString()
        {
            var command = "SELECT '\nGO\n'\nSELECT 2";

            using (var processor = CreateProcessor())
            {
                processor.Execute(command);
            }

            MockedDbProviderFactory.Verify(factory => factory.CreateConnection());

            foreach (var mockedCommand in MockedCommands)
            {
                MockedDbProviderFactory.Verify(factory => factory.CreateCommand());
                mockedCommand.VerifySet(cmd => cmd.Connection = MockedConnection.Object);
                mockedCommand.VerifySet(cmd => cmd.CommandText = command);
                mockedCommand.Verify(cmd => cmd.ExecuteNonQuery());
                mockedCommand.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
            }

            MockedConnection.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
        }

        /// <summary>
        /// Defines the test method Issue442.
        /// </summary>
        [Test]
        public void Issue442()
        {
            var command = "SELECT '\n\n\n';\nSELECT 2;";

            using (var processor = CreateProcessor())
            {
                processor.Execute(command);
            }

            MockedDbProviderFactory.Verify(factory => factory.CreateConnection());

            foreach (var mockedCommand in MockedCommands)
            {
                MockedDbProviderFactory.Verify(factory => factory.CreateCommand());
                mockedCommand.VerifySet(cmd => cmd.Connection = MockedConnection.Object);
                mockedCommand.VerifySet(cmd => cmd.CommandText = command);
                mockedCommand.Verify(cmd => cmd.ExecuteNonQuery());
                mockedCommand.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
            }

            MockedConnection.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
        }

        /// <summary>
        /// Defines the test method Issue842.
        /// </summary>
        [Test]
        public void Issue842()
        {
            var command = @"insert into MyTable (Id, Data)\nvalues (42, 'This is a list of games played by people\n\nDooM\nPokemon GO\nPotato-o-matic');";

            using (var processor = CreateProcessor())
            {
                processor.Execute(command);
            }

            MockedDbProviderFactory.Verify(factory => factory.CreateConnection());

            foreach (var mockedCommand in MockedCommands)
            {
                MockedDbProviderFactory.Verify(factory => factory.CreateCommand());
                mockedCommand.VerifySet(cmd => cmd.Connection = MockedConnection.Object);
                mockedCommand.VerifySet(cmd => cmd.CommandText = command);
                mockedCommand.Verify(cmd => cmd.ExecuteNonQuery());
                mockedCommand.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
            }

            MockedConnection.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
        }

        /// <summary>
        /// Defines the test method TestThatGoWithRunCount.
        /// </summary>
        [Test]
        public void TestThatGoWithRunCount()
        {
            using (var processor = CreateProcessor())
            {
                processor.Execute("SELECT 1\nGO 3\nSELECT 2\nGO\nSELECT 3\nGO 2");
            }

            var expected = new (string command, int count)[]
            {
                ("SELECT 1", 3),
                ("SELECT 2", 1),
                ("SELECT 3", 2),
            };

            Assert.AreEqual(expected.Length, MockedCommands.Count);

            MockedDbProviderFactory.Verify(factory => factory.CreateConnection());

            for (var index = 0; index < MockedCommands.Count; index++)
            {
                var (command, count) = expected[index];
                var mockedCommand = MockedCommands[index];
                MockedDbProviderFactory.Verify(factory => factory.CreateCommand());
                mockedCommand.VerifySet(cmd => cmd.Connection = MockedConnection.Object);
                mockedCommand.VerifySet(cmd => cmd.CommandText = command);
                mockedCommand.Verify(cmd => cmd.ExecuteNonQuery(), Times.Exactly(count));
                mockedCommand.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
            }

            MockedConnection.Protected().Verify("Dispose", Times.Exactly(1), ItExpr.IsAny<bool>());
        }

        /// <summary>
        /// Creates the processor.
        /// </summary>
        /// <returns>IMigrationProcessor.</returns>
        protected abstract IMigrationProcessor CreateProcessor();
    }
}
