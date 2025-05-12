using NodaMoney.Context;

namespace NodaMoney.Tests.MoneyRoundingSpec;

public class ApplyStandardRounding
{
    [Fact]
    public void WhenRoundingDecimalBasedCurrency_ShouldRoundCorrectly()
    {
        // Arrange
        var strategy = new StandardRounding();
        var currencyInfo = CurrencyInfo.FromCode("EUR");

        // Act
        var result = strategy.Round(10.235m, currencyInfo, null);

        // Assert
        result.Should().Be(10.24m);
    }

    [Fact]
    public void WhenRoundingNonDecimalBasedCurrency_ShouldRoundCorrectly()
    {
        // Arrange
        var strategy = new StandardRounding();
        var currencyInfo = CurrencyInfo.FromCode("MRU"); // 1/5

        // Act
        var result = strategy.Round(10.22m, currencyInfo, null);

        // Assert
        result.Should().Be(10.2m);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(29)]
    public void WhenDecimalsOutOfRange_ShouldThrowArgumentOutOfRangeException(int decimals)
    {
        // Arrange
        var strategy = new StandardRounding();
        var currencyInfo = CurrencyInfo.FromCode("EUR");

        // Act
        var act = () => strategy.Round(10.235m, currencyInfo, decimals);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
