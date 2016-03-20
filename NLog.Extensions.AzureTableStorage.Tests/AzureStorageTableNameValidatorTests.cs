using Xunit;

namespace NLog.Extensions.AzureTableStorage.Tests
{
    public class AzureStorageTableNameValidatorTests
    {
        [Fact]
        public void IsValidReturnFalseIfTableNameIsReservedWord()
        {
            var validator = new AzureStorageTableNameValidator();
            Assert.False(validator.IsValid("tables")); //reserved (invalid)
        }


        [Fact]
        public void IsValidReturnFalseIfTableNameVaiolatesRules_number()
        {
            var validator = new AzureStorageTableNameValidator();
            Assert.False(validator.IsValid("5"));//invalid
        }


        [Fact]
        public void IsValidReturnFalseIfTableNameVaiolatesRules_startsWithNumber()
        {
            var validator = new AzureStorageTableNameValidator();
            Assert.False(validator.IsValid("5products"));//invalid
        }


        [Fact]
        public void IsValidReturnFalseIfTableNameVaiolatesRules_containSpaces()
        {
            var validator = new AzureStorageTableNameValidator();
            Assert.False(validator.IsValid("products and customers"));//invalid
        }


        [Fact]
        public void IsValidReturnTrueIfTableNameIsValid()
        {
            var validator = new AzureStorageTableNameValidator();
            Assert.True(validator.IsValid("myTable"));//valid name
        }

    }
}
