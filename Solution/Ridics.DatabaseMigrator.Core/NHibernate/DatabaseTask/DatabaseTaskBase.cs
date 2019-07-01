using System;
using FluentNHibernate.Conventions;
using NHibernate;

namespace Ridics.DatabaseMigrator.Core.NHibernate.DatabaseTask
{
    public abstract class DatabaseTaskBase
    {
        private readonly ISessionFactory m_sessionFactory;
        private readonly ISessionFactory m_tmpDbSessionFactory;

        protected DatabaseTaskBase(ISessionFactory sessionFactory, ISessionFactory tmpDbSessionFactory)
        {
            m_sessionFactory = sessionFactory;
            m_tmpDbSessionFactory = tmpDbSessionFactory;
        }

        protected bool CheckDatabaseExists()
        {
            try
            {
                using (var session = m_sessionFactory.OpenSession())
                {
                    var connection = session.Connection;

                    if (connection.ServerVersion.IsEmpty())
                    {
                        return false;
                    }

                    session.Close();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected void RunCommand(string command, int? timeoutInSeconds = null)
        {
            using (var session = m_tmpDbSessionFactory.OpenSession())
            {
                using (var cmd = session.Connection.CreateCommand())
                {
                    if (timeoutInSeconds.HasValue)
                    {
                        cmd.CommandTimeout = timeoutInSeconds.Value;
                    }

                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();
                }

                session.Close();
            }
        }
    }
}
