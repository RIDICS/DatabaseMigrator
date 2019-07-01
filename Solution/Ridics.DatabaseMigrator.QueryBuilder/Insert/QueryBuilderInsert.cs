using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace Ridics.DatabaseMigrator.QueryBuilder.Insert
{
    public class QueryBuilderInsert : IQueryBuilderInsert, IQueryBuilderRow
    {
        private readonly IDbConnection m_connection;
        private readonly IDbTransaction m_transaction;
        private readonly List<object> m_rows = new List<object>();
        private string m_tableName;

        public QueryBuilderInsert(IDbConnection connection, IDbTransaction transaction)
        {
            m_connection = connection;
            m_transaction = transaction;
        }

        public IQueryBuilderRow Insert(string tableName)
        {
            m_tableName = tableName;
            return this;
        }

        public IQueryBuilderRow Row(object values)
        {
            m_rows.Add(values);
            return this;
        }

        public void RunAlsoIfEmpty()
        {
            if (m_rows.Count > 0)
            {
                Run();
            }
        }

        public void Run()
        {
            if (m_rows.Count == 0)
            {
                throw new ArgumentException("Row to insert is not defined");
            }

            var query = string.Format("{0}{1}", InsertStatement, ValuesStatement);

            m_connection.Execute(query, m_rows, m_transaction);
        }

        private string InsertStatement => string.Format(@"INSERT INTO ""{0}""", m_tableName);

        private string ValuesStatement
        {
            get
            {
                var insertColumnsStatement = string.Join(", ", ColumnNames.ConvertAll(x => string.Format(@"""{0}""", x)));
                var insertValuesStatement = string.Join(", ", ColumnNames.ConvertAll(x => string.Format(@"@{0}", x)));

                return string.Format(@" ({0}) VALUES ({1})", insertColumnsStatement, insertValuesStatement);
            }
        }

        private List<string> ColumnNames
        {
            get
            {
                var properties = m_rows.First().GetType().GetProperties();
                return properties.Select(x => x.Name).ToList();
            }
        }
    }
}