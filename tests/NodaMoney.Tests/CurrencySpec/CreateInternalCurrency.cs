namespace NodaMoney.Tests.CurrencySpec;

public class CreateInternalCurrency
{
    [Theory]
    [InlineData("EUR")]
    [InlineData("MYR")]
    [InlineData("USD")]
    public void WhenIsoCodeIsThreeCapitalLetters_IsoCurrencyIsCreated(string code)
    {
        // Arrange / Act
        var currency = new Currency(code);

        // Assert
        currency.Code.Should().Be(code);
        currency.IsIso4217.Should().BeTrue();
    }

    [Theory]
    [InlineData("E")]
    [InlineData("EU")]
    [InlineData("EURO")]
    [InlineData("eur")]
    [InlineData("EU1")]
    public void WhenCodeIsNotThreeCapitalLetters_ThrowArgumentException(string code)
    {
        Action act = () => new Currency(code);

        act.Should().Throw<ArgumentException>().WithMessage("Currency code should only exist out of three capital letters*");
    }

    [Fact]
    public void WhenCodeIsNull_ThrowArgumentNullException()
    {
        Action act = () => new Currency((string)null);

        act.Should().Throw<ArgumentException>().WithMessage("Value cannot be null*");
    }

    [Fact]
    public void WhenDefaultCurrency_CurrencyIsXXX()
    {
        // Arrange / Act
        var noCurrency = new Currency("XXX");
        Currency defaultCurrency = default;

        // Assert
        noCurrency.Should().NotBeNull();
        noCurrency.Should().Be(default(Currency));
        noCurrency.Code.Should().Be("XXX");
        noCurrency.IsIso4217.Should().BeTrue();
        defaultCurrency.Should().NotBeNull();
        defaultCurrency.Should().Be(default(Currency));
        defaultCurrency.Code.Should().Be("XXX");
        defaultCurrency.IsIso4217.Should().BeTrue();

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
    public void WhenCurrencyType_SizeIs2Bytes()
    {
        int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Currency));

        size.Should().Be(2);
    }
}
