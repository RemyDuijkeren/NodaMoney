using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests
{
    public class CurrencyBuilderTests
    {
        public class GivenIWantToCreateCustomCurrency
        {
            [Fact]
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

            [Fact]
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

            [Fact]
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

            [Fact]
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

        public class GivenIWantToUnregisterCurrency
        {
            [Fact]
            public void WhenUnregisterIsoCurrency_ThenThisMustSucceed()
            {
                var euro = Currency.FromCode("PEN"); // should work

                CurrencyBuilder.Unregister("PEN", "ISO-4217");
                Action action = () => Currency.FromCode("PEN");

                action.ShouldThrow<ArgumentException>().WithMessage("*unknown*currency*");                
            }

            [Fact]
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

            [Fact]
            public void WhenCurrencyDoesntExist_ThenThisShouldThrow()
            {
                Action action = () => CurrencyBuilder.Unregister("ABC", "virtual");

                action.ShouldThrow<ArgumentException>().WithMessage("*specifies a currency that is not found*");
            }
        }

        public class GivenIWantToReplaceIsoCurrencyWithOwnVersion
        {
            [Fact]
            public void WhenReplacingEuroWithCustom_ThenThisShouldSucceed()
            {
                // Panamanian balboa
                Currency oldEuro = CurrencyBuilder.Unregister("PAB", "ISO-4217");

                var builder = new CurrencyBuilder("PAB", "ISO-4217");
                builder.LoadDataFromCurrency(oldEuro);
                builder.EnglishName = "New Panamanian balboa";
                builder.DecimalDigits = 1;

                builder.Register();

                Currency newEuro = Currency.FromCode("PAB");
                newEuro.Symbol.Should().Be("B/.");
                newEuro.EnglishName.Should().Be("New Panamanian balboa");
                newEuro.DecimalDigits.Should().Be(1);
            }
        }
    }
}