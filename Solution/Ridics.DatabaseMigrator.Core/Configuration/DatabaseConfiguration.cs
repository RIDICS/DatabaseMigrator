using FluentMigrator;

namespace Ridics.DatabaseMigrator.Core.Configuration
{
    public class DatabaseConfiguration
    {
        public string Name { get; set; }
        
        public string DatabaseDialect { get; set; }

        public string DatabaseTag { get; set; }

        public string ConnectionString { get; set; }

        public long? MigrationVersion { get; set; }

        public MigrationDirection MigrationDirection { get; set; }

        public bool TransactionPerSession { get; set; }

        public bool SkipConfiguration { get; set; }

        public bool CreateDatabaseIfNotExists { get; set; }

        public string CreateDatabaseConnectionString { get; set; }
    }
}