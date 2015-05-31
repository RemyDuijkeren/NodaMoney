using System;
using System.Globalization;

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
                builder.Sign = "฿"; //symbol?                
                builder.ISONumber = "123";  // iso number
                builder.DecimalDigits = 4;                
                builder.IsObsolete = false;
                //builder.IsVirtual = true; or IsCustom or both
                //builder.AlternativeSigns = arry[];
                //builder.LegalTender = false;
                //builder.ValidFrom = DateTime.Now; or Validity(from/to or until)
                //builder.ValidTo = DateTime.Now;
                //builder.UsedInCultureRegion.Add(ci);
                //builder.NativeName = ""; 
                //builder.DecimalSeparator = ",";
                //builder.GroupSeparator = ".";
                //builder.GroupSizes = new int[] { 3, 2 };
                //builder.Priority = 2;
                //builder.SubUnit = "Cents";
                //builder.SubUnitToUnit = 100;
                //builder.SignFirst = true;
                //builder.HtmlEntity = "&#x20AC;";
                //builder.SmallestDenomination = 1;
                //numberFormatInfo.Currency
                //numberFormatInfo.CurrencyNegativePattern = 12;
                //numberFormatInfo.CurrencyPositivePattern = 2;
                //numberFormatInfo.CurrencySymbol = "EUR";
                //numberFormatInfo.DigitSubstitution = DigitShapes.None;

                builder.Register();

                Currency bitcoin = Currency.FromCode("BTC");
                

                bitcoin.Sign.Should().Be("฿");
            }

            //Currency bitcoin = "BTC";

            //Currency(string code, string number, double decimalDigits, string englishName, string sign)
            //{ "AED", new Currency("AED", "784", 2, "United Arab Emirates dirham", "د.إ") },

            //http://www.codeproject.com/Articles/15175/NET-Internationalization-The-Developer-s-Guide-to
            //https://msdn.microsoft.com/en-us/library/system.globalization.cultureandregioninfobuilder%28v=vs.110%29.aspx
            // Save to Unicode Technical Standard #35. It is an extensible XML format for the exchange of structured locale data
            //builder.Save("en-GB.ldml");

            //CultureAndRegionInfoBuilder builder = CultureAndRegionInfoBuilder.CreateFromLdml("en-GB.ldml");
            //builder.Register();

            //CultureAndRegionInfoBuilder.Unregister("en-GB");

            // load in the data from the existing culture and region
            //builder.LoadDataFromCultureInfo(cultureInfo);
            //builder.LoadDataFromRegionInfo(regionInfo);

            //foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.UserCustomCulture))
            //CultureTypes.UserCustomCulture
            //CultureAndRegionInfoBuilderHelper.Export(new CultureInfo("cy-GB"), "cy-GB.ldml","en-GB", "en-GB");
        }
    }
}