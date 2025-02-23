namespace NodaMoney.Tests.CurrencyInfoSpec;

public class InitiateInternallyACurrency
{
    [Fact]
    public void WhenParamsAreCorrect_ThenCreatingShouldSucceed()
    {
        var eur = new CurrencyInfo("EUR", 978, MinorUnit.Two, "Euro", "€");

        eur.Code.Should().Be("EUR");
        eur.Number.Should().Be(978);
        eur.DecimalDigits.Should().Be(2);
        eur.EnglishName.Should().Be("Euro");
        eur.Symbol.Should().Be("€");
    }

    [Fact]
    public void WhenCodeIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => new CurrencyInfo(null!, 978, MinorUnit.Two, "Euro", "€");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WhenNumberIsNull_ThenNumberShouldDefaultToEmpty()
    {
        //var eur = new Currency("EUR", null, 2, "Euro", "€");

        //eur.Number.Should().Be(string.Empty);

        var eur = new CurrencyInfo("EUR", 0, MinorUnit.Two, "Euro", "€");

        eur.Number.Should().Be(0);
    }

    [Fact]
    public void WhenEnglishNameIsNull_ThenEnglishNameShouldDefaultToEmpty()
    {
        var eur = new CurrencyInfo("EUR", 978, MinorUnit.Two, null, "€");

        eur.EnglishName.Should().Be(string.Empty);
    }

    [Fact]
    public void WhenSignIsNull_ThenSignShouldDefaultToGenericCurrencySign()
    {
        var eur = new CurrencyInfo("EUR", 978, MinorUnit.Two, "Euro", null);

        eur.Symbol.Should().Be(CurrencyInfo.GenericCurrencySign);
    }

    [Fact]
    public void WhenDecimalDigitIsLowerThenMinusOne_ThenCreatingShouldThrow()
    {
        //Action action = () => { var eur = new Currency("EUR", 978, -2, "Euro", "€"); };

        //action.Should().Throw<ArgumentOutOfRangeException>();
    }
}
