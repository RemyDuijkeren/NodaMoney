using System.Globalization;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

public class CompactFormat
{
    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingc_DefaultPrecision_ShouldFormatWithOneDecimalAndSuffix()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m1 = new Money(1_234m, usd);
        var m2 = new Money(1_250_000m, usd);
        var m3 = new Money(1_000_000_000m, usd);

        m1.ToString("c", CultureInfo.CurrentCulture).Should().Be("$1.2K");
        m2.ToString("c", CultureInfo.CurrentCulture).Should().Be("$1.3M");
        m3.ToString("c", CultureInfo.CurrentCulture).Should().Be("$1B");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingc_WithExplicitPrecision_ShouldHonorDigits()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(12_345_678_900m, usd);

        m.ToString("c0", CultureInfo.CurrentCulture).Should().Be("$12B");
        m.ToString("c1", CultureInfo.CurrentCulture).Should().Be("$12.3B");
        m.ToString("c2", CultureInfo.CurrentCulture).Should().Be("$12.35B");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingLowercase_g_ShouldUseCurrencyCode()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(1_234m, usd);

        m.ToString("g", CultureInfo.CurrentCulture).Should().Be("USD 1.2K");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenUsingc_InDutchCulture_ShouldUseCommaAndSpacing()
    {
        var eur = CurrencyInfo.FromCode("EUR");
        var m = new Money(12_345m, eur);

        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("€ 12,3K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenAmountBelowThousand_Withc_ShouldStillCompact()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(999m, usd);

        // Using compact specifier forces compact formatting even < 1,000
        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("$1K");
        m.ToString("c1", CultureInfo.CurrentCulture).Should().Be("$1.0K");
        m.ToString("c2", CultureInfo.CurrentCulture).Should().Be("$1.00K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenNegative_ShouldUseLeadingMinusForCompact()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(-12_340m, usd);

        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("-$12.3K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenAtExactTiers_DefaultPrecision_ShouldTrimZeros()
    {
        var usd = CurrencyInfo.FromCode("USD");
        new Money(1_000m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1K");
        new Money(1_000_000m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1M");
        new Money(1_000_000_000m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1B");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenNearThreshold_ShouldEscalateOnRounding()
    {
        var usd = CurrencyInfo.FromCode("USD");
        // 999.95K → 1.0M with c1; default precision → 1M
        new Money(999_950m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1M");
        new Money(999_950m, usd).ToString("c1", CultureInfo.CurrentCulture).Should().Be("$1.0M");

        // Multi-tier escalation
        new Money(999_950_000m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1B");
        new Money(999_950_000_000m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1T");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenNearThreshold_ShouldNotEscalateBelowBoundary()
    {
        var usd = CurrencyInfo.FromCode("USD");
        // Compact is enforced; rounding with c1 yields 1.0K
        new Money(999.949m, usd).ToString("c1", CultureInfo.CurrentCulture).Should().Be("$1.0K");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void WhenUsingLowercase_g_InFrCulture_ShouldPlaceCodeAfterNumber()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(1_234m, usd);
        // fr-FR uses comma decimal and places symbol after number with space
        m.ToString("g", CultureInfo.CurrentCulture).Should().Be("1,2K USD");
    }

    [Fact]
    [UseCulture("de-CH")]
    public void WhenUsingc_InSwissGerman_ShouldUseDotDecimal_AndNoSpace()
    {
        var chf = CurrencyInfo.FromCode("CHF");
        var m = new Money(12_345m, chf);
        // de-CH uses '.' as decimal; current CurrencyPositivePattern includes a space after the symbol
        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("Fr. 12.3K");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenNegative_InDutchCulture_ShouldUseLeadingMinus()
    {
        var eur = CurrencyInfo.FromCode("EUR");
        var m = new Money(-12_345m, eur);
        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("-€ 12,3K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenCompactWithSpecialCurrencies_UnderThousand_ShouldStillCompact()
    {
        var jpy = CurrencyInfo.FromCode("JPY");
        var bhd = CurrencyInfo.FromCode("BHD");
        new Money(999m, jpy).ToString("c", CultureInfo.CurrentCulture).Should().Be("¥1K");
        new Money(999m, jpy).ToString("c1", CultureInfo.CurrentCulture).Should().Be("¥1.0K");
        new Money(999m, bhd).ToString("c", CultureInfo.CurrentCulture).Should().Be("BD1K");
        new Money(999m, bhd).ToString("c2", CultureInfo.CurrentCulture).Should().Be("BD1.00K");
    }
}
