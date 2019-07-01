namespace Ridics.DatabaseMigrator.QueryBuilder.Insert
{
    public interface IQueryBuilderRow : IQueryBuilderRun
    {
        IQueryBuilderRow Row(object values);
        void RunAlsoIfEmpty();
    }
}