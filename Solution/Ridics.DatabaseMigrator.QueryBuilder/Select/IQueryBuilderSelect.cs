namespace Ridics.DatabaseMigrator.QueryBuilder.Select
{
    public interface IQueryBuilderSelect
    {
        IQueryBuilderFrom<T> Select<T>(string columnName);

        IQueryBuilderFrom<T> Select<T>() where T : class;
    }

    public interface IQueryBuilderSelect<T>
    {
        IQueryBuilderFrom<T> Select(string columnName);
    }
}
