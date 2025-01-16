using System;
using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.CurrencyInfoBuilderSpec;

[Collection(nameof(NoParallelization))]
public class ReplaceCurrency
{
    [Fact]
    public void MustWork_When_ReplacingCurrencyWithCustom()
    {
        // Arrange
        CurrencyInfo removed = CurrencyInfoBuilder.Unregister("PAB"); // Panamanian balboa

        var builder = new CurrencyInfoBuilder("PAB");
        builder.LoadDataFromCurrencyInfo(removed);
        builder.EnglishName = "New Panamanian balboa";
        builder.DecimalDigits = 1;

        builder.Register();

        // Act
        CurrencyInfo replaced = CurrencyInfo.FromCode("PAB");

        // Assert
        replaced.Symbol.Should().Be("B/.");
        replaced.EnglishName.Should().Be("New Panamanian balboa");
        replaced.DecimalDigits.Should().Be(1);
        replaced.Should().NotBe(removed);
    }
}
