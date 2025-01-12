using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyFiveMostUsedCurrenciesSpec;

public class GivenIWantEuros
{
    [Fact]
    public void WhenDecimal_ThenCreatingShouldSucceed()
    {
            // from decimal (other integral types are implicitly converted to decimal)
            var euros = Money.Euro(10.00m);

            euros.Currency.Should().Be(Currency.FromCode("EUR"));
            euros.Amount.Should().Be(10.00m);
        }

    [Fact]
    public void WhenDecimalAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
            // from decimal (other integral types are implicitly converted to decimal)
            var euros1 = Money.Euro(10.005m);
            var euros2 = Money.Euro(10.005m, MidpointRounding.AwayFromZero);

            euros2.Currency.Should().Be(Currency.FromCode("EUR"));
            euros2.Amount.Should().Be(10.01m);
            euros1.Amount.Should().NotBe(euros2.Amount);
        }

    [Fact]
    public void WhenDouble_ThenCreatingShouldSucceed()
    {
            // from double (float is implicitly converted to double)
            var euros = Money.Euro(10.00);

            euros.Currency.Should().Be(Currency.FromCode("EUR"));
            euros.Amount.Should().Be(10.00m);
        }

    [Fact]
    public void WhenDoubleAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
            // from double (float is implicitly converted to double)
            var euros1 = Money.Euro(10.005);
            var euros2 = Money.Euro(10.005, MidpointRounding.AwayFromZero);

            euros2.Currency.Should().Be(Currency.FromCode("EUR"));
            euros2.Amount.Should().Be(10.01m);
            euros1.Amount.Should().NotBe(euros2.Amount);
        }

    [Fact]
    public void WhenLong_ThenCreatingShouldSucceed()
    {
            // from long (byte, short and int are implicitly converted to long)
            var euros = Money.Euro(10L);

            euros.Currency.Should().Be(Currency.FromCode("EUR"));
            euros.Amount.Should().Be(10.00m);
        }

    [Fact]
    public void WhenULong_ThenCreatingShouldSucceed()
    {
            var euros = Money.Euro(10UL);

            euros.Currency.Should().Be(Currency.FromCode("EUR"));
            euros.Amount.Should().Be(10.00m);
        }
}

public class GivenIWantDollars
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

public class GivenIWantPonds
{
    [Fact]
    public void WhenDecimal_ThenCreatingShouldSucceed()
    {
            //from decimal (other integral types are implicitly converted to decimal)
            var pounds = Money.PoundSterling(10.00m);

            pounds.Should().NotBeNull();
            pounds.Currency.Should().Be(Currency.FromCode("GBP"));
            pounds.Amount.Should().Be(10.00m);
        }

    [Fact]
    public void WhenDecimalAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
            //from decimal (other integral types are implicitly converted to decimal)
            var pounds1 = Money.PoundSterling(10.005m);
            var pounds2 = Money.PoundSterling(10.005m, MidpointRounding.AwayFromZero);

            pounds2.Currency.Should().Be(Currency.FromCode("GBP"));
            pounds2.Amount.Should().Be(10.01m);
            pounds1.Amount.Should().NotBe(pounds2.Amount);
        }

    [Fact]
    public void WhenDouble_ThenCreatingShouldSucceed()
    {
            //from double (float is implicitly converted to double)
            var pounds = Money.PoundSterling(10.00);

            pounds.Should().NotBeNull();
            pounds.Currency.Should().Be(Currency.FromCode("GBP"));
            pounds.Amount.Should().Be(10.00m);
        }

    [Fact]
    public void WhenDoubleAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
            //from double (float is implicitly converted to double)
            var pounds1 = Money.PoundSterling(10.005);
            var pounds2 = Money.PoundSterling(10.005, MidpointRounding.AwayFromZero);

            pounds2.Currency.Should().Be(Currency.FromCode("GBP"));
            pounds2.Amount.Should().Be(10.01m);
            pounds1.Amount.Should().NotBe(pounds2.Amount);
        }

    [Fact]
    public void WhenLong_ThenCreatingShouldSucceed()
    {
            //from long (byte, short and int are implicitly converted to long)
            var pounds = Money.PoundSterling(10L);

            pounds.Should().NotBeNull();
            pounds.Currency.Should().Be(Currency.FromCode("GBP"));
            pounds.Amount.Should().Be(10.00m);
        }

    [Fact]
    public void WhenULong_ThenCreatingShouldSucceed()
    {
            var pounds = Money.PoundSterling(10UL);

            pounds.Should().NotBeNull();
            pounds.Currency.Should().Be(Currency.FromCode("GBP"));
            pounds.Amount.Should().Be(10.00m);
        }
}

public class GivenIWantYuan
{
    [Fact]
    public void WhenDecimal_ThenCreatingShouldSucceed()
    {
            //from decimal (other integral types are implicitly converted to decimal)
            var pounds = Money.Yuan(10.00m);

            pounds.Should().NotBeNull();
            pounds.Currency.Should().Be(Currency.FromCode("CNY"));
            pounds.Amount.Should().Be(10.00m);
        }

    [Fact]
    public void WhenDecimalAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
            //from decimal (other integral types are implicitly converted to decimal)
            var pounds1 = Money.Yuan(10.005m);
            var pounds2 = Money.Yuan(10.005m, MidpointRounding.AwayFromZero);

            pounds2.Currency.Should().Be(Currency.FromCode("CNY"));
            pounds2.Amount.Should().Be(10.01m);
            pounds1.Amount.Should().NotBe(pounds2.Amount);
        }

    [Fact]
    public void WhenDouble_ThenCreatingShouldSucceed()
    {
            //from double (float is implicitly converted to double)
            var pounds = Money.Yuan(10.00);

            pounds.Should().NotBeNull();
            pounds.Currency.Should().Be(Currency.FromCode("CNY"));
            pounds.Amount.Should().Be(10.00m);
        }

    [Fact]
    public void WhenDoubleAndRoundingAwayFromZero_ThenCreatingShouldSucceed()
    {
            //from double (float is implicitly converted to double)
            var pounds1 = Money.Yuan(10.005);
            var pounds2 = Money.Yuan(10.005, MidpointRounding.AwayFromZero);

            pounds2.Currency.Should().Be(Currency.FromCode("CNY"));
            pounds2.Amount.Should().Be(10.01m);
            pounds1.Amount.Should().NotBe(pounds2.Amount);
        }

    [Fact]
    public void WhenLong_ThenCreatingShouldSucceed()
    {
            //from long (byte, short and int are implicitly converted to long)
            var pounds = Money.Yuan(10L);

            pounds.Should().NotBeNull();
            pounds.Currency.Should().Be(Currency.FromCode("CNY"));
            pounds.Amount.Should().Be(10.00m);
        }

    [Fact]
    public void WhenULong_ThenCreatingShouldSucceed()
    {
            var pounds = Money.Yuan(10UL);

            pounds.Should().NotBeNull();
            pounds.Currency.Should().Be(Currency.FromCode("CNY"));
            pounds.Amount.Should().Be(10.00m);
        }
}
