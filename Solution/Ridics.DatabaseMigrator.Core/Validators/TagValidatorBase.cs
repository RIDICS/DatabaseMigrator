using System;
using System.Collections.Generic;
using System.Linq;
using FluentMigrator;

namespace Ridics.DatabaseMigrator.Core.Validators
{
    public abstract class TagValidatorBase
    {
        protected bool ValidateTags(IList<TagsAttribute> attributes, IList<string> tagsToMatch)
        {
            return GetResultForTags(attributes, TagBehavior.RequireAll, tagsToMatch) || GetResultForTags(attributes, TagBehavior.RequireAny, tagsToMatch);
        }

        private bool GetResultForTags(IList<TagsAttribute> tags, TagBehavior behavior, IList<string> matchTagsList)
        {
            var tagNames = tags.Where(t => t.Behavior == behavior).SelectMany(t => t.TagNames).ToArray();

            switch (behavior)
            {
                case TagBehavior.RequireAll:
                    return tagNames.Any() && matchTagsList.All(t => tagNames.Any(t.Equals));
                case TagBehavior.RequireAny:
                    return tagNames.Any() && matchTagsList.Any(t => tagNames.Any(t.Equals)); ;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behavior), behavior, null);
            }
        }

        public virtual IList<string> FilterTags(IList<string> tagsToMatch)
        {
            return tagsToMatch.Where(x => x.StartsWith(TagPrefix)).ToList();
        }

        public virtual IList<TagsAttribute> FilterAttributes(IList<TagsAttribute> attributeList)
        {
            return attributeList.Where(x => x.GetType() == AttributeType).ToList();
        }

        protected void CheckTagAttributeTypes(IList<TagsAttribute> attributes)
        {
            foreach (var tagsAttribute in attributes)
            {
                if (tagsAttribute.GetType() != AttributeType)
                {
                    throw new ArgumentException($"Unexpected attribute type {tagsAttribute.GetType()}, expected was {AttributeType}");
                }
            }
        }

        public abstract bool Validate(IList<TagsAttribute> attributes, IList<string> tagsToMatch);
        protected abstract Type AttributeType { get; }
        protected abstract string TagPrefix { get; }
    }
}