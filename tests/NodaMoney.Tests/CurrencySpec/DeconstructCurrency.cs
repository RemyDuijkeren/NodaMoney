using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencySpec;

public class DeconstructCurrency
{
    [Fact]
    public void WhenDeConstructing_ThenShouldSucceed()
    {
        var currency = Currency.FromCode("EUR");

        var (code, symbol) = currency;

        code.Should().Be("EUR");
        symbol.Should().Be("€");
    }
}
