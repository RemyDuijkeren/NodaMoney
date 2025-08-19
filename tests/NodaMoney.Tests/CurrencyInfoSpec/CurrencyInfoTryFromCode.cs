namespace NodaMoney.Tests.CurrencyInfoSpec;

public class CurrencyInfoTryFromCode
{
    [Fact]
    public void WhenCodeExists_ReturnTrueAndCurrencyInfo()
    {
        // Arrange & Act
        var result = CurrencyInfo.TryFromCode("EUR", out CurrencyInfo currency);

        // Assert
        result.Should().BeTrue();
        currency.Should().NotBeNull();
        currency!.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
        currency.IsHistoric.Should().BeFalse();
    }

    [Fact]
    public void WhenCodeIsUnknown_ReturnFalse()
    {
        // Arrange & Act
        var result = CurrencyInfo.TryFromCode("AAA", out CurrencyInfo currency);

        // Assert
        result.Should().BeFalse();
        currency.Should().BeNull();
    }

    [Fact]
    public void WhenCodeIsNull_ReturnFalse()
    {
        // Arrange & Act
        var result = CurrencyInfo.TryFromCode(null!, out CurrencyInfo currency);

        // Assert
        result.Should().BeFalse();
        currency.Should().BeNull();
    }
}
