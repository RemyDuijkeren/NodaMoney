using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneySpec;

public class MoneyInMalagasyAriaryWhichHasFiveSubunits
{
    [Theory]
    [InlineData(0.01, 0.0)]
    [InlineData(0.09, 0.0)]
    [InlineData(0.10, 0.0)]
    [InlineData(0.15, 0.2)]
    [InlineData(0.22, 0.2)]
    [InlineData(0.29, 0.2)]
    [InlineData(0.30, 0.4)]
    [InlineData(0.33, 0.4)]
    [InlineData(0.38, 0.4)]
    [InlineData(0.40, 0.4)]
    [InlineData(0.41, 0.4)]
    [InlineData(0.45, 0.4)]
    [InlineData(0.46, 0.4)]
    [InlineData(0.50, 0.4)]
    [InlineData(0.54, 0.6)]
    [InlineData(0.57, 0.6)]
    [InlineData(0.60, 0.6)]
    [InlineData(0.68, 0.6)]
    [InlineData(0.70, 0.8)]
    [InlineData(0.74, 0.8)]
    [InlineData(0.77, 0.8)]
    [InlineData(0.80, 0.8)]
    [InlineData(0.83, 0.8)]
    [InlineData(0.85, 0.8)]
    [InlineData(0.86, 0.8)]
    [InlineData(0.90, 0.8)]
    [InlineData(0.91, 1.0)]
    [InlineData(0.95, 1.0)]
    [InlineData(0.99, 1.0)]
    public void WhenOnlyAmount_ThenItShouldRoundUp(decimal input, decimal expected)
    {
        // 1 MGA = 5 iraimbilanja
        var money = new Money(input, "MGA");

        money.Amount.Should().Be(expected);
    }
}
