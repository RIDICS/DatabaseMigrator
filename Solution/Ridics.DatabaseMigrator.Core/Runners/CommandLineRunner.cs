using System;
using System.Collections.Generic;
using FluentMigrator;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ridics.DatabaseMigrator.Core.Configuration;
using Ridics.DatabaseMigrator.Core.NHibernate.DatabaseTask;

namespace Ridics.DatabaseMigrator.Core.Runners
{
    public class CommandLineRunner : ApplicationRunnerBase
    {
        public const string NoInteractiveFlag = "--no-interactive";

        private readonly AppConfiguration m_appConfiguration;
        private readonly string[] m_args;

        public CommandLineRunner(AppConfiguration appConfiguration, string[] args) : base(appConfiguration, false)
        {
            m_appConfiguration = appConfiguration;
            m_args = args;
        }

        protected override int Run()
        {
            var app = new CommandLineApplication();

            app.HelpOption("-?|-h|--help");

            var noInteractive = app.Option(
                NoInteractiveFlag,
                "Perform run without user input (no wait for close)",
                CommandOptionType.NoValue);

            var extendTemplate = app.Option(
                "-e|--extendTemplate <TEMPLATE-NAME>",
                "Extend application template",
                CommandOptionType.SingleValue);

            var truncateDatabase = app.Option(
                "--truncateDatabase",
                "Remove all tables from database (available only with --extendTemplate argument)",
                CommandOptionType.NoValue);

            var processor = app.Option(
                "-d|--dialect <DIALECT>",
                $"The database dialect ({string.Join(",", m_appConfiguration.SupportedDialects)})",
                CommandOptionType.SingleValue);

            var connectionString = app.Option(
                "-c|--connection <CONNECTION-STRING>",
                "The connection string to connect to the database",
                CommandOptionType.SingleValue);

            var databaseTag = app.Option(
                "-t|--tag <DATABASE-TAG>",
                "The name of database to migrate",
                CommandOptionType.SingleValue);

            var migrationVersion = app.Option(
                "-mv|--migrationVersion <MIGRATION-VERSION>",
                "Version of database to migrate",
                CommandOptionType.SingleValue);

            var migrationDirection = app.Option(
                "-md|--migrationDirection <MIGRATION-DIRECTION>",
                "Direction of database migration",
                CommandOptionType.SingleValue);

            var transactionPerSession = app.Option(
                "-st|--sessionTransaction",
                "Use one transaction for whole session",
                CommandOptionType.NoValue);

            var createDatabaseIfNotExists = app.Option(
                "-cdb|--createDatabase",
                "Creates database if not exists. CREATE-DATABASE-CONNECTION-STRING must be specified",
                CommandOptionType.NoValue);

            var createDatabaseConnectionString = app.Option(
                "-cdbcs|--createDatabaseConnectionString <CREATE-DATABASE-CONNECTION-STRING>",
                "Direction of database migration",
                CommandOptionType.SingleValue);

            var environmentTag = app.Option(
                "-et|--environmentTag <ENVIRONMENT-TAG>",
                "The name of environment to migrate",
                CommandOptionType.SingleValue);

            var migrationTypeTag = app.Option(
                "-mtt|--migrationTypeTag <MIGRATION-TYPE-TAG>",
                "Type of migration to migrate",
                CommandOptionType.SingleValue);

            app.OnExecute(
                () =>
                {
                    var tagsConfig = GetTagsConfiguration(environmentTag, migrationTypeTag);

                    if (extendTemplate.HasValue())
                    {
                        var templateName = extendTemplate.Value();
                        var databaseConfigs = m_config.GetSection("DatabaseConfigurations")
                            .Get<IList<DatabaseConfiguration>>();

                        if (databaseConfigs != null && databaseConfigs.Count > 0)
                        {
                            foreach (var databaseConfiguration in databaseConfigs)
                            {
                                if (databaseConfiguration.Name != templateName)
                                {
                                    continue;
                                }

                                if (processor.HasValue())
                                    databaseConfiguration.DatabaseDialect = processor.Value();
                                if (connectionString.HasValue())
                                    databaseConfiguration.ConnectionString = connectionString.Value();
                                if (databaseTag.HasValue())
                                    databaseConfiguration.DatabaseTag = databaseTag.Value();
                                if (migrationDirection.HasValue())
                                    databaseConfiguration.MigrationDirection =
                                        ResolveMigrationDirection(migrationDirection.Value());
                                if (migrationVersion.HasValue())
                                    databaseConfiguration.MigrationVersion = migrationVersion.HasValue()
                                        ? long.Parse(migrationVersion.Value())
                                        : (long?) null;
                                if (transactionPerSession.HasValue())
                                    databaseConfiguration.TransactionPerSession = transactionPerSession.HasValue();
                                if (createDatabaseIfNotExists.HasValue())
                                    databaseConfiguration.CreateDatabaseIfNotExists =
                                        createDatabaseIfNotExists.HasValue();
                                if (createDatabaseConnectionString.HasValue())
                                    databaseConfiguration.CreateDatabaseConnectionString =
                                        createDatabaseConnectionString.HasValue()
                                            ? createDatabaseConnectionString.Value()
                                            : null;

                                if (truncateDatabase.HasValue())
                                {
                                    var serviceCollection = CreateContainer(databaseConfiguration, tagsConfig);
                                    var databaseDropper = serviceCollection.GetRequiredService<DatabaseDropper>();

                                    databaseDropper.DropDatabaseIfExists(m_logger);
                                }

                                RunMigration(databaseConfiguration, tagsConfig);
                            }
                        }
                    }
                    else
                    {
                        var databaseConfig = CreateDatabaseConfiguration(processor, connectionString, databaseTag,
                            migrationVersion,
                            migrationDirection, transactionPerSession, createDatabaseIfNotExists,
                            createDatabaseConnectionString);
                        RunMigration(databaseConfig, tagsConfig);
                    }

                    return CloseApplication(noInteractive.HasValue());
                });

            return app.Execute(m_args);
        }

        private DatabaseConfiguration CreateDatabaseConfiguration(CommandOption processor,
            CommandOption connectionString,
            CommandOption databaseTag, CommandOption migrationVersion, CommandOption migrationDirection,
            CommandOption transactionPerSession, CommandOption createDatabaseIfNotExists,
            CommandOption createDatabaseConnectionString)
        {
            if (connectionString.HasValue() && processor.HasValue() && databaseTag.HasValue())
            {
                return new DatabaseConfiguration
                {
                    DatabaseDialect = processor.Value(),
                    ConnectionString = connectionString.Value(),
                    DatabaseTag = databaseTag.Value(),
                    MigrationDirection = ResolveMigrationDirection(migrationDirection.Value()),
                    MigrationVersion = migrationVersion.HasValue() ? long.Parse(migrationVersion.Value()) : (long?) null,
                    TransactionPerSession = transactionPerSession.HasValue(),
                    CreateDatabaseIfNotExists = createDatabaseIfNotExists.HasValue(),
                    CreateDatabaseConnectionString =
                        createDatabaseConnectionString.HasValue() ? createDatabaseConnectionString.Value() : null
                };
            }

            throw new ArgumentException("Processor, connection string and database tag must be specified");
        }

        private MigrationDirection ResolveMigrationDirection(string direction)
        {
            return direction.Equals("down", StringComparison.InvariantCultureIgnoreCase)
                ? MigrationDirection.Down
                : MigrationDirection.Up;
        }

        private TagsConfiguration GetTagsConfiguration(CommandOption environmentTag, CommandOption migrationTypeTag)
        {
            var configuration = m_config.GetSection("TagsConfiguration").Get<TagsConfiguration>();

            if (environmentTag.HasValue())
            {
                configuration.EnvironmentTag = environmentTag.Value();
            }

            if (migrationTypeTag.HasValue())
            {
                configuration.MigrationTypeTag = migrationTypeTag.Value();
            }

            return configuration;
        }
    }
}
