using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes.Types;

namespace Ridics.DatabaseMigrator.Shared.TagsAttributes
{
    public class MigrationTypeTagsAttribute : TagsWithTypeAttribute
    {
        public MigrationTypeTagsAttribute(TagBehavior behavior, params string[] migrationTypeTag) : base(TagsType.MigrationType, behavior, migrationTypeTag)
        {
        }

        public MigrationTypeTagsAttribute(params string[] migrationTypeTag) : base(TagsType.MigrationType, migrationTypeTag)
        {
        }
    }
}