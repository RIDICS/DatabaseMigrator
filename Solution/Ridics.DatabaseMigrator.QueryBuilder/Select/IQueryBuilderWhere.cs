using Ridics.DatabaseMigrator.QueryBuilder.Shared;

namespace Ridics.DatabaseMigrator.QueryBuilder.Select
{
    public interface IQueryBuilderWhere<T> : IQueryBuilderRun<T>
    {
        IQueryBuilderWhere<T> Where(string columnName, WhereStatementNullableOperator statementOperator);

        IQueryBuilderWhere<T> Where(string columnName, object value);

        IQueryBuilderWhere<T> Where(string columnName, object value, WhereStatementOperator statementOperator);

        IQueryBuilderWhere<T> Where(WhereStatementRelation statementRelation);
    }
}
