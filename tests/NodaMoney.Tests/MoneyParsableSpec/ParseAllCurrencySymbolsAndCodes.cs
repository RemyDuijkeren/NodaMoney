using System.Linq;

namespace NodaMoney.Tests.MoneyParsableSpec;

public class ParseAllCurrencySymbolsAndCodes
{
    [Fact]
    public void WhenParsingSymbol()
    {
        foreach (var currencyInfo in CurrencyInfo.GetAllCurrencies().Where(c => c.Symbol != CurrencyInfo.NoCurrency.Symbol))
        {
            string toParse = $"1 {currencyInfo.Symbol}";

            Money money = Money.Parse(toParse, currencyInfo);
            money.Currency.Should().Be((Currency)currencyInfo);
        }
    }

    [Fact]
    public void WhenParsingInternationalSymbol()
    {
        foreach (var currencyInfo in CurrencyInfo.GetAllCurrencies().Where(c => c.InternationalSymbol != CurrencyInfo.NoCurrency.Symbol))
        {
            string toParse = $"1 {currencyInfo.InternationalSymbol}";

            Money money = Money.Parse(toParse, currencyInfo);
            money.Currency.Should().Be((Currency)currencyInfo);
        }
    }

    [Fact]
    public void WhenParsingAlternativeSymbol()
    {
        foreach (var currencyInfo in CurrencyInfo.GetAllCurrencies().Where(c => c.AlternativeSymbols.Any()))
        {
            foreach (var alternativeSymbol in currencyInfo.AlternativeSymbols)
            {
                string toParse = $"1 {alternativeSymbol}";

                Money money = Money.Parse(toParse, currencyInfo);
                money.Currency.Should().Be((Currency)currencyInfo);
            }
        }
    }

    [Fact]
    public void WhenParsingCodes()
    {
        foreach (var currencyInfo in CurrencyInfo.GetAllCurrencies())
        {
            string toParse = $"1 {currencyInfo.Code}";

            Money money = Money.Parse(toParse, currencyInfo);
            money.Currency.Should().Be((Currency)currencyInfo);
        }
    }
}
