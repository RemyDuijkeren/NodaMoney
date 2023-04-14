using System.Linq;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencySpec
{
    public class GivenIWantToParseAllCurrencySymbols
    {
        [Fact]
        public void WhenParsingCurrencySymbol_ThenCurrencySymbolShouldBeParsed()
        {
            foreach (var currencySymbol in Currency.GetAllCurrencies()
                .Select(currency => currency.Symbol)
                .Distinct())
            {
                var symbol = CurrencySymbolParser.Parse("1" + currencySymbol);
                symbol.Should().NotBeNullOrEmpty();
                symbol.Should().Be(currencySymbol);
            }
        }

        [Fact]
        public void WhenParsingCurrencyCodes_ThenCurrencyCodeShouldBeParsed()
        {
            foreach (var currencyCode in Currency.GetAllCurrencies()
                .Select(currency => currency.Code)
                .Distinct())
            {
                var code = CurrencySymbolParser.Parse("1" + currencyCode);
                code.Should().NotBeNullOrEmpty();
                code.Should().Be(currencyCode);
            }
        }

        [Fact(Skip = "For debugging.")]
        public void WhenAskingForCodeAndSymbol_ThenSomeCharactersUnfortunatelyAreNumeric()
        {
            var currencyCodeAndSymbolCharacters = Currency.GetAllCurrencies()
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
}
