using System.Collections.Generic;
using FluentMigrator;
using Ridics.DatabaseMigrator.Core.Validators;
using Ridics.DatabaseMigrator.Shared.TagsAttributes;
using Ridics.DatabaseMigrator.Test.Data;
using Xunit;

namespace Ridics.DatabaseMigrator.Test.Tests
{
    public class MigrationTypeValidatorTest
    {
        private readonly MigrationTypeValidator m_migrationTypeValidator;

        public MigrationTypeValidatorTest()
        {
            m_migrationTypeValidator = new MigrationTypeValidator();
        }

        public static readonly TheoryData<IList<string>, TagBehavior, IList<string>> MigrationTypeTagDataPass = new TheoryData<IList<string>, TagBehavior, IList<string>>
        {
            {new[] {TestData.TagValues[0]}, TagBehavior.RequireAny, new[]{TestData.TagValues[0]}},
            {new[] {TestData.TagValues[1]}, TagBehavior.RequireAny, new[]{TestData.TagValues[0], TestData.TagValues[1]}},
            {new[] {TestData.TagValues[2]}, TagBehavior.RequireAll, new[]{TestData.TagValues[2]}},

            {new[] {TestData.TagValues[0], TestData.TagValues[1]}, TagBehavior.RequireAny, new[]{TestData.TagValues[0]}},
            {new[] {TestData.TagValues[1], TestData.TagValues[2]}, TagBehavior.RequireAny, new[]{TestData.TagValues[0], TestData.TagValues[1]}},
            {new[] {TestData.TagValues[2], TestData.TagValues[0]}, TagBehavior.RequireAll, new[]{TestData.TagValues[0]}},
            {new[] {TestData.TagValues[2], TestData.TagValues[0]}, TagBehavior.RequireAll, new[]{TestData.TagValues[0], TestData.TagValues[2]}},
        };

        public static readonly TheoryData<IList<string>, TagBehavior, IList<string>> MigrationTypeTagDataFail = new TheoryData<IList<string>, TagBehavior, IList<string>>
        {
            {new[] {TestData.TagValues[0]}, TagBehavior.RequireAny, new[]{TestData.TagValues[1]}},
            {new[] {TestData.TagValues[2]}, TagBehavior.RequireAny, new[]{TestData.TagValues[0], TestData.TagValues[1]}},
            {new[] {TestData.TagValues[0]}, TagBehavior.RequireAny, new string[0]},
            {new[] {TestData.TagValues[0]}, TagBehavior.RequireAll, new[]{TestData.TagValues[2]}},
            {new[] {TestData.TagValues[1]}, TagBehavior.RequireAll, new[]{TestData.TagValues[2], TestData.TagValues[1]}},
            {new[] {TestData.TagValues[1]}, TagBehavior.RequireAll, new string[0]},
        };

        public static readonly TheoryData<IList<string>> MigrationTypeTagValuesData = new TheoryData<IList<string>>
        {
            new string[0],
            new[] {TestData.TagValues[0]},
            new[] {TestData.TagValues[2], TestData.TagValues[1]},
        };

        [Theory]
        [MemberData(nameof(MigrationTypeTagDataPass))]
        public void Validate_Pass(string[] attributeTags, TagBehavior behavior, IList<string> tagsToMatch)
        {
            var attribute = new MigrationTypeTagsAttribute(behavior, attributeTags);

            var result = m_migrationTypeValidator.Validate(new List<TagsAttribute> { attribute }, tagsToMatch);

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(MigrationTypeTagDataFail))]
        public void Validate_Fail(string[] attributeTags, TagBehavior behavior, IList<string> tagsToMatch)
        {
            var attribute = new MigrationTypeTagsAttribute(behavior, attributeTags);

            var result = m_migrationTypeValidator.Validate(new List<TagsAttribute> { attribute }, tagsToMatch);

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(MigrationTypeTagValuesData))]
        public void Validate_MissingAttribute_Fail(IList<string> tagsToMatch)
        {
            var result = m_migrationTypeValidator.Validate(new List<TagsAttribute>(), tagsToMatch);

            Assert.False(result);
        }
    }
}