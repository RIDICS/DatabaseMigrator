namespace Ridics.DatabaseMigrator.QueryBuilder.Delete
{
    public interface IQueryBuilderDelete
    {
        IQueryBuilderWhere DeleteFrom(string tableName);
    }
}