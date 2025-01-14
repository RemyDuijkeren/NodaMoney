using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyFiveMostUsedCurrenciesSpec;

public class GivenIWantYens
{
    [Fact]
    public void WhenDecimal_ThenCreatingShouldSucceed()
    {
        //from decimal (other integral types are implicitly converted to decimal)
        var yens = Money.Yen(10.00m);

        yens.Should().NotBeNull();
        yens.Currency.Should().Be(Currency.FromCode("JPY"));
        yens.Amount.Should().Be(10.00m);
    }

    [Fact]
    public void WhenDecimalAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
        //from decimal (other integral types are implicitly converted to decimal)
        var yen1 = Money.Yen(10.5m);
        var yen2 = Money.Yen(10.5m, MidpointRounding.AwayFromZero);

        yen2.Currency.Should().Be(Currency.FromCode("JPY"));
        yen2.Amount.Should().Be(11m);
        yen1.Amount.Should().NotBe(yen2.Amount);
    }

    [Fact]
    public void WhenDouble_ThenCreatingShouldSucceed()
    {
        //from double (float is implicitly converted to double)
        var yens = Money.Yen(10.00);

        yens.Should().NotBeNull();
        yens.Currency.Should().Be(Currency.FromCode("JPY"));
        yens.Amount.Should().Be(10.00m);
    }

    [Fact]
    public void WhenDoubleAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
        //from double (float is implicitly converted to double)
        var yen1 = Money.Yen(10.5);
        var yen2 = Money.Yen(10.5, MidpointRounding.AwayFromZero);

        yen2.Currency.Should().Be(Currency.FromCode("JPY"));
        yen2.Amount.Should().Be(11m);
        yen1.Amount.Should().NotBe(yen2.Amount);
    }

    [Fact]
    public void WhenLong_ThenCreatingShouldSucceed()
    {
        //from long (byte, short and int are implicitly converted to long)
        var yens = Money.Yen(10L);

        yens.Should().NotBeNull();
        yens.Currency.Should().Be(Currency.FromCode("JPY"));
        yens.Amount.Should().Be(10.00m);
    }

    [Fact]
    public void WhenULong_ThenCreatingShouldSucceed()
    {
        var yens = Money.Yen(10UL);

        yens.Should().NotBeNull();
        yens.Currency.Should().Be(Currency.FromCode("JPY"));
        yens.Amount.Should().Be(10.00m);
    }
}
