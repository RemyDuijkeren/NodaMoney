namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

public class MultiplyAndDivideMoney
{
    public static IEnumerable<object[]> TestDataDecimal =>
    [
        // Original test cases
        [10m, 15m, 150m],
        [100.12m, 0.5m, 50.06m],
        [100.12m, 5m, 500.60m],
        [-100.12m, 0.5m, -50.06m],
        [-100.12m, 5m, -500.60m],

        // Edge cases with zero
        [0m, 15m, 0m], // Zero amount

        // Extreme values
        [decimal.MaxValue / 10m, 0.1m, decimal.MaxValue / 100m],     // Near maximum value
        [decimal.MinValue / 10m, 0.1m, decimal.MinValue / 100m],     // Near minimum value

        // Precision edge cases
        [0.0000001m, 10m, 0.000001m],                     // Very small value
        [1_000_000_000_000_000m, 0.000_000_000_1m, 100_000m],   // Precision shift
        [0.1m, 0.1m, 0.01m],                              // Double precision loss test
        [0.333333333333333333m, 3m, 0.99m],               // Rounding with repeating decimal

        // Sign changes
        [100m, -1m, -100m],        // Positive to negative
        [-100m, -1m, 100m],        // Negative to positive

        // Special cases for financial calculations
        [9.99m, 0.99m, 9.8901m],   // Common retail pricing
        [19.99m, 1.1m, 21.989m],   // Tax calculation approximation

        // Large number with small multiplier
        [1000000m, 0.000001m, 1m],

        // Small number with a large multiplier
        [0.01m, 1000000m, 10000m],

        // Boundary value that requires rounding
        [1.234567890123456789012345678m, 1m, 1.234567890123456789012345678m],

        // Cases involving significant zeros
        [1.10m, 2m, 2.20m],
        [1.00m, 2.10m, 2.10m]
    ];

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void MultiplyOperatorByDecimal_ReturnMultipliedMoney(decimal value, decimal multiplier, decimal expected)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result1 = money * multiplier;
        var result2 = multiplier * money;

        // Assert
        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void MultiplyMethodByDecimal_ReturnMultipliedMoney(decimal value, decimal multiplier, decimal expected)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result = Money.Multiply(money, multiplier);

        // Assert
        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void DivideOperatorByDecimal_ReturnDividedMoney(decimal expected, decimal divider, decimal value)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result = money / divider;

        // Assert
        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataDecimal))]
    public void DivideMethodByDecimal_ReturnDividedMoney(decimal expected, decimal divider, decimal value)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result = Money.Divide(money, divider);

        // Assert
        Money expected1 = new Money(expected, "EUR");
        result.Should().Be(expected1);
        result.Should().NotBeSameAs(money);
    }

    [Fact]
    public void MultiplyIsMoreThenMaxValue_ThrowOverflowException()
    {
        // Arrange
        Money maxValueMoney = new(decimal.MaxValue);

        // Act
        Action action = () => Money.Multiply(maxValueMoney, 2m);

        // Assert
        action.Should().Throw<OverflowException>().WithMessage("Value was either too large or too small for a Money.");
    }

    [Fact]
    public void DivideByZero_ThrowDivideByZeroException()
    {
        // Arrange
        Money maxValueMoney = new(decimal.MaxValue);

        // Act
        Action action = () => Money.Divide(maxValueMoney, 0m);

        // Assert
        action.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void MultiplyByIntIsMoreThenMaxValue_ThrowDivideByZeroException()
    {
        // Arrange
        Money maxValueMoney = new(decimal.MaxValue);

        // Act
        Action action = () => Money.Multiply(maxValueMoney, 2);

        // Assert
        action.Should().Throw<OverflowException>().WithMessage("Value was either too large or too small for a Money.");
    }

    [Fact]
    public void DivideByZeroInt_ThrowDivideByZeroException()
    {
        // Arrange
        Money maxValueMoney = new(decimal.MaxValue);

        // Act
        Action action = () => Money.Divide(maxValueMoney, 0);

        // Assert
        action.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void Divide_ShouldRound()
    {
        // Arrange
        decimal amount = 1000m;
        decimal divider = 7m;
        decimal unrounded = 142.857142857142857142857142857m;
        Money money = new(amount, "EUR");
        Money expected = new(unrounded, "EUR"); // rounds

        // Act
        var result = Money.Divide(money, divider);

        // Assert
        result.Should().Be(expected);
        result.Scale.Should().Be(2, "euro is 2 decimals");
        result.Amount.Should().NotBe(unrounded);
    }

    [Fact]
    public void Multiply_ShouldRound()
    {
        // Arrange
        decimal amount = 1000m;
        decimal multiply = 1m/7m;
        decimal unrounded = 142.857142857142857142857142857m;
        Money money = new(amount, "EUR");
        Money expected = new(unrounded, "EUR"); // rounds

        // Act
        var result = Money.Multiply(money, multiply);

        // Assert
        result.Should().Be(expected);
        result.Scale.Should().Be(2, "euro is 2 decimals");
        result.Amount.Should().NotBe(unrounded);
    }

    public static IEnumerable<object[]> TestDataInteger =>
    [
        // Original test cases
        [10m, 15, 150],
        [100.12m, 3, 300.36m],
        [100.12m, 5, 500.60m],
        [-100.12m, 3, -300.36m],
        [-100.12m, 5, -500.60m],

        // Zero handling
        [0m, 15, 0], // Zero money

        // Extreme values
        [decimal.MaxValue / 2, 1, decimal.MaxValue / 2], // Large value, small multiplier
        [decimal.MinValue / 2, 1, decimal.MinValue / 2], // Large negative value, small multiplier
        [0.01m, 1_000_000, 10_000], // Small value, large multiplier
        [0.01m, int.MaxValue / 1000000, 21.47483647m], // Small value with max int (scaled)

        // Boundary cases
        [1m, int.MaxValue, 2147483647m], // Max int multiplier
        [1m, int.MinValue, -2147483648m], // Min int multiplier
        [0.01m, 10, 0.10m], // Precision test

        // Sign changes
        [100m, -1, -100], // Positive to negative
        [-100m, -1, 100], // Negative to positive
        [1.5m, -2, -3.0m], // Fractional with sign change

        // Currency-specific cases (assuming currency with 2 decimal places)
        [9.99m, 2, 19.98m], // Common retail pricing
        [19.99m, 3, 59.97m], // Bundle pricing
        [1.25m, 4, 5.00m], // Quarter multiplication

        // Rounding edge cases
        [0.33333333333333333333333333m, 3, 0.99m], // Repeating decimal

        // Mixed precision
        [123.45m, 2, 246.90m], // Standard money format

        // Specific financial cases
        [29.95m, 12, 359.40m], // Monthly to yearly conversion
        [9.99m, 100, 999.00m], // Percentage calculation (100%)
        [20m, 5, 100m], // Simple whole number case
        [20.25m, 4, 81.00m], // Testing preservation of trailing zeros

        // Near overflow/underflow
        [1e-28m, 1000, 1e-25m], // Very small number

        // Special values
        [0.01m, 1, 0.01m], // Smallest typical currency unit
        [0.01m, 1000, 10.000m] // Twi decimal places with trailing zeros
    ];

    [Theory, MemberData(nameof(TestDataInteger))]
    public void MultiplyOperatorByInt_ReturnMultipliedMoney(decimal value, int multiplier, decimal expected)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result1 = money * multiplier;
        var result2 = multiplier * money;

        // Assert
        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataInteger))]
    public void MultiplyMethodByInt_ReturnMultipliedMoney(decimal value, int multiplier, decimal expected)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result = Money.Multiply(money, multiplier);

        // Assert
        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataInteger))]
    public void DivideOperatorByInt_ReturnDividedMoney(decimal expected, int divider, decimal value)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result = money / divider;

        // Assert
        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    [Theory, MemberData(nameof(TestDataInteger))]
    public void DivideMethodByInt_ReturnDividedMoney(decimal expected, int divider, decimal value)
    {
        // Arrange
        var money = new Money(value, "EUR");

        // Act
        var result = Money.Divide(money, divider);

        // Assert
        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money);
    }

    public static IEnumerable<object[]> TestDataMoney =>
    [
        [150m, 15m, 10m],
        [100.12m, 3m, decimal.Divide(100.12m, 3m)],
        [100m, 3m, decimal.Divide(100m, 3m)]
    ];

    [Theory, MemberData(nameof(TestDataMoney))]
    public void DivideOperatorByMoney_ReturnDecimalRatio(decimal value1, decimal value2, decimal expected)
    {
        // Arrange
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "EUR");

        // Act
        var result = money1 / money2;

        // Assert
        result.Should().BeOfType(typeof(decimal));
        result.Should().Be(expected); // ratio
    }

    [Theory, MemberData(nameof(TestDataMoney))]
    public void DivideMethodByMoney_ReturnDecimalRatio(decimal value1, decimal value2, decimal expected)
    {
        // Arrange
        var money1 = new Money(value1, "EUR");
        var money2 = new Money(value2, "EUR");

        // Act
        var result = Money.Divide(money1, money2);

        // Assert
        result.Should().BeOfType(typeof(decimal));
        result.Should().Be(expected); // ratio
    }
}
