using System.Data;

namespace Ridics.DatabaseMigrator.QueryBuilder
{
    public static class Query
    {
        public static IQueryBuilder Conn(IDbConnection connection, IDbTransaction transaction)
        {
            return new QueryBuilder(connection, transaction);
        }
    }
}
