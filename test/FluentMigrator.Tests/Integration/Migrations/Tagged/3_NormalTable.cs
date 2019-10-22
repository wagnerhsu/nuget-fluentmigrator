// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="3_NormalTable.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace FluentMigrator.Tests.Integration.Migrations.Tagged
{
    /// <summary>
    /// Class NormalTable.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Migration(3)]
    public class NormalTable : Migration
    {
        /// <summary>
        /// The table name
        /// </summary>
        private const string TableName = "NormalTable";

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32();
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}
