using System.Linq;
using FluentMigrator;
using Ridics.DatabaseMigrator.QueryBuilder;
using Ridics.DatabaseMigrator.SampleApp.Migrations.TagTypes;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.DatabaseMigrator.SampleApp.Migrations.Migrations.MyTesting
{
    [DatabaseTags(DatabaseTagTypes.RidicsTestDB)]
    [MigrationTypeTags(CoreMigrationTypeTagTypes.Data, CoreMigrationTypeTagTypes.All)]
    [Migration(002)]
    public class M_002_DefaultValues : ForwardOnlyMigration
    {
        public M_002_DefaultValues()
        {
        }

        public override void Up()
        {
            Insert.IntoTable("User")
                .Row(new
                {
                    Username = "admin",
                    FirstName = "Admin",
                    LastName = "Exampl",
                    PasswordHash = "password-hash",
                });

            Insert.IntoTable("Role")
                .Row(new { Name = "Admin" })
                .Row(new { Name = "User" });

            Execute.WithConnection((connection, transaction) =>
            {
                var userId = Query.Conn(connection, transaction).Select<int>("Id").From("User")
                    .Where("Username", "admin")
                    .Run().Single();

                var roleId = Query.Conn(connection, transaction).Select<int>("Id").From("Role")
                    .Where("Name", "Admin")
                    .Run().Single();

                Query.Conn(connection, transaction).Insert("User_Role")
                    .Row(new { UserId = userId, RoleId = roleId })
                    .Run();
            });
        }
    }
}