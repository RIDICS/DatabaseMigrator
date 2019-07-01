using System;
using System.Collections.Generic;
using System.IO;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using Ridics.DatabaseMigrator.Core.Configuration;
using Ridics.DatabaseMigrator.Core.NHibernate;
using Ridics.DatabaseMigrator.Core.NHibernate.DatabaseTask;
using Ridics.DatabaseMigrator.Core.Runners.Conventions;
using Ridics.DatabaseMigrator.Core.Runners.Utils;
using Ridics.DatabaseMigrator.Core.Validators;
using Ridics.DatabaseMigrator.Shared;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.DatabaseMigrator.Core.Runners
{
    public abstract class ApplicationRunnerBase : IApplicationRunner
    {
        private readonly bool m_forceNoInteractive;
        private readonly AppConfiguration m_appConfiguration;

        protected IConfigurationRoot m_config;
        private IMigrationRunner m_runner;
        private DatabaseCreator m_databaseCreator;
        protected ILogger<ApplicationRunnerBase> m_logger;

        protected ApplicationRunnerBase(AppConfiguration appConfiguration, bool forceNoInteractive)
        {
            m_forceNoInteractive = forceNoInteractive;
            m_appConfiguration = appConfiguration;
        }

        protected ServiceProvider CreateContainer(DatabaseConfiguration dbConfig, TagsConfiguration tagsConfiguration)
        {
            var pathsConfiguration = m_config.GetSection("PathsConfiguration").Get<PathConfiguration>();
            var databaseCreateConfiguration = m_config.GetSection("DatabaseCreateConfiguration").Get<DatabaseCreateConfiguration>();

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddNLog()
                .AddNHibernate(dbConfig)
                .AddSingleton(dbConfig)
                .AddSingleton(pathsConfiguration)
                .AddSingleton(databaseCreateConfiguration)
                .AddSingleton(typeof(DatabaseDropper))
                .AddFluentMigratorCore()
                .AddTransient<IScriptPathResolver, ScriptPathResolver>()
                .AddScoped<IMigrationRunner, ConstrainedMigrationRunner>()
                .AddScoped<IMigrationRunnerConventions, CustomMigrationRunnerConventions>()
                .AddScoped<TagValidatorBase, EnvironmentTagValidator>()
                .AddScoped<TagValidatorBase, DatabaseTagValidator>()
                .AddScoped<TagValidatorBase, MigrationTypeValidator>()
                .AddScoped<IVersionLoader>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<RunnerOptions>>();
                    var connAccessor = sp.GetRequiredService<IConnectionStringAccessor>();
                    var hasConnection = !string.IsNullOrEmpty(connAccessor.ConnectionString);
                    if (options.Value.NoConnection || !hasConnection)
                    {
                        return ActivatorUtilities.CreateInstance<ConnectionlessVersionLoader>(sp);
                    }

                    return ActivatorUtilities.CreateInstance<ValidatingVersionLoader>(sp);
                })
                .AddLogging(lb => lb.AddDebug().AddFluentMigratorConsole())
                .ConfigureRunner(builder =>
                    builder
                        .AddSupportedDatabases(m_appConfiguration.SupportedDialects)
                        .WithGlobalConnectionString(dbConfig.ConnectionString)
                        .ScanIn(m_appConfiguration.MigrationAssemblies)
                        .For.All())
                .Configure<SelectingProcessorAccessorOptions>(opt => opt.ProcessorId = dbConfig.DatabaseDialect)
                .Configure<RunnerOptions>(opt =>
                {
                    opt.Tags = GetTags(dbConfig, tagsConfiguration);
                    opt.TransactionPerSession = dbConfig.TransactionPerSession;
                });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });
            NLog.LogManager.LoadConfiguration("NLog.config");

            m_databaseCreator = serviceProvider.GetRequiredService<DatabaseCreator>();
            m_runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            m_logger = serviceProvider.GetRequiredService<ILogger<ApplicationRunnerBase>>();

            return serviceProvider;
        }

        protected void RunMigration(DatabaseConfiguration dbConfig, TagsConfiguration tagsConfiguration)
        {
            CreateContainer(dbConfig, tagsConfiguration);

            LogMigrationPerform(dbConfig);

            CreateDatabaseIfRequired(dbConfig);

            if (dbConfig.MigrationDirection == MigrationDirection.Down)
            {
                if (!dbConfig.MigrationVersion.HasValue)
                {
                    throw new ArgumentException("Migration down must have specified migration version");
                }

                m_runner.MigrateDown(dbConfig.MigrationVersion.Value);
            }
            else
            {
                if (!dbConfig.MigrationVersion.HasValue)
                {
                    m_runner.MigrateUp();
                }
                else
                {
                    m_runner.MigrateUp(dbConfig.MigrationVersion.Value);
                }
            }
        }

        private void CreateDatabaseIfRequired(DatabaseConfiguration dbConfig)
        {
            if (dbConfig.CreateDatabaseIfNotExists)
            {
                if (string.IsNullOrEmpty(dbConfig.CreateDatabaseConnectionString))
                {
                    throw new ArgumentException(
                        "CreateDatabaseConnectionString must be specified to enable creating database if not exists.");
                }

                m_databaseCreator.CreateDatabaseIfNotExists();
            }
        }

        private void LogMigrationPerform(DatabaseConfiguration dbConfig)
        {
            m_logger.LogInformation(
                "\n*** Performing migration *** \n\n Database: {0} \n Dialect: {1} \n Direction: {2} \n Version: {3} \n ConnectionString: {4}\n\n**************************** \n",
                dbConfig.DatabaseTag,
                dbConfig.DatabaseDialect,
                dbConfig.MigrationDirection.ToString(),
                dbConfig.MigrationVersion,
                dbConfig.ConnectionString);
        }

        protected int CloseApplication(bool forceNoInteractive = false)
        {
            forceNoInteractive = forceNoInteractive || m_forceNoInteractive;

            m_logger.LogInformation("Migration done.");

            NLog.LogManager.Shutdown();

            if (!forceNoInteractive && Environment.UserInteractive)
            {
                Console.WriteLine("Please hit the key to close program.");
                Console.ReadKey();
            }

            return 0;
        }

        public int RunApplication()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            m_config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            return Run();
        }

        protected abstract int Run();

        private string[] GetTags(DatabaseConfiguration dbConfig, TagsConfiguration tagsConfiguration)
        {
            var tagList = new List<string>();

            if (!string.IsNullOrEmpty(dbConfig.DatabaseTag))
            {
                tagList.Add($"{TagsPrefixes.DatabasePrefix}{dbConfig.DatabaseTag}");
            }

            if (!string.IsNullOrEmpty(tagsConfiguration.EnvironmentTag))
            {
                tagList.Add($"{TagsPrefixes.EnvironmentPrefix}{tagsConfiguration.EnvironmentTag}");
            }

            if (!string.IsNullOrEmpty(tagsConfiguration.MigrationTypeTag))
            {
                tagList.Add($"{TagsPrefixes.MigrationTypePrefix}{tagsConfiguration.MigrationTypeTag}");
            }

            return tagList.ToArray();
        }
    }
}
