namespace NodaMoney.Tests.CurrencyInfoSpec;

public class CurrencyInfoFromCode
{
    [Fact]
    public void WhenCodeExists_ReturnCurrencyInfo()
    {
        // Arrange & Act
        var currency = CurrencyInfo.FromCode("EUR");

        // Assert
        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
        currency.IsHistoric.Should().BeFalse();
    }

    [Fact]
    public void WhenCodeIsUnknown_ThrowInvalidCurrencyException()
    {
        // Arrange & Act
        Action action = () => CurrencyInfo.FromCode("AAA");

        // Assert
        action.Should().Throw<InvalidCurrencyException>();
    }

    [Fact]
    public void WhenCodeIsNull_ThrowInvalidCurrencyException()
    {
        // Arrange & Act
        Action action = () => CurrencyInfo.FromCode(null);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenEstionianKrone_ReturnObsoleteCurrencyInfo()
    {
        // Arrange & Act
        var currency = CurrencyInfo.FromCode("EEK");

        // Assert
        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("kr");
        currency.IsHistoric.Should().BeTrue();
    }
}
