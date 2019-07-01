using System;

namespace Ridics.DatabaseMigrator.Core
{
    public class StoredMigrationInfo
    {
        public long Version { get; }

        public string Description { get; }

        public DateTime AppliedOn { get; }

        public StoredMigrationInfo(
            long version,
            string description,
            DateTime appliedOn
        )
        {
            Version = version;
            Description = description;
            AppliedOn = appliedOn;
        }
    }
}
