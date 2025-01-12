using System;
using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.CurrencySpec;

public class GivenIWantCurrencyFromIsoCode
{
    [Fact]
    public void WhenIsoCodeIsExisting_ThenCreatingShouldSucceed()
    {
        var currency = Currency.FromCode("EUR");

        currency.Should().NotBeNull();
        currency.Code.Should().Be("EUR");
        // currency.Symbol.Should().Be("€");
        // currency.EnglishName.Should().Be("Euro");
        // currency.IsValid.Should().BeTrue();
    }

    [Fact]
    public void WhenIsoCodeIsUnknown_ThenCreatingShouldThrow()
    {
        Action action = () => Currency.FromCode("AAA");

        action.Should().Throw<InvalidCurrencyException>();
    }

    [Fact]
    public void WhenIsoCodeIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => Currency.FromCode(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenEstionianKrone_ThenItShouldBeObsolete()
    {
        var currency = Currency.FromCode("EEK");

        currency.Should().NotBeNull();
        currency.Code.Should().Be("EEK");
        // currency.Symbol.Should().Be("kr");
        // currency.IsValid.Should().BeFalse();
    }
}

public class GivenIWantToCompareCurrencies
{
    private Currency _euro1 = Currency.FromCode("EUR");

    private Currency _euro2 = Currency.FromCode("EUR");

    private Currency _dollar = Currency.FromCode("USD");

    [Fact]
    public void WhenComparingEquality_ThenCurrencyShouldBeEqual()
    {
        // Compare using Equal()
        _euro1.Should().Be(_euro2);
        _euro1.Should().NotBe(_dollar);
        _euro1.Should().NotBeNull();
        _euro1.Should().NotBe(new object(), "comparing Currency to a different object should fail!");
    }

    [Fact]
    public void WhenComparingStaticEquality_ThenCurrencyShouldBeEqual()
    {
        // Compare using static Equal()
        Currency.Equals(_euro1, _euro2).Should().BeTrue();
        Currency.Equals(_euro1, _dollar).Should().BeFalse();
    }

    [Fact]
    public void WhenComparingWithEqualityOperator_ThenCurrencyShouldBeEqual()
    {
        // Compare using Euality operators
        (_euro1 == _euro2).Should().BeTrue();
        (_euro1 != _dollar).Should().BeTrue();
    }

    [Fact]
    public void WhenComparingHashCodes_ThenCurrencyShouldBeEqual()
    {
        // Compare using GetHashCode()
        _euro1.GetHashCode().Should().Be(_euro2.GetHashCode());
        _euro1.GetHashCode().Should().NotBe(_dollar.GetHashCode());
    }
}

public class GivenIWantToDeconstructCurrency
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
