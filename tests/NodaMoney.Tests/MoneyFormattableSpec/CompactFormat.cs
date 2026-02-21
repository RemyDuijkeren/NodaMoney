using System.Globalization;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

public class CompactFormat
{
    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingc_DefaultPrecision_ShouldFormatWithNoSuffixBelowThousandAndSuffixAbove()
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
    public void WhenUsingc_WithExplicitPrecision_ShouldHonorSignificantDigits()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(12_345_678_900m, usd);

        // scaled = 12.345...B
        m.ToString("c1", CultureInfo.CurrentCulture).Should().Be("$10B"); // Correct to 1 sig digit
        m.ToString("c2", CultureInfo.CurrentCulture).Should().Be("$12B");
        m.ToString("c3", CultureInfo.CurrentCulture).Should().Be("$12.3B");
        m.ToString("c4", CultureInfo.CurrentCulture).Should().Be("$12.35B");
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

        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("€ 12K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenAmountBelowThousand_Withc_ShouldNotCompact()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m12_34 = new Money(12.34m, usd);
        var m123_43 = new Money(123.43m, usd);
        var m950 = new Money(950m, usd);

        // < 100: normal currency decimals
        m12_34.ToString("c", CultureInfo.CurrentCulture).Should().Be("$12.34");

        // 100 - 999: no decimals
        m123_43.ToString("c", CultureInfo.CurrentCulture).Should().Be("$123");
        m950.ToString("c", CultureInfo.CurrentCulture).Should().Be("$950");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenNegative_ShouldUseLeadingMinusForCompact()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(-12_340m, usd);

        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("-$12K");
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
        // 999_950m with sigDigits=2 -> 1.0M -> 1M (no trailing zero)
        new Money(999_950m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1M");
        // with sigDigits=3 -> 1.00M -> 1M (no trailing zero)
        new Money(999_950m, usd).ToString("c3", CultureInfo.CurrentCulture).Should().Be("$1M");

        // Multi-tier escalation
        new Money(999_950_000m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1B");
        new Money(999_950_000_000m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1T");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenNearThreshold_ShouldNotEscalateBelowBoundary()
    {
        var usd = CurrencyInfo.FromCode("USD");
        // Compact is NOT enforced below 1000; 999.949 -> 1000? Wait.
        // If it's 999.949, it's < 1000. It should show no decimals. -> 1000?
        // Wait, if it rounds to 1000, it SHOULD compact.
        new Money(999.949m, usd).ToString("c", CultureInfo.CurrentCulture).Should().Be("$1K");
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
        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("Fr. 12K");
    }

    [Fact]
    [UseCulture("nl-NL")]
    public void WhenNegative_InDutchCulture_ShouldUseLeadingMinus()
    {
        var eur = CurrencyInfo.FromCode("EUR");
        var m = new Money(-12_345m, eur);
        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("-€ 12K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenCompactWithSpecialCurrencies_UnderThousand_ShouldNotCompact()
    {
        var jpy = CurrencyInfo.FromCode("JPY");
        var bhd = CurrencyInfo.FromCode("BHD");
        new Money(999m, jpy).ToString("c", CultureInfo.CurrentCulture).Should().Be("¥999");
        new Money(999m, bhd).ToString("c", CultureInfo.CurrentCulture).Should().Be("BD999");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenValueIsUnderHundred_WithCompactFormat_ShouldShowNormalDecimalsNoCompact()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m50 = new Money(50m, usd);
        var m10 = new Money(10m, usd);
        var m5_34 = new Money(1.34m, usd);
        var m1 = new Money(1m, usd);
        var m0_5 = new Money(0.5m, usd);

        m50.ToString("c", CultureInfo.CurrentCulture).Should().Be("$50.00");
        m10.ToString("c", CultureInfo.CurrentCulture).Should().Be("$10.00");
        m5_34.ToString("c", CultureInfo.CurrentCulture).Should().Be("$1.34");
        m1.ToString("c", CultureInfo.CurrentCulture).Should().Be("$1.00");
        m0_5.ToString("c", CultureInfo.CurrentCulture).Should().Be("$0.50");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenValueIsExactlyOneHundred_WithCompactFormat_ShouldNotBeZeroOneK()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m100 = new Money(100m, usd);

        m100.ToString("c", CultureInfo.CurrentCulture).Should().Be("$100");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingRMExamples_ShouldMatch()
    {
        var myr = CurrencyInfo.FromCode("MYR"); // Ringgit Malaysia

        // sigDigits = 1 “very coarse”
        new Money(0.34m, myr).ToString("c1").Should().Be("RM0.34");
        new Money(0.5m, myr).ToString("c1").Should().Be("RM0.50");
        new Money(1.34m, myr).ToString("c1").Should().Be("RM1.34");
        new Money(1.95m, myr).ToString("c1").Should().Be("RM1.95");
        new Money(1.5m, myr).ToString("c1").Should().Be("RM1.50");
        new Money(12.34m, myr).ToString("c1").Should().Be("RM12.34");
        new Money(15.5m, myr).ToString("c1").Should().Be("RM15.50");
        new Money(123.43m, myr).ToString("c1").Should().Be("RM123");
        new Money(950m, myr).ToString("c1").Should().Be("RM950");
        new Money(1_234m, myr).ToString("c1").Should().Be("RM1K");
        new Money(12_345m, myr).ToString("c1").Should().Be("RM10K");
        new Money(123_456m, myr).ToString("c1").Should().Be("RM100K");
        new Money(1_234_567m, myr).ToString("c1").Should().Be("RM1M");
        new Money(12_345_678m, myr).ToString("c1").Should().Be("RM10M");

        // sigDigits = 2 (default) “normal dashboard precision”
        new Money(0.34m, myr).ToString("c").Should().Be("RM0.34");
        new Money(0.5m, myr).ToString("c").Should().Be("RM0.50");
        new Money(1.34m, myr).ToString("c").Should().Be("RM1.34");
        new Money(1.95m, myr).ToString("c").Should().Be("RM1.95");
        new Money(1.5m, myr).ToString("c").Should().Be("RM1.50");
        new Money(12.34m, myr).ToString("c").Should().Be("RM12.34");
        new Money(15.5m, myr).ToString("c").Should().Be("RM15.50");
        new Money(123.43m, myr).ToString("c").Should().Be("RM123");
        new Money(950m, myr).ToString("c").Should().Be("RM950");
        new Money(1_234m, myr).ToString("c").Should().Be("RM1.2K");
        new Money(12_345m, myr).ToString("c").Should().Be("RM12K");
        new Money(123_456m, myr).ToString("c").Should().Be("RM123K");
        new Money(1_234_567m, myr).ToString("c").Should().Be("RM1.2M");
        new Money(12_345_678m, myr).ToString("c").Should().Be("RM12M");

        // sigDigits = 3 “more detailed”
        new Money(0.34m, myr).ToString("c3").Should().Be("RM0.34");
        new Money(0.5m, myr).ToString("c3").Should().Be("RM0.50");
        new Money(1.34m, myr).ToString("c3").Should().Be("RM1.34");
        new Money(1.95m, myr).ToString("c3").Should().Be("RM1.95");
        new Money(1.5m, myr).ToString("c3").Should().Be("RM1.50");
        new Money(12.34m, myr).ToString("c3").Should().Be("RM12.34");
        new Money(15.5m, myr).ToString("c3").Should().Be("RM15.50");
        new Money(123.43m, myr).ToString("c3").Should().Be("RM123");
        new Money(950m, myr).ToString("c3").Should().Be("RM950");
        new Money(1_234m, myr).ToString("c3").Should().Be("RM1.23K");
        new Money(12_345m, myr).ToString("c3").Should().Be("RM12.3K");
        new Money(123_456m, myr).ToString("c3").Should().Be("RM123K");
        new Money(1_234_567m, myr).ToString("c3").Should().Be("RM1.23M");
        new Money(12_345_678m, myr).ToString("c3").Should().Be("RM12.3M");

        // sigDigits = 4 “very detailed”
        new Money(0.34m, myr).ToString("c4").Should().Be("RM0.34");
        new Money(0.5m, myr).ToString("c4").Should().Be("RM0.50");
        new Money(1.34m, myr).ToString("c4").Should().Be("RM1.34");
        new Money(1.95m, myr).ToString("c4").Should().Be("RM1.95");
        new Money(1.5m, myr).ToString("c4").Should().Be("RM1.50");
        new Money(12.34m, myr).ToString("c4").Should().Be("RM12.34");
        new Money(15.5m, myr).ToString("c4").Should().Be("RM15.50");
        new Money(123.43m, myr).ToString("c4").Should().Be("RM123");
        new Money(950m, myr).ToString("c4").Should().Be("RM950");
        new Money(1_234m, myr).ToString("c4").Should().Be("RM1.234K");
        new Money(12_345m, myr).ToString("c4").Should().Be("RM12.35K");
        new Money(123_456m, myr).ToString("c4").Should().Be("RM123.5K");
        new Money(1_234_567m, myr).ToString("c4").Should().Be("RM1.235M");
        new Money(12_345_678m, myr).ToString("c4").Should().Be("RM12.35M");
    }
    [Fact]
    [UseCulture("en-US")]
    public void WhenValueIsZero_ShouldShowZeroWithDecimals()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(0m, usd);

        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("$0.00");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenValueIsVerySmall_ShouldShowNormalDecimals()
    {
        var usd = CurrencyInfo.FromCode("USD");
        new Money(0.001m, usd).ToString("c").Should().Be("$0.00");
        new Money(0.01m, usd).ToString("c").Should().Be("$0.01");
        new Money(0.05m, usd).ToString("c").Should().Be("$0.05");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenValueIsBetweenOneHundredAndThousand_WithExplicitPrecision_ShouldStillShowNoDecimals()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m123_45 = new Money(123.45m, usd);

        // Precision is for compacting (>= 1000). For 100-999, it's always N0.
        m123_45.ToString("c1", CultureInfo.CurrentCulture).Should().Be("$123");
        m123_45.ToString("c4", CultureInfo.CurrentCulture).Should().Be("$123");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingi_CompactFormat_ShouldUseInternationalSymbol()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(1_234m, usd);

        m.ToString("i", CultureInfo.CurrentCulture).Should().Be("US$1.2K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingl_CompactFormat_ShouldUseEnglishName()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(1_234m, usd);

        m.ToString("l", CultureInfo.CurrentCulture).Should().Be("1.2K United States dollar");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingManySigDigits_ShouldWorkForCompactedValues()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(123_456.789m, usd);

        m.ToString("c4", CultureInfo.CurrentCulture).Should().Be("$123.5K");
        m.ToString("c5", CultureInfo.CurrentCulture).Should().Be("$123.46K");
        m.ToString("c6", CultureInfo.CurrentCulture).Should().Be("$123.457K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingHighSigDigits_ShouldTrimTrailingZeros()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(123_000m, usd);

        m.ToString("c6", CultureInfo.CurrentCulture).Should().Be("$123K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenValueIsVeryLarge_ShouldContinueToUseT()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(1_000_000_000_000_000m, usd); // 1 Quadrillion

        m.ToString("c", CultureInfo.CurrentCulture).Should().Be("$1,000T");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenRoundingNearThresholds_ShouldBeConsistent()
    {
        var usd = CurrencyInfo.FromCode("USD");

        // Money(99.995, usd) rounds to 100.00 immediately in constructor.
        // abs = 100.00, so it skips Case 1 and goes to Case 2 (100-999).
        new Money(99.995m, usd).ToString("c").Should().Be("$100");

        // 99.994 rounds to 99.99 in constructor. Case 1 (abs < 100)
        new Money(99.994m, usd).ToString("c").Should().Be("$99.99");

        // 999.95 rounds to 1000 in constructor. Case 3 (Compact).
        new Money(999.95m, usd).ToString("c").Should().Be("$1K");

        // 999.4 rounds to 999 in constructor. Case 2 (100-999).
        new Money(999.4m, usd).ToString("c").Should().Be("$999");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingCustomCurrency_WithManyDecimals_ShouldShowThemUnderHundred()
    {
        var crypto = CurrencyInfo.FromCode("BTC"); // Already registered in CurrencyRegistry
        var m = new Money(1.23456789m, crypto);
        m.ToString("c").Should().Be("₿1.23456789");

        var mLarge = new Money(1234.56789m, crypto);
        mLarge.ToString("c").Should().Be("₿1.2K");
        mLarge.ToString("c6").Should().Be("₿1.23457K");
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenUsingi_WithExplicitPrecision_ShouldWork()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var m = new Money(12_345m, usd);

        m.ToString("i3", CultureInfo.CurrentCulture).Should().Be("US$12.3K");
    }
}
