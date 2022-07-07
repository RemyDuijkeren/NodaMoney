using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using System.Xml.Serialization;
using NodaMoney.Tests.Helpers;
using FluentAssertions.Common;
using Xunit.Abstractions;

namespace NodaMoney.Tests.CurrencySpec
{
    public class GivenIWantToKnowAllCurrencies
    {
        [Fact]
        public void WhenAskingForIt_ThenAllCurrenciesShouldBeReturned()
        {
            var currencies = Currency.GetAllCurrencies();

            currencies.Should().NotBeEmpty();
            currencies.Should().HaveCountGreaterThan(100);
        }

        //// [Fact(Skip = "For debugging.")]
        //public void WriteAllRegionsToFile()
        //{
        //    using (var stream = File.Open(@"..\..\Regions.txt", FileMode.Create))
        //    using (var writer = new StreamWriter(stream))
        //    {
        //        foreach (var c in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        //        {
        //            var reg = new RegionInfo(c.LCID);
        //            writer.WriteLine("CultureName: {0}", c.Name);
        //            writer.WriteLine("CultureEnglishName: {0}", c.EnglishName);
        //            writer.WriteLine("Name: {0}", reg.Name);
        //            writer.WriteLine("NativeName: {0}", reg.NativeName);
        //            writer.WriteLine("EnglishName: {0}", reg.EnglishName);
        //            writer.WriteLine("DisplayName: {0}", reg.DisplayName);
        //            writer.WriteLine("CurrencySymbol: {0}", reg.CurrencySymbol);
        //            writer.WriteLine("ISOCurrencySymbol: {0}", reg.ISOCurrencySymbol);
        //            writer.WriteLine("CurrencyEnglishName: {0}", reg.CurrencyEnglishName);
        //            writer.WriteLine("CurrencyNativeName: {0}", reg.CurrencyNativeName);
        //            writer.WriteLine(string.Empty);
        //        }
        //    }
        //}

        //// [Fact(Skip = "For debugging.")]
        //public void WriteAllCurrenciesToFile()
        //{
        //    using (var stream = File.Open(@"..\..\ISOCurrencies1.txt", FileMode.Create))
        //    using (var writer = new StreamWriter(stream))
        //    {
        //        foreach (var currency in Currency.GetAllCurrencies())
        //        {
        //            writer.WriteLine("EnglishName: {0}", currency.EnglishName);
        //            writer.WriteLine("Code: {0}, Number: {1}, Sign: {2}", currency.Code, currency.Number, currency.Symbol);
        //            writer.WriteLine(
        //                "MajorUnit: {0}, MinorUnit: {1}, DecimalDigits: {2}",
        //                currency.MajorUnit,
        //                currency.MinorUnit,
        //                currency.DecimalDigits);
        //            writer.WriteLine(string.Empty);
        //        }
        //    }
        //}

        //// [Fact(Skip = "For debugging.")]
        //public void WriteAllCurrencySymbolsToFile()
        //{
        //    var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        //    var symbolLookup = new Dictionary<String, String>();

        //    using (var stream = File.Open(@"..\..\ISOSymbols.txt", FileMode.Create))
        //    using (var writer = new StreamWriter(stream))
        //    {
        //        foreach (var culture in cultures)
        //        {
        //            var regionInfo = new RegionInfo(culture.LCID);
        //            symbolLookup[regionInfo.ISOCurrencySymbol] = regionInfo.CurrencySymbol;
        //        }

        //        foreach (var keyvalue in symbolLookup.OrderBy(s => s.Key))
        //        {
        //            writer.WriteLine("Code: {0}, Sign: {1}", keyvalue.Key, keyvalue.Value);
        //            writer.WriteLine(string.Empty);
        //        }
        //    }
        //}

        [Theory]
        [InlineData("EUR")]
        [InlineData("MYR")]
        [InlineData("USD")]
        public void WhenTryToFitCodeIn2Bytes_ThenItShouldBePossible(string currencyCode)
        {
            // EUR = 69, 85, 82 => 4, 20, 17
            byte[] A_InputBytes = currencyCode.ToCharArray().Select(c => (byte)(c - 'A')).ToArray();
            var A_InputArray = A_InputBytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).ToArray();
            
            // store in ushort = 2bytes (15bits needed, 1bit left)
            ushort B_Storage = (ushort)(A_InputBytes[0]<<10 | A_InputBytes[1]<<5| A_InputBytes[2]);
            var B_StorageString = Convert.ToString(B_Storage, 2).PadLeft(16, '0');
            string[] B_StorageArray = {B_StorageString.Substring(0, 8), B_StorageString.Substring(8, 8)};

            // shift into bytes again with clearing left 3 bits (by using & 0b0001_1111 = 0x1F = 31)
            var C_OutputBytes = new[] { (byte)(B_Storage>>10), (byte)(B_Storage>>5 & 0x1F), (byte)(B_Storage & 0x1F) };
            var C_OutputArray = C_OutputBytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).ToArray();
            
            var outputCode = new string(C_OutputBytes.Select(b => (char)(b + 'A')).ToArray());

            outputCode.Should().Be(currencyCode);
            
            // ushort for storing code (2bytes) = 15bits needed, 1bit left => use 1bit to mark if ISO? ISO=0, 1=other?
            // byte for storing namespace (4bits=15 or 3bits=7) and minor unit (4bits=15 or 35bit=31)? or use CurrencyInfo to retrieve?
        }
    }
    
    public class GivenIWantCurrencyFromIsoCode
    {
        [Fact]
        public void WhenIsoCodeIsExisting_ThenCreatingShouldSucceed()
        {
            var currency = Currency.FromCode("EUR");

            currency.Should().NotBeNull();
            currency.Symbol.Should().Be("€");
            currency.Code.Should().Be("EUR");
            currency.EnglishName.Should().Be("Euro");
            currency.IsValid.Should().BeTrue();
        }

        [Fact]
        public void WhenIsoCodeIsUnknown_ThenCreatingShouldThrow()
        {
            Action action = () => Currency.FromCode("AAA");

            action.Should().Throw<InvalidCurrencyException>();
        }

        [Fact]
        public void WhenIsoCodeIsNull_ThenCreatingShouldThrow()
        {
            Action action = () => Currency.FromCode(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenEstionianKrone_ThenItShouldBeObsolete()
        {
            var currency = Currency.FromCode("EEK");

            currency.Should().NotBeNull();
            currency.Symbol.Should().Be("kr");
            currency.IsValid.Should().BeFalse();
        }
    }

    public class GivenIWantCurrencyFromRegionOrCulture
    {
        [Fact]
        public void WhenUsingRegionInfo_ThenCreatingShouldSucceed()
        {
            var currency = Currency.FromRegion(new RegionInfo("NL"));

            currency.Should().NotBeNull();
            currency.Symbol.Should().Be("€");
            currency.Code.Should().Be("EUR");
            currency.EnglishName.Should().Be("Euro");
        }

        [Fact]
        public void WhenRegionInfoIsNull_ThenCreatingShouldThrow()
        {
            Action action = () => Currency.FromRegion((RegionInfo)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenUsingRegionName_ThenCreatingShouldSucceed()
        {
            var currency = Currency.FromRegion("NL");

            currency.Should().NotBeNull();
            currency.Symbol.Should().Be("€");
            currency.Code.Should().Be("EUR");
            currency.EnglishName.Should().Be("Euro");
        }

        [Fact]
        public void WhenUsingRegionNameThatIsNull_ThenCreatingShouldThrow()
        {
            Action action = () => Currency.FromRegion((string)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenUsingCultureInfo_ThenCreatingShouldSucceed()
        {
            var currency = Currency.FromCulture(CultureInfo.CreateSpecificCulture("nl-NL"));

            currency.Should().NotBeNull();
            currency.Symbol.Should().Be("€");
            currency.Code.Should().Be("EUR");
            currency.EnglishName.Should().Be("Euro");
        }

        [Fact]
        public void WhenCultureInfoIsNull_ThenCreatingShouldThrow()
        {
            Action action = () => Currency.FromCulture(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenCultureInfoIsNeutralCulture_ThenCreatingShouldThrow()
        {
            Action action = () => Currency.FromCulture(new CultureInfo("en"));

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void WhenUsingCultureName_ThenCreatingShouldSucceed()
        {
            var currency = Currency.FromRegion("nl-NL");

            currency.Should().NotBeNull();
            currency.Symbol.Should().Be("€");
            currency.Code.Should().Be("EUR");
            currency.EnglishName.Should().Be("Euro");
        }
    }

    [Collection(nameof(NoParallelization))]
    public class GivenIWantCurrentCurrency
    {
        [Fact]
        [UseCulture("en-US")]
        public void WhenCurrentCultureIsUS_ThenCurrencyIsDollar()
        {
            var currency = Currency.CurrentCurrency;

            currency.Should().Be(Currency.FromCode("USD"));
        }

        [Fact]
        [UseCulture("nl-NL")]
        public void WhenCurrentCultureIsNL_ThenCurrencyIsEuro()
        {
            var currency = Currency.CurrentCurrency;

            currency.Should().Be(Currency.FromCode("EUR"));
        }

        [Fact]
        [UseCulture(null)]
        public void WhenCurrentCultureIsInvariant_ThenCurrencyIsDefault()
        {
            var currency = Currency.CurrentCurrency;

            currency.Should().Be(default(Currency));
        }
    }

    public class GivenIWantToCompareCurrencies
    {
        private Currency _euro1 = Currency.FromCode("EUR");

        private Currency _euro2 = Currency.FromCode("EUR");

        private Currency _dollar = Currency.FromCode("USD");

        [Fact]
        public void WhenComparingEquality_ThenCurrencyShouldBeEqual()
        {
            // Compare using Equal()
            _euro1.Should().Be(_euro2);
            _euro1.Should().NotBe(_dollar);
            _euro1.Should().NotBeNull();
            _euro1.Should().NotBe(new object(), "comparing Currency to a different object should fail!");
        }

        [Fact]
        public void WhenComparingStaticEquality_ThenCurrencyShouldBeEqual()
        {
            // Compare using static Equal()
            Currency.Equals(_euro1, _euro2).Should().BeTrue();
            Currency.Equals(_euro1, _dollar).Should().BeFalse();
        }

        [Fact]
        public void WhenComparingWithEqualityOperator_ThenCurrencyShouldBeEqual()
        {
            // Compare using Euality operators
            (_euro1 == _euro2).Should().BeTrue();
            (_euro1 != _dollar).Should().BeTrue();
        }

        [Fact]
        public void WhenComparingHashCodes_ThenCurrencyShouldBeEqual()
        {
            // Compare using GetHashCode()
            _euro1.GetHashCode().Should().Be(_euro2.GetHashCode());
            _euro1.GetHashCode().Should().NotBe(_dollar.GetHashCode());
        }
    }

    public class GivenIWantToKnowSmallestPossibleAmountOfCurrency
    {
        private Currency _eur = Currency.FromCode("EUR");

        private Currency _yen = Currency.FromCode("JPY");

        private Currency _din = Currency.FromCode("BHD");

        private Currency _mga = Currency.FromCode("MGA"); // Malagasy ariary

        private Currency _xau = Currency.FromCode("XAU"); // Gold            

        [Fact]
        public void WhenEuro_ThenShouldBeDividedBy100()
        {
            _eur.MinorUnit.Should().Be(2);
            // _eur.MinorUnit.Should().Be(100);
            _eur.MinimalAmount.Should().Be(0.01m);
            _eur.DecimalDigits.Should().Be(2);
        }

        [Fact]
        public void WhenYen_ThenShouldBeDividedByNothing()
        {
            _yen.MinorUnit.Should().Be(0);
            _yen.MinimalAmount.Should().Be(1m);
            _yen.DecimalDigits.Should().Be(0);
        }

        [Fact]
        public void WhenDinar_ThenShouldBeDividedBy1000()
        {
            _din.MinorUnit.Should().Be(3);
            // _din.MinorUnit.Should().Be(1000);
            _din.MinimalAmount.Should().Be(0.001m);
            _din.DecimalDigits.Should().Be(3);
        }

        [Fact]
        public void WhenGold_ThenShouldBeDividedByNothing()
        {
            _xau.MinorUnit.Should().Be(0); // N.A.
            _xau.MinimalAmount.Should().Be(1m);
            _xau.DecimalDigits.Should().Be(0);
        }

        [Fact]
        public void WhenMalagasyAriary_ThenShouldBeDividedBy5()
        {
            // The Malagasy ariary are technically divided into five subunits, where the coins display "1/5" on their face and
            // are referred to as a "fifth"; These are not used in practice, but when written out, a single significant digit
            // is used. E.g. 1.2 UM.
            _mga.MinorUnit.Should().Be(Math.Log10(5)); // 1/5 = 10^Log1010(5) => exponent Log10(5)
            // _mga.MinorUnit.Should().Be(5); // 1/5
            _mga.MinimalAmount.Should().Be(0.2m);
            _mga.DecimalDigits.Should().Be(1); // According to ISO-4217 this is 2
        }
    }

    public class GivenIWantToInitiateInternallyACurrency
    {
        [Fact]
        public void WhenParamsAreCorrect_ThenCreatingShouldSucceed()
        {
            var eur = new Currency("EUR", 978, 2, "Euro", "€");

            eur.Code.Should().Be("EUR");
            eur.Number.Should().Be(978);
            eur.DecimalDigits.Should().Be(2);
            eur.EnglishName.Should().Be("Euro");
            eur.Symbol.Should().Be("€");
        }

        [Fact]
        public void WhenCodeIsNull_ThenCreatingShouldThrow()
        {
            Action action = () => { var eur = new Currency(null, 978, 2, "Euro", "€"); };

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void WhenNumberIsNull_ThenNumberShouldDefaultToEmpty()
        {
            //var eur = new Currency("EUR", null, 2, "Euro", "€");

            //eur.Number.Should().Be(string.Empty);

            var eur = new Currency("EUR", 0, 2, "Euro", "€");

            eur.Number.Should().Be(0);
        }

        [Fact]
        public void WhenEnglishNameIsNull_ThenEnglishNameShouldDefaultToEmpty()
        {
            var eur = new Currency("EUR", 978, 2, null, "€");

            eur.EnglishName.Should().Be(string.Empty);
        }

        [Fact]
        public void WhenSignIsNull_ThenSignShouldDefaultToGenericCurrencySign()
        {
            var eur = new Currency("EUR", 978, 2, "Euro", null);

            eur.Symbol.Should().Be(Currency.GenericCurrencySign);
        }

        [Fact]
        public void WhenDecimalDigitIsLowerThenMinusOne_ThenCreatingShouldThrow()
        {
            //Action action = () => { var eur = new Currency("EUR", 978, -2, "Euro", "€"); };

            //action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }

    public class GivenIWantToSerializeCurrencyWitXmlSerializer
    {
        private Currency yen = Currency.FromCode("JPY");

        private Currency euro = Currency.FromCode("EUR");

        [Fact]
        public void WhenSerializingYen_ThenThisShouldSucceed()
        {
            //Console.WriteLine(StreamToString(Serialize(yen)));
            StreamToString(Serialize(yen));

            yen.Should().Be(Clone<Currency>(yen));
        }

        [Fact]
        public void WhenSerializingEuro_ThenThisShouldSucceed()
        {
            //Console.WriteLine(StreamToString(Serialize(euro)));
            StreamToString(Serialize(euro));

            euro.Should().Be(Clone<Currency>(euro));
        }

        public static Stream Serialize(object source)
        {
            Stream stream = new MemoryStream();
            XmlSerializer xmlSerializer = new XmlSerializer(source.GetType());
            xmlSerializer.Serialize(stream, source);
            return stream;
        }

        public static T Deserialize<T>(Stream stream)
        {
            stream.Position = 0L;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(stream);
        }

        private static T Clone<T>(object source)
        {
            return Deserialize<T>(Serialize(source));
        }

        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public class GivenIWantToDeconstructCurrency
    {
        [Fact]
        public void WhenDeConstructing_ThenShouldSucceed()
        {
            var currency = Currency.FromCode("EUR");

            var (code, number, symbol) = currency;

            code.Should().Be("EUR");
            number.Should().Be("978");
            symbol.Should().Be("€");
        }
    }

    public class GivenIWantToValidateTheDateRange
    {
        [Fact]
        public void WhenValidatingACurrencyThatIsAlwaysValid_ThenShouldSucceed()
        {
            var currency = Currency.FromCode("EUR");

            currency.ValidFrom.Should().BeNull();
            currency.ValidTo.Should().BeNull();

            currency.IsValidOn(DateTime.Today).Should().BeTrue();
        }

        [Fact]
        public void WhenValidatingACurrencyThatIsValidUntilACertainDate_ThenShouldBeValidStrictlyBeforeThatDate()
        {
            var currency = Currency.FromCode("VEB");

            currency.ValidFrom.Should().BeNull();
            currency.ValidTo.Should().Be(new DateTime(2008, 1, 1));

            currency.IsValidOn(DateTime.MinValue).Should().BeTrue();
            currency.IsValidOn(DateTime.MaxValue).Should().BeFalse();
            currency.IsValidOn(new DateTime(2007, 12, 31)).Should().BeTrue();
            // assumes that the until date given in the wikipedia article is excluding.
            // assumption based on the fact that some dates are the first of the month/year
            // and that the euro started at 1999-01-01. Given that the until date of e.g. the Dutch guilder
            // is 1999-01-01, the until date must be excluding
            currency.IsValidOn(new DateTime(2008, 1, 1)).Should().BeTrue("the until date is excluding");
        }

        [Fact]
        public void WhenValidatingACurrencyThatIsValidFromACertainDate_ThenShouldBeValidFromThatDate()
        {
            var currency = Currency.FromCode("VES");

            currency.ValidFrom.Should().Be(new DateTime(2018, 8, 20));
            currency.ValidTo.Should().BeNull();

            currency.IsValidOn(DateTime.MinValue).Should().BeFalse();
            currency.IsValidOn(DateTime.MaxValue).Should().BeTrue();
            currency.IsValidOn(new DateTime(2018, 8, 19)).Should().BeFalse();
            currency.IsValidOn(new DateTime(2018, 8, 20)).Should().BeTrue();
        }
    }

    public class GivenIWantDefaultCurrency
    {
        [Fact]
        public void WhenCreatingVariableWithDefault_ThenShouldBeEqualToNoCurrency()
        {
            // Arrange / Act
            Currency currency = default;

            // Assert
            var expected = Currency.FromCode("XXX");

            currency.Should().NotBeNull();
            currency.Should().Be(expected);
            currency.Code.Should().Be(expected.Code);
            currency.Namespace.Should().Be(expected.Namespace);
            currency.Symbol.Should().Be(expected.Symbol);
            currency.EnglishName.Should().Be(expected.EnglishName);
            currency.IsValid.Should().Be(expected.IsValid);
        }

        [Fact]
        public void WhenNoCurrency_ThenItShouldBeEqualToDefault()
        {
            // Arrange / Act
            Currency noCurrency = Currency.FromCode("XXX");

            // Assert
            noCurrency.Should().NotBeNull();
            noCurrency.Should().Be(default(Currency));

            // Assert with XUnit methods, because https://stackoverflow.com/questions/61556309/fluent-assertions-be-vs-equals
            Assert.Equal(default, noCurrency);
            Assert.Equal(default(Currency), (object)noCurrency);
            Assert.True(noCurrency == default);
            Assert.True(noCurrency == default(Currency));
            Assert.True(noCurrency.Equals(default));
            Assert.True(noCurrency.Equals((object)default(Currency)));
            Assert.True(object.Equals(noCurrency, default(Currency)));
        }
    }
}
