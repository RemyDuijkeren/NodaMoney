using System.Collections.Generic;
using System.Linq;

namespace NodaMoney.Tests.Iso4127Spec;

public class CompareNodaMoneyWithIso4127 : IClassFixture<Iso4127ListFixture>
{
    private readonly Iso4127ListFixture _iso4127List;

    public CompareNodaMoneyWithIso4127(Iso4127ListFixture fixture)
    {
        _iso4127List = fixture;
    }

    [Fact]
    public void WhenCurrenciesInIso4127List_ThenShouldAlsoExistInNodaMoney()
    {
        var missingCurrencies = _iso4127List.Currencies
                                            .Where(a => CurrencyInfo.GetAllCurrencies().All(c => c.Code != a.Currency))
                                            .ToList();

        missingCurrencies.Should().HaveCount(0,
            $"expected defined currencies to contain {string.Join(", ", missingCurrencies.Select(a => a.Currency + " " + a.CurrencyName))}");
    }

    [Fact]
    public void WhenCurrenciesInRegistryAndCurrent_ThenTheyShouldAlsoBeDefinedInTheIsoList()
    {
        var notDefinedCurrencies =
            CurrencyInfo.GetAllCurrencies()
                        .Where(c => c.IsIso4217)
                        //.Where(c => c.IsActiveOn(_iso4127List.PublishDate))
                        .Where(c => c.IsHistoric == false)
                        .Where(c => !string.IsNullOrEmpty(c.NumericCode))
                        .Where(c => _iso4127List.Currencies.All(a => a.Currency != c.Code))
                        .Where(c => c.ExpiredOn is null ||
                                    (c.ExpiredOn is not null && c.ExpiredOn <= DateTime.UtcNow))
                        .ToList();

        notDefinedCurrencies.Should().HaveCount(0, $"did not expect currencies to contain {string.Join(", ", notDefinedCurrencies.Select(a => a.Code))}");
    }

    [Fact(Skip = "Names contains countries at the moment")]
    //[Fact]
    public void WhenCompareCurrencies_ThenTheyShouldHaveTheSameEnglishName()
    {
        var differences = new List<string>();

        foreach (var nodaCurrency in CurrencyInfo.GetAllCurrencies())
        {
            var iso4127Currency = _iso4127List.Currencies.FirstOrDefault(x => x.Currency == nodaCurrency.Code);

            if (iso4127Currency == null)
                continue;

            // ignore casing (for now)
            if (!string.Equals(nodaCurrency.EnglishName, iso4127Currency.CurrencyName, StringComparison.InvariantCultureIgnoreCase))
            {
                differences.Add($"{nodaCurrency.Code}: expected '{iso4127Currency.CurrencyName}' but found '{nodaCurrency.EnglishName}'");
            }
        }

        differences.Should().HaveCount(0, string.Join(Environment.NewLine, differences));
    }

    [Fact]
    public void WhenCompareCurrencies_ThenTheyShouldHaveTheSameNumber()
    {
        var differences = new List<string>();

        foreach (var nodaCurrency in CurrencyInfo.GetAllCurrencies())
        {
            var iso4127Currency = _iso4127List.Currencies.FirstOrDefault(x => x.Currency == nodaCurrency.Code);

            if (iso4127Currency == null)
                continue;

            // ignore casing (for now)
            if (!string.Equals(nodaCurrency.NumericCode, iso4127Currency.CurrencyNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                differences.Add($"{nodaCurrency.Code}: expected {iso4127Currency.CurrencyNumber} but found {nodaCurrency.Number}");
            }
        }

        differences.Should().HaveCount(0, string.Join(Environment.NewLine, differences));
    }

    [Fact]
    public void WhenCompareCurrencies_ThenTheyShouldHaveTheSameNumberOfMinorDigits()
    {
        var differences = new List<string>();

        foreach (var nodaCurrency in CurrencyInfo.GetAllCurrencies())
        {
            var iso4127Currency = _iso4127List.Currencies.FirstOrDefault(x => x.Currency == nodaCurrency.Code);

            if (iso4127Currency == null)
                continue;

            if (!string.Equals(nodaCurrency.DecimalDigits.ToString(), iso4127Currency.CurrencyMinorUnits, StringComparison.InvariantCultureIgnoreCase))
            {
                if (nodaCurrency.MinorUnit == MinorUnit.NotApplicable && iso4127Currency.CurrencyMinorUnits == "N.A.")
                    continue;
                if (nodaCurrency.MinorUnit == MinorUnit.OneFifth && iso4127Currency.CurrencyMinorUnits == "2")
                    continue;

                differences.Add($"{nodaCurrency.Code}: expected {iso4127Currency.CurrencyMinorUnits} minor units but found {nodaCurrency.DecimalDigits}");
            }
        }

        differences.Should().HaveCount(0, string.Join(Environment.NewLine, differences));
    }
}
