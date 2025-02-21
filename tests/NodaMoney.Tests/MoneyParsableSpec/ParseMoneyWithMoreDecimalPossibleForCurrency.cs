using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class ParseMoneyWithMoreDecimalPossibleForCurrency
{
    [Fact, UseCulture("ja-JP")]
    public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedDown()
    {
        var yen = Money.Parse("¥ 98,765.4");

        yen.Should().Be(new Money(98_765m, "JPY"));
    }

    [Fact, UseCulture("ja-JP")]
    public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedUp()
    {
        var yen = Money.Parse("¥ 98,765.5");

        yen.Should().Be(new Money(98_766m, "JPY"));
    }

    [Fact, UseCulture("de-CH")]
    public void WhenParsingSwissFranc_ThenThisShouldBeRoundedUp()
    {
        // CHF 98.765,45 : Period (.) as the thousands separator (common in everyday usage)
        // CHF 98’765.45 : Apostrophe (’) as the thousands separator (formal and financial contexts)

        var money = Money.Parse("CHF 98’765.475");

        money.Should().Be(new Money(98765.48m, "CHF"));
    }
}
