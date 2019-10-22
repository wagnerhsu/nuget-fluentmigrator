// ***********************************************************************
// Assembly         : FluentMigrator.Runner.SqlServer
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="SqlServer2005TypeMap.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Data;

namespace FluentMigrator.Runner.Generators.SqlServer
{
    /// <summary>
    /// Class SqlServer2005TypeMap.
    /// Implements the <see cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000TypeMap" />
    /// </summary>
    /// <seealso cref="FluentMigrator.Runner.Generators.SqlServer.SqlServer2000TypeMap" />
    public class SqlServer2005TypeMap : SqlServer2000TypeMap
    {
        /// <summary>
        /// Setups the type maps.
        /// </summary>
        protected override void SetupTypeMaps()
        {
            base.SetupTypeMaps();

            // Officially this is 1073741823 but we will allow the int.MaxValue Convention
            SetTypeMap(DbType.String, "NVARCHAR(MAX)", int.MaxValue);
            SetTypeMap(DbType.AnsiString, "VARCHAR(MAX)", AnsiTextCapacity);
            SetTypeMap(DbType.Binary, "VARBINARY(MAX)", ImageCapacity);

            SetTypeMap(DbType.Xml, "XML");
        }
    }
}
