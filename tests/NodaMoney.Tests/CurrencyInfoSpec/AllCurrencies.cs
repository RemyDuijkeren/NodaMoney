using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencyInfoSpec;

public class AllCurrencies
{
    [Fact]
    public void WhenAskingForIt_ThenAllCurrenciesShouldBeReturned()
    {
        var currencies = CurrencyInfo.GetAllCurrencies();

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
}
