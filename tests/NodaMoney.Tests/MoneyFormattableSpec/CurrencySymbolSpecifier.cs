using System.Globalization;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

public class CurrencySymbolSpecifier
{
    [Fact]
    [UseCulture("en-US")]
    public void Lowercase_c_ShouldUseLocalCurrencySymbol()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var money = new Money(1234.56m, usd);

        money.ToString("c", CultureInfo.CurrentCulture).Should().Be("$1,234.56");
        money.ToString("c0", CultureInfo.CurrentCulture).Should().Be("$1,235");
        money.ToString("c3", CultureInfo.CurrentCulture).Should().Be("$1,234.560");
    }

    [Fact]
    [UseCulture("en-US")]
    public void Uppercase_C_ShouldUseInternationalCurrencySymbol()
    {
        var usd = CurrencyInfo.FromCode("USD");
        var money = new Money(1234.56m, usd);

        money.ToString("C", CultureInfo.CurrentCulture).Should().Be("US$1,234.56");
        money.ToString("C0", CultureInfo.CurrentCulture).Should().Be("US$1,235");
        money.ToString("C3", CultureInfo.CurrentCulture).Should().Be("US$1,234.560");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void InFrenchCulture_c_ShouldUseLocalPlacementAndSymbol()
    {
        var cad = CurrencyInfo.FromCode("CAD");
        var money = new Money(1234.56m, cad);

        // fr-FR places symbol after the number with a (narrow no‑break) space and uses comma as decimal separator
        money.ToString("c", CultureInfo.CurrentCulture).Should().Be("1\u202F234,56 $");
    }

    [Fact]
    [UseCulture("fr-FR")]
    public void InFrenchCulture_C_ShouldUseInternationalSymbolAndPlacement()
    {
        var cad = CurrencyInfo.FromCode("CAD");
        var money = new Money(1234.56m, cad);

        // International symbol for CAD is CA$
        money.ToString("C", CultureInfo.CurrentCulture).Should().Be("1\u202F234,56 CA$");
    }
}
