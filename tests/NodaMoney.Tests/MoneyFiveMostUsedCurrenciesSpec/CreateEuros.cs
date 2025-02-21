namespace NodaMoney.Tests.MoneyFiveMostUsedCurrenciesSpec;

public class CreateEuros
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
