// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TestUpdateAllRowsMigration.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace FluentMigrator.Tests.Integration.Migrations
{
    /// <summary>
    /// Class AddBirthDateToUser.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Migration(4)]
    public class AddBirthDateToUser : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Alter.Table("Bar")
                .AddColumn("SomeDate")
                .AsDateTime()
                .Nullable();

            Update.Table("Bar")
                .Set(new { SomeDate = DateTime.Today })
                .AllRows();

            Alter.Table("Bar")
                .AlterColumn("SomeDate")
                .AsDateTime()
                .NotNullable();
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down()
        {
            Delete.Column("SomeDate")
                .FromTable("Bar");
        }
    }
}
