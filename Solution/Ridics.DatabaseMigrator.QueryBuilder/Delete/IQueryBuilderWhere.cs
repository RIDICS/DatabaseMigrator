using Ridics.DatabaseMigrator.QueryBuilder.Shared;

namespace Ridics.DatabaseMigrator.QueryBuilder.Delete
{
    public interface IQueryBuilderWhere : IQueryBuilderRun
    {
        IQueryBuilderWhere Where(string columnName, WhereStatementNullableOperator statementOperator);

        IQueryBuilderWhere Where(string columnName, object value);

        IQueryBuilderWhere Where(string columnName, object value, WhereStatementOperator statementOperator);

        IQueryBuilderWhere Where(WhereStatementRelation statementRelation);
    }
}