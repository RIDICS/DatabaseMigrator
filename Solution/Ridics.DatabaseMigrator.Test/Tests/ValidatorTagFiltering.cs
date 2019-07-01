using System.Collections.Generic;
using FluentMigrator;
using Ridics.DatabaseMigrator.Core.Validators;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;
using Ridics.DatabaseMigrator.Test.Data;
using Xunit;

namespace Ridics.DatabaseMigrator.Test.Tests
{
    public class ValidatorTagFiltering
    {
        private static readonly TagValidatorBase[] m_tagsValidators =
        {
            new DatabaseTagValidator(),
            new EnvironmentTagValidator(),
            new MigrationTypeValidator(),
        };

        private static readonly TagsAttribute[] m_tagsAttributes =
        {
            new DatabaseTagsAttribute(TestData.TagValues[0]),
            new EnvironmentTagsAttribute(TestData.TagValues[0]),
            new MigrationTypeTagsAttribute(TestData.TagValues[0]),
        };

        private static readonly string[] m_databaseTagValues =
        {
            TagsPrefixes.DatabasePrefix + TestData.TagValues[0],
            TagsPrefixes.DatabasePrefix + TestData.TagValues[1],
            TagsPrefixes.DatabasePrefix + TestData.TagValues[2],

        };

        private static readonly string[] m_environmentTagValues =
        {
            TagsPrefixes.EnvironmentPrefix + TestData.TagValues[0],
            TagsPrefixes.EnvironmentPrefix + TestData.TagValues[1],
            TagsPrefixes.EnvironmentPrefix + TestData.TagValues[2],

        };

        private static readonly string[] m_migrationTypeTagValues =
        {
            TagsPrefixes.MigrationTypePrefix + TestData.TagValues[0],
            TagsPrefixes.MigrationTypePrefix + TestData.TagValues[1],
            TagsPrefixes.MigrationTypePrefix + TestData.TagValues[2],

        };

        private static readonly string[][] m_tagValues =
        {
            m_databaseTagValues,
            m_environmentTagValues,
            m_migrationTypeTagValues
        };

        public static readonly TheoryData<TagValidatorBase, IList<TagsAttribute>, IList<TagsAttribute>> TestTagAttributesData = new TheoryData<TagValidatorBase, IList<TagsAttribute>, IList<TagsAttribute>>
        {
            {m_tagsValidators[0], new TagsAttribute[0], new TagsAttribute[0]},
            {m_tagsValidators[1], new TagsAttribute[0], new TagsAttribute[0]},
            {m_tagsValidators[2], new TagsAttribute[0], new TagsAttribute[0]},

            {m_tagsValidators[0], new[]{m_tagsAttributes[1], m_tagsAttributes[2]}, new TagsAttribute[0]},
            {m_tagsValidators[1], new[]{m_tagsAttributes[0], m_tagsAttributes[2]}, new TagsAttribute[0]},
            {m_tagsValidators[2], new[]{m_tagsAttributes[0], m_tagsAttributes[1]}, new TagsAttribute[0]},
            
            {m_tagsValidators[0], new[]{m_tagsAttributes[0], m_tagsAttributes[0]}, new[]{m_tagsAttributes[0], m_tagsAttributes[0]}},
            {m_tagsValidators[1], new[]{m_tagsAttributes[1], m_tagsAttributes[1]}, new[]{m_tagsAttributes[1], m_tagsAttributes[1]}},
            {m_tagsValidators[2], new[]{m_tagsAttributes[2], m_tagsAttributes[2]}, new[]{m_tagsAttributes[2], m_tagsAttributes[2]}},

            {m_tagsValidators[0], new[]{m_tagsAttributes[0], m_tagsAttributes[1], m_tagsAttributes[2]}, new[]{m_tagsAttributes[0]}},
            {m_tagsValidators[1], new[]{m_tagsAttributes[0], m_tagsAttributes[1], m_tagsAttributes[2]}, new[]{m_tagsAttributes[1]}},
            {m_tagsValidators[2], new[]{m_tagsAttributes[0], m_tagsAttributes[1], m_tagsAttributes[2]}, new[]{m_tagsAttributes[2]}},

            {m_tagsValidators[0], new[]{m_tagsAttributes[0], m_tagsAttributes[0], m_tagsAttributes[2]}, new[]{m_tagsAttributes[0], m_tagsAttributes[0]}},
            {m_tagsValidators[1], new[]{m_tagsAttributes[1], m_tagsAttributes[1], m_tagsAttributes[2]}, new[]{m_tagsAttributes[1], m_tagsAttributes[1]}},
            {m_tagsValidators[2], new[]{m_tagsAttributes[0], m_tagsAttributes[2], m_tagsAttributes[2]}, new[]{m_tagsAttributes[2], m_tagsAttributes[2]}},

        };

        public static readonly TheoryData<TagValidatorBase, IList<string>, IList<string>> TestTagValuesData = new TheoryData<TagValidatorBase, IList<string>, IList<string>>
        {
            {m_tagsValidators[0], new string[0], new string[0]},
            {m_tagsValidators[1], new string[0], new string[0]},
            {m_tagsValidators[2], new string[0], new string[0]},

            {m_tagsValidators[0], new[]{m_tagValues[1][0], m_tagValues[1][1], m_tagValues[2][1], m_tagValues[2][2]}, new string[0]},
            {m_tagsValidators[1], new[]{m_tagValues[0][0], m_tagValues[0][1], m_tagValues[2][1], m_tagValues[2][2]}, new string[0]},
            {m_tagsValidators[2], new[]{m_tagValues[0][0], m_tagValues[0][1], m_tagValues[1][1], m_tagValues[1][2]}, new string[0]},

            {m_tagsValidators[0], new[]{m_tagValues[0][0], m_tagValues[0][0]}, new[]{m_tagValues[0][0], m_tagValues[0][0]}},
            {m_tagsValidators[1], new[]{m_tagValues[1][1], m_tagValues[1][1]}, new[]{m_tagValues[1][1], m_tagValues[1][1]}},
            {m_tagsValidators[2], new[]{m_tagValues[2][2], m_tagValues[2][2]}, new[]{m_tagValues[2][2], m_tagValues[2][2]}},

            {m_tagsValidators[0], new[]{m_tagValues[0][0], m_tagValues[0][2], m_tagValues[1][0], m_tagValues[1][1], m_tagValues[2][1], m_tagValues[2][2]}, new[]{m_tagValues[0][0], m_tagValues[0][2]}},
            {m_tagsValidators[1], new[]{m_tagValues[0][0], m_tagValues[0][2], m_tagValues[1][0], m_tagValues[1][1], m_tagValues[2][1], m_tagValues[2][2]}, new[]{m_tagValues[1][0], m_tagValues[1][1]}},
            {m_tagsValidators[2], new[]{m_tagValues[0][0], m_tagValues[0][2], m_tagValues[1][0], m_tagValues[1][1], m_tagValues[2][1], m_tagValues[2][2]}, new[]{m_tagValues[2][1], m_tagValues[2][2]}},

            {m_tagsValidators[0], new[]{m_tagValues[0][0], m_tagValues[0][0], m_tagValues[1][0], m_tagValues[1][1], m_tagValues[2][1], m_tagValues[2][2]}, new[]{m_tagValues[0][0], m_tagValues[0][0]}},
            {m_tagsValidators[1], new[]{m_tagValues[0][0], m_tagValues[0][2], m_tagValues[1][1], m_tagValues[1][1], m_tagValues[2][1], m_tagValues[2][2]}, new[]{m_tagValues[1][1], m_tagValues[1][1]}},
            {m_tagsValidators[2], new[]{m_tagValues[0][0], m_tagValues[0][2], m_tagValues[1][0], m_tagValues[1][1], m_tagValues[2][2], m_tagValues[2][2]}, new[]{m_tagValues[2][2], m_tagValues[2][2]}},
        };

        [Theory]
        [MemberData(nameof(TestTagAttributesData))]
        public void FilterTagAttributes(TagValidatorBase tagValidator, IList<TagsAttribute> attributes, IList<TagsAttribute> expectedFilteredAttributes)
        {
            var filteredAttributes = tagValidator.FilterAttributes(attributes);

            Assert.Equal(expectedFilteredAttributes.Count, filteredAttributes.Count);

            Assert.Equal(expectedFilteredAttributes, filteredAttributes);
        }

        [Theory]
        [MemberData(nameof(TestTagValuesData))]
        public void FilterTagValues(TagValidatorBase tagValidator, IList<string> tags, IList<string> expectedFilteredTags)
        {
            var filteredTags = tagValidator.FilterTags(tags);

            Assert.Equal(expectedFilteredTags.Count, filteredTags.Count);

            Assert.Equal(expectedFilteredTags, filteredTags);
        }
    }
}