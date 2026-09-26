namespace NodaMoney.Tests.FastMoneyBinaryOperatorsSpec;

public class MultiplyAndDivideFastMoney
{
    [Fact]
    public void MultiplyOperator_TenEurTimesTwoPointTwo_ReturnsTwentyTwoEur()
    {
        // Arrange
        var money = new FastMoney(10m, "EUR");

        // Act
        var result = money * 2.2m;

        // Assert
        result.Should().Be(new FastMoney(22m, "EUR"));
    }

    [Fact]
    public void DivideOperator_TenEurDividedByTwoPointTwo_ReturnsFourPointFiveFourFiveFiveEur()
    {
        // Arrange
        var money = new FastMoney(10m, "EUR");

        // Act
        var result = money / 2.2m;

        // Assert
        result.Should().Be(new FastMoney(4.5455m, "EUR")); // ToEven at four decimals, matching decimal.Divide then ToOACurrency
    }

    [Fact]
    public void MultiplyOperator_NegativeTimesNegative_MatchesDecimalPath()
    {
        // Arrange
        var money = new FastMoney(-10m, "EUR");
        const decimal multiplier = -2.2m;

        // Act
        var result = money * multiplier;

        // Assert
        result.Should().Be(new FastMoney(decimal.Multiply(money.Amount, multiplier), money.Currency));
        result.Amount.Should().BePositive();
    }

    [Fact]
    public void DivideOperator_NegativeDividedByNegative_MatchesDecimalPath()
    {
        // Arrange
        var money = new FastMoney(-10m, "EUR");
        const decimal divisor = -2.2m;

        // Act
        var result = money / divisor;

        // Assert
        result.Should().Be(new FastMoney(decimal.Divide(money.Amount, divisor), money.Currency));
        result.Amount.Should().BePositive();
    }

    [Fact]
    public void MultiplyOperator_ProductTiesAtFifthDecimal_RoundsToEven()
    {
        // Arrange: 0.0015 * 0.1 = 0.00015, exactly halfway between the ticks 0.0001 and 0.0002; ToEven picks 0.0002.
        var money = new FastMoney(0.0015m, "EUR");
        const decimal multiplier = 0.1m;

        // Act
        var result = money * multiplier;

        // Assert
        result.Should().Be(new FastMoney(decimal.Multiply(money.Amount, multiplier), money.Currency));
        result.Should().Be(new FastMoney(0.0002m, "EUR"));
    }

    [Fact]
    public void DivideOperator_QuotientTiesAtFifthDecimal_RoundsToEven()
    {
        // Arrange: 0.0003 / 0.4 = 0.00075, exactly halfway between the ticks 0.0007 and 0.0008; ToEven picks 0.0008.
        var money = new FastMoney(0.0003m, "EUR");
        const decimal divisor = 0.4m;

        // Act
        var result = money / divisor;

        // Assert
        result.Should().Be(new FastMoney(decimal.Divide(money.Amount, divisor), money.Currency));
        result.Should().Be(new FastMoney(0.0008m, "EUR"));
    }

    [Fact]
    public void MultiplyOperator_MultiplierMantissaAboveSixtyFourBits_TakesDecimalPath()
    {
        // Arrange: 28-digit mantissa needs more than 64 bits internally, even though the represented value is small.
        var money = new FastMoney(10m, "EUR");
        const decimal multiplier = 1.0000000000000000000000000001m;

        // Act
        var result = money * multiplier;

        // Assert
        result.Should().Be(new FastMoney(decimal.Multiply(money.Amount, multiplier), money.Currency));
    }

    [Fact]
    public void DivideOperator_DivisorWithMoreThanEighteenDecimals_TakesDecimalPath()
    {
        // Arrange
        var money = new FastMoney(10m, "EUR");
        const decimal divisor = 0.1234567890123456789m; // scale 19, above the fixed-point limit

        // Act
        var result = money / divisor;

        // Assert
        result.Should().Be(new FastMoney(decimal.Divide(money.Amount, divisor), money.Currency));
    }

    [Fact]
    public void MultiplyOperator_ProductOverflowsLong_ThrowOverflowExceptionWithFastMoneyMessage()
    {
        // Arrange
        Currency eur = CurrencyInfo.FromCode("EUR");
        FastMoney maxValue = FastMoney.MaxValue with { Currency = eur };

        // Act
        Action action = () => { var result = maxValue * 2.5m; };

        // Assert
        action.Should().Throw<OverflowException>()
            .WithMessage("Value was either too large or too small for a FastMoney.");
    }

    [Fact]
    public void DivideOperator_DivideByZero_ThrowDivideByZeroException()
    {
        // Arrange
        var money = new FastMoney(10m, "EUR");

        // Act
        Action action = () => { var result = money / 0m; };

        // Assert
        action.Should().Throw<DivideByZeroException>();
    }

    public static IEnumerable<object[]> FixedPointSweepData()
    {
        decimal[] amounts = [10m, -10m, 123.4567m, -123.4567m, 0.0001m, -0.0001m];
        decimal[] operands =
        [
            2.2m,          // scale 1
            -2.2m,         // scale 1
            0.05m,         // scale 2
            -0.05m,        // scale 2
            3.141m,        // scale 3
            -12.5678m,     // scale 4
            -1.23456m,     // scale 5
            0.000625m      // scale 6
        ];

        foreach (decimal amount in amounts)
        foreach (decimal operand in operands)
            yield return [amount, operand];
    }

    [Theory, MemberData(nameof(FixedPointSweepData))]
    public void MultiplyOperator_SweepAcrossSignsAndScales_MatchesDecimalPath(decimal amount, decimal operand)
    {
        // Arrange
        var money = new FastMoney(amount, "EUR");

        // Act
        var result = money * operand;

        // Assert
        result.Should().Be(new FastMoney(decimal.Multiply(money.Amount, operand), money.Currency));
    }

    [Theory, MemberData(nameof(FixedPointSweepData))]
    public void DivideOperator_SweepAcrossSignsAndScales_MatchesDecimalPath(decimal amount, decimal operand)
    {
        // Arrange
        var money = new FastMoney(amount, "EUR");

        // Act
        var result = money / operand;

        // Assert
        result.Should().Be(new FastMoney(decimal.Divide(money.Amount, operand), money.Currency));
    }
}
