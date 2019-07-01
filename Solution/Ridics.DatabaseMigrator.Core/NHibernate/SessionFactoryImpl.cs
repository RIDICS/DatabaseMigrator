using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Stat;

namespace Ridics.DatabaseMigrator.Core.NHibernate
{
    public class SessionFactoryImpl : ITempDatabaseSessionFactory
    {
        private readonly string m_conectionString;
        private readonly string m_dialect;
        private ISessionFactory m_sessionFactory;


        public SessionFactoryImpl(string conectionString, string dialect)
        {
            m_conectionString = conectionString;
            m_dialect = dialect;
        }

        private ISessionFactory SessionFactory => m_sessionFactory ?? (m_sessionFactory = InintSession()); //TODO Add locking

        private ISessionFactory InintSession()
        {
            return Fluently.Configure()
                .Database(() => ResolveConfiguration(m_conectionString, m_dialect))
                .Mappings(MappingsLoader)
                .BuildSessionFactory();
        }

        private void MappingsLoader(MappingConfiguration mappingConfiguration)
        {
        }

        private static IPersistenceConfigurer ResolveConfiguration(string connectionString, string dialect)
        {
            if (dialect == MigratorDatabaseDialect.SqlServer)
            {
                return MsSqlConfiguration.MsSql2012.ConnectionString(connectionString);
            }

            if (dialect == MigratorDatabaseDialect.Postgres)
            {
                return PostgreSQLConfiguration.Standard.ConnectionString(connectionString);
            }

            if (dialect == MigratorDatabaseDialect.Sqlite)
            {
                return SQLiteConfiguration.Standard.ConnectionString(connectionString);
            }

            throw new ArgumentException($"Missing implementation for dialect: {dialect}", nameof(dialect));
        }

        public void Dispose()
        {
            SessionFactory.Dispose();
        }

        public Task CloseAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return SessionFactory.CloseAsync(cancellationToken);
        }

        public Task EvictAsync(Type persistentClass, CancellationToken cancellationToken = new CancellationToken())
        {
            return SessionFactory.EvictAsync(persistentClass, cancellationToken);
        }

        public Task EvictAsync(Type persistentClass, object id, CancellationToken cancellationToken = new CancellationToken())
        {
            return SessionFactory.EvictAsync(persistentClass, id, cancellationToken);
        }

        public Task EvictEntityAsync(string entityName, CancellationToken cancellationToken = new CancellationToken())
        {
            return SessionFactory.EvictEntityAsync(entityName, cancellationToken);
        }

        public Task EvictEntityAsync(string entityName, object id, CancellationToken cancellationToken = new CancellationToken())
        {
            return SessionFactory.EvictEntityAsync(entityName, id, cancellationToken);
        }

        public Task EvictCollectionAsync(string roleName, CancellationToken cancellationToken = new CancellationToken())
        {
            return SessionFactory.EvictCollectionAsync(roleName, cancellationToken);
        }

        public Task EvictCollectionAsync(string roleName, object id, CancellationToken cancellationToken = new CancellationToken())
        {
            return SessionFactory.EvictCollectionAsync(roleName, id, cancellationToken);
        }

        public Task EvictQueriesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return SessionFactory.EvictQueriesAsync(cancellationToken);
        }

        public Task EvictQueriesAsync(string cacheRegion, CancellationToken cancellationToken = new CancellationToken())
        {
            return SessionFactory.EvictQueriesAsync(cacheRegion, cancellationToken);
        }

        public ISessionBuilder WithOptions()
        {
            return SessionFactory.WithOptions();
        }

        [Obsolete]
        public ISession OpenSession(DbConnection connection)
        {
            return SessionFactory.OpenSession(connection);
        }

        [Obsolete]
        public ISession OpenSession(IInterceptor sessionLocalInterceptor)
        {
            return SessionFactory.OpenSession(sessionLocalInterceptor);
        }

        [Obsolete]
        public ISession OpenSession(DbConnection conn, IInterceptor sessionLocalInterceptor)
        {
            return SessionFactory.OpenSession(conn, sessionLocalInterceptor);
        }

        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public IStatelessSessionBuilder WithStatelessOptions()
        {
            return SessionFactory.WithStatelessOptions();
        }

        public IStatelessSession OpenStatelessSession()
        {
            return SessionFactory.OpenStatelessSession();
        }

        public IStatelessSession OpenStatelessSession(DbConnection connection)
        {
            return SessionFactory.OpenStatelessSession(connection);
        }

        public IClassMetadata GetClassMetadata(Type persistentClass)
        {
            return SessionFactory.GetClassMetadata(persistentClass);
        }

        public IClassMetadata GetClassMetadata(string entityName)
        {
            return SessionFactory.GetClassMetadata(entityName);
        }

        public ICollectionMetadata GetCollectionMetadata(string roleName)
        {
            return SessionFactory.GetCollectionMetadata(roleName);
        }

        public IDictionary<string, IClassMetadata> GetAllClassMetadata()
        {
            return SessionFactory.GetAllClassMetadata();
        }

        public IDictionary<string, ICollectionMetadata> GetAllCollectionMetadata()
        {
            return SessionFactory.GetAllCollectionMetadata();
        }

        public void Close()
        {
            SessionFactory.Close();
        }

        public void Evict(Type persistentClass)
        {
            SessionFactory.Evict(persistentClass);
        }

        public void Evict(Type persistentClass, object id)
        {
            SessionFactory.Evict(persistentClass, id);
        }

        public void EvictEntity(string entityName)
        {
            SessionFactory.EvictEntity(entityName);
        }

        public void EvictEntity(string entityName, object id)
        {
            SessionFactory.EvictEntity(entityName, id);
        }

        public void EvictCollection(string roleName)
        {
            SessionFactory.EvictCollection(roleName);
        }

        public void EvictCollection(string roleName, object id)
        {
            SessionFactory.EvictCollection(roleName, id);
        }

        public void EvictQueries()
        {
            SessionFactory.EvictQueries();
        }

        public void EvictQueries(string cacheRegion)
        {
            SessionFactory.EvictQueries(cacheRegion);
        }

        public FilterDefinition GetFilterDefinition(string filterName)
        {
            return SessionFactory.GetFilterDefinition(filterName);
        }

        public ISession GetCurrentSession()
        {
            return SessionFactory.GetCurrentSession();
        }

        public IStatistics Statistics => SessionFactory.Statistics;

        public bool IsClosed => SessionFactory.IsClosed;

        public ICollection<string> DefinedFilterNames => SessionFactory.DefinedFilterNames;
    }
}