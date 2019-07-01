using System;
using System.Collections.Generic;
using FluentMigrator;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;

namespace Ridics.DatabaseMigrator.Core.Validators
{
    public class DatabaseTagValidator : TagValidatorBase
    {
        ///  <summary>
        ///  Check database tags based on following truth table (database attribute and argument has to be always specified):
        ///    Database attribute exists   |    Database argument specified   |  Result
        /// -------------------------------+----------------------------------+---------
        ///               0                |                0                 |     0
        ///               1                |                0                 |     0
        ///               0                |                1                 |     0
        ///               1                |                1                 |  Check tags
        ///  </summary>
        ///  <param name="attributes">List of database tags attributes</param>
        ///  <param name="tagsToMatch">Database tags to match</param>
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

        protected override Type AttributeType => typeof(DatabaseTagsAttribute);
        protected override string TagPrefix => TagsPrefixes.DatabasePrefix;
    }
}