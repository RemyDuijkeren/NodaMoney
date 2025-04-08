using System.Globalization;
using NodaMoney.Exchange;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.ExchangeRateSpec;

[Collection(nameof(NoParallelization))]
public class ParseCurrencyPair
{
    [Fact, UseCulture("en-US")]
    public void WhenInUsCulture_ThenParsingShouldSucceed()
    {
        var fx1 = ExchangeRate.Parse("EUR/USD 1.2591");

        fx1.BaseCurrency.Code.Should().Be("EUR");
        fx1.QuoteCurrency.Code.Should().Be("USD");
        fx1.Value.Should().Be(1.2591M);

        var fx2 = ExchangeRate.Parse("EUR/USD1.2591");

        fx2.BaseCurrency.Code.Should().Be("EUR");
        fx2.QuoteCurrency.Code.Should().Be("USD");
        fx2.Value.Should().Be(1.2591M);
    }

    [Fact, UseCulture("nl-NL")]
    public void WhenInNlCulture_ThenParsingShouldSucceed()
    {
        var fx1 = ExchangeRate.Parse("EUR/USD 1,2591");

        fx1.BaseCurrency.Code.Should().Be("EUR");
        fx1.QuoteCurrency.Code.Should().Be("USD");
        fx1.Value.Should().Be(1.2591M);

        var fx2 = ExchangeRate.Parse("EUR/USD1,2591");

        fx2.BaseCurrency.Code.Should().Be("EUR");
        fx2.QuoteCurrency.Code.Should().Be("USD");
        fx2.Value.Should().Be(1.2591M);
    }

    [Fact, UseCulture("en-US")]
    public void WhenInNlCulture_ThenParsingShouldSucceedWithCultureSpecified()
    {
        var fx = ExchangeRate.Parse("EUR/USD 1,2591", CultureInfo.GetCultureInfo("nl-NL"));

        fx.BaseCurrency.Code.Should().Be("EUR");
        fx.QuoteCurrency.Code.Should().Be("USD");
        fx.Value.Should().Be(1.2591M);
    }

    [Fact]
    public void WhenIsNotANumber_ThenThrowException()
    {
        Action action = () => ExchangeRate.Parse("EUR/USD 1,ABC");

        action.Should().Throw<FormatException>();
    }

    [Fact, UseCulture("en-US")]
    public void WhenHasSameCurrenciesAndValueEqualTo1_ThenParsingShouldSucceed()
    {
        var fx1 = ExchangeRate.Parse("EUR/EUR 1");

        fx1.BaseCurrency.Code.Should().Be("EUR");
        fx1.QuoteCurrency.Code.Should().Be("EUR");
        fx1.Value.Should().Be(1M);
    }

    [Fact, UseCulture("en-US")]
    public void WhenHasSameCurrenciesAndValueNotEqualTo1_ThenThrowException()
    {
        Action action = () => ExchangeRate.Parse("EUR/EUR 1.2591");

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void WhenIsNull_ThenThrowException()
    {
        Action action = () => ExchangeRate.Parse(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenIsEmpty_ThenThrowException()
    {
        Action action = () => ExchangeRate.Parse("");

        action.Should().Throw<FormatException>();
    }
}
