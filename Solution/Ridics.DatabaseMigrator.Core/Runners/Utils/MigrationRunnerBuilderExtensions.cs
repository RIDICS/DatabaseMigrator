using System.Collections.Generic;
using FluentMigrator.Runner;

namespace Ridics.DatabaseMigrator.Core.Runners.Utils
{
    public static class MigrationRunnerBuilderExtensions
    {
        public static IMigrationRunnerBuilder AddSupportedDatabases(this IMigrationRunnerBuilder builder, IEnumerable<string> supportedDialects)
        {
            foreach (var supportedDialect in supportedDialects)
            {
                switch (supportedDialect)
                {
                    case MigratorDatabaseDialect.DB2:
                        builder = builder.AddDb2();
                        break;
                    case MigratorDatabaseDialect.Firebird:
                        builder = builder.AddFirebird();
                        break;
                    case MigratorDatabaseDialect.Hana:
                        builder = builder.AddHana();
                        break;
                    case MigratorDatabaseDialect.MySql:
                        builder = builder.AddMySql5();
                        break;
                    case MigratorDatabaseDialect.Oracle:
                        builder = builder.AddOracle();
                        break;
                    case MigratorDatabaseDialect.Postgres:
                        builder = builder.AddPostgres();
                        break;
                    case MigratorDatabaseDialect.Redshift:
                        builder = builder.AddRedshift();
                        break;
                    case MigratorDatabaseDialect.SqlAnywhere:
                        builder = builder.AddSqlAnywhere();
                        break;
                    case MigratorDatabaseDialect.SqlServer:
                        builder = builder.AddSqlServer();
                        break;
                    case MigratorDatabaseDialect.Sqlite:
                        builder = builder.AddSQLite();
                        break;
                }
            }
            
            return builder;
        }
    }
}
