namespace NodaMoney.Tests.MoneyFiveMostUsedCurrenciesSpec;

public class CreatePonds
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
        var pounds = Money.PoundSterling(10.005D);

        pounds.Should().NotBeNull();
        pounds.Currency.Should().Be(Currency.FromCode("GBP"));
        pounds.Amount.Should().Be(10.00m);
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
