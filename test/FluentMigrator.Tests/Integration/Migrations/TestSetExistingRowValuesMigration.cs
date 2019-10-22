// ***********************************************************************
// Assembly         : FluentMigrator.Tests
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="TestSetExistingRowValuesMigration.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace FluentMigrator.Tests.Integration.Migrations
{
    /// <summary>
    /// Class AddLastLoginDateToUser.
    /// Implements the <see cref="FluentMigrator.Migration" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Migration" />
    [Migration(5)]
   public class AddLastLoginDateToUser : Migration
   {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
      {
         Alter.Table("Bar")
             .AddColumn("LastLoginDate")
             .AsDateTime()
             .NotNullable()
             .SetExistingRowsTo(DateTime.Today);
      }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public override void Down()
      {
         Delete.Column("LastLoginDate")
             .FromTable("Bar");
      }
   }
}
