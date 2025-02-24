namespace NodaMoney.Tests.UnroundedMoneySpec;

public class CreateFromMoney
{
    [Fact]
    public void WhenMoney()
    {
        // Arrange
        Money rounded = new Money(0.3333333333333333m, "EUR");

        // Act
        var unrounded = new UnroundedMoney(rounded);

        // Assert
        unrounded.Amount.Should().Be(0.33m);
        unrounded.Currency.Should().Be(rounded.Currency);
    }

}
