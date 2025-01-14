using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencySpec;

public class GivenIWantDefaultCurrency
{
    [Fact]
    public void WhenCreatingVariableWithDefault_ThenShouldBeEqualToNoCurrency()
    {
        // Arrange / Act
        Currency currency = default;

        // Assert
        var expected = Currency.FromCode("XXX");

        currency.Should().NotBeNull();
        currency.Should().Be(expected);
        currency.Code.Should().Be(expected.Code);
        currency.Symbol.Should().Be(expected.Symbol);
    }

    [Fact]
    public void WhenNoCurrency_ThenItShouldBeEqualToDefault()
    {
        // Arrange / Act
        Currency noCurrency = Currency.FromCode("XXX");

        // Assert
        noCurrency.Should().NotBeNull();
        noCurrency.Should().Be(default(Currency));

        // Assert with XUnit methods, because https://stackoverflow.com/questions/61556309/fluent-assertions-be-vs-equals
        Assert.Equal(default, noCurrency);
        Assert.Equal(default(Currency), (object)noCurrency);
        Assert.True(noCurrency == default);
        Assert.True(noCurrency == default(Currency));
        Assert.True(noCurrency.Equals(default));
        Assert.True(noCurrency.Equals((object)default(Currency)));
        Assert.True(object.Equals(noCurrency, default(Currency)));
    }
}
