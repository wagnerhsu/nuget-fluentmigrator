// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Core
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DataReaderExtensions.cs" company="FluentMigrator Project">
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

using System.Data;

namespace FluentMigrator.Runner.Processors
{
    /// <summary>
    /// Class DataReaderExtensions.
    /// </summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Reads the data set.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>DataSet.</returns>
        public static DataSet ReadDataSet(this IDataReader reader)
        {
            var result = new DataSet();
            do
            {
                result.Tables.Add(reader.ReadTable());
            } while (reader.NextResult());

            return result;
        }

        /// <summary>
        /// Reads the table.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>DataTable.</returns>
        public static DataTable ReadTable(this IDataReader reader)
        {
            var table = new DataTable();

            if (reader.Read())
            {
                reader.CreateColumns(table);

                var values = new object[reader.FieldCount];

                do
                {
                    reader.GetValues(values);
                    table.Rows.Add(values);
                } while (reader.Read());
            }

            return table;
        }

        /// <summary>
        /// Creates the columns.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="table">The table.</param>
        private static void CreateColumns(this IDataReader reader, DataTable table)
        {
            for (var i = 0; i != reader.FieldCount; ++i)
            {
                table.Columns.Add(reader.CreateColumn(i));
            }
        }

        /// <summary>
        /// Creates the column.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fieldIndex">Index of the field.</param>
        /// <returns>DataColumn.</returns>
        private static DataColumn CreateColumn(this IDataReader reader, int fieldIndex)
        {
            var fieldName = reader.GetName(fieldIndex);
            var fieldType = reader.GetFieldType(fieldIndex);
            return new DataColumn(fieldName, fieldType);
        }
    }
}
