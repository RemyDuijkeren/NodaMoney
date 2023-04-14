using System;
using System.Globalization;

using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyParsableSpec
{
    public class GivenIWantToParseImplicitCurrency
    {
        [Fact, UseCulture(CultureNames.BelgiumDutch)]
        public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "€ -98.765,43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43m, "EUR"));
        }

        [Fact, UseCulture(CultureNames.BelgiumFrench)]
        public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "-98.765,43 €";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "EUR"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
        {
            const string Value = "-98.765,43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "EUR"));
        }

        [Fact, UseCulture(CultureNames.JapanJapanese)]
        public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
        {
            const string Value = "¥ -98,765";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765m, "JPY"));
        }

        [Fact, UseCulture(CultureNames.ChinaMainlandChinese)]
        public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
        {
            const string Value = "¥ -98,765";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765m, "CNY"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingYenYuanInNetherlands_ThenThisShouldFail()
        {
            // ¥ symbol is used for Japanese money and Chinese money
            const string Value = "¥ -98,765";
            Action action = () => Money.Parse(Value);

            action.Should().Throw<FormatException>().WithMessage("*Specify currency or culture explicitly*");
        }

        [Fact, UseCulture(CultureNames.UnitedStatesEnglish)]
        public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
        {
            const string Value = "$-98,765.43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43m, "USD"));
        }

        [Fact, UseCulture(CultureNames.ArgentinaSpanish)]
        public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
        {
            const string Value = "$-98.765,43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43m, "ARS"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldFail()
        {
            // $ symbol is used for multiple currencies
            const string Value = "$ -98.765,43";
            Action action = () => Money.Parse(Value);

            action.Should().Throw<FormatException>().WithMessage("*Specify currency or culture explicitly*");
        }

        [Fact, UseCulture(CultureNames.UnitedStatesEnglish)]
        public void WhenParsingEuroSymbolInUSA_ThenThisShouldReturnUSDollar()
        {
            const string Value = "€-98,765.43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43m, "EUR"));
        }

        [Fact, UseCulture(CultureNames.SwitzerlandGerman)]
        public void WhenParsingChfSymbolInSwitzerlandGermanSpeaking_ThenThisShouldReturnSwissFranc()
        {
            const string Value = "-98’765.23 CHF";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.23m, "CHF"));
        }

        [Fact]
        public void WhenValueIsNull_ThenThowExeception()
        {
            const string Value = null;
            Action action = () => Money.Parse(Value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenValueIsEmpty_ThenThowExeception()
        {
            const string Value = "";
            Action action = () => Money.Parse(Value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenCurrencyIsUnknown_ThenThowExeception()
        {
            const string Value = "XYZ -98.765,43";
            Action action = () => Money.Parse(Value);

            action.Should().Throw<FormatException>().WithMessage("*must be a known currency*");
        }
    }

    public class GivenIWantToParseExplicitCurrency
    {
        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
        {
            const string Value = "¥ -98.765";
            var money = Money.Parse(Value, Currency.FromCode("JPY"));

            money.Should().Be(new Money(-98765, "JPY"));
        }

        [Fact, UseCulture(CultureNames.UnitedStatesEnglish)]
        public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
        {
            const string Value = "$-98,765.43";
            var money = Money.Parse(Value, Currency.FromCode("ARS"));

            money.Should().Be(new Money(-98765.43m, "ARS"));
        }

        [Fact, UseCulture(CultureNames.ArgentinaSpanish)]
        public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
        {
            const string Value = "$-98.765,43";
            var money = Money.Parse(Value, Currency.FromCode("USD"));

            money.Should().Be(new Money(-98765.43m, "USD"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
        {
            // $ symbol is used for multiple currencies
            const string Value = "$-98.765,43";
            var money = Money.Parse(Value, Currency.FromCode("USD"));

            money.Should().Be(new Money(-98765.43m, "USD"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingSwissFrancInNetherlands_ThenThisShouldSucceed()
        {
            const string Value = "CHF-98.765,43";
            var money = Money.Parse(Value, Currency.FromCode("CHF"));

            money.Should().Be(new Money(-98765.43m, "CHF"));
        }

        [Fact, UseCulture(CultureNames.BelgiumDutch)]
        public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "€ -98.765,43";
            var money = Money.Parse(Value, Currency.FromCode("EUR"));

            money.Should().Be(new Money(-98765.43m, "EUR"));
        }

        [Fact, UseCulture(CultureNames.BelgiumFrench)]
        public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "-98.765,43 €";
            var money = Money.Parse(Value, Currency.FromCode("EUR"));

            money.Should().Be(new Money(-98765.43, "EUR"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
        {
            const string Value = "-98.765,43";
            var money = Money.Parse(Value, Currency.FromCode("USD"));

            money.Should().Be(new Money(-98765.43, "USD"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldFail()
        {
            const string Value = "€ -98.765,43";
            Action action = () => Money.Parse(Value, Currency.FromCode("USD"));

            action.Should().Throw<FormatException>(); //.WithMessage("Input string was not in a correct format.");                
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenValueIsNull_ThenThowExeception()
        {
            const string Value = null;
            Action action = () => Money.Parse(Value, Currency.FromCode("EUR"));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenValueIsEmpty_ThenThowExeception()
        {
            const string Value = "";
            Action action = () => Money.Parse(Value, Currency.FromCode("EUR"));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenValueIsNullWithOverrideMethod_ThenThowExeception()
        {
            const string Value = null;
            Action action = () => Money.Parse(Value, NumberStyles.Currency, null, Currency.FromCode("EUR"));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenValueIsEmptyWithOverrideMethod_ThenThowExeception()
        {
            const string Value = "";
            Action action = () => Money.Parse(Value, NumberStyles.Currency, null, Currency.FromCode("EUR"));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, UseCulture(CultureNames.SwitzerlandGerman)]
        public void WhenParsingSwissFrancInSwitzerlandGermanSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "CHF-98’765.43";
            var money = Money.Parse(Value, Currency.FromCode("CHF"));

            money.Should().Be(new Money(-98765.43m, "CHF"));
        }
    }

    public class GivenIWantToParseNegativeMoney
    {
        [Fact, UseCulture(CultureNames.UnitedStatesEnglish)]
        public void WhenMinusSignBeforeDollarSign_ThenThisShouldSucceed()
        {
            const string Value = "-$98,765.43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "USD"));
        }

        [Fact, UseCulture(CultureNames.UnitedStatesEnglish)]
        public void WhenMinusSignAfterDollarSign_ThenThisShouldSucceed()
        {
            const string Value = "$-98,765.43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "USD"));
        }

        [Fact, UseCulture(CultureNames.UnitedStatesEnglish)]
        public void WhenDollarsWithParentheses_ThenThisShouldSucceed()
        {
            const string Value = "($98,765.43)";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "USD"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenMinusSignBeforeEuroSign_ThenThisShouldSucceed()
        {
            const string Value = "-€ 98.765,43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "EUR"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenMinusSignAfterEuroSign_ThenThisShouldSucceed()
        {
            const string Value = "€ -98.765,43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "EUR"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenEurosWithParentheses_ThenThisShouldSucceed()
        {
            const string Value = "(€ 98.765,43)";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "EUR"));
        }

        [Fact, UseCulture(CultureNames.SwitzerlandGerman)]
        public void WhenMinusSignBeforeChfSign_ThenThisShouldSucceed()
        {
            const string Value = "-CHF 98’765.43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "CHF"));
        }

        [Fact, UseCulture(CultureNames.SwitzerlandGerman)]
        public void WhenMinusSignAfterCHFSign_ThenThisShouldSucceed()
        {
            const string Value = "CHF -98’765.43";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "CHF"));
        }

        [Fact, UseCulture(CultureNames.SwitzerlandGerman)]
        public void WhenChfWithParentheses_ThenThisShouldSucceed()
        {
            const string Value = "(CHF 98’765.43)";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(-98765.43, "CHF"));
        }
    }

    public class GivenIWantToParseMoneyWithMoreDecimalPossibleForCurrency
    {
        [Fact, UseCulture(CultureNames.JapanJapanese)]
        public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedDown()
        {
            const string Value = "¥ 98,765.4";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(98765m, "JPY"));
        }

        [Fact, UseCulture(CultureNames.JapanJapanese)]
        public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedUp()
        {
            const string Value = "¥ 98,765.5";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(98766m, "JPY"));
        }

        [Fact, UseCulture(CultureNames.SwitzerlandGerman)]
        public void WhenParsingSwissFranc_ThenThisShouldBeRoundedUp()
        {
            const string Value = "CHF 98’765.475";
            var money = Money.Parse(Value);

            money.Should().Be(new Money(98765.48m, "CHF"));
        }
    }

    public class GivenIWantToTryParseImplicitCurrency
    {
        [Fact, UseCulture(CultureNames.BelgiumDutch)]
        public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "€ -98.765,43";
            Money.TryParse(Value, out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43m, "EUR"));
        }

        [Fact, UseCulture(CultureNames.BelgiumFrench)]
        public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "-98.765,43 €";
            Money.TryParse(Value, out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43, "EUR"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingNumberWithoutCurrency_ThenThisUseCurrencyOfCurrentCulture()
        {
            const string Value = "-98.765,43";
            Money.TryParse(Value, out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43, "EUR"));
        }

        [Fact, UseCulture(CultureNames.JapanJapanese)]
        public void WhenParsingYenYuanSymbolInJapan_ThenThisShouldReturnJapaneseYen()
        {
            const string Value = "¥ -98,765";
            Money.TryParse(Value, out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765m, "JPY"));
        }

        [Fact, UseCulture(CultureNames.ChinaMainlandChinese)]
        public void WhenParsingYenYuanSymbolInChina_ThenThisShouldReturnChineseYuan()
        {
            const string Value = "¥ -98,765";
            Money.TryParse(Value, out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765m, "CNY"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingYenYuanInNetherlands_ThenThisShouldReturnFalse()
        {
            // ¥ symbol is used for Japanese money and Chinese money
            const string Value = "¥ -98,765";
            Money.TryParse(Value, out Money money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }

        [Fact, UseCulture(CultureNames.UnitedStatesEnglish)]
        public void WhenParsingDollarSymbolInUSA_ThenThisShouldReturnUSDollar()
        {
            const string Value = "$-98,765.43";
            Money.TryParse(Value, out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43m, "USD"));
        }

        [Fact, UseCulture(CultureNames.ArgentinaSpanish)]
        public void WhenParsingDollarSymbolInArgentina_ThenThisShouldReturnArgentinePeso()
        {
            const string Value = "$-98.765,43";
            Money.TryParse(Value, out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43m, "ARS"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingDollarSymbolInNetherlands_ThenThisShouldReturnFalse()
        {
            // $ symbol is used for multiple currencies
            const string Value = "$ -98.765,43";
            Money.TryParse(Value, out Money money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenValueIsNull_ThenReturnFalse()
        {
            const string Value = null;
            Money.TryParse(Value, out Money money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenValueIsEmpty_ThenReturnFalse()
        {
            const string Value = "";
            Money.TryParse(Value, out Money money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }

        [Fact, UseCulture(CultureNames.SwitzerlandGerman)]
        public void WhenInSwitzerlandGermanSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "CHF -98’765.43";
            Money.TryParse(Value, out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43m, "CHF"));
        }
    }

    public class GivenIWantToTryParseExplicitCurrency
    {
        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingYenInNetherlands_ThenThisShouldSucceed()
        {
            const string Value = "¥ -98.765";
            Money.TryParse(Value, Currency.FromCode("JPY"), out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765m, "JPY"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingSwissFrancInNetherlands_ThenThisShouldSucceed()
        {
            const string Value = "CHF -98.765";
            Money.TryParse(Value, Currency.FromCode("CHF"), out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765m, "CHF"));
        }

        [Fact, UseCulture(CultureNames.UnitedStatesEnglish)]
        public void WhenParsingArgentinePesoInUSA_ThenThisShouldReturnArgentinePeso()
        {
            const string Value = "$-98,765.43";
            Money.TryParse(Value, Currency.FromCode("ARS"), out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43m, "ARS"));
        }

        [Fact, UseCulture(CultureNames.ArgentinaSpanish)]
        public void WhenParsingUSDollarSymbolInArgentina_ThenThisShouldReturnUSDollar()
        {
            const string Value = "$-98.765,43";
            Money.TryParse(Value, Currency.FromCode("USD"), out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43m, "USD"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingUSDollarInNetherlands_ThenThisShouldSucceed()
        {
            // $ symbol is used for multiple currencies
            const string Value = "$-98.765,43";
            Money.TryParse(Value, Currency.FromCode("USD"), out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43m, "USD"));
        }

        [Fact, UseCulture(CultureNames.BelgiumDutch)]
        public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "€ -98.765,43";
            Money.TryParse(Value, Currency.FromCode("EUR"), out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43m, "EUR"));
        }

        [Fact, UseCulture(CultureNames.BelgiumFrench)]
        public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "-98.765,43 €";
            Money.TryParse(Value, Currency.FromCode("EUR"), out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43, "EUR"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingNumberWithoutCurrency_ThenThisShouldSucceed()
        {
            const string Value = "-98.765,43";
            Money.TryParse(Value, Currency.FromCode("USD"), out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765.43, "USD"));
        }

        [Fact, UseCulture(CultureNames.NetherlandsDutch)]
        public void WhenParsingUSDollarWithEuroCurrency_ThenThisShouldReturnFalse()
        {
            const string Value = "€ -98.765,43";
            Money.TryParse(Value, Currency.FromCode("USD"), out Money money).Should().BeFalse();

            money.Should().Be(new Money(0m, Currency.FromCode("XXX")));
        }

        [Fact, UseCulture(CultureNames.SwitzerlandGerman)]
        public void WhenParsingSwissFrancInSwitzerlandGermanSpeaking_ThenThisShouldSucceed()
        {
            const string Value = "CHF -98’765";
            Money.TryParse(Value, Currency.FromCode("CHF"), out Money money).Should().BeTrue();

            money.Should().Be(new Money(-98765m, "CHF"));
        }
    }
}
