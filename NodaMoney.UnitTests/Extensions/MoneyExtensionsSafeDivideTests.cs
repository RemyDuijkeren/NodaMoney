using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodaMoney.Extensions.UnitTests
{
    public class MoneyExtensionsSafeDivideTests
    {
        [TestClass]
        public class GivenIWantToSafelyDivideMoney
        {
            private Money _eurocent5 = new Money(0.05m, "EUR");
            private Money _euro1 = new Money(1.0m, "EUR");

            [TestMethod]
            public void WhenDividing5CentsByTwo_ThenDivisionShouldNotLoseCents()
            {
                // Foemmel's Conundrum test (see PEAA, page 495)
                // http://thierryroussel.free.fr/java/books/martinfowler/www.martinfowler.com/isa/money.html

                var enumerable = _eurocent5.SafeDivide(2).ToList();

                enumerable.Should().NotBeNullOrEmpty()
                    .And.HaveCount(2)
                    .And.ContainItemsAssignableTo<Money>()
                    .And.ContainInOrder(new[]
                    { 
                        new Money(0.02m, "EUR"),
                        new Money(0.03m, "EUR")
                    });

                enumerable.Sum(m => m.Amount).Should().Be(_eurocent5.Amount);
            }

            [TestMethod]
            public void WhenDividing5CentsByThree_ThenDivisionShouldNotLoseCents()
            {
                var enumerable = _eurocent5.SafeDivide(3).ToList();

                enumerable.Should().NotBeNullOrEmpty()
                    .And.HaveCount(3)
                    .And.ContainItemsAssignableTo<Money>()
                    .And.ContainInOrder(new[]
                    {
                        new Money(0.02m, "EUR"),
                        new Money(0.02m, "EUR"),
                        new Money(0.01m, "EUR")
                    });

                enumerable.Sum(m => m.Amount).Should().Be(_eurocent5.Amount);
            }

            [TestMethod]
            public void WhenDividing1EuroByGivenRatios_ThenDivisionShouldNotLoseCents()
            {             
                var enumerable = _euro1.SafeDivide(new[] { 2, 3, 3 }).ToList();

                enumerable.Should().NotBeNullOrEmpty()
                    .And.HaveCount(3)
                    .And.ContainItemsAssignableTo<Money>()
                    .And.ContainInOrder(new[]
                    {
                        new Money(0.25m, "EUR"),
                        new Money(0.38m, "EUR"),
                        new Money(0.37m, "EUR")
                    });

                enumerable.Sum(m => m.Amount).Should().Be(_euro1.Amount);
            }

            [TestMethod]
            public void WhenDividing1EuroByGivenRatiosWhereOneIsVerySmall_ThenDivisionShouldNotLoseCents()
            {
                var enumerable = _euro1.SafeDivide(new[] { 200, 300, 1 }).ToList();

                enumerable.Should().NotBeNullOrEmpty()
                    .And.HaveCount(3)
                    .And.ContainItemsAssignableTo<Money>()
                    .And.ContainInOrder(new[]
                    {
                        new Money(0.40m, "EUR"),
                        new Money(0.60m, "EUR"),
                        new Money(0.0m, "EUR")
                    });

                enumerable.Sum(m => m.Amount).Should().Be(_euro1.Amount);
            }

            [TestMethod]
            public void WhenDividingByMinus1_ThenAnExceptionShouldBeThrowed()
            {               
                Action action = () => _euro1.SafeDivide(-1).ToList();

                action.ShouldThrow<ArgumentOutOfRangeException>();
            }

            [TestMethod]
            public void WhenDividingByGivenRatiosWithMinus1_ThenAnExceptionShouldBeThrowed()
            {
                Action action = () => _euro1.SafeDivide(new[] { 2, -1, 3 }).ToList();

                action.ShouldThrow<ArgumentOutOfRangeException>()
                    .WithMessage("*1*");
            }
        }
    }
}
