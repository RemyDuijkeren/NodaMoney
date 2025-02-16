using System.Linq;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

public class GivenIWantToParseAllCurrencySymbols
{
    [Fact]
    public void WhenParsingCurrencySymbol_ThenCurrencySymbolShouldBeParsed()
    {
        foreach (var currencyInfo in CurrencyInfo.GetAllCurrencies().Where(c => c.Symbol != CurrencyInfo.NoCurrency.Symbol))
        {
            string toParse = $"1 {currencyInfo.Symbol}";

            Money money = Money.Parse(toParse, currencyInfo);
            money.Currency.Should().NotBeNull();
            money.Currency.Code.Should().Be(currencyInfo.Code);
            money.Currency.Symbol.Should().Be(currencyInfo.Symbol);
        }
    }

    [Fact(Skip = "Need to fix this.")]
    public void WhenParsingCurrencyCodes_ThenCurrencyCodeShouldBeParsed()
    {
        foreach (var currencyInfo in CurrencyInfo.GetAllCurrencies())
        {
            string toParse = $"1 {currencyInfo.Code}";

            Money money = Money.Parse(toParse, currencyInfo);
            money.Currency.Should().NotBeNull();
            money.Currency.Code.Should().Be(currencyInfo.Code);
            money.Currency.Symbol.Should().Be(currencyInfo.Symbol);
        }
    }

    [Fact(Skip = "For debugging.")]
    public void WhenAskingForCodeAndSymbol_ThenSomeCharactersUnfortunatelyAreNumeric()
    {
        var currencyCodeAndSymbolCharacters = CurrencyInfo.GetAllCurrencies()
                                                          .SelectMany(currency => currency.Code + currency.Symbol)
                                                          .Distinct();

        var numericCodeAndSymbolCharacters = currencyCodeAndSymbolCharacters
                                             .Where(character => !IsNotNumericCharacter(character))
                                             .ToList();

        numericCodeAndSymbolCharacters.Should().NotBeEmpty();
        numericCodeAndSymbolCharacters.Should().Contain('.');
    }


    /// <summary>
    /// Indicates whether a given character is considered part of the currency code or symbol.
    /// </summary>
    /// <param name="character">The character to examine.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the specified <paramref name="character"/>
    /// is considered part of the curreny code or symbol;
    /// otherwise, returns <see langword="false"/>.
    /// </returns>
    internal static bool IsNotNumericCharacter(char character) =>
        !char.IsDigit(character) &&
        !char.IsWhiteSpace(character) &&
        !char.IsPunctuation(character) &&
        !char.IsSeparator(character);
}
