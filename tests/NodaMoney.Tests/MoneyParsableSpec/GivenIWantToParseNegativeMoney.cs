using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToParseNegativeMoney
{
    [Fact, UseCulture("en-US")]
    public void WhenMinusSignBeforeDollarSign_ThenThisShouldSucceed()
    {
        string value = "-$765.43";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-765.43, "USD"));
    }

    [Fact, UseCulture("en-US")]
    public void WhenMinusSignAfterDollarSign_ThenThisShouldSucceed()
    {
        string value = "$-765.43";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-765.43, "USD"));
    }

    [Fact, UseCulture("en-US")]
    public void WhenDollarsWithParentheses_ThenThisShouldSucceed()
    {
        string value = "($765.43)";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-765.43, "USD"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenMinusSignBeforeEuroSign_ThenThisShouldSucceed()
    {
        string value = "-€ 765,43";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-765.43, "EUR"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenMinusSignAfterEuroSign_ThenThisShouldSucceed()
    {
        string value = "€ -765,43";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-765.43, "EUR"));
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenEurosWithParentheses_ThenThisShouldSucceed()
    {
        string value = "(€ 765,43)";
        var dollar = Money.Parse(value);

        dollar.Should().Be(new Money(-765.43, "EUR"));
    }
}
