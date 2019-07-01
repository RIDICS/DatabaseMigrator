using System.Collections.Generic;

namespace Ridics.DatabaseMigrator.QueryBuilder.Select
{
    public interface IQueryBuilderRun<T>
    {
        IEnumerable<T> Run();
    }
}
