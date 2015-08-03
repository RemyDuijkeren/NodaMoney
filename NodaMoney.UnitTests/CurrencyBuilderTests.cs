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
            public void WhenRegisterBitCoin_ThenShouldBeAvailable()
            {
                var builder = new CurrencyBuilder("BTC", "virtual");
                builder.Code = "BTC"; // ISOCode
                builder.EnglishName = "Bitcoin";
                builder.Namespace = "virtual";
                builder.Symbol = "฿";
                builder.ISONumber = "123"; // iso number
                builder.DecimalDigits = 4;
                builder.IsObsolete = false;

                // builder.IsVirtual = true; or IsCustom or both
                // builder.AlternativeSymbols = arry[];
                // builder.LegalTender = false;
                // builder.ValidFrom = DateTime.Now; or Validity(from/to or until)
                // builder.ValidTo = DateTime.Now;
                // builder.UsedInCultureRegion.Add(ci);
                // builder.NativeName = ""; 
                // builder.DecimalSeparator = ",";
                // builder.GroupSeparator = ".";
                // builder.GroupSizes = new int[] { 3, 2 };
                // builder.Priority = 2;
                // builder.SubUnit = "Cents";
                // builder.SubUnitToUnit = 100;
                // builder.SignFirst = true;
                // builder.HtmlEntity = "&#x20AC;";
                // builder.SmallestDenomination = 1;
                // numberFormatInfo.Currency
                // numberFormatInfo.CurrencyNegativePattern = 12;
                // numberFormatInfo.CurrencyPositivePattern = 2;
                // numberFormatInfo.CurrencySymbol = "EUR";
                // numberFormatInfo.DigitSubstitution = DigitShapes.None;

                builder.Register();

                Currency bitcoin = Currency.FromCode("BTC");
                bitcoin.Symbol.Should().Be("฿");
            }

            [TestMethod]
            public void WhenRegisterBitCoin_ThenShouldBeAvailableByNamespace()
            {
                var builder = new CurrencyBuilder("BTC1", "virtual");
                builder.Code = "BTC1"; // ISOCode
                builder.EnglishName = "Bitcoin";
                builder.Namespace = "virtual";
                builder.Symbol = "฿";
                builder.ISONumber = "123"; // iso number
                builder.DecimalDigits = 4;
                builder.IsObsolete = false;

                builder.Register();

                Currency bitcoin = Currency.FromCode("BTC1", "virtual");
                bitcoin.Symbol.Should().Be("฿");
            }

            [TestMethod]
            public void WhenFromExistingCurrency_ThenThisShouldSucceed()
            {
                var builder = new CurrencyBuilder("BTC2", "virtual");

                var euro = Currency.FromCode("EUR");
                builder.LoadDataFromCurrency(euro);

                builder.Code.Should().Be("BTC2");
                builder.Namespace.Should().Be("virtual");
                builder.EnglishName.Should().Be(euro.EnglishName);                
                builder.Symbol.Should().Be(euro.Symbol);
                builder.ISONumber.Should().Be(euro.Number);
                builder.DecimalDigits.Should().Be(euro.DecimalDigits);
                builder.IsObsolete.Should().Be(euro.IsObsolete);
            }

            // Currency bitcoin = "BTC";

            // http://www.codeproject.com/Articles/15175/NET-Internationalization-The-Developer-s-Guide-to
            // https://msdn.microsoft.com/en-us/library/system.globalization.cultureandregioninfobuilder%28v=vs.110%29.aspx
            // Save to Unicode Technical Standard #35. It is an extensible XML format for the exchange of structured locale data
            // builder.Save("en-GB.ldml");

            // CultureAndRegionInfoBuilder builder = CultureAndRegionInfoBuilder.CreateFromLdml("en-GB.ldml");
            // builder.Register();

            // foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.UserCustomCulture))
            // CultureTypes.UserCustomCulture
            // CultureAndRegionInfoBuilderHelper.Export(new CultureInfo("cy-GB"), "cy-GB.ldml","en-GB", "en-GB");
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
                builder.Code = "XYZ"; // ISOCode
                builder.EnglishName = "Xyz";
                builder.Namespace = "virtual";
                builder.Symbol = "฿";
                builder.ISONumber = "123";  // iso number
                builder.DecimalDigits = 4;
                builder.IsObsolete = false;

                builder.Register();
                Currency xyz = Currency.FromCode("XYZ"); // should work

                CurrencyBuilder.Unregister("XYZ", "virtual");
                Action action = () => Currency.FromCode("XYZ");

                action.ShouldThrow<ArgumentException>().WithMessage("*unknown*currency*");
            }

            [TestMethod]
            public void WhenCurrencyDoesntExist_ThenThisShouldThrow()
            {
                Action action = () => CurrencyBuilder.Unregister("ABC", "virtual");

                action.ShouldThrow<ArgumentException>().WithMessage("*specifies a currency that is not found*");
            }
        }
    }
}