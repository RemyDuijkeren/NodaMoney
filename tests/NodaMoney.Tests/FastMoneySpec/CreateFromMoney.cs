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

    [Fact]
    public void WhenFromOACurrency()
    {
        // Arrange
        long cy = 123456789;

        // Act
        var fast = FastMoney.FromOACurrency(cy, Currency.FromCode("EUR"));

        // Assert
        fast.Amount.Should().Be(12345.6789m);
        fast.Currency.Should().Be(Currency.FromCode("EUR"));
    }
}
