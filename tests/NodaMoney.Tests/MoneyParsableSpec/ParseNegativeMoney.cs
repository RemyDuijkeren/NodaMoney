using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class ParseNegativeMoney
{
    [Fact, UseCulture("en-US")]
    public void WhenMinusSignBeforeDollarSign_ThenThisShouldSucceed()
    {
        string value = "-$98,765.43";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-98_765.43, "USD"));
    }

    [Fact, UseCulture("en-US")]
    public void WhenMinusSignAfterDollarSign_ThenThisShouldSucceed()
    {
        string value = "$-98,765.43";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-98_765.43, "USD"));
    }

    [Fact, UseCulture("en-US")]
    public void WhenDollarsWithParentheses_ThenThisShouldSucceed()
    {
        string value = "($98,765.43)";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-98_765.43, "USD"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenMinusSignBeforeEuroSign_ThenThisShouldSucceed()
    {
        string value = "-€ 98.765,43";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-98_765.43, "EUR"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenMinusSignAfterEuroSign_ThenThisShouldSucceed()
    {
        string value = "€ -98.765,43";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-98_765.43, "EUR"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenEurosWithParentheses_ThenThisShouldSucceed()
    {
        string value = "(€ 98.765,43)";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-98_765.43, "EUR"));
    }

    [Fact, UseCulture("de-CH")]
    public void WhenMinusSignBeforeChfSign_ThenThisShouldSucceed()
    {
        var money = Money.Parse("-CHF 98’765.43");

        money.Should().Be(new Money(-98765.43, "CHF"));
    }

    [Fact, UseCulture("de-CH")]
    public void WhenMinusSignAfterCHFSign_ThenThisShouldSucceed()
    {
        var money = Money.Parse("CHF -98’765.43");

        money.Should().Be(new Money(-98765.43, "CHF"));
    }

    [Fact, UseCulture("de-CH")]
    public void WhenChfWithParentheses_ThenThisShouldSucceed()
    {
        var money = Money.Parse("(CHF 98’765.43)");

        money.Should().Be(new Money(-98765.43, "CHF"));
    }
}
