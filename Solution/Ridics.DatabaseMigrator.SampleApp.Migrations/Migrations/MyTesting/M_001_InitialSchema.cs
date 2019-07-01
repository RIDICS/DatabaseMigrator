using FluentMigrator;
using Ridics.DatabaseMigrator.SampleApp.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.DatabaseMigrator.SampleApp.Migrations.Migrations.MyTesting
{
    [DatabaseTags(DatabaseTagTypes.RidicsTestDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Structure, CoreMigrationTypeTagTypes.All)]
    [Migration(001)]
    public class M_001_InitialSchema : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("User")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_User(Id)").Identity()
                .WithColumn("Username").AsString(50).NotNullable().Unique("UQ_User(Username)")
                .WithColumn("FirstName").AsString(50)
                .WithColumn("LastName").AsString(50)
                .WithColumn("PasswordHash").AsString().NotNullable();

            Create.Table("Role")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey("PK_Role(Id)").Identity()
                .WithColumn("Name").AsString(50).NotNullable().Unique("UQ_Role(Name)");

            Create.Table("User_Role")
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("FK_User_Role(UserId)", "User", "Id")
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("FK_User_Role(RoleId)", "Role", "Id");
        }
    }
}
