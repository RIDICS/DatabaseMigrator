﻿using System.Collections.Generic;
using System.Linq;
using Ridics.DatabaseMigrator.Core;
using Ridics.DatabaseMigrator.Core.Configuration;
using Ridics.DatabaseMigrator.Core.Runners;
using Ridics.DatabaseMigrator.SampleApp.Migrations.TagTypes;

namespace Ridics.DatabaseMigrator.SampleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            var migrateNoInteractive = false;
            var appConfiguration = new AppConfiguration
            {
                SupportedDialects = new List<string>
                {
                    MigratorDatabaseDialect.Sqlite,
                },
                MigrationAssemblies = new[]
                {
                    typeof(DatabaseTagTypes).Assembly,
                }
            };

            IApplicationRunner applicationRunner = new CommandLineRunner(appConfiguration, args);

            if (args != null && args.Contains(CommandLineRunner.NoInteractiveFlag))
            {
                migrateNoInteractive = true;

                args = args.Where(x => !x.Contains(CommandLineRunner.NoInteractiveFlag)).ToArray();
            }

            if (args == null || args.Length == 0)
            {
                applicationRunner = new ConfigurationRunner(appConfiguration, migrateNoInteractive);
            }

            return applicationRunner.RunApplication();
        }
    }
}
