// ***********************************************************************
// Assembly         : FluentMigrator
// Author           : eivin
// Created          : 10-10-2019
//
// Last Modified By : eivin
// Last Modified On : 10-10-2019
// ***********************************************************************
// <copyright file="DeleteDefaultConstraintExpressionBuilder.cs" company="FluentMigrator Project">
//     Sean Chambers and the FluentMigrator project 2008-2018
// </copyright>
// <summary></summary>
// ***********************************************************************
using FluentMigrator.Expressions;

namespace FluentMigrator.Builders.Delete.DefaultConstraint
{
    /// <summary>
    /// An expression builder for a <see cref="DeleteDefaultConstraintExpression" />
    /// </summary>
    public class DeleteDefaultConstraintExpressionBuilder : ExpressionBuilderBase<DeleteDefaultConstraintExpression>,
                                                            IDeleteDefaultConstraintOnTableSyntax,
                                                            IDeleteDefaultConstraintOnColumnOrInSchemaSyntax
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteDefaultConstraintExpressionBuilder" /> class.
        /// </summary>
        /// <param name="expression">The underlying expression</param>
        public DeleteDefaultConstraintExpressionBuilder(DeleteDefaultConstraintExpression expression) : base(expression)
        {
        }

        /// <inheritdoc />
        public IDeleteDefaultConstraintOnColumnOrInSchemaSyntax OnTable(string tableName)
        {
            Expression.TableName = tableName;
            return this;
        }

        /// <inheritdoc />
        public void OnColumn(string columnName)
        {
            Expression.ColumnName = columnName;
        }

        /// <inheritdoc />
        public IDeleteDefaultConstraintOnColumnSyntax InSchema(string schemaName)
        {
            Expression.SchemaName = schemaName;
            return this;
        }
    }
}
