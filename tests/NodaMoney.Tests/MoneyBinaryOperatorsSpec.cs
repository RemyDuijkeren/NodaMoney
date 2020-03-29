using System;
using System.Collections.Generic;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec
{
    public class GivenIWantToAddAndSubtractMoney
    {
        public static IEnumerable<object[]> TestData => new[]
        {
            new object[] { 101m, 99m, 200m }, // whole numbers
            new object[] { 100m, 0.01m, 100.01m }, // fractions
            new object[] { 100.999m, 0.9m, 101.899m }, // overflow 
            new object[] { 100.5m, 0.9m, 101.4m }, // overflow
            new object[] { 100.999m, -0.9m, 100.099m }, // negative
            new object[] { -100.999m, -0.9m, -101.899m } // negative
        };

        [Theory, MemberData(nameof(TestData))]
        public void WhenUsingAdditionOperator_ThenMoneyShouldBeAdded(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1);
            var money2 = new Money(value2);

            var result = money1 + money2;

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money1);
            result.Should().NotBeSameAs(money2);
        }

        [Theory, MemberData(nameof(TestData))]
        public void WhenUsingAdditionMethod_ThenMoneyShouldBeAdded(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1);
            var money2 = new Money(value2);

            var result = Money.Add(money1, money2);

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money1);
            result.Should().NotBeSameAs(money2);
        }

        [Theory, MemberData(nameof(TestData))]
        public void WhenUsingSubtractionOperator_ThenMoneyShouldBeSubtracted(decimal expected, decimal value2, decimal value1)
        {
            var money1 = new Money(value1);
            var money2 = new Money(value2);

            var result = money1 - money2;

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money1);
            result.Should().NotBeSameAs(money2);
        }

        [Theory, MemberData(nameof(TestData))]
        public void WhenUsingSubtractionMethod_ThenMoneyShouldBeSubtracted(decimal expected, decimal value2, decimal value1)
        {
            var money1 = new Money(value1);
            var money2 = new Money(value2);

            var result = Money.Subtract(money1, money2);

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money1);
            result.Should().NotBeSameAs(money2);
        }

        [Theory, MemberData(nameof(TestData))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public void WhenUsingAdditionOperatorWithDifferentCurrency_ThenThrowException(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1, "EUR");
            var money2 = new Money(value2, "USD");

            Action action = () => { var result = money1 + money2; };

            action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
        }

        [Theory, MemberData(nameof(TestData))]
        public void WhenUsingAdditionMethodWithDifferentCurrency_ThenThrowException(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1, "EUR");
            var money2 = new Money(value2, "USD");

            Action action = () => Money.Add(money1, money2);

            action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
        }

        [Theory, MemberData(nameof(TestData))]
        public void WhenUsingSubtractionOperatorWithDifferentCurrency_ThenThrowException(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1, "EUR");
            var money2 = new Money(value2, "USD");

            Action action = () => { var result = money1 - money2; };

            action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
        }

        [Theory, MemberData(nameof(TestData))]
        public void WhenUsingSubtractionMethodWithDifferentCurrency_ThenThrowException(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1, "EUR");
            var money2 = new Money(value2, "USD");

            Action action = () => Money.Subtract(money1, money2);

            action.Should().Throw<InvalidCurrencyException>().WithMessage("The requested operation expected the currency*");
        }
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
    }

    [Collection(nameof(NoParallelization))]
    public class GivenIWantToAddAndSubtractMoneyWithDecimal
    {
        public static IEnumerable<object[]> TestData => new[]
        {
            new object[] { 101m, 99m, 200m }, // whole numbers
            new object[] { 100m, 0.01m, 100.01m }, // fractions
            new object[] { 100.999m, 0.9m, 101.899m }, // overflow 
            new object[] { 100.5m, 0.9m, 101.4m }, // overflow
            new object[] { 100.999m, -0.9m, 100.099m }, // negative
            new object[] { -100.999m, -0.9m, -101.899m } // negative
        };

        [Theory, MemberData(nameof(TestData))]
        [UseCulture("en-us")]
        public void WhenUsingAdditionOperator_ThenMoneyShouldBeAdded(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1, "EUR");

            Money result1 = money1 + value2;
            Money result2 = value2 + money1;

            result1.Should().Be(new Money(expected, "EUR"));
            result1.Should().NotBeSameAs(money1);
            result2.Should().Be(new Money(expected, "EUR"));
            result2.Should().NotBeSameAs(money1);
        }

        [Theory, MemberData(nameof(TestData))]
        [UseCulture("en-us")]
        public void WhenUsingAdditionMethod_ThenMoneyShouldBeAdded(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1, "EUR");

            var result = Money.Add(money1, value2);

            result.Should().Be(new Money(expected, "EUR"));
            result.Should().NotBeSameAs(money1);
        }

        [Theory, MemberData(nameof(TestData))]
        [UseCulture("en-us")]
        public void WhenUsingSubtractionOperator_ThenMoneyShouldBeAdded(decimal expected, decimal value2, decimal value1)
        {
            var money1 = new Money(value1, "EUR");

            Money result1 = money1 - value2;
            Money result2 = value2 - money1;

            result1.Should().Be(new Money(expected, "EUR"));
            result1.Should().NotBeSameAs(money1);
            result2.Should().Be(new Money(expected, "EUR"));
            result2.Should().NotBeSameAs(money1);
        }

        [Theory, MemberData(nameof(TestData))]
        [UseCulture("en-us")]
        public void WhenUsingSubtractionMethod_ThenMoneyShouldBeSubtracted(decimal expected, decimal value2, decimal value1)
        {
            var money1 = new Money(value1, "EUR");

            var result = Money.Subtract(money1, value2);

            result.Should().Be(new Money(expected, "EUR"));
            result.Should().NotBeSameAs(money1);
        }
    }

    public class GivenIWantToMultiplyAndDivideMoney
    {
        public static IEnumerable<object[]> TestDataDecimal => new[]
        {
            new object[] { 10m, 15m, 150m },
            new object[] { 100.12m, 0.5m, 50.06m },
            new object[] { 100.12m, 5m, 500.60m },
            new object[] { -100.12m, 0.5m, -50.06m },
            new object[] { -100.12m, 5m, -500.60m }
        };

        [Theory, MemberData(nameof(TestDataDecimal))]
        public void WhenUsingMultiplyOperatorWithDecimal_ThenMoneyShouldBeMultiplied(decimal value, decimal multiplier, decimal expected)
        {
            var money = new Money(value);

            var result1 = money * multiplier;
            var result2 = multiplier * money;
                
            result1.Should().Be(new Money(expected));            
            result1.Should().NotBeSameAs(money);
            result2.Should().Be(new Money(expected));
            result2.Should().NotBeSameAs(money);
        }

        [Theory, MemberData(nameof(TestDataDecimal))]
        public void WhenUsingMultiplyMethodWithDecimal_ThenMoneyShouldBeMultiplied(decimal value, decimal multiplier, decimal expected)
        {
            var money = new Money(value);

            var result = Money.Multiply(money, multiplier);

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money);
        }

        [Theory, MemberData(nameof(TestDataDecimal))]
        public void WhenUsingDivisionOperatorWithDecimal_ThenMoneyShouldBeDivided(decimal expected, decimal divider, decimal value)
        {
            var money = new Money(value);

            var result = money / divider;

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money);
        }

        [Theory, MemberData(nameof(TestDataDecimal))]
        public void WhenUsingDivisionMethodWithDecimal_ThenMoneyShouldBeDivided(decimal expected, decimal divider, decimal value)
        {
            var money = new Money(value);

            var result = Money.Divide(money, divider);

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money);
        }

        public static IEnumerable<object[]> TestDataInteger => new[]
        {
            new object[] { 10m, 15, 150 },
            new object[] { 100.12m, 3, 300.36m },
            new object[] { 100.12m, 5, 500.60m },
            new object[] { -100.12m, 3, -300.36m },
            new object[] { -100.12m, 5, -500.60m }
        };

        [Theory, MemberData(nameof(TestDataInteger))]
        public void WhenUsingMultiplyOperatorWithInteger_ThenMoneyShouldBeMultiplied(decimal value, int multiplier, decimal expected)
        {
            var money = new Money(value);

            var result1 = money * multiplier;
            var result2 = multiplier * money;

            result1.Should().Be(new Money(expected));
            result1.Should().NotBeSameAs(money);
            result2.Should().Be(new Money(expected));
            result2.Should().NotBeSameAs(money);
        }

        [Theory, MemberData(nameof(TestDataInteger))]
        public void WhenUsingMultiplyMethodWithInteger_ThenMoneyShouldBeMultiplied(decimal value, int multiplier, decimal expected)
        {
            var money = new Money(value);

            var result = Money.Multiply(money, multiplier);

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money);
        }

        [Theory, MemberData(nameof(TestDataInteger))]
        public void WhenUsingDivisionOperatorWithInteger_ThenMoneyShouldBeDivided(decimal expected, int divider, decimal value)
        {
            var money = new Money(value);

            var result = money / divider;

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money);
        }

        [Theory, MemberData(nameof(TestDataInteger))]
        public void WhenUsingDivisionMethodWithInteger_ThenMoneyShouldBeDivided(decimal expected, int divider, decimal value)
        {
            var money = new Money(value);

            var result = Money.Divide(money, divider);

            result.Should().Be(new Money(expected));
            result.Should().NotBeSameAs(money);
        }

        public static IEnumerable<object[]> TestDataMoney => new[]
        {
            new object[] { 150m, 15m, 10m },
            new object[] { 100.12m, 3m, 100.12m/3m },
        };

        [Theory, MemberData(nameof(TestDataMoney))]
        public void WhenUsingDivisionOperatorWithMoney_ThenResultShouldBeRatio(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1);
            var money2 = new Money(value2);

            var result = money1 / money2;

            result.Should().Be(expected); // ratio
        }

        [Theory, MemberData(nameof(TestDataMoney))]
        public void WhenUsingDivisionMethodWithMoney_ThenResultShouldBeRatio(decimal value1, decimal value2, decimal expected)
        {
            var money1 = new Money(value1);
            var money2 = new Money(value2);

            var result = Money.Divide(money1, money2);

            result.Should().Be(expected); // ratio
        }
    }
}
