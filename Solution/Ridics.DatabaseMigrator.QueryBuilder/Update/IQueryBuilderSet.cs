namespace Ridics.DatabaseMigrator.QueryBuilder.Update
{
    public interface IQueryBuilderSet : IQueryBuilderRun, IQueryBuilderWhere
    {
        IQueryBuilderSet Set(string columnName, object value);
    }
}