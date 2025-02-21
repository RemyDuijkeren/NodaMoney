namespace NodaMoney.Tests.MoneyUnaryOperatorsSpec;

public class IncrementAndDecrementMoneyUnary
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenIncrementing_ThenAmountShouldIncrementWithMinorUnit(Money money, Currency expectedCurrency, decimal expectedDifference)
    {
        decimal amountBefore = money.Amount;

        Money result = ++money;

        result.Currency.Should().Be(expectedCurrency);
        result.Amount.Should().Be(amountBefore + expectedDifference);
        money.Currency.Should().Be(expectedCurrency);
        money.Amount.Should().Be(amountBefore + expectedDifference);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenDecrementing_ThenAmountShouldDecrementWithMinorUnit(Money money, Currency expectedCurrency, decimal expectedDifference)
    {
        decimal amountBefore = money.Amount;

        Money result = --money;

        result.Currency.Should().Be(expectedCurrency);
        result.Amount.Should().Be(amountBefore - expectedDifference);
        money.Currency.Should().Be(expectedCurrency);
        money.Amount.Should().Be(amountBefore - expectedDifference);
    }

    public static TheoryData<Money, Currency, decimal> TestData => new TheoryData<Money, Currency, decimal>
    {
        { new Money(765m, Currency.FromCode("JPY")), Currency.FromCode("JPY"), Currency.FromCode("JPY").MinimalAmount },
        { new Money(765.43m, Currency.FromCode("EUR")), Currency.FromCode("EUR"), Currency.FromCode("EUR").MinimalAmount },
        { new Money(765.43m, Currency.FromCode("USD")), Currency.FromCode("USD"), Currency.FromCode("USD").MinimalAmount },
        { new Money(765.432m, Currency.FromCode("BHD")), Currency.FromCode("BHD"), Currency.FromCode("BHD").MinimalAmount }
    };
}
