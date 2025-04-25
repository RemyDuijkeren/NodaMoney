namespace NodaMoney.Tests.MoneyNumericInterfaces;

public class NumericOperations
{
    [Fact]
    public void WhenMultiplicativeIdentity_ReturnOne()
    {
        // Act
        decimal result = Money.MultiplicativeIdentity;

        // Assert
        result.Should().Be(1m);
    }


    [Fact]
    public void WhenMultipleWithMultiplicativeIdentity_ReturnStartValue()
    {
        // Arrange
        Money startValue = new(123, "EUR");

        // Act
        Money result = startValue * Money.MultiplicativeIdentity;

        // Assert
        result.Should().Be(startValue);
    }

    [Fact]
    public void WhenAdditiveIdentity_ReturnZeroNoCurrencyMoney()
    {
        // Act
        Money result = Money.AdditiveIdentity;

        // Assert
        result.Should().Be(new Money(0m, Currency.NoCurrency));
    }

    [Fact]
    public void WhenAddingAdditiveIdentity_ReturnStartValue()
    {
        // Arrange
        Money startValue = new(123, "EUR");

        // Act
        Money result = startValue + Money.AdditiveIdentity;

        // Assert
        result.Should().Be(startValue);
    }

    [Fact]
    public void MinValue_ShouldInitializeCorrectly_WhenAccessed()
    {
        // Act
        var result = Money.MinValue;

        // Assert
        result.Amount.Should().Be(decimal.MinValue);
        result.Currency.Should().Be(Currency.NoCurrency);
    }

    [Fact]
    public void MinValueWithEur_ShouldInitializeCorrectly_WhenAccessed()
    {
        // Arrange
        Currency eur = CurrencyInfo.FromCode("EUR");

        // Act
        var result = Money.MinValue with { Currency = eur };

        // Assert
        result.Amount.Should().Be(decimal.MinValue);
        result.Currency.Should().Be(eur);
    }

    [Fact]
    public void MaxValue_ShouldInitializeCorrectly_WhenAccessed()
    {
        // Act
        var result = Money.MaxValue;

        // Assert
        result.Amount.Should().Be(decimal.MaxValue);
        result.Currency.Should().Be(Currency.NoCurrency);
    }

    [Fact]
    public void MaxValueWithEur_ShouldInitializeCorrectly_WhenAccessed()
    {
        // Arrange
        Currency eur = CurrencyInfo.FromCode("EUR");

        // Act
        var result = Money.MaxValue with { Currency = eur };

        // Assert
        result.Amount.Should().Be(decimal.MaxValue);
        result.Currency.Should().Be(eur);
    }
}
