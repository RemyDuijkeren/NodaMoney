namespace NodaMoney.Tests.FastMoneySpec;

public class CreateMoneyWithDoubleValue
{
    public static TheoryData<double, decimal> DoubleWithCurrencyTestData => new()
    {
        { 0.03, 0.03m },
        { 0.3333333333333333, 0.3333m },
    };

    [Theory, MemberData(nameof(DoubleWithCurrencyTestData))]
    public void WhenValueIsDoubleAndWithCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
    {
        var money = new FastMoney(input, "EUR");

        money.Amount.Should().Be(expected);
    }

    public static TheoryData<double, decimal> DoubleWithoutCurrencyTestData => new()
    {
        { 0.03, 0.03m },
        { 0.3333333333333333, 0.3333m },
    };

    [Theory, MemberData(nameof(DoubleWithoutCurrencyTestData))]
    public void WhenValueIsDoubleWithoutCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
    {
        var money = new FastMoney(input, "EUR");

        money.Amount.Should().Be(expected);
    }

    // A double is rounded to 15 significant digits before it becomes the amount, so a decimal midpoint stays a midpoint
    // on .NET 11, where the runtime's own conversion keeps the full binary value.
    public static TheoryData<double, decimal> SignificantDigitsTestData => new()
    {
        { 0.00005, 0.0000m }, // midpoint, rounds to even
        { 0.00015, 0.0002m }, // midpoint, rounds to even
        { 10.00005, 10.0000m },
        { 1234.56789, 1234.5679m },
    };

    [Theory, MemberData(nameof(SignificantDigitsTestData))]
    public void WhenValueIsDouble_ThenItShouldBeRoundedTo15SignificantDigitsFirst(double input, decimal expected)
    {
        var money = new FastMoney(input, "EUR");

        money.Amount.Should().Be(expected);
    }
}
