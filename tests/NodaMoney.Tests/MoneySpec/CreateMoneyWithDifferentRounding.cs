namespace NodaMoney.Tests.MoneySpec;

public class CreateMoneyWithDifferentRounding
{
    [Fact]
    public void RoundUp_When_OnlyAmount()
    {
        decimal amount = 0.525m;
        var defaultRounding = new Money(amount, "EUR");
        var differentRounding = new Money(amount, "EUR", MidpointRounding.AwayFromZero);

        defaultRounding.Amount.Should().Be(0.52m);
        differentRounding.Amount.Should().Be(0.53m);
    }

    [Fact]
    public void RoundUp_When_AmountAndCode()
    {
        decimal amount = 0.525m;
        var defaultRounding = new Money(amount, "EUR");
        var differentRounding = new Money(amount, "EUR", MidpointRounding.AwayFromZero);

        defaultRounding.Amount.Should().Be(0.52m);
        differentRounding.Amount.Should().Be(0.53m);
    }

    [Fact]
    public void NoRounding_When_MinorUnitIsNotApplicable()
    {
        // Arrange

        // Act
        var money = new Money(0.8m, CurrencyInfo.NoCurrency);

        // Assert
        CurrencyInfo.NoCurrency.MinorUnit.Should().Be(MinorUnit.NotApplicable);
        money.Amount.Should().Be(0.8m);
    }

}
