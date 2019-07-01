namespace Ridics.DatabaseMigrator.QueryBuilder.Select
{
    public interface IQueryBuilderFrom<T>
    {
        IQueryBuilderWhere<T> From(string tableName);
    }
}
