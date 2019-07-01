namespace Ridics.DatabaseMigrator.Shared
{
    public interface IScriptPathResolver
    {
        string ResolvePathForScript(string scriptName);

        string GetDatabaseDir(string databaseTag);

        string GetServerTypeDir(string databaseDialect);
    }
}