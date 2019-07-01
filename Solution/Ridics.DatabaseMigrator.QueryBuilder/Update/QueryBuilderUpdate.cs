using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;
using Dapper;
using Ridics.DatabaseMigrator.QueryBuilder.Shared;

namespace Ridics.DatabaseMigrator.QueryBuilder.Update
{
    public class QueryBuilderUpdate : IQueryBuilderUpdate, IQueryBuilderSet, IQueryBuilderWhere
    {
        private readonly IDbConnection m_connection;
        private readonly IDbTransaction m_transaction;
        private readonly List<WhereStatement> m_whereStatements = new List<WhereStatement>();
        private readonly List<WhereStatementRelation> m_whereStatementRelations = new List<WhereStatementRelation>();
        private readonly List<SetStatement> m_setStatements = new List<SetStatement>();
        private string m_tableName;
        private readonly QueryStatementBuilder m_statementBuilder;

        public QueryBuilderUpdate(IDbConnection connection, IDbTransaction transaction)
        {
            m_connection = connection;
            m_transaction = transaction;
            m_statementBuilder = new QueryStatementBuilder();
        }

        public IQueryBuilderSet Update(string tableName)
        {
            m_tableName = tableName;
            return this;
        }

        public IQueryBuilderSet Set(string columnName, object value)
        {
            m_setStatements.Add(new SetStatement(columnName, value));
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
            var whereStatementQuery = CreateWhereStatement();

            var setStatementQuery = CreateSetStatement();

            var parameters = m_statementBuilder.ConcatStatementsParameters(setStatementQuery, whereStatementQuery);

            m_connection.Execute(
                m_statementBuilder.ConcatSqlStatements(CreateUpdateStatement(), setStatementQuery, whereStatementQuery),
                parameters, m_transaction);
        }

        private StatementQuery CreateUpdateStatement()
        {
            var sql = $@"UPDATE ""{m_tableName}""";
            return new StatementQuery(sql);
        }

        private StatementQuery CreateSetStatement()
        {
            var setStatementSqlBuilder = new StringBuilder();
            object parameters = new ExpandoObject();
            var parametersCollection = (ICollection<KeyValuePair<string, object>>) parameters;

            if (m_setStatements.Count > 0)
            {
                var i = 0;

                foreach (var setStatement in m_setStatements)
                {
                    setStatementSqlBuilder
                        .Append(setStatementSqlBuilder.Length == 0 ? "SET" : " ,")
                        .AppendFormat(@" ""{0}"" = @SetValue{1}", setStatement.ColumnName, i);

                    parametersCollection.Add(new KeyValuePair<string, object>($"SetValue{i}", setStatement.Value));
                    i++;
                }
            }

            var whereStatementSql = setStatementSqlBuilder.ToString();

            return new StatementQuery(whereStatementSql, parametersCollection);
        }

        private StatementQuery CreateWhereStatement()
        {
            return m_statementBuilder.BuildWhereStatementsQuery(m_whereStatements, m_whereStatementRelations);
        }
    }
}