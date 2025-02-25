namespace NodaMoney.Tests.ExactMoneySpec;

public class MoneyInMalagasyAriaryWhichHasFiveSubunits
{
    [Theory]
    [InlineData(0.01)]
    [InlineData(0.09)]
    [InlineData(0.10)]
    [InlineData(0.15)]
    [InlineData(0.22)]
    [InlineData(0.29)]
    [InlineData(0.30)]
    [InlineData(0.33)]
    [InlineData(0.38)]
    [InlineData(0.40)]
    [InlineData(0.41)]
    [InlineData(0.45)]
    [InlineData(0.46)]
    [InlineData(0.50)]
    [InlineData(0.54)]
    [InlineData(0.57)]
    [InlineData(0.60)]
    [InlineData(0.68)]
    [InlineData(0.70)]
    [InlineData(0.74)]
    [InlineData(0.77)]
    [InlineData(0.80)]
    [InlineData(0.83)]
    [InlineData(0.85)]
    [InlineData(0.86)]
    [InlineData(0.90)]
    [InlineData(0.91)]
    [InlineData(0.95)]
    [InlineData(0.99)]
    public void WhenOnlyAmount_ThenItShouldRoundUp(decimal input)
    {
        // 1 MGA = 5 iraimbilanja
        var money = new ExactMoney(input, "MGA");

        money.Amount.Should().Be(input);
    }
}
