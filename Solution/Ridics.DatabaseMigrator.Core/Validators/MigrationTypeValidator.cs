using System;
using System.Collections.Generic;
using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.DatabaseMigrator.Core.Validators
{
    public class MigrationTypeValidator : TagValidatorBase
    {
        ///  <summary>
        ///  Check migration type tags based on following truth table (migration type attribute and argument has to be always specified):
        ///  Migration type attribute exists  |  Migration type argument specified  |  Result
        /// ----------------------------------+-------------------------------------+---------
        ///                 0                 |                  0                  |     0
        ///                 1                 |                  0                  |     0
        ///                 0                 |                  1                  |     0
        ///                 1                 |                  1                  |  Check tags
        ///  </summary>
        ///  <param name="attributes">List of migration type tags attributes</param>
        ///  <param name="tagsToMatch">Migration type tags to match</param>
        ///  <returns>Boolean result according to truth table</returns>
        public override bool Validate(IList<TagsAttribute> attributes, IList<string> tagsToMatch)
        {
            CheckTagAttributeTypes(attributes);

            if (attributes.Count != 0 && tagsToMatch.Count != 0)
            {
                return ValidateTags(attributes, tagsToMatch);
            }

            return false;
        }

        protected override Type AttributeType => typeof(MigrationTypeTagsAttribute);
        protected override string TagPrefix => TagsPrefixes.MigrationTypePrefix;
    }
}