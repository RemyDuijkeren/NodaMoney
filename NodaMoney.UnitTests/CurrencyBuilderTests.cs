using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodaMoney.UnitTests
{
    public class CurrencyBuilderTests
    {
        [TestClass]
        public class GivenIWantToCreateCustomCurrency
        {
            [TestMethod]
            public void WhenRegisterBitCoinInIsoNamespace_ThenShouldBeAvailable()
            {
                var builder = new CurrencyBuilder("BTC", "ISO-4217");
                builder.EnglishName = "Bitcoin";
                builder.Symbol = "฿";
                builder.ISONumber = "123"; // iso number
                builder.DecimalDigits = 8;
                builder.IsObsolete = false;

                Currency result = builder.Register();

                Currency bitcoin = Currency.FromCode("BTC");
                bitcoin.Symbol.Should().Be("฿");
                bitcoin.ShouldBeEquivalentTo(result);
            }

            [TestMethod]
            public void WhenRegisterBitCoin_ThenShouldBeAvailableByExplicitNamespace()
            {
                var builder = new CurrencyBuilder("BTC1", "virtual");
                builder.EnglishName = "Bitcoin";
                builder.Symbol = "฿";
                builder.ISONumber = "123"; // iso number
                builder.DecimalDigits = 8;
                builder.IsObsolete = false;

                Currency result = builder.Register();

                Currency bitcoin = Currency.FromCode("BTC1", "virtual");
                bitcoin.Symbol.Should().Be("฿");
                bitcoin.ShouldBeEquivalentTo(result);
            }

            [TestMethod]
            public void WhenBuildBitCoin_ThenItShouldSuccedButNotBeRegistered()
            {
                var builder = new CurrencyBuilder("BTC2", "virtual");
                builder.EnglishName = "Bitcoin";
                builder.Symbol = "฿";
                builder.ISONumber = "123"; // iso number
                builder.DecimalDigits = 8;
                builder.IsObsolete = false;

                Currency result = builder.Build();
                result.Symbol.Should().Be("฿");

                Action action = () => Currency.FromCode("BTC2", "virtual");
                action.ShouldThrow<ArgumentException>().WithMessage("BTC2 is an unknown virtual currency code!");
            }

            [TestMethod]
            public void WhenFromExistingCurrency_ThenThisShouldSucceed()
            {
                var builder = new CurrencyBuilder("BTC3", "virtual");

                var euro = Currency.FromCode("EUR");
                builder.LoadDataFromCurrency(euro);

                builder.Code.Should().Be("BTC3");
                builder.Namespace.Should().Be("virtual");
                builder.EnglishName.Should().Be(euro.EnglishName);
                builder.Symbol.Should().Be(euro.Symbol);
                builder.ISONumber.Should().Be(euro.Number);
                builder.DecimalDigits.Should().Be(euro.DecimalDigits);
                builder.IsObsolete.Should().Be(euro.IsObsolete);
                builder.ValidFrom.Should().Be(euro.ValidFrom);
                builder.ValidTo.Should().Be(euro.ValidTo);
            }
        }

        [TestClass]
        public class GivenIWantToUnregisterCurrency
        {
            [TestMethod]
            public void WhenUnregisterIsoCurrency_ThenThisMustSucceed()
            {
                var euro = Currency.FromCode("EUR"); // should work

                CurrencyBuilder.Unregister("EUR", "ISO-4217");
                Action action = () => Currency.FromCode("EUR");

                action.ShouldThrow<ArgumentException>().WithMessage("*unknown*currency*");

                // register again for other unit-tests
                var builder = new CurrencyBuilder("EUR", "ISO-4217");
                builder.LoadDataFromCurrency(euro);
                builder.Register();
            }

            [TestMethod]
            public void WhenUnregisterCustomCurrency_ThenThisMustSucceed()
            {
                var builder = new CurrencyBuilder("XYZ", "virtual");
                builder.EnglishName = "Xyz";
                builder.Symbol = "฿";
                builder.ISONumber = "123"; // iso number
                builder.DecimalDigits = 4;
                builder.IsObsolete = false;

                builder.Register();
                Currency xyz = Currency.FromCode("XYZ", "virtual"); // should work

                CurrencyBuilder.Unregister("XYZ", "virtual");
                Action action = () => Currency.FromCode("XYZ", "virtual");

                action.ShouldThrow<ArgumentException>().WithMessage("*unknown*currency*");
            }

            [TestMethod]
            public void WhenCurrencyDoesntExist_ThenThisShouldThrow()
            {
                Action action = () => CurrencyBuilder.Unregister("ABC", "virtual");

                action.ShouldThrow<ArgumentException>().WithMessage("*specifies a currency that is not found*");
            }
        }

        [TestClass]
        public class GivenIWantToReplaceIsoCurrencyWithOwnVersion
        {
            [TestMethod]
            public void WhenReplacingEuroWithCustom_ThenThisShouldSucceed()
            {
                Currency oldEuro = CurrencyBuilder.Unregister("EUR", "ISO-4217");

                var builder = new CurrencyBuilder("EUR", "ISO-4217");
                builder.LoadDataFromCurrency(oldEuro);
                builder.EnglishName = "New Euro";
                builder.DecimalDigits = 1;

                builder.Register();

                Currency newEuro = Currency.FromCode("EUR");
                newEuro.Symbol.Should().Be("€");
                newEuro.EnglishName.Should().Be("New Euro");
                newEuro.DecimalDigits.Should().Be(1);
            }
        }
    }
}