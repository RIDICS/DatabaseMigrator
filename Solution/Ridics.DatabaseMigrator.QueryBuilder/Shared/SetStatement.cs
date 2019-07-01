namespace Ridics.DatabaseMigrator.QueryBuilder.Shared
{
    public class SetStatement
    {
        public SetStatement(string columnName, object value)
        {
            ColumnName = columnName;
            Value = value;
        }

        public object Value { get; }

        public string ColumnName { get; }
    }
}