namespace NodaMoney.Tests.FastMoneySpec;

public class CreateFromMoney
{
    [Fact]
    public void WhenMoney()
    {
        // Arrange
        Money rounded = new Money(0.3333333333333333m, "EUR");

        // Act
        var fast = new FastMoney(rounded);

        // Assert
        fast.Amount.Should().Be(0.33m);
        fast.Currency.Should().Be(rounded.Currency);
    }

}
