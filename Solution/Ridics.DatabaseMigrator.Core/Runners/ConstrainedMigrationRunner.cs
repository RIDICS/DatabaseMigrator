using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ridics.DatabaseMigrator.Core.Exceptions;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.DatabaseMigrator.Core.Runners
{
    public class ConstrainedMigrationRunner : MigrationRunner, IMigrationRunner
    {
        public ConstrainedMigrationRunner(
            IOptions<RunnerOptions> options, IOptionsSnapshot<ProcessorOptions> processorOptions,
            IProfileLoader profileLoader, IProcessorAccessor processorAccessor, IMaintenanceLoader maintenanceLoader,
            IMigrationInformationLoader migrationLoader, ILogger<MigrationRunner> logger, IStopWatch stopWatch,
            IMigrationRunnerConventionsAccessor migrationRunnerConventionsAccessor, IAssemblySource assemblySource,
            MigrationValidator migrationValidator, IServiceProvider serviceProvider
        ) : base(options, processorOptions, profileLoader,
            processorAccessor, maintenanceLoader, migrationLoader, logger, stopWatch, migrationRunnerConventionsAccessor, assemblySource,
            migrationValidator, serviceProvider
        )
        {
        }

        public new void MigrateUp()
        {
            MigrateUp(true);
        }

        public new void MigrateUp(bool useAutomaticTransactionManagement)
        {
            MigrateUp(long.MaxValue, useAutomaticTransactionManagement);
        }

        public new void MigrateUp(long targetVersion)
        {
            MigrateUp(targetVersion, true);
        }

        public new void MigrateUp(long targetVersion, bool useAutomaticTransactionManagement)
        {
            VersionLoader.LoadVersionInfo();

            var validationVersionLoader = VersionLoader as ValidatingVersionLoader;

            if (validationVersionLoader == null)
            {
                throw new ArgumentException(
                    string.Format("Class of type {0} is required, {1} given",
                        typeof(ValidatingVersionLoader).FullName,
                        VersionLoader.GetType().FullName
                    )
                );
            }

            var storedMigrationInfos = validationVersionLoader.StoredMigrationInfos;
            var availableMigrations = MigrationLoader.LoadMigrations().ToList().ToDictionary(x => x.Key, x => x.Value);

            FilterAndValidateMigrations(storedMigrationInfos, availableMigrations);
            ValidateEnvironmentAttributeUsage(availableMigrations);

            base.MigrateUp(targetVersion, useAutomaticTransactionManagement);
        }

        private void FilterAndValidateMigrations(IDictionary<long, StoredMigrationInfo> storedMigrationInfos, Dictionary<long, IMigrationInfo> availableMigrations)
        {
            long lastStoredVersion = 0;
            foreach (var storedMigrationInfo in storedMigrationInfos.Values.ToList())
            {
                lastStoredVersion = lastStoredVersion < storedMigrationInfo.Version ? storedMigrationInfo.Version : lastStoredVersion;

                ValidateExecutedMigrationsConsistency(storedMigrationInfo, availableMigrations);

                storedMigrationInfos.Remove(storedMigrationInfo.Version);
                availableMigrations.Remove(storedMigrationInfo.Version);
            }

            ValidateMigrationVersions(availableMigrations, lastStoredVersion);
        }

        private void ValidateEnvironmentAttributeUsage(Dictionary<long, IMigrationInfo> availableMigrations)
        {
            foreach (var migrationInfo in availableMigrations.Values)
            {
                var type = migrationInfo.Migration.GetType();

                var environmentTags = type.GetCustomAttributes<EnvironmentTagsAttribute>(true).ToList();
                if (environmentTags.Count == 0)
                    continue;

                var tags = type.GetCustomAttributes<TagsAttribute>(true).Where(x => x.GetType() != typeof(EnvironmentTagsAttribute)).ToList();

                var isStructureMigration = tags.SelectMany(t => t.TagNames).Contains(CoreMigrationTypeTagTypes.Structure);

                if (isStructureMigration)
                {
                    throw new InvalidMigrationException(migrationInfo.Migration,
                        "Invalid use of environment attribute. Environment attribute cannot be used with MigrationTypeTag.Structure tag. Every environment must have same structure");
                }
            }
        }

        private void ValidateExecutedMigrationsConsistency(StoredMigrationInfo storedMigrationInfo, Dictionary<long, IMigrationInfo> availableMigrations)
        {
            if (!availableMigrations.ContainsKey(storedMigrationInfo.Version))
            {
                throw new MissingMigrationsException(string.Format(
                    "Previously executed migration {0} : {1} not found", storedMigrationInfo.Version, storedMigrationInfo.Description
                ));
            }

            var availableMigration = availableMigrations[storedMigrationInfo.Version];
            var availableMigrationName = availableMigration.Migration.GetType().Name;

            if (!storedMigrationInfo.Description.Equals(availableMigrationName))
            {
                throw new InconsistentMigrationException(string.Format(
                    "Previously executed migration {0} : {1} found with different name {2}",
                    storedMigrationInfo.Version, storedMigrationInfo.Description, availableMigrationName
                ));
            }
        }

        private void ValidateMigrationVersions(Dictionary<long, IMigrationInfo> availableMigrations, long lastStoredVersion)
        {
            foreach (var version in availableMigrations.Keys)
            {
                if (version < lastStoredVersion)
                {
                    throw new InconsistentMigrationException(string.Format(
                        "Found not executed migration with lower version then last executed version {0} < {1}",
                        version, lastStoredVersion
                    ));
                }
            }
        }
    }
}
