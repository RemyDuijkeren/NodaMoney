using System.Collections.Generic;

namespace NodaMoney.Tests.FastMoneyBinaryOperatorsSpec;

public class AddAndSubtractFastMoney
{
    public static IEnumerable<object[]> TestData =>
    [
        [101, 99, 200],
        [1, 10, 11],
        [1, -10, -9],
        [100, 0.01, 100.01],
        [0, 10, 10],
        [10, -10, 0],
    ];

    [Theory, MemberData(nameof(TestData))]
    public void AddOperator_ReturnSumFastMoney(decimal value1, decimal value2, decimal expected)
    {
        // Arrange
        var money1 = new FastMoney(value1, "EUR");
        var money2 = new FastMoney(value2, "EUR");

        // Act
        var result = money1 + money2;

        // Assert
        result.Should().Be(new FastMoney(expected, "EUR"));
    }

    [Theory, MemberData(nameof(TestData))]
    public void SubtractOperator_ReturnSubtractedFastMoney(decimal expected, decimal value2, decimal value1)
    {
        // Arrange
        var money1 = new FastMoney(value1, "EUR");
        var money2 = new FastMoney(value2, "EUR");

        // Act
        var result = money1 - money2;

        // Assert
        result.Should().Be(new FastMoney(expected, "EUR"));
    }

    [Fact]
    public void AddOperator_MaxValueEurPlusOneEur_ThrowOverflowExceptionWithFastMoneyMessageAndArithmeticInnerException()
    {
        // Arrange
        Currency eur = CurrencyInfo.FromCode("EUR");
        FastMoney maxValueEur = FastMoney.MaxValue with { Currency = eur };
        FastMoney oneEur = new(1m, eur);

        // Act
        Action action = () => FastMoney.Add(maxValueEur, oneEur);

        // Assert
        action.Should().Throw<OverflowException>()
            .WithMessage("Value was either too large or too small for a FastMoney.")
            .WithInnerException<OverflowException>();
    }

    [Fact]
    public void SubtractOperator_MinValueEurMinusOneEur_ThrowOverflowExceptionWithFastMoneyMessageAndArithmeticInnerException()
    {
        // Arrange
        Currency eur = CurrencyInfo.FromCode("EUR");
        FastMoney minValueEur = FastMoney.MinValue with { Currency = eur };
        FastMoney oneEur = new(1m, eur);

        // Act
        Action action = () => FastMoney.Subtract(minValueEur, oneEur);

        // Assert
        action.Should().Throw<OverflowException>()
            .WithMessage("Value was either too large or too small for a FastMoney.")
            .WithInnerException<OverflowException>();
    }

    [Fact]
    public void MultiplyOperator_MaxValueTimesTwo_ThrowOverflowExceptionWithFastMoneyMessage()
    {
        // Arrange
        FastMoney maxValue = FastMoney.MaxValue;

        // Act
        Action action = () => { var result = maxValue * 2L; };

        // Assert
        action.Should().Throw<OverflowException>()
            .WithMessage("Value was either too large or too small for a FastMoney.");
    }

    [Fact]
    public void IncrementOperator_MaxValue_ThrowOverflowExceptionWithFastMoneyMessage()
    {
        // Arrange
        FastMoney maxValue = FastMoney.MaxValue;

        // Act
        Action action = () => { var result = ++maxValue; };

        // Assert
        action.Should().Throw<OverflowException>()
            .WithMessage("Value was either too large or too small for a FastMoney.");
    }

    [Fact]
    public void DecrementOperator_MinValue_ThrowOverflowExceptionWithFastMoneyMessage()
    {
        // Arrange
        FastMoney minValue = FastMoney.MinValue;

        // Act
        Action action = () => { var result = --minValue; };

        // Assert
        action.Should().Throw<OverflowException>()
            .WithMessage("Value was either too large or too small for a FastMoney.")
            .WithInnerException<OverflowException>();
    }

    [Fact]
    public void AddOperator_MaxValueEurPlusOneDecimal_ThrowOverflowExceptionWithFastMoneyMessage()
    {
        // Arrange
        Currency eur = CurrencyInfo.FromCode("EUR");
        FastMoney maxValueEur = FastMoney.MaxValue with { Currency = eur };

        // Act
        Action action = () => { var result = maxValueEur + 1m; };

        // Assert
        action.Should().Throw<OverflowException>()
            .WithMessage("Value was either too large or too small for a FastMoney.")
            .WithInnerException<OverflowException>();
    }

    [Fact]
    public void SubtractOperator_MinValueEurMinusOneDecimal_ThrowOverflowExceptionWithFastMoneyMessage()
    {
        // Arrange
        Currency eur = CurrencyInfo.FromCode("EUR");
        FastMoney minValueEur = FastMoney.MinValue with { Currency = eur };

        // Act
        Action action = () => { var result = minValueEur - 1m; };

        // Assert
        action.Should().Throw<OverflowException>()
            .WithMessage("Value was either too large or too small for a FastMoney.")
            .WithInnerException<OverflowException>();
    }
}
