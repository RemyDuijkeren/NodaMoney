using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.CurrencyInfoBuilderSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToReplaceIsoCurrencyWithOwnVersion
{
    [Fact]
    public void WhenReplacingEuroWithCustom_ThenThisShouldSucceed()
    {
        // Panamanian balboa
        CurrencyInfo oldEuro = CurrencyInfoBuilder.Unregister("PAB");

        var builder = new CurrencyInfoBuilder("PAB");
        builder.LoadDataFromCurrencyInfo(oldEuro);
        builder.EnglishName = "New Panamanian balboa";
        builder.DecimalDigits = 1;

        builder.Register();

        CurrencyInfo newEuro = CurrencyInfo.FromCode("PAB");
        newEuro.Symbol.Should().Be("B/.");
        newEuro.EnglishName.Should().Be("New Panamanian balboa");
        newEuro.DecimalDigits.Should().Be(1);
    }
}
