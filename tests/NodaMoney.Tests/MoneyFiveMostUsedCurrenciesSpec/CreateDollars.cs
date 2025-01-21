using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyFiveMostUsedCurrenciesSpec;

public class CreateDollars
{
    [Fact]
    public void WhenDecimal_ThenCreatingShouldSucceed()
    {
        //from decimal (other integral types are implicitly converted to decimal)
        var dollars = Money.USDollar(10.00m);

        dollars.Currency.Should().Be(Currency.FromCode("USD"));
        dollars.Amount.Should().Be(10.00m);
    }

    [Fact]
    public void WhenDecimalAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
        // from decimal (other integral types are implicitly converted to decimal)
        var dollars1 = Money.USDollar(10.005m);
        var dollars2 = Money.USDollar(10.005m, MidpointRounding.AwayFromZero);

        dollars2.Currency.Should().Be(Currency.FromCode("USD"));
        dollars2.Amount.Should().Be(10.01m);
        dollars1.Amount.Should().NotBe(dollars2.Amount);
    }

    [Fact]
    public void WhenDouble_ThenCreatingShouldSucceed()
    {
        //from double (float is implicitly converted to double)
        var dollars = Money.USDollar(10.00);

        dollars.Currency.Should().Be(Currency.FromCode("USD"));
        dollars.Amount.Should().Be(10.00m);
    }

    [Fact]
    public void WhenDoubleAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
        //from double (float is implicitly converted to double)
        var dollars1 = Money.USDollar(10.005);
        var dollars2 = Money.USDollar(10.005, MidpointRounding.AwayFromZero);

        dollars2.Currency.Should().Be(Currency.FromCode("USD"));
        dollars2.Amount.Should().Be(10.01m);
        dollars1.Amount.Should().NotBe(dollars2.Amount);
    }

    [Fact]
    public void WhenLong_ThenCreatingShouldSucceed()
    {
        //from long (byte, short and int are implicitly converted to long)
        var dollars = Money.USDollar(10L);

        dollars.Currency.Should().Be(Currency.FromCode("USD"));
        dollars.Amount.Should().Be(10.00m);
    }

    [Fact]
    public void WhenULong_ThenCreatingShouldSucceed()
    {
        //from long (byte, short and int are implicitly converted to long)
        var dollars = Money.USDollar(10UL);

        dollars.Currency.Should().Be(Currency.FromCode("USD"));
        dollars.Amount.Should().Be(10.00m);
    }
}
