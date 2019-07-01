using FluentMigrator;
using Ridics.DatabaseMigrator.SampleApp.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.DatabaseMigrator.SampleApp.Migrations.Migrations.MyTesting
{
    [DatabaseTags(DatabaseTagTypes.RidicsTestDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(000)]
    public class M_000_CreateDatabase : ForwardOnlyMigration
    {
        public override void Up()
        {
        }
    }
}
