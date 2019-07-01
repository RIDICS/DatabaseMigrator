using System.Data;
using Ridics.DatabaseMigrator.QueryBuilder.Delete;
using Ridics.DatabaseMigrator.QueryBuilder.Insert;
using Ridics.DatabaseMigrator.QueryBuilder.Select;
using Ridics.DatabaseMigrator.QueryBuilder.Update;

namespace Ridics.DatabaseMigrator.QueryBuilder
{
    public class QueryBuilder : IQueryBuilder
    {
        private readonly IDbConnection m_connection;
        private readonly IDbTransaction m_transaction;

        public QueryBuilder(IDbConnection connection, IDbTransaction transaction)
        {
            m_connection = connection;
            m_transaction = transaction;
        }

        public IQueryBuilderFrom<T> Select<T>(string columnName)
        {
            var queryBuilder = new QueryBuilderSelect<T>(m_connection, m_transaction);
            queryBuilder.Select(columnName);
            return queryBuilder;
        }

        public IQueryBuilderFrom<T> Select<T>() where T : class
        {
            var queryBuilder = new QueryBuilderSelect<T>(m_connection, m_transaction);

            return queryBuilder;
        }

        public IQueryBuilderRow Insert(string tableName)
        {
            var queryBuilder = new QueryBuilderInsert(m_connection, m_transaction);
            queryBuilder.Insert(tableName);
            return queryBuilder;
        }

        public IQueryBuilderSet Update(string tableName)
        {
            var queryBuilder = new QueryBuilderUpdate(m_connection, m_transaction);
            queryBuilder.Update(tableName);
            return queryBuilder;
        }

        public Delete.IQueryBuilderWhere DeleteFrom(string tableName)
        {
            var queryBuilder = new QueryBuilderDelete(m_connection, m_transaction);
            queryBuilder.DeleteFrom(tableName);
            return queryBuilder;
        }
    }
}
