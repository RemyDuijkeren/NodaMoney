using System;
using System.Globalization;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec
{
    [Collection(nameof(NoParallelization))]
    public class GivenIWantToParseImplicitCurrency
    {
        [Fact, UseCulture("nl-BE")]
        public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
        {
            var euro = Money.Parse("€ 765,43");

            euro.Should().Be(new Money(765.43m, "EUR"));
        }

        [Fact, UseCulture("fr-BE")]
        public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
        {
            var euro = Money.Parse("765,43 €");

            euro.Should().Be(new Money(765.43, "EUR"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
        {
            var euro = Money.Parse("765,43");

            euro.Should().Be(new Money(765.43, "EUR"));
        }

        [Fact, UseCulture("ja-JP")]
        public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
        {
            var yen = Money.Parse("¥ 765");

            yen.Should().Be(new Money(765m, "JPY"));
        }

        [Fact, UseCulture("zh-CN")]
        public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
        {
            var yuan = Money.Parse("¥ 765");

            yuan.Should().Be(new Money(765m, "CNY"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingYenYuanInNetherlands_ThenThisShouldFail()
        {
            // ¥ symbol is used for Japanese yen and Chinese yuan
            Action action = () => Money.Parse("¥ 765");

            action.Should().Throw<FormatException>().WithMessage("*multiple known currencies*");
        }

        [Fact, UseCulture("en-US")]
        public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
        {
            var dollar = Money.Parse("$765.43");

            dollar.Should().Be(new Money(765.43m, "USD"));
        }

        [Fact, UseCulture("es-AR")]
        public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
        {
            var peso = Money.Parse("$765,43");

            peso.Should().Be(new Money(765.43m, "ARS"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldFail()
        {
            // $ symbol is used for multiple currencies
            Action action = () => Money.Parse("$ 765,43");

            action.Should().Throw<FormatException>().WithMessage("*multiple known currencies*");
        }

        [Fact, UseCulture("en-US")]
        public void WhenParsingEuroSymbolInUSA_ThenThisShouldReturnUSDollar()
        {
            var euro = Money.Parse("€765.43");

            euro.Should().Be(new Money(765.43m, "EUR"));
        }

        [Fact]
        public void WhenValueIsNull_ThenThowExeception()
        {
            Action action = () => Money.Parse(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenValueIsEmpty_ThenThowExeception()
        {
            Action action = () => Money.Parse("");

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenCurrencyIsUnknown_ThenThowExeception()
        {
            Action action = () => Money.Parse("XYZ 765,43");

            action.Should().Throw<FormatException>().WithMessage("*unknown currency*");
        }
    }

    [Collection(nameof(NoParallelization))]
    public class GivenIWantToParseExplicitCurrency
    {
        [Fact, UseCulture("nl-NL")]
        public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
        {
            var yen = Money.Parse("¥ 765", Currency.FromCode("JPY"));

            yen.Should().Be(new Money(765, "JPY"));
        }

        [Fact, UseCulture("en-US")]
        public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
        {
            var peso = Money.Parse("$765.43", Currency.FromCode("ARS"));

            peso.Should().Be(new Money(765.43m, "ARS"));
        }

        [Fact, UseCulture("es-AR")]
        public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
        {
            var dollar = Money.Parse("$765,43", Currency.FromCode("USD"));

            dollar.Should().Be(new Money(765.43m, "USD"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
        {
            // $ symbol is used for multiple currencies
            var dollar = Money.Parse("$765,43", Currency.FromCode("USD"));

            dollar.Should().Be(new Money(765.43m, "USD"));
        }

        [Fact, UseCulture("nl-BE")]
        public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
        {
            var euro = Money.Parse("€ 765,43", Currency.FromCode("EUR"));

            euro.Should().Be(new Money(765.43m, "EUR"));
        }

        [Fact, UseCulture("fr-BE")]
        public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
        {
            var euro = Money.Parse("765,43 €", Currency.FromCode("EUR"));

            euro.Should().Be(new Money(765.43, "EUR"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
        {
            var euro = Money.Parse("765,43", Currency.FromCode("USD"));

            euro.Should().Be(new Money(765.43, "USD"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldFail()
        {
            Action action = () => Money.Parse("€ 765,43", Currency.FromCode("USD"));

            action.Should().Throw<FormatException>(); //.WithMessage("Input string was not in a correct format.");                
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenValueIsNull_ThenThowExeception()
        {
            Action action = () => Money.Parse(null, Currency.FromCode("EUR"));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenValueIsEmpty_ThenThowExeception()
        {
            Action action = () => Money.Parse("", Currency.FromCode("EUR"));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenValueIsNullWithOverrideMethod_ThenThowExeception()
        {
            Action action = () => Money.Parse(null, NumberStyles.Currency, null, Currency.FromCode("EUR"));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenValueIsEmptyWithOverrideMethod_ThenThowExeception()
        {
            Action action = () => Money.Parse("", NumberStyles.Currency, null, Currency.FromCode("EUR"));

            action.Should().Throw<ArgumentNullException>();
        }
    }

    [Collection(nameof(NoParallelization))]
    public class GivenIWantToParseNegativeMoney
    {
        [Fact, UseCulture("en-US")]
        public void WhenMinusSignBeforeDollarSign_ThenThisShouldSucceed()
        {
            string value = "-$765.43";
            var dollar = Money.Parse(value);

            dollar.Should().Be(new Money(-765.43, "USD"));
        }

        [Fact, UseCulture("en-US")]
        public void WhenMinusSignAfterDollarSign_ThenThisShouldSucceed()
        {
            string value = "$-765.43";
            var dollar = Money.Parse(value);

            dollar.Should().Be(new Money(-765.43, "USD"));
        }

        [Fact, UseCulture("en-US")]
        public void WhenDollarsWithParentheses_ThenThisShouldSucceed()
        {
            string value = "($765.43)";
            var dollar = Money.Parse(value);

            dollar.Should().Be(new Money(-765.43, "USD"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenMinusSignBeforeEuroSign_ThenThisShouldSucceed()
        {
            string value = "-€ 765,43";
            var dollar = Money.Parse(value);

            dollar.Should().Be(new Money(-765.43, "EUR"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenMinusSignAfterEuroSign_ThenThisShouldSucceed()
        {
            string value = "€ -765,43";
            var dollar = Money.Parse(value);

            dollar.Should().Be(new Money(-765.43, "EUR"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenEurosWithParentheses_ThenThisShouldSucceed()
        {
            string value = "(€ 765,43)";
            var dollar = Money.Parse(value);

            dollar.Should().Be(new Money(-765.43, "EUR"));
        }
    }

    [Collection(nameof(NoParallelization))]
    public class GivenIWantToParseMoneyWithMoreDecimalPossibleForCurrency
    {
        [Fact, UseCulture("ja-JP")]
        public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedDown()
        {
            var yen = Money.Parse("¥ 765.4");

            yen.Should().Be(new Money(765m, "JPY"));
        }

        [Fact, UseCulture("ja-JP")]
        public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedUp()
        {
            var yen = Money.Parse("¥ 765.5");

            yen.Should().Be(new Money(766m, "JPY"));
        }
    }

    [Collection(nameof(NoParallelization))]
    public class GivenIWantToTryParseImplicitCurrency
    {
        [Fact, UseCulture("nl-BE")]
        public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
        {
            Money euro;
            Money.TryParse("€ 765,43", out euro).Should().BeTrue();

            euro.Should().Be(new Money(765.43m, "EUR"));
        }

        [Fact, UseCulture("fr-BE")]
        public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
        {
            Money euro;
            Money.TryParse("765,43 €", out euro).Should().BeTrue();

            euro.Should().Be(new Money(765.43, "EUR"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
        {
            Money euro;
            Money.TryParse("765,43", out euro).Should().BeTrue();

            euro.Should().Be(new Money(765.43, "EUR"));
        }

        [Fact, UseCulture("ja-JP")]
        public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
        {
            Money yen;
            Money.TryParse("¥ 765", out yen).Should().BeTrue();

            yen.Should().Be(new Money(765m, "JPY"));
        }

        [Fact, UseCulture("zh-CN")]
        public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
        {
            Money yuan;
            Money.TryParse("¥ 765", out yuan).Should().BeTrue();

            yuan.Should().Be(new Money(765m, "CNY"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingYenYuanInNetherlands_ThenThisShouldReturnFalse()
        {
            // ¥ symbol is used for Japanese yen and Chinese yuan
            Money money;
            Money.TryParse("¥ 765", out money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }

        [Fact, UseCulture("en-US")]
        public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
        {
            Money dollar;
            Money.TryParse("$765.43", out dollar).Should().BeTrue();

            dollar.Should().Be(new Money(765.43m, "USD"));
        }

        [Fact, UseCulture("es-AR")]
        public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
        {
            Money peso;
            Money.TryParse("$765,43", out peso).Should().BeTrue();

            peso.Should().Be(new Money(765.43m, "ARS"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldReturnFalse()
        {
            // $ symbol is used for multiple currencies
            Money money;
            Money.TryParse("$ 765,43", out money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenValueIsNull_ThenReturnFalse()
        {
            Money money;
            Money.TryParse(null, out money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenValueIsEmpty_ThenReturnFalse()
        {
            Money money;
            Money.TryParse("", out money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }
    }

    [Collection(nameof(NoParallelization))]
    public class GivenIWantToTryParseExplicitCurrency
    {
        [Fact, UseCulture("nl-NL")]
        public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
        {
            Money yen;
            Money.TryParse("¥ 765", Currency.FromCode("JPY"), out yen).Should().BeTrue();

            yen.Should().Be(new Money(765, "JPY"));
        }

        [Fact, UseCulture("en-US")]
        public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
        {
            Money peso;
            Money.TryParse("$765.43", Currency.FromCode("ARS"), out peso).Should().BeTrue();

            peso.Should().Be(new Money(765.43m, "ARS"));
        }

        [Fact, UseCulture("es-AR")]
        public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
        {
            Money dollar;
            Money.TryParse("$765,43", Currency.FromCode("USD"), out dollar).Should().BeTrue();

            dollar.Should().Be(new Money(765.43m, "USD"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
        {
            // $ symbol is used for multiple currencies
            Money dollar;
            Money.TryParse("$765,43", Currency.FromCode("USD"), out dollar).Should().BeTrue();

            dollar.Should().Be(new Money(765.43m, "USD"));
        }

        [Fact, UseCulture("nl-BE")]
        public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
        {
            Money euro;
            Money.TryParse("€ 765,43", Currency.FromCode("EUR"), out euro).Should().BeTrue();

            euro.Should().Be(new Money(765.43m, "EUR"));
        }

        [Fact, UseCulture("fr-BE")]
        public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
        {
            Money euro;
            Money.TryParse("765,43 €", Currency.FromCode("EUR"), out euro).Should().BeTrue();

            euro.Should().Be(new Money(765.43, "EUR"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
        {
            Money euro;
            Money.TryParse("765,43", Currency.FromCode("USD"), out euro).Should().BeTrue();

            euro.Should().Be(new Money(765.43, "USD"));
        }

        [Fact, UseCulture("nl-NL")]
        public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldReturnFalse()
        {
            Money money;
            Money.TryParse("€ 765,43", Currency.FromCode("USD"), out money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }
    }
}
