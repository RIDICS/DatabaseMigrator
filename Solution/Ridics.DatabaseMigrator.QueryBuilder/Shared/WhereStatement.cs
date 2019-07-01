namespace Ridics.DatabaseMigrator.QueryBuilder.Shared
{
    public class WhereStatement
    {
        public WhereStatement(string columnName, object value, WhereStatementOperator statementOperator = WhereStatementOperator.Equals)
        {
            ColumnName = columnName;
            Value = value;
            StatementOperator = statementOperator;
        }

        public WhereStatement(string columnName, WhereStatementNullableOperator statementOperator)
        {
            ColumnName = columnName;
            NullableStatementOperator = statementOperator;
        }

        public string ColumnName { get; }

        public object Value { get; }

        public WhereStatementOperator? StatementOperator { get; }

        public WhereStatementNullableOperator? NullableStatementOperator { get; }
    }
}