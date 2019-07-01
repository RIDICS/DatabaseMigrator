using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using NHibernate;
using Ridics.DatabaseMigrator.Core.Configuration;
using Ridics.DatabaseMigrator.Core.Exceptions;
using Ridics.DatabaseMigrator.Shared;

namespace Ridics.DatabaseMigrator.Core.NHibernate.DatabaseTask
{
    public class DatabaseDropper : DatabaseTaskBase
    {
        private readonly IScriptPathResolver m_scriptPathResolver;
        private readonly DatabaseConfiguration m_databaseConfiguration;
        private readonly DatabaseCreateConfiguration m_databaseCreateConfiguration;

        public DatabaseDropper(ISessionFactory sessionFactory, IScriptPathResolver scriptPathResolver,
            ITempDatabaseSessionFactory tempDbSessionFactory,
            DatabaseConfiguration databaseConfiguration,
            DatabaseCreateConfiguration databaseCreateConfiguration
        ) : base(sessionFactory, tempDbSessionFactory)
        {
            m_scriptPathResolver = scriptPathResolver;
            m_databaseConfiguration = databaseConfiguration;
            m_databaseCreateConfiguration = databaseCreateConfiguration;
        }

        public void DropDatabaseIfExists(ILogger logger)
        {
            if (CheckDatabaseExists())
            {
                logger.LogInformation(
                    "\n*** Dropping database *** \n\n Database: {0} \n Dialect: {1} \n ConnectionString: {4}\n\n**************************** \n",
                    m_databaseConfiguration.DatabaseTag,
                    m_databaseConfiguration.DatabaseDialect,
                    m_databaseConfiguration.ConnectionString);

                DropDatabase(logger);
            }
        }

        public void DropDatabase(ILogger logger)
        {
            var command = File.ReadAllText(m_scriptPathResolver.ResolvePathForScript(m_databaseCreateConfiguration.DropDbScriptName));
            RunCommand(command);

            logger.LogInformation("\nVerifying database drop ");

            var i = 0;
            while (CheckDatabaseExists())
            {
                Thread.Sleep(m_databaseCreateConfiguration.CheckDropDbInMilliseconds);
                i++;

                logger.LogInformation(".");

                if (i > m_databaseCreateConfiguration.DropDbMaxRepeat)
                {
                    throw new DropDatabaseException("Unable to verify drop database");
                }
            }

            logger.LogInformation("\n\n*** Database dropped *** \n\n **************************** \n");
        }
    }
}