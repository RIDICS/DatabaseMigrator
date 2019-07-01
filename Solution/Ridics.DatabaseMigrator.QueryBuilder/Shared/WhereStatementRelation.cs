using System.Collections.Generic;

namespace Ridics.DatabaseMigrator.QueryBuilder.Shared
{
    public class WhereStatementRelation
    {
        public WhereStatementRelation(WhereStatement left, WhereStatementRelation right, StatementRelationOperator relationOperator)
        {
            RelationOperator = relationOperator;
            WhereStatements.Add(left);
            WhereStatementRelations.Add(right);
        }

        public WhereStatementRelation(WhereStatementRelation left, WhereStatement right, StatementRelationOperator relationOperator)
        {
            RelationOperator = relationOperator;
            WhereStatementRelations.Add(left);
            WhereStatements.Add(right);
        }

        public WhereStatementRelation(WhereStatementRelation left, WhereStatementRelation right, StatementRelationOperator relationOperator)
        {
            RelationOperator = relationOperator;
            WhereStatementRelations.Add(left);
            WhereStatementRelations.Add(right);
        }

        public WhereStatementRelation(WhereStatement left, WhereStatement right, StatementRelationOperator relationOperator)
        {
            RelationOperator = relationOperator;
            WhereStatements.Add(left);
            WhereStatements.Add(right);
        }

        public StatementRelationOperator RelationOperator { get; }

        public List<WhereStatement> WhereStatements { get; } = new List<WhereStatement>();

        public List<WhereStatementRelation> WhereStatementRelations { get; } = new List<WhereStatementRelation>();
    }
}