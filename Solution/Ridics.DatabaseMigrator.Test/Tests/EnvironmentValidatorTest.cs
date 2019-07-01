using System.Collections.Generic;
using FluentMigrator;
using Ridics.DatabaseMigrator.Core.Validators;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;
using Ridics.DatabaseMigrator.Test.Data;
using Xunit;

namespace Ridics.DatabaseMigrator.Test.Tests
{
    public class EnvironmentValidatorTest
    {
        private readonly EnvironmentTagValidator m_environmentTagValidator;

        public EnvironmentValidatorTest()
        {
            m_environmentTagValidator = new EnvironmentTagValidator();
        }

        public static readonly TheoryData<string, IList<string>> EnvironmentTagDataPass = new TheoryData<string, IList<string>>
        {
            {TestData.TagValues[0], new[]{TestData.TagValues[0]}},
            {TestData.TagValues[1], new[]{TestData.TagValues[0], TestData.TagValues[1]}},
            {TestData.TagValues[2], new[]{TestData.TagValues[2], TestData.TagValues[0]}},
        };

        public static readonly TheoryData<string, IList<string>> EnvironmentTagDataFail = new TheoryData<string, IList<string>>
        {
            {TestData.TagValues[0], new string[0]},
            {TestData.TagValues[1], new string[0]},
            {TestData.TagValues[0], new[]{TestData.TagValues[1]}},
            {TestData.TagValues[0], new[]{TestData.TagValues[2]}},
            {TestData.TagValues[1], new[]{TestData.TagValues[2], TestData.TagValues[0]}},
            {TestData.TagValues[2], new[]{TestData.TagValues[0], TestData.TagValues[1]}},
        };

        public static readonly TheoryData<IList<string>> EnvironmentTagValuesData = new TheoryData<IList<string>>
        {
            new string[0],
            new[]{TestData.TagValues[0]},
            new[]{TestData.TagValues[2], TestData.TagValues[1]},
        };

        [Theory]
        [MemberData(nameof(EnvironmentTagDataPass))]
        public void Validate_Pass(string attributeTag, IList<string> tagsToMatch)
        {
            var attribute = new EnvironmentTagsAttribute(attributeTag);

            var result = m_environmentTagValidator.Validate(new List<TagsAttribute> { attribute }, tagsToMatch);

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(EnvironmentTagDataFail))]
        public void Validate_Fail(string attributeTag, IList<string> tagsToMatch)
        {
            var attribute = new EnvironmentTagsAttribute(attributeTag);
            
            var result = m_environmentTagValidator.Validate(new List<TagsAttribute> { attribute }, tagsToMatch);

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(EnvironmentTagValuesData))]
        public void Validate_MissingAttribute_Pass(IList<string> tagsToMatch)
        {
            var result = m_environmentTagValidator.Validate(new List<TagsAttribute>(), tagsToMatch);

            Assert.True(result);
        }
    }
}