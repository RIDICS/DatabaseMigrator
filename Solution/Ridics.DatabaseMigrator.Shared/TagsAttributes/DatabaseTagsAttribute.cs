using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes.Types;

namespace Ridics.DatabaseMigrator.Shared.TagsAttributes
{
    public class DatabaseTagsAttribute : TagsWithTypeAttribute
    {
        public DatabaseTagsAttribute(string databaseTag) : base(TagsType.DatabaseType, TagBehavior.RequireAny, databaseTag)
        {
        }
    }
}