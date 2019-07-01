namespace Ridics.DatabaseMigrator.QueryBuilder.Update
{
    public interface IQueryBuilderUpdate
    {
        IQueryBuilderSet Update(string tableName);
    }
}