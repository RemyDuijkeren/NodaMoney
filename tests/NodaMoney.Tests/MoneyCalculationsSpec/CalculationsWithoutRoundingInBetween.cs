namespace NodaMoney.Tests.MoneyCalculationsSpec;

using UnroundedMoney = (decimal Amount, Currency Currency);

public class CalculationsWithoutRoundingInBetween
{
    [Fact]
    public void MultiplyDivide()
    {
        // Test that 1000 * 15 / 100 == 150

        // Arrange
        decimal value = 1000;
        decimal multiplier = 15;
        decimal divisor = 100;

        var subject = new Money(value, "USD");

        // Act
        //decimal result1 = value * multiplier / divisor;
        var result = subject * multiplier / divisor;

        // Assert
        result.Should().Be(new Money(150, "USD"));
    }


    [Fact]
    public void WhenDecimalLocalFunction()
    {
        decimal Fx(decimal amount)
        {
            return amount / 3m * 3m;
        }

        // Calculating with decimal
        decimal result1 = Fx(5m);
        result1.Should().Be((5m / 3m) * 3m);
        result1.Should().Be(5.0000000000000000000000000001M);

        // Calculating with Money
        var subject = new Money(5m, "USD");
        var result = (subject / 3m) * 3m;
        result.Amount.Should().Be(5.01M); // expect 5m
        //Money result = subject.Perform(fx);

        // Calculating with decimal and final as Rounded Money
        var subject2 = new Money(Fx(5m));
        subject2.Amount.Should().Be(5.00M); // ok
    }

    [Fact]
    public void WhenDecimalDelegateFunction()
    {
        Func<decimal, decimal> fx = amount => amount / 3m * 3m;

        // Calculating with decimal
        decimal result1 = fx(5m);
        result1.Should().Be((5m / 3m) * 3m);
        result1.Should().Be(5.0000000000000000000000000001M);

        // Calculating with Money
        var subject = new Money(5m, "USD");
        var result = (subject / 3m) * 3m;
        result.Amount.Should().Be(5.01M); // expect 5m
        //Money result = subject.Perform(fx);

        // Calculating with decimal and final as Rounded Money
        var subject2 = new Money(fx(5m));
        subject2.Amount.Should().Be(5.00M); // ok
    }

    [Fact]
    public void WhenMoneyFunction()
    {
        Money Fx(Money money)
        {
            var result = (money.Amount / 3m) * 3m;
            return new Money(result, money.Currency);
        }

        // Calculating with Money
        var subject = new Money(5m, "USD");
        var result = (subject / 3m) * 3m;
        result.Amount.Should().Be(5.01M); // expect 5m
        //Money result = subject.Perform(fx);

        // Calculating with Money and final as Rounded Money
        var subject3 = new Money(5m);
        var result3 = Fx(subject3);
        result3.Amount.Should().Be(5M); // expect 5m

        (decimal amount, Currency currency) = Fx(subject3);
        (decimal Amount, Currency Currency) unrounded = (amount, currency);
    }

    [Fact]
    public void WhenTupleFunction()
    {
        (decimal Amount, Currency Currency) Fx(Money money)
        {
            var result = (money.Amount / 3m) * 3m;
            return (result, money.Currency);
        }

        // Calculating with Money
        var subject = new Money(5m, "USD");
        var result = (subject / 3m) * 3m;
        result.Amount.Should().Be(5.01M); // expect 5m

        // Calculating with Money and final as Rounded Money
        var subject3 = new Money(5m);
        var result3 = Fx(subject3);
        result3.Amount.Should().Be(5.0000000000000000000000000001M);

        (decimal Amount, Currency Currency) unrounded = Fx(subject3);
    }

    [Fact]
    public void WhenNamedTupleFunction()
    {
        UnroundedMoney Fx(Money money)
        {
            var result = (money.Amount / 3m) * 3m;
            return new UnroundedMoney(result, money.Currency);
        }

        // Calculating with Money
        var subject = new Money(5m, "USD");
        var result = (subject / 3m) * 3m;
        result.Amount.Should().Be(5.01M); // expect 5m

        // Calculating with Money and final as Rounded Money
        var subject3 = new Money(5m);
        var result3 = Fx(subject3);
        result3.Amount.Should().Be(5.0000000000000000000000000001M);

        UnroundedMoney unrounded = Fx(subject3);
    }
}
