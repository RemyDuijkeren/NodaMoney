using System;
using System.Globalization;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.CurrencyInfoSpec;

public class GivenIWantToKnowAllCurrencies
{
    [Fact]
    public void WhenAskingForIt_ThenAllCurrenciesShouldBeReturned()
    {
        var currencies = CurrencyInfo.GetAllCurrencies();

        currencies.Should().NotBeEmpty();
        currencies.Should().HaveCountGreaterThan(100);
    }

    //// [Fact(Skip = "For debugging.")]
    //public void WriteAllRegionsToFile()
    //{
    //    using (var stream = File.Open(@"..\..\Regions.txt", FileMode.Create))
    //    using (var writer = new StreamWriter(stream))
    //    {
    //        foreach (var c in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
    //        {
    //            var reg = new RegionInfo(c.LCID);
    //            writer.WriteLine("CultureName: {0}", c.Name);
    //            writer.WriteLine("CultureEnglishName: {0}", c.EnglishName);
    //            writer.WriteLine("Name: {0}", reg.Name);
    //            writer.WriteLine("NativeName: {0}", reg.NativeName);
    //            writer.WriteLine("EnglishName: {0}", reg.EnglishName);
    //            writer.WriteLine("DisplayName: {0}", reg.DisplayName);
    //            writer.WriteLine("CurrencySymbol: {0}", reg.CurrencySymbol);
    //            writer.WriteLine("ISOCurrencySymbol: {0}", reg.ISOCurrencySymbol);
    //            writer.WriteLine("CurrencyEnglishName: {0}", reg.CurrencyEnglishName);
    //            writer.WriteLine("CurrencyNativeName: {0}", reg.CurrencyNativeName);
    //            writer.WriteLine(string.Empty);
    //        }
    //    }
    //}

    //// [Fact(Skip = "For debugging.")]
    //public void WriteAllCurrenciesToFile()
    //{
    //    using (var stream = File.Open(@"..\..\ISOCurrencies1.txt", FileMode.Create))
    //    using (var writer = new StreamWriter(stream))
    //    {
    //        foreach (var currency in Currency.GetAllCurrencies())
    //        {
    //            writer.WriteLine("EnglishName: {0}", currency.EnglishName);
    //            writer.WriteLine("Code: {0}, Number: {1}, Sign: {2}", currency.Code, currency.Number, currency.Symbol);
    //            writer.WriteLine(
    //                "MajorUnit: {0}, MinorUnit: {1}, DecimalDigits: {2}",
    //                currency.MajorUnit,
    //                currency.MinorUnit,
    //                currency.DecimalDigits);
    //            writer.WriteLine(string.Empty);
    //        }
    //    }
    //}

    //// [Fact(Skip = "For debugging.")]
    //public void WriteAllCurrencySymbolsToFile()
    //{
    //    var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
    //    var symbolLookup = new Dictionary<String, String>();

    //    using (var stream = File.Open(@"..\..\ISOSymbols.txt", FileMode.Create))
    //    using (var writer = new StreamWriter(stream))
    //    {
    //        foreach (var culture in cultures)
    //        {
    //            var regionInfo = new RegionInfo(culture.LCID);
    //            symbolLookup[regionInfo.ISOCurrencySymbol] = regionInfo.CurrencySymbol;
    //        }

    //        foreach (var keyvalue in symbolLookup.OrderBy(s => s.Key))
    //        {
    //            writer.WriteLine("Code: {0}, Sign: {1}", keyvalue.Key, keyvalue.Value);
    //            writer.WriteLine(string.Empty);
    //        }
    //    }
    //}
}

public class GivenIWantCurrencyInfoFromIsoCode
{
    [Fact]
    public void WhenIsoCodeIsExisting_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromCode("EUR");

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
        currency.IsHistoric.Should().BeFalse();
    }

    [Fact]
    public void WhenIsoCodeIsUnknown_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromCode("AAA");

        action.Should().Throw<InvalidCurrencyException>();
    }

    [Fact]
    public void WhenIsoCodeIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromCode(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenEstionianKrone_ThenItShouldBeObsolete()
    {
        var currency = CurrencyInfo.FromCode("EEK");

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("kr");
        currency.IsHistoric.Should().BeTrue();
    }
}

public class GivenIWantCurrencyFromRegionOrCulture
{
    [Fact]
    public void WhenUsingRegionInfo_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromRegion(new RegionInfo("NL"));

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }

    [Fact]
    public void WhenRegionInfoIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromRegion((RegionInfo)null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenUsingRegionName_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromRegion("NL");

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }

    [Fact]
    public void WhenUsingRegionNameThatIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromRegion((string)null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenUsingCultureInfo_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromCulture(CultureInfo.CreateSpecificCulture("nl-NL"));

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }

    [Fact]
    public void WhenCultureInfoIsNull_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromCulture(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCultureInfoIsNeutralCulture_ThenCreatingShouldThrow()
    {
        Action action = () => CurrencyInfo.FromCulture(new CultureInfo("en"));

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WhenUsingCultureName_ThenCreatingShouldSucceed()
    {
        var currency = CurrencyInfo.FromRegion("nl-NL");

        currency.Should().NotBeNull();
        currency.Symbol.Should().Be("€");
        currency.Code.Should().Be("EUR");
        currency.EnglishName.Should().Be("Euro");
    }
}

public class GivenIWantToKnowSmallestPossibleAmountOfCurrency
{
    private CurrencyInfo _eur = CurrencyInfo.FromCode("EUR");

    private CurrencyInfo _yen = CurrencyInfo.FromCode("JPY");

    private CurrencyInfo _din = CurrencyInfo.FromCode("BHD");

    private CurrencyInfo _mga = CurrencyInfo.FromCode("MGA"); // Malagasy ariary

    private CurrencyInfo _xau = CurrencyInfo.FromCode("XAU"); // Gold

    [Fact]
    public void WhenEuro_ThenShouldBeDividedBy100()
    {
        _eur.MinorUnit.Should().Be(MinorUnit.Two);
        // _eur.MinorUnit.Should().Be(100);
        _eur.MinimalAmount.Should().Be(0.01m);
        _eur.DecimalDigits.Should().Be(2);
    }

    [Fact]
    public void WhenYen_ThenShouldBeDividedByNothing()
    {
        _yen.MinorUnit.Should().Be(MinorUnit.Zero);
        _yen.MinimalAmount.Should().Be(1m);
        _yen.DecimalDigits.Should().Be(0);
    }

    [Fact]
    public void WhenDinar_ThenShouldBeDividedBy1000()
    {
        _din.MinorUnit.Should().Be(MinorUnit.Three);
        // _din.MinorUnit.Should().Be(1000);
        _din.MinimalAmount.Should().Be(0.001m);
        _din.DecimalDigits.Should().Be(3);
    }

    [Fact]
    public void WhenGold_ThenShouldBeDividedByNothing()
    {
        _xau.MinorUnit.Should().Be(MinorUnit.NotApplicable); // N.A.
        _xau.MinimalAmount.Should().Be(1m);
        _xau.DecimalDigits.Should().Be(0);
    }

    [Fact]
    public void WhenMalagasyAriary_ThenShouldBeDividedBy5()
    {
        // The Malagasy ariary are technically divided into five subunits, where the coins display "1/5" on their face and
        // are referred to as a "fifth"; These are not used in practice, but when written out, a single significant digit
        // is used. E.g. 1.2 UM.
        _mga.MinorUnit.Should().Be(MinorUnit.OneFifth); // 1/5 = 10^Log1010(5) => exponent Log10(5)
        // _mga.MinorUnit.Should().Be(5); // 1/5
        _mga.MinimalAmount.Should().Be(0.2m);
        _mga.DecimalDigits.Should().Be(1); // According to ISO-4217 this is 2
    }
}

public class GivenIWantToInitiateInternallyACurrency
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
        Action action = () => { var eur = new CurrencyInfo(null, 978, MinorUnit.Two, "Euro", "€"); };

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

public class GivenIWantToValidateTheDateRange
{
    [Fact]
    public void WhenValidatingACurrencyThatIsAlwaysActive_ThenShouldSucceed()
    {
        var currency = CurrencyInfo.FromCode("EUR");

        currency.IntroducedOn.Should().BeNull();
        currency.ExpiredOn.Should().BeNull();

        currency.IsActiveOn(DateTime.Today).Should().BeTrue();
        currency.IsHistoric.Should().BeFalse();
    }

    [Fact]
    public void WhenValidatingACurrencyThatIsActiveUntilACertainDate_ThenShouldBeActiveStrictlyBeforeThatDate()
    {
        var currency = CurrencyInfo.FromCode("VEB");

        currency.IntroducedOn.Should().BeNull();
        currency.ExpiredOn.Should().Be(new DateTime(2008, 1, 1));

        currency.IsActiveOn(DateTime.MinValue).Should().BeTrue();
        currency.IsActiveOn(DateTime.MaxValue).Should().BeFalse();
        currency.IsActiveOn(new DateTime(2007, 12, 31)).Should().BeTrue();
        // assumes that the until date given in the wikipedia article is excluding.
        // assumption based on the fact that some dates are the first of the month/year
        // and that the euro started at 1999-01-01. Given that the until date of e.g. the Dutch guilder
        // is 1999-01-01, the until date must be excluding
        currency.IsActiveOn(new DateTime(2008, 1, 1)).Should().BeTrue("the until date is excluding");
    }

    [Fact]
    public void WhenValidatingACurrencyThatIsActiveFromACertainDate_ThenShouldBeActiveFromThatDate()
    {
        var currency = CurrencyInfo.FromCode("VES");

        currency.IntroducedOn.Should().Be(new DateTime(2018, 8, 20));
        currency.ExpiredOn.Should().BeNull();

        currency.IsActiveOn(DateTime.MinValue).Should().BeFalse();
        currency.IsActiveOn(DateTime.MaxValue).Should().BeTrue();
        currency.IsActiveOn(new DateTime(2018, 8, 19)).Should().BeFalse();
        currency.IsActiveOn(new DateTime(2018, 8, 20)).Should().BeTrue();
    }
}

[Collection(nameof(NoParallelization))]
public class GivenIWantCurrentCurrency
{
    [Fact]
    [UseCulture("en-US")]
    public void WhenCurrentCultureIsUS_ThenCurrencyIsDollar()
    {
        var currency = CurrencyInfo.CurrentCurrency;

        currency.Should().Be(CurrencyInfo.FromCode("USD"));
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenCurrentCultureIsNL_ThenCurrencyIsEuro()
    {
        var currency = CurrencyInfo.CurrentCurrency;

        currency.Should().Be(CurrencyInfo.FromCode("EUR"));
    }

    [Fact]
    [UseCulture(null)]
    public void WhenCurrentCultureIsInvariant_ThenCurrencyIsDefault()
    {
        var currency = CurrencyInfo.CurrentCurrency;

        currency.Should().Be(CurrencyInfo.NoCurrency);
    }
}
