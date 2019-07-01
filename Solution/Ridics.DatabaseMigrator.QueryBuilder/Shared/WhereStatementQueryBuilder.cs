using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Ridics.DatabaseMigrator.QueryBuilder.Shared
{
    public class WhereStatementQueryBuilder
    {
        public StatementQuery BuildWhereStatementsQuery(
            IList<WhereStatement> whereStatements,
            IList<WhereStatementRelation> whereStatementRelations
        )
        {
            if (whereStatements.Count == 0 && whereStatementRelations.Count == 0)
            {
                return new StatementQuery(string.Empty);
            }

            var whereStatementSqlBuilder = new StringBuilder();
            whereStatementSqlBuilder.Append("Where");
            var queryParamIndex = 0;

            var whereStatementsQueryPart = BuildWhereStatements(whereStatements, queryParamIndex, StatementRelationOperator.And);
            var relationStatementsQueryPart =
                BuildWhereStatementRelations(whereStatementRelations, whereStatementsQueryPart.QueryParamIndex,
                    StatementRelationOperator.And);

            var mergedQueryPart = MergeQueryParts(whereStatementsQueryPart.StatementQueryPart,
                relationStatementsQueryPart.StatementQueryPart,
                StatementRelationOperator.And);

            whereStatementSqlBuilder.Append(mergedQueryPart.Sql);

            return new StatementQuery(whereStatementSqlBuilder.ToString(), mergedQueryPart.Parameters);
        }

        private StatementQuery MergeQueryParts(
            StatementQuery statementQueryPart,
            StatementQuery statementRelationsQueryPart,
            StatementRelationOperator statementRelationOperator
        )
        {
            if (statementQueryPart == null && statementRelationsQueryPart == null)
            {
                throw new ArgumentException("Both statement query parts can not be null");
            }

            if (statementQueryPart == null)
            {
                return statementRelationsQueryPart;
            }

            if (statementRelationsQueryPart == null)
            {
                return statementQueryPart;
            }

            var sql = $"({statementQueryPart.Sql}) {ResolveRelation(statementRelationOperator)} ({statementRelationsQueryPart.Sql})";
            var parameters = statementQueryPart.Parameters.Concat(statementRelationsQueryPart.Parameters).ToList();
            return new StatementQuery(sql, parameters);
        }

        private StatementQueryBuildPart BuildWhereStatementRelations(
            IList<WhereStatementRelation> whereStatementRelations,
            int queryParamIndex,
            StatementRelationOperator relationOperator
        )
        {
            var paramIndex = queryParamIndex;

            StatementQuery mergedQueryPart = null;

            foreach (var whereStatementRelation in whereStatementRelations)
            {
                var whereStatementsQueryPart = BuildWhereStatements(whereStatementRelation.WhereStatements, paramIndex,
                    whereStatementRelation.RelationOperator);
                var relationStatementsQueryPart =
                    BuildWhereStatementRelations(whereStatementRelation.WhereStatementRelations, whereStatementsQueryPart.QueryParamIndex,
                        whereStatementRelation.RelationOperator);

                var itemMergedQueryPart = MergeQueryParts(whereStatementsQueryPart.StatementQueryPart,
                    relationStatementsQueryPart.StatementQueryPart,
                    whereStatementRelation.RelationOperator);

                paramIndex = relationStatementsQueryPart.QueryParamIndex;

                mergedQueryPart = MergeQueryParts(mergedQueryPart, itemMergedQueryPart, relationOperator);
            }

            return new StatementQueryBuildPart
            {
                StatementQueryPart = mergedQueryPart,
                QueryParamIndex = paramIndex
            };
        }

        private StatementQueryBuildPart BuildWhereStatements(
            ICollection<WhereStatement> whereStatements,
            int queryParamIndex,
            StatementRelationOperator statementRelationOperator
        )
        {
            if (whereStatements.Count <= 0)
            {
                return new StatementQueryBuildPart
                {
                    StatementQueryPart = null,
                    QueryParamIndex = queryParamIndex
                };
            }

            var parameters = new ExpandoObject();
            var parametersCollection = (ICollection<KeyValuePair<string, object>>) parameters;
            var whereStatementSqlBuilder = new StringBuilder();

            var firstStatement = true;

            foreach (var whereStatement in whereStatements)
            {
                if (firstStatement)
                {
                    firstStatement = false;
                }
                else
                {
                    whereStatementSqlBuilder.AppendFormat(@" {0}", ResolveRelation(statementRelationOperator));
                }

                if (whereStatement.NullableStatementOperator.HasValue)
                {
                    whereStatementSqlBuilder.AppendFormat(@" ""{0}"" {1}", whereStatement.ColumnName,
                        ResolveNullable(whereStatement.NullableStatementOperator.Value));
                }
                else
                {
                    if (!whereStatement.StatementOperator.HasValue)
                    {
                        throw new ArgumentException("Where statement operator not defined");
                    }

                    if (whereStatement.StatementOperator == WhereStatementOperator.In)
                    {
                        var arrayString = $"('{string.Join("','", (object[]) whereStatement.Value)}')";

                        whereStatementSqlBuilder.AppendFormat(@" ""{0}"" {1} {2}", whereStatement.ColumnName,
                            ResolveOperator(whereStatement.StatementOperator.Value), arrayString);
                    }
                    else
                    {
                        whereStatementSqlBuilder.AppendFormat(@" ""{0}"" {1} @WhereValue{2}", whereStatement.ColumnName,
                            ResolveOperator(whereStatement.StatementOperator.Value), queryParamIndex);

                        parametersCollection.Add(new KeyValuePair<string, object>($"WhereValue{queryParamIndex}",
                            whereStatement.Value));

                        queryParamIndex++;
                    }
                }
            }

            return new StatementQueryBuildPart
            {
                QueryParamIndex = queryParamIndex,
                StatementQueryPart = new StatementQuery(whereStatementSqlBuilder.ToString(), parametersCollection)
            };
        }

        private string ResolveOperator(WhereStatementOperator statementOperator)
        {
            switch (statementOperator)
            {
                case WhereStatementOperator.In:
                    return "IN";
                case WhereStatementOperator.GreaterThen:
                    return ">";
                case WhereStatementOperator.LessThen:
                    return "<";
                case WhereStatementOperator.GreaterThenOrEquals:
                    return ">=";
                case WhereStatementOperator.LessThenOrEquals:
                    return "<=";
                case WhereStatementOperator.NotEqual:
                    return "<>";
                default:
                    return "=";
            }
        }

        private string ResolveRelation(StatementRelationOperator statementRelation)
        {
            return statementRelation == StatementRelationOperator.Or ? "OR" : "AND";
        }

        private string ResolveNullable(WhereStatementNullableOperator nullableOperator)
        {
            return nullableOperator == WhereStatementNullableOperator.Null ? "IS NULL" : "IS NOT NULL";
        }
    }
}