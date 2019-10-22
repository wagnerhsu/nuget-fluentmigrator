// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServerCeTestTable.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Data.SqlServerCe;
using System.Collections.Generic;
using System.Text;
using FluentMigrator.Runner.Generators.SqlServer;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Processors.SqlServer;

namespace FluentMigrator.Tests.Helpers
{

    /// <summary>
    /// Class SqlServerCeTestTable.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class SqlServerCeTestTable : IDisposable
    {
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        private SqlCeConnection Connection { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the quoter.
        /// </summary>
        /// <value>The quoter.</value>
        private IQuoter Quoter { get; set; }
        /// <summary>
        /// The constraints
        /// </summary>
        private List<string> constraints = new List<string>();
        /// <summary>
        /// The indexies
        /// </summary>
        private List<string> indexies = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCeTestTable"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="processor">The processor.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public SqlServerCeTestTable(string table, SqlServerCeProcessor processor, params string[] columnDefinitions)
        {
            Connection = (SqlCeConnection)processor.Connection;
            Quoter = new SqlServer2000Quoter();

            Name = table;
            Create(columnDefinitions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCeTestTable"/> class.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="columnDefinitions">The column definitions.</param>
        public SqlServerCeTestTable(SqlServerCeProcessor processor, params string[] columnDefinitions)
        {
            Connection = (SqlCeConnection)processor.Connection;
            Quoter = new SqlServer2000Quoter();

            Name = "TestTable";
            Create(columnDefinitions);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Drop();
        }

        /// <summary>
        /// Creates the specified column definitions.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        public void Create(IEnumerable<string> columnDefinitions)
        {

            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ");
            sb.Append(Quoter.QuoteTableName(Name));

            foreach (string definition in columnDefinitions)
            {
                sb.Append("(");
                sb.Append(definition);
                sb.Append("), ");
            }

            sb.Remove(sb.Length - 2, 2);

            using (var command = new SqlCeCommand(sb.ToString(), Connection))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Withes the unique constraint on.
        /// </summary>
        /// <param name="column">The column.</param>
        public void WithUniqueConstraintOn(string column)
        {
            WithUniqueConstraintOn(column, "UC_" + column);
        }

        /// <summary>
        /// Withes the unique constraint on.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="name">The name.</param>
        public void WithUniqueConstraintOn(string column, string name)
        {
            var sb = new StringBuilder();
            var constraintName = Quoter.Quote(name);
            constraints.Add(constraintName);
            sb.Append(string.Format("ALTER TABLE {0} ADD CONSTRAINT {1} UNIQUE ({2})", Quoter.QuoteTableName(Name), constraintName, column));
            using (var command = new SqlCeCommand(sb.ToString(), Connection))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Withes the index on.
        /// </summary>
        /// <param name="column">The column.</param>
        public void WithIndexOn(string column)
        {
            WithIndexOn(column, "UI_" + column);
        }

        /// <summary>
        /// Withes the index on.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="name">The name.</param>
        public void WithIndexOn(string column, string name)
        {
            var sb = new StringBuilder();
            var indexName = Quoter.QuoteIndexName(name);
            indexies.Add(indexName);
            sb.Append(string.Format("CREATE UNIQUE INDEX {0} ON {1} ({2})", indexName, Quoter.QuoteTableName(Name), column));
            using (var command = new SqlCeCommand(sb.ToString(), Connection))
                command.ExecuteNonQuery();
        }

        /// <summary>
        /// Drops this instance.
        /// </summary>
        public void Drop()
        {
            foreach (var contraint in constraints)
            {
                using (var command = new SqlCeCommand("ALTER TABLE " + Quoter.QuoteTableName(Name) + " DROP CONSTRAINT " + contraint, Connection))
                    command.ExecuteNonQuery();
            }

            foreach (var index in indexies)
            {
                using (var command = new SqlCeCommand("DROP INDEX " + Quoter.QuoteTableName(Name) + "." + index, Connection))
                    command.ExecuteNonQuery();
            }

            using (var command = new SqlCeCommand("DROP TABLE " + Quoter.QuoteTableName(Name), Connection))
                command.ExecuteNonQuery();
        }
    }
}
