using UnroundedMoney = (decimal Amount, NodaMoney.Currency Currency);

namespace NodaMoney.Tests.MoneyCalculationsSpec;

public class DivisionMultiplicationWithRemainder
{
    [Theory]
    [InlineData(100, "XXX", 100)]
    [InlineData(100, "EUR", 99.99)] // expect 100, fail!
    public void WhenMoney(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        Money subject = new(amount, currency);

        // Act
        Money intermediate = subject / 3m;
        Money result = intermediate * 3m;

        // Assert
        result.Amount.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(100, "XXX", 100)]
    [InlineData(100, "EUR", 100)]
    public void WhenDecimalLocalFunction(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        decimal Fx(decimal input) => input / 3m * 3m; // local function

        // Act
        decimal decimalResult = Fx(amount);
        Money result = new Money(Fx(amount), currency);

        // Assert
        decimalResult.Should().Be(expectedResult);
        result.Amount.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(100, "XXX", 100)]
    [InlineData(100, "EUR", 100)]
    public void WhenDecimalDelegateFunction(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        Func<decimal, decimal> fx = input => input / 3m * 3m; // delegate function

        // Act
        decimal decimalResult = fx(amount);
        Money result = new Money(fx(amount), currency);

        // Assert
        decimalResult.Should().Be(expectedResult);
        result.Amount.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(100, "XXX", 100)]
    [InlineData(100, "EUR", 100)]
    public void WhenMoneyFunction(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        Money Fx(Money money)
        {
            var result = (money.Amount / 3m) * 3m;
            return new Money(result, money.Currency);
        }

        var subject = new Money(amount, currency);

        // Act
        var result = Fx(subject);

        // Assert
        result.Amount.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(100, "XXX", 100)]
    [InlineData(100, "EUR", 100)]
    public void WhenTupleFunction(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        (decimal Amount, Currency Currency) Fx(Money money)
        {
            var result = (money.Amount / 3m) * 3m;
            return (result, money.Currency);
        }

        var subject = new Money(amount, currency);

        // Act
        (decimal Amount, Currency Currency) result = Fx(subject);
        Money moneyResult = new(result.Amount, result.Currency);

        // Assert
        result.Amount.Should().Be(expectedResult);
        moneyResult.Amount.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(100, "XXX", 100)]
    [InlineData(100, "EUR", 100)]
    public void WhenNamedTupleFunction(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        UnroundedMoney Fx(Money money)
        {
            var result = (money.Amount / 3m) * 3m;
            return new UnroundedMoney(result, money.Currency);
        }

        var subject = new Money(amount, currency);

        // Act
        UnroundedMoney result = Fx(subject);
        Money moneyResult = new(result.Amount, result.Currency);

        // Assert
        result.Amount.Should().Be(expectedResult);
        moneyResult.Amount.Should().Be(expectedResult);
    }
}
