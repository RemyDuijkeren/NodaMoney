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
        public class GivenIWantToParseStringToMoney
        {
            [TestMethod]
            public void WhenParsingYenInJapan_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("ja-JP"))
                {
                    var yen = Money.Parse("¥ 765");

                    yen.Should().Be(new Money(765m, "JPY"));
                }
            }

            [TestMethod]
            public void WhenParsingYenInNetherlands_ThenThisShouldFail()
            {
                using (new SwitchCulture("nl-NL"))
                {
                    Action action = () => Money.Parse("¥ 765");

                    action.ShouldThrow<FormatException>().WithMessage("*multiple known currencies*");
                }
            }

            [TestMethod]
            public void WhenInBelgiumDutchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-BE"))
                {
                    // var yen = Money.Parse("¥ 765");
                    var euro = Money.Parse("€ 765,43");
                    // var dollar = Money.Parse("$ 765,43");
                    // var dinar = Money.Parse("BD 765,432");

                    // yen.Should().Be(new Money(765m, "JPY"));
                    euro.Should().Be(new Money(765.43m, "EUR"));
                    // dollar.Should().Be(new Money(765.43m, "USD"));
                    // dinar.Should().Be(new Money(765.432m, "BHD"));
                }
            }

            [TestMethod]
            public void WhenInBelgiumFrenchSpeaking_ThenThisShouldSucceed()
            {
                using (new SwitchCulture("nl-BE"))
                {
                    string value = "765,43 €";
                    var euro = Money.Parse(value);

                    euro.Should().Be(new Money(765.43, "EUR"));
                }
            }
        }
    }
}