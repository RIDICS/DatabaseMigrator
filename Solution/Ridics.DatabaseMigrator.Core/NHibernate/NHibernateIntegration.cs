using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Ridics.DatabaseMigrator.Core.Configuration;
using Ridics.DatabaseMigrator.Core.NHibernate.DatabaseTask;

namespace Ridics.DatabaseMigrator.Core.NHibernate
{
    public static class NHibernateIntegration
    {
        public static ServiceCollection AddNHibernate(this ServiceCollection services, DatabaseConfiguration dbConfig)
        {
            var tempDbSessionFactory = new SessionFactoryImpl(dbConfig.CreateDatabaseConnectionString, dbConfig.DatabaseDialect);
            services.AddSingleton<ITempDatabaseSessionFactory>(tempDbSessionFactory);

            var dbSessionFactory = new SessionFactoryImpl(dbConfig.ConnectionString, dbConfig.DatabaseDialect);
            services.AddSingleton<ISessionFactory>(dbSessionFactory);

            services.AddTransient<DatabaseCreator>();

            return services;
        }
    }
}