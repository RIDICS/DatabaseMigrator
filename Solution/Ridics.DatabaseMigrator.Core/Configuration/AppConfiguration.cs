using System.Collections.Generic;
using System.Reflection;

namespace Ridics.DatabaseMigrator.Core.Configuration
{
    public class AppConfiguration
    {
        public IList<string> SupportedDialects { get; set; }
        public Assembly[] MigrationAssemblies { get; set; }
    }
}
