namespace NodaMoney.Tests.MoneySpec;

public class CreateMoneyWithDoubleValue
{
    public static TheoryData<double, decimal> DoubleWithCurrencyTestData => new()
    {
        { 0.03, 0.03m },
        { 0.3333333333333333, 0.33m },
    };

    [Theory, MemberData(nameof(DoubleWithCurrencyTestData))]
    public void WhenValueIsDoubleAndWithCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
    {
        var money = new Money(input, "EUR");

        money.Amount.Should().Be(expected);
    }

    public static TheoryData<double, decimal> DoubleWithoutCurrencyTestData => new()
    {
        { 0.03, 0.03m },
        { 0.3333333333333333, 0.33m },
    };

    [Theory, MemberData(nameof(DoubleWithoutCurrencyTestData))]
    public void WhenValueIsDoubleWithoutCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
    {
        var money = new Money(input, "EUR");

        money.Amount.Should().Be(expected);
    }

    // A double is rounded to 15 significant digits before it becomes the amount. From .NET 11 the runtime's own
    // conversion keeps the full binary value instead (0.1 becomes 0.1000000000000000055511151231), which would push
    // decimal midpoints off the midpoint and leave binary noise in currencies that do not round.
    public static TheoryData<double, string, decimal> SignificantDigitsTestData => new()
    {
        { 10.005, "USD", 10.00m }, // midpoint, rounds to even
        { 10.015, "USD", 10.02m }, // midpoint, rounds to even
        { -10.005, "USD", -10.00m },
        { 0.1, "MGA", 0.0m }, // midpoint between 0.0 and 0.2
        { 0.9, "MGA", 0.8m }, // midpoint between 0.8 and 1.0
        { 0.1, "XXX", 0.1m }, // XXX does not round
        { 1234.56789, "XXX", 1234.56789m },
        { 0.1 + 0.2, "XXX", 0.3m },
        { 1E-20, "XXX", 0.00000000000000000001m },
        { 1234567890123456789d, "XXX", 1234567890123460000m },
    };

    [Theory, MemberData(nameof(SignificantDigitsTestData))]
    public void WhenValueIsDouble_ThenItShouldBeRoundedTo15SignificantDigitsFirst(double input, string code, decimal expected)
    {
        var money = new Money(input, code);

        money.Amount.Should().Be(expected);
    }
}
