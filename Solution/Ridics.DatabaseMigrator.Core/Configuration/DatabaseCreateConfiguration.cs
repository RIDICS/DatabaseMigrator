namespace Ridics.DatabaseMigrator.Core.Configuration
{
    public class DatabaseCreateConfiguration
    {
        public int MaxCheckDbExistAttempt { get; set; }

        public int CheckDbCreateDelayInMilliseconds { get; set; }

        public int CreateDbTimeoutInSeconds { get; set; }

        public string CreateDbScriptName { get; set; }
        
        public int CheckDropDbInMilliseconds { get; set; }
        
        public int DropDbMaxRepeat { get; set; }

        public string DropDbScriptName { get; set; }
    }
}