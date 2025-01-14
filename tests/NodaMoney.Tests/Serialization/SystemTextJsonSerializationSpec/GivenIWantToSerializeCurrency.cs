using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.Serialization.SystemTextJsonSerializationSpec;

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
        string json = JsonSerializer.Serialize(currency);

        // Assert
        json.Should().Be($"\"{code}\"");

        var clone = JsonSerializer.Deserialize<Currency>(json);
        clone.Should().Be(currency);
    }
}
