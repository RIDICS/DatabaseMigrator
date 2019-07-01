using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes.Types;

namespace Ridics.DatabaseMigrator.Shared.TagsAttributes
{
    public class EnvironmentTagsAttribute : TagsWithTypeAttribute
    {
        public EnvironmentTagsAttribute(params string[] environmentTags) : base(TagsType.EnvironmentType, TagBehavior.RequireAny, environmentTags)
        {
        }
    }
}