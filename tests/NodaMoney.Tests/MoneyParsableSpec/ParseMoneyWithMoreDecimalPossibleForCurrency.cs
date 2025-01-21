using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyParsableSpec;

[Collection(nameof(NoParallelization))]
public class ParseMoneyWithMoreDecimalPossibleForCurrency
{
    [Fact, UseCulture("ja-JP")]
    public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedDown()
    {
        var yen = Money.Parse("¥ 765.4");

        yen.Should().Be(new Money(765m, "JPY"));
    }

    [Fact, UseCulture("ja-JP")]
    public void WhenParsingJapaneseYen_ThenThisShouldBeRoundedUp()
    {
        var yen = Money.Parse("¥ 765.5");

        yen.Should().Be(new Money(766m, "JPY"));
    }
}
