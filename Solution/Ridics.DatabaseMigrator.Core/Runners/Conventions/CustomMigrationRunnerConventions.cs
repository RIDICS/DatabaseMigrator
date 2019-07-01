using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using Ridics.DatabaseMigrator.Core.Validators;

namespace Ridics.DatabaseMigrator.Core.Runners.Conventions
{
    public class CustomMigrationRunnerConventions : MigrationRunnerConventions, IMigrationRunnerConventions
    {
        private readonly IList<TagValidatorBase> m_tagsValidators;

        public CustomMigrationRunnerConventions(IEnumerable<TagValidatorBase> validators)
        {
            m_tagsValidators = validators.ToList();
        }

        public new Func<Type, IEnumerable<string>, bool> TypeHasMatchingTags => CustomTypeHasMatchingTags;

        private bool CustomTypeHasMatchingTags(Type type, IEnumerable<string> tagsToMatch)
        {
            var allTagsAttributes = type.GetCustomAttributes<TagsAttribute>(true).ToList();
            var tagsToMatchList = tagsToMatch.ToList();

            foreach (var validator in m_tagsValidators)
            {
                var filteredAttributeList = validator.FilterAttributes(allTagsAttributes);
                var filteredTagsToMatch = validator.FilterTags(tagsToMatchList);

                var result = validator.Validate(filteredAttributeList, filteredTagsToMatch);
                if (!result) return false;
            }

            return true;
        }
    }
}