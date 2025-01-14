using FluentAssertions;
using Xunit;
using Newtonsoft.Json;

namespace NodaMoney.Tests.Serialization.NewtonsoftJsonSerializerSpec;

public class GivenIWantToSerializeCurrency
{
    [Theory]
    [InlineData("JPY")]
    [InlineData("EUR")]
    [InlineData("USD")]
    [InlineData("BHD")]
    [InlineData("BTC")]
    [InlineData("XXX")]
    public void WhenSerializingCurrency_ThenThisShouldSucceed(string code)
    {
        // Arrange
        var currency = Currency.FromCode(code);

        // Act
        string json = JsonConvert.SerializeObject(currency);

        // Assert
        json.Should().Be($"\"{code}\"");

        var clone = JsonConvert.DeserializeObject<Currency>(json);
        clone.Should().Be(currency);
    }
}
