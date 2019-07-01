using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Ridics.DatabaseMigrator.Core.Configuration;

namespace Ridics.DatabaseMigrator.Core.Runners
{
    public class ConfigurationRunner : ApplicationRunnerBase
    {
        public ConfigurationRunner(AppConfiguration appConfiguration, bool forceNoInteractive) : base(appConfiguration, forceNoInteractive)
        {
        }

        protected override int Run()
        {
            var databaseConfigs = m_config.GetSection("DatabaseConfigurations").Get<IList<DatabaseConfiguration>>();
            var tagsConfig = m_config.GetSection("TagsConfiguration").Get<TagsConfiguration>();

            if (databaseConfigs != null && databaseConfigs.Count > 0)
            {
                foreach (var databaseConfiguration in databaseConfigs)
                {
                    if (!databaseConfiguration.SkipConfiguration)
                    {
                        RunMigration(databaseConfiguration, tagsConfig);
                    }
                }
            }

            return CloseApplication();
        }
    }
}