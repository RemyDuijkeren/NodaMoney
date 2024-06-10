using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencyUnitSpec;

public class CreateCurrencyV2
{
    [Theory]
    [InlineData("EUR")]
    [InlineData("MYR")]
    [InlineData("USD")]
    public void CurrencyIsCreated_GivenCodeIsThreeCapitalLetters(string code)
    {
            var currency = new Currency(code);

            currency.Code.Should().Be(code);
            //currency.IsIso4217.Should().BeTrue();
    }

    [Theory]
    [InlineData("E")]
    [InlineData("EU")]
    [InlineData("EURO")]
    [InlineData("eur")]
    [InlineData("EU1")]
    public void ThrowArgumentException_GivenCodeIsNotThreeCapitalLetters(string code)
    {
            Action act = () => new Currency(code);

            act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ThrowArgumentNullException_GivenCodeIsNull()
    {
            Action act = () => new Currency(null);

            act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CurrencyIsXXX_GivenDefaultCurrency()
    {
            // Arrange / Act
            var noCurrency = new Currency("XXX");
            Currency defaultCurrency = default;

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
            Assert.Equal(defaultCurrency, noCurrency);
            Assert.Equal(Currency.NoCurrency, noCurrency);
    }

    [Fact]
    public void SizeIs2Bytes_GivenCurrencyType()
    {
        int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Currency));

        size.Should().Be(2);
    }
}
