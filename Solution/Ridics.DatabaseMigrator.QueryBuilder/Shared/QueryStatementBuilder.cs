using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Ridics.DatabaseMigrator.QueryBuilder.Shared
{
    public class QueryStatementBuilder
    {
        private readonly WhereStatementQueryBuilder m_whereQueryBuilder;

        public QueryStatementBuilder()
        {
            m_whereQueryBuilder = new WhereStatementQueryBuilder();
        }

        public StatementQuery BuildWhereStatementsQuery(IList<WhereStatement> whereStatements, IList<WhereStatementRelation> whereStatementRelations)
        {
            return m_whereQueryBuilder.BuildWhereStatementsQuery(whereStatements, whereStatementRelations);
        }

        private string ResolveOperator(WhereStatementOperator? statementOperator)
        {
            if (!statementOperator.HasValue)
            {
                throw new ArgumentException("Where statement operator not defined");
            }

            var operatorValue = statementOperator.Value;

            if (operatorValue == WhereStatementOperator.In)
            {
                return "IN";
            }

            if (operatorValue == WhereStatementOperator.GreaterThen)
            {
                return ">";
            }

            if (operatorValue == WhereStatementOperator.LessThen)
            {
                return "<";
            }

            if (operatorValue == WhereStatementOperator.GreaterThenOrEquals)
            {
                return ">=";
            }

            if (operatorValue == WhereStatementOperator.LessThenOrEquals)
            {
                return "<=";
            }

            if (operatorValue == WhereStatementOperator.NotEqual)
            {
                return "<>";
            }

            return "=";
        }

        public string ConcatSqlStatements(params string[] statements)
        {
            return string.Join(" ", statements);
        }

        public string ConcatSqlStatements(params StatementQuery[] statements)
        {
            return ConcatSqlStatements(statements.Select(x => x.Sql).ToArray());
        }

        public ICollection<KeyValuePair<string, object>> ConcatStatementsParameters(params StatementQuery[] statements)
        {
            object parameters = new ExpandoObject();
            var parametersCollection = (ICollection<KeyValuePair<string, object>>) parameters;

            foreach (var statementQuery in statements)
            {
                parametersCollection = parametersCollection.Concat(statementQuery.Parameters).ToList();
            }

            return parametersCollection;
        }
    }
}