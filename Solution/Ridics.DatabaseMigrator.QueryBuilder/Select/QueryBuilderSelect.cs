using System.Collections.Generic;
using System.Data;
using Dapper;
using Ridics.DatabaseMigrator.QueryBuilder.Shared;

namespace Ridics.DatabaseMigrator.QueryBuilder.Select
{
    public class QueryBuilderSelect<T> : IQueryBuilderSelect<T>, IQueryBuilderFrom<T>, IQueryBuilderWhere<T>
    {
        private readonly IDbConnection m_connection;
        private readonly IDbTransaction m_transaction;
        private string m_selectColumnName;
        private string m_tableName;
        private readonly List<WhereStatement> m_whereStatements = new List<WhereStatement>();
        private readonly List<WhereStatementRelation> m_whereStatementRelations = new List<WhereStatementRelation>();
        private readonly QueryStatementBuilder m_statementBuilder;

        public QueryBuilderSelect(IDbConnection connection, IDbTransaction transaction)
        {
            m_connection = connection;
            m_transaction = transaction;
            m_statementBuilder = new QueryStatementBuilder();
        }

        public IQueryBuilderFrom<T> Select(string columnName)
        {
            m_selectColumnName = columnName;
            return this;
        }

        public IQueryBuilderWhere<T> From(string tableName)
        {
            m_tableName = tableName;
            return this;
        }

        public IQueryBuilderWhere<T> Where(string columnName, WhereStatementNullableOperator statementOperator)
        {
            m_whereStatements.Add(new WhereStatement(columnName, statementOperator));
            return this;
        }

        public IQueryBuilderWhere<T> Where(string columnName, object value)
        {
            m_whereStatements.Add(new WhereStatement(columnName, value));
            return this;
        }

        public IQueryBuilderWhere<T> Where(string columnName, object value, WhereStatementOperator statementOperator)
        {
            m_whereStatements.Add(new WhereStatement(columnName, value, statementOperator));
            return this;
        }

        public IQueryBuilderWhere<T> Where(WhereStatementRelation statementRelation)
        {
            m_whereStatementRelations.Add(statementRelation);
            return this;
        }

        public IEnumerable<T> Run()
        {
            string escapedSelect;
            if (m_selectColumnName == null)
            {
                escapedSelect = "*";
            }
            else
            {
                escapedSelect = $@"""{m_selectColumnName}""";
            }

            var selectStatementSql = string.Format(@"SELECT {0} FROM ""{1}""", escapedSelect, m_tableName);

            var whereStatementQuery = m_statementBuilder.BuildWhereStatementsQuery(m_whereStatements, m_whereStatementRelations);

            return m_connection.Query<T>(
                m_statementBuilder.ConcatSqlStatements(selectStatementSql, whereStatementQuery.Sql),
                whereStatementQuery.Parameters, m_transaction);
        }
    }
}
