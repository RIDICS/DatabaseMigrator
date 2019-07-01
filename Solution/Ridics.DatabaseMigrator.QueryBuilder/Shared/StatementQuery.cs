using System.Collections.Generic;
using System.Dynamic;

namespace Ridics.DatabaseMigrator.QueryBuilder.Shared
{
    public class StatementQuery
    {
        public string Sql { get; }

        public ICollection<KeyValuePair<string, object>> Parameters { get; }

        public StatementQuery(string sql, ICollection<KeyValuePair<string, object>> parameters)
        {
            Sql = sql;
            Parameters = parameters;
        }

        public StatementQuery(string sql)
        {
            Sql = sql;
            Parameters = new ExpandoObject();
        }
    }
}