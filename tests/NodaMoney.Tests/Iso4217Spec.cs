using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Net;
using Xunit;
using FluentAssertions;

namespace NodaMoney.Tests.Iso4127Spec
{
    public class Iso4127ListFixture
    {
        public Iso4127Currency[] currencies { get; private set; }
        public DateTime PublishDate { get; private set; }

        public Iso4127ListFixture()
        {
            var fileName = "iso4127.xml";

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using var client = new WebClient();
            client.DownloadFile(new Uri("https://www.currency-iso.org/dam/downloads/lists/list_one.xml"), fileName);

            var document = XDocument.Load(fileName);

            currencies = document.Element("ISO_4217").Element("CcyTbl").Elements("CcyNtry")
                            .Select(e =>
                            new Iso4127Currency
                            {
                                CountryName = e.Element("CtryNm").Value,
                                CurrencyName = e.Element("CcyNm")?.Value,
                                Currency = e.Element("Ccy")?.Value,
                                CurrencyNumber = e.Element("CcyNbr")?.Value,
                                CurrencyMinorUnits = e.Element("CcyMnrUnts")?.Value
                            })
                            .Where(a => !string.IsNullOrEmpty(a.Currency)) // ignore currencies without a currency name
                            .ToArray();

            PublishDate = DateTime.Parse(document.Element("ISO_4217").Attribute("Pblshd").Value);
        }
    }

    public class Iso4127Currency
    {
        public string CountryName { get; set; }
        public string CurrencyName { get; set; }
        public string Currency { get; set; }
        public string CurrencyNumber { get; set; }
        public string CurrencyMinorUnits { get; set; }
    }

    public class GivenIWantToCompareNodaMoneyWithIso4127 : IClassFixture<Iso4127ListFixture>
    {
        private Iso4127ListFixture _iso4127List;

        public GivenIWantToCompareNodaMoneyWithIso4127(Iso4127ListFixture fixture)
        {
            _iso4127List = fixture;
        }

        [Fact]
        public void WhenCurrenciesInIso4127List_ThenShouldAlsoExistInNodaMoney()
        {
            var missingCurrencies = _iso4127List.currencies
                                        .Where(a => !Currency.GetAllCurrencies().Any(c => c.Code == a.Currency))
                                        .ToList();

            missingCurrencies.Should().HaveCount(0, $"expected defined currencies to contain {string.Join(", ", missingCurrencies.Select(a => a.Currency + " " + a.CurrencyName))}");
        }

        [Fact]
        public void WhenCurrenciesInRegistryAndCurrent_ThenTheyShouldAlsoBeDefinedInTheIsoList()
        {
            var notDefinedCurrencies =
                Currency.GetAllCurrencies()
                .Where(c => c.IsValidOn(_iso4127List.PublishDate))
                .Where(c => !string.IsNullOrEmpty(c.IsoNumber))
                .Where(c => !_iso4127List.currencies.Any(a => a.Currency == c.Code))
                .ToList();

            notDefinedCurrencies.Should().HaveCount(0, $"did not expect currencies to contain {string.Join(", ", notDefinedCurrencies.Select(a => a.Code))}");
        }

        [Fact(Skip="Names contains countries at the moment")]
        public void WhenCompareCurrencies_ThenTheyShouldHaveTheSameEnglishName()
        {
            var differences = new List<string>();
            foreach (var c in Currency.GetAllCurrencies())
            {
                var found = _iso4127List.currencies.Where(x => x.Currency == c.Code).ToList();

                if (found.Count == 0)
                    continue;
                var a = found.First();
                // ignore casing (for now)
                if (!string.Equals(c.EnglishName, a.CurrencyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    differences.Add($"{ c.Code}: expected '{a.CurrencyName}' but found '{c.EnglishName}'");
                }
            }
            differences.Should().HaveCount(0, string.Join(Environment.NewLine, differences));
        }

        [Fact]
        public void WhenCompareCurrencies_ThenTheyShouldHaveTheSameNumber()
        {
            var differences = new List<string>();
            foreach (var c in Currency.GetAllCurrencies())
            {
                var found = _iso4127List.currencies.Where(x => x.Currency == c.Code).ToList();

                if (found.Count == 0)
                    continue;
                var a = found.First();
                // ignore casing (for now)
                if (!string.Equals(c.IsoNumber, a.CurrencyNumber, StringComparison.InvariantCultureIgnoreCase))
                {
                    differences.Add($"{c.Code}: expected {a.CurrencyNumber} but found {c.Number}");
                }
            }
            differences.Should().HaveCount(0, string.Join(Environment.NewLine, differences));
        }

        [Fact]
        public void WhenCompareCurrencies_ThenTheyShouldHaveTheSameNumberOfMinorDigits()
        {
            var differences = new List<string>();
            foreach (var c in Currency.GetAllCurrencies())
            {
                var found = _iso4127List.currencies.Where(x => x.Currency == c.Code).ToList();

                if (found.Count == 0)
                    continue;
                var a = found.First();
                if (!string.Equals(c.DecimalDigits.ToString(), a.CurrencyMinorUnits, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (c.DecimalDigits == CurrencyRegistry.NotApplicable && a.CurrencyMinorUnits == "N.A.")
                        continue;
                    if (c.DecimalDigits == CurrencyRegistry.Z07 && a.CurrencyMinorUnits == "2")
                        continue;

                    differences.Add($"{c.Code}: expected {a.CurrencyMinorUnits} minor units but found {c.DecimalDigits}");
                }
            }
            differences.Should().HaveCount(0, string.Join(Environment.NewLine, differences));
        }
    }
}
