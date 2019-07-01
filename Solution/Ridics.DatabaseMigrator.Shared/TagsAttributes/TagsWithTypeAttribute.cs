using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes.Types;

namespace Ridics.DatabaseMigrator.Shared.TagsAttributes
{
    public class TagsWithTypeAttribute : TagsAttribute
    {
        public TagsType TagsType { get; }

        protected TagsWithTypeAttribute(TagsType tagsType, TagBehavior behavior, params string[] tags) : base(behavior, tags)
        {
            TagsType = tagsType;
        }

        protected TagsWithTypeAttribute(TagsType tagsType, params string[] tags) : base(tags)
        {
            TagsType = tagsType;
        }
    }
}