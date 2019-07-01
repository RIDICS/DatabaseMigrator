using Ridics.DatabaseMigrator.QueryBuilder.Delete;
using Ridics.DatabaseMigrator.QueryBuilder.Insert;
using Ridics.DatabaseMigrator.QueryBuilder.Select;
using Ridics.DatabaseMigrator.QueryBuilder.Update;

namespace Ridics.DatabaseMigrator.QueryBuilder
{
    public interface IQueryBuilder : IQueryBuilderSelect, IQueryBuilderInsert, IQueryBuilderUpdate, IQueryBuilderDelete
    {
    }
}
