using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.UnaryOperatorsSpec
{
    public class GivenIWantToIncrementAndDecrementMoneyUnary
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void WhenIncrementing_ThenAmountShouldIncrementWithMinorUnit(Money money, Currency expectedCurrency, decimal expectedDifference)
        {
            decimal amountBefore = money.Amount;

            Money result = ++money;

            result.Currency.Should().Be(expectedCurrency);
            result.Amount.Should().Be(amountBefore + expectedDifference);
            money.Currency.Should().Be(expectedCurrency);
            money.Amount.Should().Be(amountBefore + expectedDifference);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void WhenDecrementing_ThenAmountShouldDecrementWithMinorUnit(Money money, Currency expectedCurrency, decimal expectedDifference)
        {
            decimal amountBefore = money.Amount;

            Money result = --money;

            result.Currency.Should().Be(expectedCurrency);
            result.Amount.Should().Be(amountBefore - expectedDifference);
            money.Currency.Should().Be(expectedCurrency);
            money.Amount.Should().Be(amountBefore - expectedDifference);
        }

        public static TheoryData<Money, Currency, decimal> TestData => new TheoryData<Money, Currency, decimal>
        {
            { new Money(765m, Currency.FromCode("JPY")), Currency.FromCode("JPY"), Currency.FromCode("JPY").MinorUnit },
            { new Money(765.43m, Currency.FromCode("EUR")), Currency.FromCode("EUR"), Currency.FromCode("EUR").MinorUnit },
            { new Money(765.43m, Currency.FromCode("USD")), Currency.FromCode("USD"), Currency.FromCode("USD").MinorUnit },
            { new Money(765.432m, Currency.FromCode("BHD")), Currency.FromCode("BHD"), Currency.FromCode("BHD").MinorUnit }
        };
    }

    public class GivenIWantToAddAndSubtractMoneyUnary
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
}
