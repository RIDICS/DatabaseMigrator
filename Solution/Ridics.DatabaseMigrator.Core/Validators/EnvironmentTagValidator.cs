using System;
using System.Collections.Generic;
using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.DatabaseMigrator.Core.Validators
{
    public class EnvironmentTagValidator : TagValidatorBase
    {
        ///  <summary>
        ///  Check environment tags based on following truth table:
        ///  Environment attribute exists  |  Environment argument specified  |  Result
        /// -------------------------------+----------------------------------+---------
        ///               0                |                0                 |     1
        ///               1                |                0                 |     0
        ///               0                |                1                 |     1
        ///               1                |                1                 |  Check tags
        ///  </summary>
        ///  <param name="attributes">List of environment tags attribute</param>
        ///  <param name="tagsToMatch">Environment tags to match</param>
        ///  <returns>Boolean result according to truth table</returns>
        public override bool Validate(IList<TagsAttribute> attributes, IList<string> tagsToMatch)
        {
            CheckTagAttributeTypes(attributes);

            var environmentResult = true;

            if (tagsToMatch.Count != 0 && attributes.Count != 0)
            {
                environmentResult = ValidateTags(attributes, tagsToMatch);

            }
            else if (attributes.Count != 0 && tagsToMatch.Count == 0)
            {
                environmentResult = false;
            }

            return environmentResult;
        }

        protected override Type AttributeType => typeof(EnvironmentTagsAttribute);
        protected override string TagPrefix => TagsPrefixes.EnvironmentPrefix;
    }
}