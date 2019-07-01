using System.Collections.Generic;
using System.Data;
using Dapper;
using Ridics.DatabaseMigrator.QueryBuilder.Shared;

namespace Ridics.DatabaseMigrator.QueryBuilder.Delete
{
    public class QueryBuilderDelete : IQueryBuilderDelete, IQueryBuilderWhere
    {
        private readonly IDbConnection m_connection;
        private readonly IDbTransaction m_transaction;
        private readonly List<WhereStatement> m_whereStatements = new List<WhereStatement>();
        private readonly List<WhereStatementRelation> m_whereStatementRelations = new List<WhereStatementRelation>();
        private readonly QueryStatementBuilder m_statementBuilder;
        private string m_tableName;

        public QueryBuilderDelete(IDbConnection connection, IDbTransaction transaction)
        {
            m_connection = connection;
            m_transaction = transaction;
            m_statementBuilder = new QueryStatementBuilder();
        }

        public IQueryBuilderWhere DeleteFrom(string tableName)
        {
            m_tableName = tableName;
            return this;
        }

        public IQueryBuilderWhere Where(string columnName, WhereStatementNullableOperator statementOperator)
        {
            m_whereStatements.Add(new WhereStatement(columnName, statementOperator));
            return this;
        }

        public IQueryBuilderWhere Where(string columnName, object value)
        {
            m_whereStatements.Add(new WhereStatement(columnName, value));
            return this;
        }

        public IQueryBuilderWhere Where(string columnName, object value, WhereStatementOperator statementOperator)
        {
            m_whereStatements.Add(new WhereStatement(columnName, value, statementOperator));
            return this;
        }

        public IQueryBuilderWhere Where(WhereStatementRelation statementRelation)
        {
            m_whereStatementRelations.Add(statementRelation);
            return this;
        }

        public void Run()
        {
            var whereStatementQuery = m_statementBuilder.BuildWhereStatementsQuery(m_whereStatements, m_whereStatementRelations);

            m_connection.Execute(
                m_statementBuilder.ConcatSqlStatements(CreateDeleteStatement(), whereStatementQuery),
                whereStatementQuery.Parameters,
                m_transaction);
        }

        private StatementQuery CreateDeleteStatement()
        {
            var sql = $@"DELETE FROM ""{m_tableName}""";
            return new StatementQuery(sql);
        }
    }
}
