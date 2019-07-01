using System;
using System.IO;
using System.Threading;
using NHibernate;
using Ridics.DatabaseMigrator.Core.Configuration;
using Ridics.DatabaseMigrator.Shared;

namespace Ridics.DatabaseMigrator.Core.NHibernate.DatabaseTask
{
    public class DatabaseCreator : DatabaseTaskBase
    {
        private readonly IScriptPathResolver m_scriptPathResolver;
        private readonly DatabaseCreateConfiguration m_databaseCreateConfiguration;

        public DatabaseCreator(ISessionFactory sessionFactory, IScriptPathResolver scriptPathResolver,
            ITempDatabaseSessionFactory tmpDbSessionFactory,
            DatabaseCreateConfiguration databaseCreateConfiguration
        ) : base(sessionFactory, tmpDbSessionFactory)
        {
            m_scriptPathResolver = scriptPathResolver;
            m_databaseCreateConfiguration = databaseCreateConfiguration;
        }

        public void CreateDatabaseIfNotExists()
        {
            if (!CheckDatabaseExists())
            {
                CreateDatabase();
            }
        }

        public void CreateDatabase()
        {
            var command = File.ReadAllText(m_scriptPathResolver.ResolvePathForScript(m_databaseCreateConfiguration.CreateDbScriptName));
            RunCommand(command, m_databaseCreateConfiguration.CreateDbTimeoutInSeconds);
            WaitForDbCreated();
        }

        private void WaitForDbCreated()
        {
            for (var checkDbExistAttempt = 0;
                checkDbExistAttempt < m_databaseCreateConfiguration.MaxCheckDbExistAttempt;
                checkDbExistAttempt++)
            {
                if (CheckDatabaseExists())
                {
                    return;
                }

                Thread.Sleep(m_databaseCreateConfiguration.CheckDbCreateDelayInMilliseconds);
            }

            throw new TimeoutException("Waiting for DB creation takes too long");
        }
    }
}