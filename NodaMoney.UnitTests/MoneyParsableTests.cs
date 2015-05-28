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
                    var euro = Money.Parse("765,43 €");

                    euro.Should().Be(new Money(765.43, "EUR"));
                }
            }

            [TestMethod]
            public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    var euro = Money.Parse("765,43");

                    euro.Should().Be(new Money(765.43, "EUR"));
                }
            }

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
                    var yuan = Money.Parse("¥ 765");

                    yuan.Should().Be(new Money(765m, "CNY"));
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
                    var dollar = Money.Parse("$765.43");

                    dollar.Should().Be(new Money(765.43m, "USD"));
                }
            }

            [TestMethod]
            public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
            {
                using (new SwitchCulture("es-AR"))
                {
                    var peso = Money.Parse("$765,43");

                    peso.Should().Be(new Money(765.43m, "ARS"));
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
        public class GivenIWantToParseExplicitCurrency
        {
            [TestMethod]
            public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    var yen = Money.Parse("¥ 765", Currency.FromCode("JPY"));

                    yen.Should().Be(new Money(765, "JPY"));
                }
            }

            [TestMethod]
            public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
            {
                using (new SwitchCulture("en-US"))
                {
                    var peso = Money.Parse("$765.43", Currency.FromCode("ARS"));

                    peso.Should().Be(new Money(765.43m, "ARS"));
                }
            }

            [TestMethod]
            public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
            {
                using (new SwitchCulture("es-AR"))
                {
                    var dollar = Money.Parse("$765,43", Currency.FromCode("USD"));

                    dollar.Should().Be(new Money(765.43m, "USD"));
                }
            }

            [TestMethod]
            public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
            {
                // $ symbol is used for multiple currencies
                using (new SwitchCulture("nl-NL"))
                {
                    var dollar = Money.Parse("$765,43", Currency.FromCode("USD"));

                    dollar.Should().Be(new Money(765.43m, "USD"));
                }
            }

            [TestMethod]
            public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-BE"))
                {
                    var euro = Money.Parse("€ 765,43", Currency.FromCode("EUR"));

                    euro.Should().Be(new Money(765.43m, "EUR"));
                }
            }

            [TestMethod]
            public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("fr-BE"))
                {
                    var euro = Money.Parse("765,43 €", Currency.FromCode("EUR"));

                    euro.Should().Be(new Money(765.43, "EUR"));
                }
            }

            [TestMethod]
            public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    var euro = Money.Parse("765,43", Currency.FromCode("USD"));

                    euro.Should().Be(new Money(765.43, "USD"));
                }
            }

            [TestMethod]
            public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldFail()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    Action action = () => Money.Parse("€ 765,43", Currency.FromCode("USD"));

                    action.ShouldThrow<FormatException>().WithMessage("Input string was not in a correct format.");
                }
            }
        }

        [TestClass]
        public class GivenIWantToParseNegativeMoney
        {
            [TestMethod]
            public void WhenMinusSignBeforeDollarSign_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    string value = "-$765.43";
                    var dollar = Money.Parse(value);

                    dollar.Should().Be(new Money(-765.43, "USD"));
                }
            }

            [TestMethod]
            public void WhenMinusSignAfterDollarSign_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    string value = "$-765.43";
                    var dollar = Money.Parse(value);

                    dollar.Should().Be(new Money(-765.43, "USD"));
                }
            }

            [TestMethod]
            public void WhenDollarsWithParentheses_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("en-US"))
                {
                    string value = "($765.43)";
                    var dollar = Money.Parse(value);

                    dollar.Should().Be(new Money(-765.43, "USD"));
                }
            }

            [TestMethod]
            public void WhenMinusSignBeforeEuroSign_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    string value = "-€ 765,43";
                    var dollar = Money.Parse(value);

                    dollar.Should().Be(new Money(-765.43, "EUR"));
                }
            }

            [TestMethod]
            public void WhenMinusSignAfterEuroSign_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    string value = "€ -765,43";
                    var dollar = Money.Parse(value);

                    dollar.Should().Be(new Money(-765.43, "EUR"));
                }
            }

            [TestMethod]
            public void WhenEurosWithParentheses_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    string value = "(€ 765,43)";
                    var dollar = Money.Parse(value);

                    dollar.Should().Be(new Money(-765.43, "EUR"));
                }
            }
        }

        [TestClass]
        public class GivenIWantToParseMoneyWithMoreDecimalPossibleForCurrency
        {
            [TestMethod]
            public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedDown()
            {
                using (new SwitchCulture("ja-JP"))
                {
                    var yen = Money.Parse("¥ 765.4");

                    yen.Should().Be(new Money(765m, "JPY"));
                }
            }

            [TestMethod]
            public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedUp()
            {
                using (new SwitchCulture("ja-JP"))
                {
                    var yen = Money.Parse("¥ 765.5");

                    yen.Should().Be(new Money(766m, "JPY"));
                }
            }
        }

        [TestClass]
        public class GivenIWantToTryParseImplicitCurrency
        {
            [TestMethod]
            public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-BE"))
                {
                    Money euro;
                    Money.TryParse("€ 765,43", out euro).Should().BeTrue();
                    euro.Should().Be(new Money(765.43m, "EUR"));
                }
            }

            [TestMethod]
            public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("fr-BE"))
                {
                    Money euro;
                    Money.TryParse("765,43 €", out euro).Should().BeTrue();
                    euro.Should().Be(new Money(765.43, "EUR"));
                }
            }

            [TestMethod]
            public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    Money euro;
                    Money.TryParse("765,43", out euro).Should().BeTrue();
                    euro.Should().Be(new Money(765.43, "EUR"));
                }
            }

            [TestMethod]
            public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
            {
                using (new SwitchCulture("ja-JP"))
                {
                    Money yen;
                    Money.TryParse("¥ 765", out yen).Should().BeTrue();
                    yen.Should().Be(new Money(765m, "JPY"));
                }
            }

            [TestMethod]
            public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
            {
                using (new SwitchCulture("zh-CN"))
                {
                    Money yuan;
                    Money.TryParse("¥ 765", out yuan).Should().BeTrue();
                    yuan.Should().Be(new Money(765m, "CNY"));
                }
            }

            [TestMethod]
            public void WhenParsingYenYuanInNetherlands_ThenThisShouldReturnFalse()
            {
                // ¥ symbol is used for Japanese yen and Chinese yuan
                using (new SwitchCulture("nl-NL"))
                {
                    Money money;
                    Money.TryParse("¥ 765", out money).Should().BeFalse();
                    money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
                }
            }

            [TestMethod]
            public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
            {
                using (new SwitchCulture("en-US"))
                {
                    Money dollar;
                    Money.TryParse("$765.43", out dollar).Should().BeTrue();
                    dollar.Should().Be(new Money(765.43m, "USD"));
                }
            }

            [TestMethod]
            public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
            {
                using (new SwitchCulture("es-AR"))
                {
                    Money peso;
                    Money.TryParse("$765,43", out peso).Should().BeTrue();
                    peso.Should().Be(new Money(765.43m, "ARS"));
                }
            }

            [TestMethod]
            public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldReturnFalse()
            {
                // $ symbol is used for multiple currencies
                using (new SwitchCulture("nl-NL"))
                {
                    Money money;
                    Money.TryParse("$ 765,43", out money).Should().BeFalse();
                    money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
                }
            }
        }

        [TestClass]
        public class GivenIWantToTryParseExplicitCurrency
        {
            [TestMethod]
            public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    Money yen;
                    Money.TryParse("¥ 765", Currency.FromCode("JPY"), out yen).Should().BeTrue();
                    yen.Should().Be(new Money(765, "JPY"));
                }
            }

            [TestMethod]
            public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
            {
                using (new SwitchCulture("en-US"))
                {
                    Money peso;
                    Money.TryParse("$765.43", Currency.FromCode("ARS"), out peso).Should().BeTrue();
                    peso.Should().Be(new Money(765.43m, "ARS"));
                }
            }

            [TestMethod]
            public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
            {
                using (new SwitchCulture("es-AR"))
                {
                    Money dollar;
                    Money.TryParse("$765,43", Currency.FromCode("USD"), out dollar).Should().BeTrue();
                    dollar.Should().Be(new Money(765.43m, "USD"));
                }
            }

            [TestMethod]
            public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
            {
                // $ symbol is used for multiple currencies
                using (new SwitchCulture("nl-NL"))
                {
                    Money dollar;
                    Money.TryParse("$765,43", Currency.FromCode("USD"), out dollar).Should().BeTrue();
                    dollar.Should().Be(new Money(765.43m, "USD"));
                }
            }

            [TestMethod]
            public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-BE"))
                {
                    Money euro;
                    Money.TryParse("€ 765,43", Currency.FromCode("EUR"), out euro).Should().BeTrue();

                    euro.Should().Be(new Money(765.43m, "EUR"));
                }
            }

            [TestMethod]
            public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("fr-BE"))
                {
                    Money euro;
                    Money.TryParse("765,43 €", Currency.FromCode("EUR"), out euro).Should().BeTrue();
                    euro.Should().Be(new Money(765.43, "EUR"));
                }
            }

            [TestMethod]
            public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    Money euro;
                    Money.TryParse("765,43", Currency.FromCode("USD"), out euro).Should().BeTrue();
                    euro.Should().Be(new Money(765.43, "USD"));
                }
            }

            [TestMethod]
            public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldReturnFalse()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    Money money;
                    Money.TryParse("€ 765,43", Currency.FromCode("USD"), out money).Should().BeFalse();
                    money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
                }
            }
        }
    }
}