using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyUnaryOperatorsSpec;

public class AddAndSubtractMoneyUnary
{
    private readonly Money _tenEuroPlus = new Money(10.00m, "EUR");
    private readonly Money _tenEuroMin = new Money(-10.00m, "EUR");

    [Fact]
    public void WhenUsingUnaryPlusOperator_ThenThisSucceed()
    {
            var r1 = +_tenEuroPlus;
            var r2 = +_tenEuroMin;

            r1.Amount.Should().Be(10.00m);
            r1.Currency.Code.Should().Be("EUR");
            r2.Amount.Should().Be(-10.00m);
            r2.Currency.Code.Should().Be("EUR");
        }

    [Fact]
    public void WhenUsingUnaryMinOperator_ThenThisSucceed()
    {
            var r1 = -_tenEuroPlus;
            var r2 = -_tenEuroMin;

            r1.Amount.Should().Be(-10.00m);
            r1.Currency.Code.Should().Be("EUR");
            r2.Amount.Should().Be(10.00m);
            r2.Currency.Code.Should().Be("EUR");
        }
}
