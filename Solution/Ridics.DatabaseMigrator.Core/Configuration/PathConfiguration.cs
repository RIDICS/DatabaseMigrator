using System.Collections.Generic;

namespace Ridics.DatabaseMigrator.Core.Configuration
{
    public class PathConfiguration
    {
        public Dictionary<string, string> ServerTypeDirMapping { get; set; }

        public Dictionary<string, string> DatabaseDirMapping { get; set; }

        public string BaseDir { get; set; }
    }
}