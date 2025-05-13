using UnroundedMoney = (decimal Amount, NodaMoney.Currency Currency);

namespace NodaMoney.Tests.MoneyRoundingSpec;

public class DivisionMultiplicationWithRemainder
{
    [Theory]
    [InlineData(100, "XXX", 100)] // XXX does not do rounding
    [InlineData(100, "EUR", 100)] // expect 100, fail!
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
        Money subject = new(amount, currency);
        decimal Fx(decimal input) => input / 3m * 3m; // local function

        // Act
        decimal decimalResult = Fx(amount);
        Money result = subject with { Amount = Fx(amount) };

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
        Money subject = new(amount, currency);
        Func<decimal, decimal> fx = input => input / 3m * 3m; // delegate function

        // Act
        decimal decimalResult = fx(amount);
        Money result = subject with { Amount = fx(amount) };

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
        var subject = new Money(amount, currency);

        Money Fx(Money money)
        {
            var result = (money.Amount / 3m) * 3m;
            return new Money(result, money.Currency);
        }

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
        var subject = new Money(amount, currency);

        (decimal Amount, Currency Currency) Fx(Money money)
        {
            var result = (money.Amount / 3m) * 3m;
            return (result, money.Currency);
        }

        // Act
        (decimal Amount, Currency Currency) result = Fx(subject);
        Money moneyResult = new(result.Amount, result.Currency);
        Money moneyResult1 = subject with { Amount = Fx(subject).Amount, Currency = Fx(subject).Currency };

        // Assert
        result.Amount.Should().Be(expectedResult);
        moneyResult.Amount.Should().Be(expectedResult);
        moneyResult1.Amount.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(100, "XXX", 100)]
    [InlineData(100, "EUR", 100)]
    public void WhenNamedTupleFunction(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        var subject = new Money(amount, currency);

        UnroundedMoney Fx(Money money)
        {
            var result = (money.Amount / 3m) * 3m;
            return new UnroundedMoney(result, money.Currency);
        }

        // Act
        UnroundedMoney result = Fx(subject);
        Money moneyResult = new(result.Amount, result.Currency);
        Money moneyResult1 = subject with { Amount = Fx(subject).Amount, Currency = Fx(subject).Currency };

        // Assert
        result.Amount.Should().Be(expectedResult);
        moneyResult.Amount.Should().Be(expectedResult);
        moneyResult1.Amount.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(100, "XXX", 100)]
    [InlineData(100, "EUR", 100)]
    public void WhenApplyMethod(decimal amount, string currency, decimal expectedResult)
    {
        // Arrange
        Money subject = new(amount, currency);
        Func<decimal, decimal> adjustment = input => input / 3m * 3m; // delegate function
        Func<decimal, decimal> adjustment1 = input => input / 3m * 3m; // delegate function

        // Act
        decimal decimalResult = adjustment(amount);
        Money result = subject.Apply(adjustment);
        Money result1 = subject.Apply(amt => amt / 3m * 3m);
        Money result2 = subject.Apply(input => adjustment1(input));

        // Assert
        decimalResult.Should().Be(expectedResult);
        result.Amount.Should().Be(expectedResult);
        result1.Amount.Should().Be(expectedResult);
        result2.Amount.Should().Be(expectedResult);
    }

    public delegate decimal Adjustment(decimal input);
}
