using System;

namespace Ridics.DatabaseMigrator.Core.Exceptions
{
    public class InconsistentMigrationException : Exception
    {
        public InconsistentMigrationException()
        {
        }

        public InconsistentMigrationException(string message) : base(message)
        {
        }

    }
}
