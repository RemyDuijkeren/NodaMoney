using NodaMoney.Exchange;

namespace NodaMoney.Tests.ExchangeRateSpec;

public class CreateAnExchangeRateFromAmounts
{
    [Fact]
    public void WhenAmountsArePositive_ShouldReturnQuoteDividedByBase()
    {
        // Arrange, Act
        var fx = ExchangeRate.FromAmounts(Money.Euro(100), Money.USDollar(108.25m));

        // Assert
        fx.BaseCurrency.Should().Be(Currency.FromCode("EUR"));
        fx.QuoteCurrency.Should().Be(Currency.FromCode("USD"));
        fx.Value.Should().Be(1.0825m);
    }

    [Fact]
    public void WhenRateIsUsedToConvertBaseAmount_ShouldReturnQuoteAmount()
    {
        // Arrange
        var paid = Money.Euro(100);
        var received = Money.USDollar(108.25m);

        // Act
        var fx = ExchangeRate.FromAmounts(paid, received);

        // Assert
        fx.Convert(paid).Should().Be(received);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    [InlineData(-10, 10)]
    [InlineData(10, -10)]
    public void WhenAnAmountIsZeroOrNegative_ShouldThrowArgumentOutOfRangeException(decimal baseAmount, decimal quoteAmount)
    {
        // Arrange, Act
        Action action = () => ExchangeRate.FromAmounts(Money.Euro(baseAmount), Money.USDollar(quoteAmount));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void WhenCurrenciesAreEqualAndAmountsDiffer_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange, Act
        Action action = () => ExchangeRate.FromAmounts(Money.Euro(10), Money.Euro(20));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}
