namespace NodaMoney.Tests.ExactMoneySpec;

public class CreateMoneyWithDoubleValue
{
    [Theory]
    [InlineData(0.03, 0.03)]
    [InlineData(0.3333333333333333, 0.333333333333333)]
    public void WhenValueIsDoubleAndWithCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
    {
        var money = new ExactMoney(input, "EUR");

        money.Amount.Should().Be(expected);
    }

    [Theory]
    [InlineData(0.03, 0.03)]
    [InlineData(0.3333333333333333, 0.333333333333333)]
    public void WhenValueIsDoubleWithoutCurrency_ThenMoneyShouldBeCorrect(double input, decimal expected)
    {
        var money = new ExactMoney(input, "EUR");

        money.Amount.Should().Be(expected);
    }
}
