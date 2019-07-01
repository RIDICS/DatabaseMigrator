using System;

namespace Ridics.DatabaseMigrator.Core.Exceptions
{
    public class DropDatabaseException : Exception
    {
        public DropDatabaseException(string message) : base(message)
        {
        }
    }
}
