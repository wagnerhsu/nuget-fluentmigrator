// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="PerformDBOperationTests.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Processors.SqlServer;

using NUnit.Framework;

namespace FluentMigrator.Tests.Integration
{
    /// <summary>
    /// Defines test class PerformDBOperationTests.
    /// Implements the <see cref="FluentMigrator.Tests.Integration.IntegrationTestBase" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Tests.Integration.IntegrationTestBase" />
    [TestFixture]
    [Category("Integration")]
    public class PerformDBOperationTests : IntegrationTestBase
    {
        /// <summary>
        /// Defines the test method CanCreateAndDeleteTableUsingThePerformDBOperationExpressions.
        /// </summary>
        [Test]
        [Category("SqlServer2008")]
        [Category("SqlServer2012")]
        [Category("SqlServer2014")]
        [Category("SqlServer2016")]
        public void CanCreateAndDeleteTableUsingThePerformDBOperationExpressions()
        {
            if (!IntegrationTestOptions.SqlServer2008.IsEnabled
             && !IntegrationTestOptions.SqlServer2012.IsEnabled
             && !IntegrationTestOptions.SqlServer2014.IsEnabled
             && !IntegrationTestOptions.SqlServer2016.IsEnabled)
            {
                Assert.Ignore("No processor found for the given action.");
            }

            var expression = new PerformDBOperationExpression
            {
                Operation = (connection, transaction) =>
                {
                    // I know I could be using the expressions to create and delete this table,
                    // but really I just want to test whether I can execute some commands against the connection.

                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = "CREATE TABLE dbo.TestTable(TestTableID int NULL)";

                    command.ExecuteNonQuery();

                    var command2 = connection.CreateCommand();
                    command2.Transaction = transaction;
                    command2.CommandText = "DROP TABLE dbo.TestTable";

                    command2.ExecuteNonQuery();
                }
            };

            if (IntegrationTestOptions.SqlServer2008.IsEnabled)
            {
                ExecuteWithProcessor<SqlServer2008Processor>(
                    services => { },
                    (provider, processor) => processor.Process(expression),
                    true,
                    IntegrationTestOptions.SqlServer2008);
            }

            if (IntegrationTestOptions.SqlServer2012.IsEnabled)
            {
                ExecuteWithProcessor<SqlServer2012Processor>(
                    services => { },
                    (provider, processor) => processor.Process(expression),
                    true,
                    IntegrationTestOptions.SqlServer2012);
            }

            if (IntegrationTestOptions.SqlServer2014.IsEnabled)
            {
                ExecuteWithProcessor<SqlServer2014Processor>(
                    services => { },
                    (provider, processor) => processor.Process(expression),
                    true,
                    IntegrationTestOptions.SqlServer2014);
            }

            if (IntegrationTestOptions.SqlServer2016.IsEnabled)
            {
                ExecuteWithProcessor<SqlServer2016Processor>(
                    services => { },
                    (provider, processor) => processor.Process(expression),
                    true,
                    IntegrationTestOptions.SqlServer2016);
            }
        }
    }
}
