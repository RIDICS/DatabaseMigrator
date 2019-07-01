namespace Ridics.DatabaseMigrator.QueryBuilder.Insert
{
    public interface IQueryBuilderInsert
    {
        IQueryBuilderRow Insert(string tableName);
    }
}