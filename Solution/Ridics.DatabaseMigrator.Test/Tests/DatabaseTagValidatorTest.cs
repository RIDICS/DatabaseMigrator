using System.Collections.Generic;
using FluentMigrator;
using Ridics.DatabaseMigrator.Core.Validators;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;
using Ridics.DatabaseMigrator.Test.Data;
using Xunit;

namespace Ridics.DatabaseMigrator.Test.Tests
{
    public class DatabaseTagValidatorTest
    {
        private readonly DatabaseTagValidator m_databaseTagValidator;

        public DatabaseTagValidatorTest()
        {
            m_databaseTagValidator = new DatabaseTagValidator();
        }

        public static readonly TheoryData<string, IList<string>> DatabaseTagDataSuccess = new TheoryData<string, IList<string>>
        {
            {TestData.TagValues[0], new[]{TestData.TagValues[0]}},
            {TestData.TagValues[1], new[]{TestData.TagValues[0], TestData.TagValues[1]}},
            {TestData.TagValues[2], new[]{TestData.TagValues[2], TestData.TagValues[0]}},
        };

        public static readonly TheoryData<string, IList<string>> DatabaseTagDataFail = new TheoryData<string, IList<string>>
        {
            {TestData.TagValues[0], new string[0]},
            {TestData.TagValues[1], new string[0]},
            {TestData.TagValues[0], new[]{TestData.TagValues[1]}},
            {TestData.TagValues[0], new[]{TestData.TagValues[2]}},
            {TestData.TagValues[1], new[]{TestData.TagValues[2], TestData.TagValues[0]}},
            {TestData.TagValues[2], new[]{TestData.TagValues[0], TestData.TagValues[1]}},
        };

        public static readonly TheoryData<IList<string>> DatabaseTagValuesData = new TheoryData<IList<string>>
        {
            new string[0],
            new[]{TestData.TagValues[0]},
            new[]{TestData.TagValues[2], TestData.TagValues[1]},
        };

        [Theory]
        [MemberData(nameof(DatabaseTagDataSuccess))]
        public void Validate_Pass(string attributeTag, IList<string> tagsToMatch)
        {
            var attribute = new DatabaseTagsAttribute(attributeTag);

            var result = m_databaseTagValidator.Validate(new List<TagsAttribute> {attribute}, tagsToMatch);

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(DatabaseTagDataFail))]
        public void Validate_Fail(string attributeTag, IList<string> tagsToMatch)
        {
            var attribute = new DatabaseTagsAttribute(attributeTag);

            var result = m_databaseTagValidator.Validate(new List<TagsAttribute> {attribute}, tagsToMatch);

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(DatabaseTagValuesData))]
        public void Validate_MissingAttribute_Fail(IList<string> tagsToMatch)
        {
            var result = m_databaseTagValidator.Validate(new List<TagsAttribute>(), tagsToMatch);

            Assert.False(result);
        }
    }
}
