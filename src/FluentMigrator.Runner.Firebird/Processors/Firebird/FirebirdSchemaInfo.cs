// ***********************************************************************
// Assembly         : FluentMigrator.Runner.Firebird
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="FirebirdSchemaInfo.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;

using FluentMigrator.Runner.Generators.Firebird;

namespace FluentMigrator.Runner.Processors.Firebird
{
    /// <summary>
    /// Class AdoHelper.
    /// </summary>
    public static class AdoHelper
    {
        /// <summary>
        /// Gets the int value.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        public static int? GetIntValue(object val)
        {
            if (val == DBNull.Value)
                return null;
            return int.Parse(val.ToString());
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>System.String.</returns>
        public static string GetStringValue(object val)
        {
            return val.ToString();
        }

        /// <summary>
        /// Formats the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string FormatValue(string value)
        {
            return value.Replace(@"'", @"''");
        }

    }

    /// <summary>
    /// Class TableInfo. This class cannot be inherited.
    /// </summary>
    public sealed class TableInfo
    {
        /// <summary>
        /// The query
        /// </summary>
        private static readonly string query = "select rdb$relation_name from rdb$relations where lower(rdb$relation_name) = lower('{0}')";

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="TableInfo"/> is exists.
        /// </summary>
        /// <value><c>true</c> if exists; otherwise, <c>false</c>.</value>
        public bool Exists { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="drMeta">The dr meta.</param>
        public TableInfo(DataRow drMeta)
            : this(drMeta["rdb$relation_name"].ToString().Trim(), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="exists">if set to <c>true</c> [exists].</param>
        public TableInfo(string name, bool exists)
        {
            Name = name;
            Exists = exists;
        }

        /// <summary>
        /// Reads the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="quoter">The quoter.</param>
        /// <returns>TableInfo.</returns>
        public static TableInfo Read(FirebirdProcessor processor, string tableName, FirebirdQuoter quoter)
        {
            var fbTableName = quoter.ToFbObjectName(tableName);
            var table = processor.Read(query, AdoHelper.FormatValue(fbTableName)).Tables[0];
            if (table.Rows.Count == 0)
                return new TableInfo(tableName, false);
            return new TableInfo(table.Rows[0]);
        }
    }

    /// <summary>
    /// Class ColumnInfo. This class cannot be inherited.
    /// </summary>
    public sealed class ColumnInfo
    {
        /// <summary>
        /// The query
        /// </summary>
        private static readonly string query = @"select
                    fields.rdb$field_name as field_name,
                    fields.rdb$relation_name as relation_name,
                    fields.rdb$default_source as default_source,
                    fields.rdb$field_position as field_position,
                    fields.rdb$null_flag as null_flag,
                    fieldinfo.rdb$field_precision as field_precision,
                    fieldinfo.rdb$character_length as field_character_length,
                    fieldinfo.rdb$field_type as field_type,
                    fieldinfo.rdb$field_sub_type as field_sub_type,
                    fieldtype.rdb$type_name as field_type_name

                    from rdb$relation_fields as fields
                    left outer join rdb$fields as fieldinfo on (fields.rdb$field_source = fieldinfo.rdb$field_name)
                    left outer join rdb$types as fieldtype on ( (fieldinfo.rdb$field_type = fieldtype.rdb$type) and (fieldtype.rdb$field_name = 'RDB$FIELD_TYPE') )
                    where (lower(fields.rdb$relation_name) = lower('{0}'))
                    ";

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; }
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public object DefaultValue { get; }
        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public int Position { get; }
        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <value>The type of the database.</value>
        public DbType? DBType { get { return GetDBType(); } }
        /// <summary>
        /// Gets the type of the custom.
        /// </summary>
        /// <value>The type of the custom.</value>
        public string CustomType { get { return GetCustomDBType(); } }
        /// <summary>
        /// Gets a value indicating whether this instance is nullable.
        /// </summary>
        /// <value><c>true</c> if this instance is nullable; otherwise, <c>false</c>.</value>
        public bool IsNullable { get; }
        /// <summary>
        /// Gets the precision.
        /// </summary>
        /// <value>The precision.</value>
        public int? Precision { get; }
        /// <summary>
        /// Gets the length of the character.
        /// </summary>
        /// <value>The length of the character.</value>
        public int? CharacterLength { get; }
        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>The type of the field.</value>
        public int? FieldType { get; }
        /// <summary>
        /// Gets the type of the field sub.
        /// </summary>
        /// <value>The type of the field sub.</value>
        public int? FieldSubType { get; }
        /// <summary>
        /// Gets the name of the field type.
        /// </summary>
        /// <value>The name of the field type.</value>
        public string FieldTypeName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="drColumn">The dr column.</param>
        private ColumnInfo(DataRow drColumn)
        {
            Name = AdoHelper.GetStringValue(drColumn["field_name"]).Trim();
            TableName = AdoHelper.GetStringValue(drColumn["relation_name"]).Trim();
            DefaultValue = GetDefaultValue(drColumn["default_source"]);
            Position = AdoHelper.GetIntValue(drColumn["field_position"]) ?? 0;
            IsNullable = AdoHelper.GetIntValue(drColumn["null_flag"]) != 1;
            Precision = AdoHelper.GetIntValue(drColumn["field_precision"]);
            CharacterLength = AdoHelper.GetIntValue(drColumn["field_character_length"]);
            FieldType = AdoHelper.GetIntValue(drColumn["field_type"]);
            FieldSubType = AdoHelper.GetIntValue(drColumn["field_sub_type"]);
            FieldTypeName = AdoHelper.GetStringValue(drColumn["field_type_name"]).Trim();

        }
        /// <summary>
        /// Reads the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="table">The table.</param>
        /// <returns>List&lt;ColumnInfo&gt;.</returns>
        public static List<ColumnInfo> Read(FirebirdProcessor processor, TableInfo table)
        {
            using (DataSet ds = processor.Read(query, AdoHelper.FormatValue(table.Name)))
            {
                List<ColumnInfo> rows = new List<ColumnInfo>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    rows.Add(new ColumnInfo(dr));
                return rows;
            }
        }

        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <returns>System.Nullable&lt;DbType&gt;.</returns>
        private DbType? GetDBType()
        {
            return null;
        }

        /// <summary>
        /// Gets the type of the custom database.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetCustomDBType()
        {
            #region FieldTypes by number
            switch (FieldType)
            {
                case 261:
                    if (FieldSubType.HasValue)
                    {
                        return "BLOB sub_type " + FieldSubType.Value.ToString();
                    }
                    else
                        return "BLOB";
                case 14:
                    return "CHAR";
                case 40:
                    return "CSTRING";
                case 11:
                    return "D_FLOAT";
                case 27:
                    return "DOUBLE";
                case 10:
                    return "FLOAT";
                case 16:
                    return "BIGINT";
                case 8:
                    return "INTEGER";
                case 9:
                    return "QUAD";
                case 7:
                    return "SMALLINT";
                case 12:
                    return "DATE";
                case 13:
                    return "TIME";
                case 35:
                    return "TIMESTAMP";
                case 37:
                    return "VARCHAR(" + CharacterLength.ToString() + ")";
            }
            #endregion

            switch (FieldTypeName)
            {
                case "VARYING":
                    return "VARCHAR(" + CharacterLength.ToString() + ")";
                case "LONG":
                    return "INTEGER";
                case "INT64":
                    return "BIGINT";
            }
            return FieldTypeName;
        }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotSupportedException"></exception>
        private object GetDefaultValue(object val)
        {
            if (val == null)
                return DBNull.Value;

            string src = val.ToString().Trim();
            if (string.IsNullOrEmpty(src))
                return DBNull.Value;

            if (src.StartsWith("DEFAULT ", StringComparison.InvariantCultureIgnoreCase))
            {
                string value = src.Substring(8).Trim();
                if (value.StartsWith("'"))
                {
                    return value.TrimStart('\'').TrimEnd('\'');
                }
                else if (value.Equals("current_timestamp", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SystemMethods.CurrentDateTime;
                }
                else if (value.Equals("gen_uuid()", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SystemMethods.NewGuid;
                }
                else
                {
                    if (int.TryParse(value, out var res))
                        return res;

                }
            }
            throw new NotSupportedException(string.Format("Can't parse default value {0}", src));
        }
    }

    /// <summary>
    /// Class IndexInfo. This class cannot be inherited.
    /// </summary>
    public sealed class IndexInfo
    {
        /// <summary>
        /// The query
        /// </summary>
        private static readonly string query = @"select
                rdb$index_name, rdb$relation_name, rdb$unique_flag, rdb$index_type
                from rdb$indices where rdb$relation_name = '{0}'";
        /// <summary>
        /// The single query
        /// </summary>
        private static readonly string singleQuery = @"select
                rdb$index_name, rdb$relation_name, rdb$unique_flag, rdb$index_type
                from rdb$indices where rdb$index_name = '{0}'";
        /// <summary>
        /// The index field query
        /// </summary>
        private static readonly string indexFieldQuery = @"select rdb$field_name from rdb$index_segments where rdb$index_name = '{0}'";

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is unique.
        /// </summary>
        /// <value><c>true</c> if this instance is unique; otherwise, <c>false</c>.</value>
        public bool IsUnique { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is ascending.
        /// </summary>
        /// <value><c>true</c> if this instance is ascending; otherwise, <c>false</c>.</value>
        public bool IsAscending { get; }
        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<string> Columns { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="IndexInfo"/> class.
        /// </summary>
        /// <param name="drIndex">Index of the dr.</param>
        /// <param name="processor">The processor.</param>
        private IndexInfo(DataRow drIndex, FirebirdProcessor processor)
        {
            Name = drIndex["rdb$index_name"].ToString().Trim();
            TableName = drIndex["rdb$relation_name"].ToString().Trim();
            IsUnique = drIndex["rdb$unique_flag"].ToString().Trim() == "1";
            IsAscending = drIndex["rdb$index_type"].ToString().Trim() == "0";
            Columns = new List<string>();
            using (DataSet dsColumns = processor.Read(indexFieldQuery, AdoHelper.FormatValue(Name)))
            {
                foreach (DataRow indexColumn in dsColumns.Tables[0].Rows)
                    Columns.Add(indexColumn["rdb$field_name"].ToString().Trim());
            }
        }

        /// <summary>
        /// Reads the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="table">The table.</param>
        /// <returns>List&lt;IndexInfo&gt;.</returns>
        public static List<IndexInfo> Read(FirebirdProcessor processor, TableInfo table)
        {
            using (DataSet ds = processor.Read(query, AdoHelper.FormatValue(table.Name)))
            {
                List<IndexInfo> rows = new List<IndexInfo>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    rows.Add(new IndexInfo(dr, processor));
                return rows;
            }
        }

        /// <summary>
        /// Reads the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="indexName">Name of the index.</param>
        /// <returns>IndexInfo.</returns>
        public static IndexInfo Read(FirebirdProcessor processor, string indexName)
        {
            using (DataSet ds = processor.Read(singleQuery, AdoHelper.FormatValue(indexName)))
            {
                return new IndexInfo(ds.Tables[0].Rows[0], processor);
            }
        }
    }

    /// <summary>
    /// Class ConstraintInfo. This class cannot be inherited.
    /// </summary>
    public sealed class ConstraintInfo
    {
        /// <summary>
        /// The query
        /// </summary>
        private static readonly string query = @"select
                rdb$constraint_name, rdb$constraint_type, rdb$index_name
                from rdb$relation_constraints where rdb$relation_name = '{0}'";
        /// <summary>
        /// The col query
        /// </summary>
        private static readonly string colQuery = @"select
                rdb$const_name_uq, rdb$update_rule, rdb$delete_rule
                from rdb$ref_constraints where rdb$constraint_name = '{0}'";

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is primary key.
        /// </summary>
        /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
        public bool IsPrimaryKey { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is unique.
        /// </summary>
        /// <value><c>true</c> if this instance is unique; otherwise, <c>false</c>.</value>
        public bool IsUnique { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is not null.
        /// </summary>
        /// <value><c>true</c> if this instance is not null; otherwise, <c>false</c>.</value>
        public bool IsNotNull { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is foreign key.
        /// </summary>
        /// <value><c>true</c> if this instance is foreign key; otherwise, <c>false</c>.</value>
        public bool IsForeignKey { get; }
        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        /// <value>The name of the index.</value>
        public string IndexName { get; }
        /// <summary>
        /// Gets the index of the foreign.
        /// </summary>
        /// <value>The index of the foreign.</value>
        public IndexInfo ForeignIndex { get; }
        /// <summary>
        /// Gets the update rule.
        /// </summary>
        /// <value>The update rule.</value>
        public Rule UpdateRule { get; }
        /// <summary>
        /// Gets the delete rule.
        /// </summary>
        /// <value>The delete rule.</value>
        public Rule DeleteRule { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintInfo"/> class.
        /// </summary>
        /// <param name="drConstraint">The dr constraint.</param>
        /// <param name="processor">The processor.</param>
        private ConstraintInfo(DataRow drConstraint, FirebirdProcessor processor)
        {
            Name = drConstraint["rdb$constraint_name"].ToString().Trim();
            IsPrimaryKey = drConstraint["rdb$constraint_type"].ToString().Trim() == "PRIMARY KEY";
            IsNotNull = drConstraint["rdb$constraint_type"].ToString().Trim() == "NOT NULL";
            IsForeignKey = drConstraint["rdb$constraint_type"].ToString().Trim() == "FOREIGN KEY";
            IsUnique = drConstraint["rdb$constraint_type"].ToString().Trim() == "UNIQUE";
            IndexName = drConstraint["rdb$index_name"].ToString().Trim();

            if (IsForeignKey)
            {
                using (DataSet dsForeign = processor.Read(colQuery, AdoHelper.FormatValue(Name)))
                {
                    DataRow drForeign = dsForeign.Tables[0].Rows[0];
                    ForeignIndex = IndexInfo.Read(processor, IndexName);
                    UpdateRule = GetForeignRule(drForeign["rdb$update_rule"]);
                    DeleteRule = GetForeignRule(drForeign["rdb$delete_rule"]);
                }
            }
        }

        /// <summary>
        /// Gets the foreign rule.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns>Rule.</returns>
        private Rule GetForeignRule(object val)
        {
            if (val == null)
                return Rule.None;
            string ruleName = AdoHelper.GetStringValue(val);
            switch (ruleName)
            {
                case "CASCADE":
                    return Rule.Cascade;
                case "SET NULL":
                    return Rule.SetNull;
                case "SET DEFAULT":
                    return Rule.SetDefault;
                // ReSharper disable once RedundantCaseLabel
                case "RESTRICT":
                default:
                    return Rule.None;
            }
        }

        /// <summary>
        /// Reads the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="table">The table.</param>
        /// <returns>List&lt;ConstraintInfo&gt;.</returns>
        public static List<ConstraintInfo> Read(FirebirdProcessor processor, TableInfo table)
        {
            using (DataSet ds = processor.Read(query, AdoHelper.FormatValue(table.Name)))
            {
                List<ConstraintInfo> rows = new List<ConstraintInfo>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    rows.Add(new ConstraintInfo(dr, processor));
                return rows;
            }
        }
    }

    /// <summary>
    /// Enum TriggerEvent
    /// </summary>
    public enum TriggerEvent { Insert, Update, Delete }
    /// <summary>
    /// Class TriggerInfo. This class cannot be inherited.
    /// </summary>
    public sealed class TriggerInfo
    {
        /// <summary>
        /// The query
        /// </summary>
        private static readonly string query = @"select
                rdb$trigger_name, rdb$trigger_sequence, rdb$trigger_type, rdb$trigger_source
                from rdb$triggers where rdb$relation_name = '{0}'";

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Gets the sequence.
        /// </summary>
        /// <value>The sequence.</value>
        public int Sequence { get; }
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public int Type { get; }
        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>The body.</value>
        public string Body { get; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="TriggerInfo"/> is before.
        /// </summary>
        /// <value><c>true</c> if before; otherwise, <c>false</c>.</value>
        public bool Before { get { return Type % 2 == 1; } }
        /// <summary>
        /// Gets a value indicating whether [on insert].
        /// </summary>
        /// <value><c>true</c> if [on insert]; otherwise, <c>false</c>.</value>
        public bool OnInsert { get { return Type == 1 || Type == 2; } }
        /// <summary>
        /// Gets a value indicating whether [on update].
        /// </summary>
        /// <value><c>true</c> if [on update]; otherwise, <c>false</c>.</value>
        public bool OnUpdate { get { return Type == 3 || Type == 4; } }
        /// <summary>
        /// Gets a value indicating whether [on delete].
        /// </summary>
        /// <value><c>true</c> if [on delete]; otherwise, <c>false</c>.</value>
        public bool OnDelete { get { return Type == 5 || Type == 6; } }
        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <value>The event.</value>
        public TriggerEvent Event { get { return OnInsert ? TriggerEvent.Insert : OnUpdate ? TriggerEvent.Update : TriggerEvent.Delete; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerInfo"/> class.
        /// </summary>
        /// <param name="drTrigger">The dr trigger.</param>
        private TriggerInfo(DataRow drTrigger)
        {
            Name = drTrigger["rdb$trigger_name"].ToString().Trim();
            Sequence = AdoHelper.GetIntValue(drTrigger["rdb$trigger_sequence"]) ?? 0;
            Type = AdoHelper.GetIntValue(drTrigger["rdb$trigger_type"]) ?? 1;
            Body = drTrigger["rdb$trigger_source"].ToString().Trim();
        }

        /// <summary>
        /// Reads the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="table">The table.</param>
        /// <returns>List&lt;TriggerInfo&gt;.</returns>
        public static List<TriggerInfo> Read(FirebirdProcessor processor, TableInfo table)
        {
            using (DataSet ds = processor.Read(query, AdoHelper.FormatValue(table.Name)))
            {
                List<TriggerInfo> rows = new List<TriggerInfo>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                    rows.Add(new TriggerInfo(dr));
                return rows;
            }
        }
    }

    /// <summary>
    /// Class SequenceInfo. This class cannot be inherited.
    /// </summary>
    public sealed class SequenceInfo
    {
        /// <summary>
        /// The query
        /// </summary>
        private static readonly string query = @"select rdb$generator_name from rdb$generators where rdb$generator_name = '{0}'";
        /// <summary>
        /// The query value
        /// </summary>
        private static readonly string queryValue = "select gen_id(\"{0}\", 0) as gen_val from rdb$database";

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
        /// <summary>
        /// Gets the current value.
        /// </summary>
        /// <value>The current value.</value>
        public int CurrentValue{get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceInfo"/> class.
        /// </summary>
        /// <param name="drSequence">The dr sequence.</param>
        /// <param name="processor">The processor.</param>
        private SequenceInfo(DataRow drSequence, FirebirdProcessor processor)
        {
            Name = drSequence["rdb$generator_name"].ToString().Trim();
            using (DataSet ds = processor.Read(queryValue, Name))
            {
                CurrentValue = AdoHelper.GetIntValue(ds.Tables[0].Rows[0]["gen_val"]) ?? 0;
            }
        }

        /// <summary>
        /// Reads the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="sequenceName">Name of the sequence.</param>
        /// <param name="quoter">The quoter.</param>
        /// <returns>SequenceInfo.</returns>
        public static SequenceInfo Read(FirebirdProcessor processor, string sequenceName, FirebirdQuoter quoter)
        {
            var fbSequenceName = quoter.ToFbObjectName(sequenceName);
            using (DataSet ds = processor.Read(query, AdoHelper.FormatValue(fbSequenceName)))
            {
                return new SequenceInfo(ds.Tables[0].Rows[0], processor);
            }
        }
    }

}
