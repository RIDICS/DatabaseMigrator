using System.IO;
using Ridics.DatabaseMigrator.Core.Configuration;
using Ridics.DatabaseMigrator.Shared;

namespace Ridics.DatabaseMigrator.Core
{
    public class ScriptPathResolver : IScriptPathResolver
    {
        private readonly DatabaseConfiguration m_databaseConfiguration;
        private readonly PathConfiguration m_pathConfiguration;

        public ScriptPathResolver(DatabaseConfiguration databaseConfiguration, PathConfiguration pathConfiguration)
        {
            m_databaseConfiguration = databaseConfiguration;
            m_pathConfiguration = pathConfiguration;
        }

        public string ResolvePathForScript(string scriptName)
        {
            return Path.Combine(m_pathConfiguration.BaseDir, GetDatabaseDir(m_databaseConfiguration.DatabaseTag),
                GetServerTypeDir(m_databaseConfiguration.DatabaseDialect), scriptName);
        }

        public string GetDatabaseDir(string databaseTag) => m_pathConfiguration.DatabaseDirMapping[databaseTag];

        public string GetServerTypeDir(string databaseDialect) => m_pathConfiguration.ServerTypeDirMapping[databaseDialect];
    }
}