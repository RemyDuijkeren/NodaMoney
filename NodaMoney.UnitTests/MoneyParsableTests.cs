using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaMoney.UnitTests.Helpers;

namespace NodaMoney.UnitTests
{
    static internal class MoneyParsableTests
    {
        [TestClass]
        public class GivenIWantToParseImplicitCurrencySymbolUsedInMultipleCurrencies
        {
            [TestMethod]
            public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
            {
                using (new SwitchCulture("ja-JP"))
                {
                    var yen = Money.Parse("¥ 765");

                    yen.Should().Be(new Money(765m, "JPY"));
                }
            }

            [TestMethod]
            public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
            {
                using (new SwitchCulture("zh-CN"))
                {
                    var yen = Money.Parse("¥ 765");

                    yen.Should().Be(new Money(765m, "CNY"));
                }
            }

            [TestMethod]
            public void WhenParsingYenYuanInNetherlands_ThenThisShouldFail()
            {
                // ¥ symbol is used for Japanese yen and Chinese yuan
                using (new SwitchCulture("nl-NL"))
                {
                    Action action = () => Money.Parse("¥ 765");

                    action.ShouldThrow<FormatException>().WithMessage("*multiple known currencies*");
                }
            }

            [TestMethod]
            public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
            {
                using (new SwitchCulture("en-US"))
                {
                    var yen = Money.Parse("$765.43");

                    yen.Should().Be(new Money(765.43m, "USD"));
                }
            }

            [TestMethod]
            public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
            {
                using (new SwitchCulture("es-AR"))
                {
                    var yen = Money.Parse("$765,43");

                    yen.Should().Be(new Money(765.43m, "ARS"));
                }
            }

            [TestMethod]
            public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldFail()
            {
                // $ symbol is used for multiple currencies
                using (new SwitchCulture("nl-NL"))
                {
                    Action action = () => Money.Parse("$ 765,43");

                    action.ShouldThrow<FormatException>().WithMessage("*multiple known currencies*");
                }
            }
        }

        [TestClass]
        public class GivenIWantToParseImplicitCurrency
        {
            [TestMethod]
            public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-BE"))
                {
                    var euro = Money.Parse("€ 765,43");

                    euro.Should().Be(new Money(765.43m, "EUR"));
                }
            }

            [TestMethod]
            public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("fr-BE"))
                {
                    string value = "765,43 €";
                    var euro = Money.Parse(value);

                    euro.Should().Be(new Money(765.43, "EUR"));
                }
            }
        }

        // var yen = Money.Parse("¥ 765");
        // var euro = Money.Parse("€ 765,43");
        // var dollar = Money.Parse("$ 765,43");
        // var dinar = Money.Parse("BD 765,432");

        // var n1 = Money.Parse("¤765.43");
        // var d1 = Money.Parse("$765.43");
        // var p1 = Money.Parse("£765.43");
        // var e1 = Money.Parse("765.43 €");
        // var e1 = Money.Parse("kr 765.43");
        
        // yen.Should().Be(new Money(765m, "JPY"));
        // euro.Should().Be(new Money(765.43m, "EUR"));
        // dollar.Should().Be(new Money(765.43m, "USD"));
        // dinar.Should().Be(new Money(765.432m, "BHD"));
    }
}