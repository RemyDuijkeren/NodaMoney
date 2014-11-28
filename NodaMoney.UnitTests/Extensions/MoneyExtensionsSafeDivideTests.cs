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
            [TestMethod]
            public void WhenDividing5CentsByTwo_ThenDivisionShouldNotLoseCents()
            {
                // Foemmel's Conundrum test (see PEAA, page 495)
                // http://thierryroussel.free.fr/java/books/martinfowler/www.martinfowler.com/isa/money.html
                var euro = new Money(0.05m, "EUR");

                var enumerable = euro.SafeDivide(2).ToList();

                enumerable.Should().NotBeNullOrEmpty()
                    .And.HaveCount(2)
                    .And.ContainItemsAssignableTo<Money>()
                    .And.ContainInOrder(new[]
                    { 
                        new Money(0.02m, "EUR"),
                        new Money(0.03m, "EUR")
                    });

                enumerable.Sum(m => m.Amount).Should().Be(0.05m);
            }

            [TestMethod]
            public void WhenDividing5CentsByThree_ThenDivisionShouldNotLoseCents()
            {
                var euro = new Money(0.05m, "EUR");

                var enumerable = euro.SafeDivide(3).ToList();

                enumerable.Should().NotBeNullOrEmpty()
                    .And.HaveCount(3)
                    .And.ContainItemsAssignableTo<Money>()
                    .And.ContainInOrder(new[]
                    {
                        new Money(0.02m, "EUR"),
                        new Money(0.02m, "EUR"),
                        new Money(0.01m, "EUR")
                    });

                enumerable.Sum(m => m.Amount).Should().Be(0.05m);
            }

            [TestMethod]
            public void WhenDividing1EuroByGivenRatios_ThenDivisionShouldNotLoseCents()
            {
                var euro = new Money(1m, "EUR");

                var enumerable = euro.SafeDivide(new[] { 2, 3, 3 }).ToList();

                enumerable.Should().NotBeNullOrEmpty()
                    .And.HaveCount(3)
                    .And.ContainItemsAssignableTo<Money>()
                    .And.ContainInOrder(new[]
                    {
                        new Money(0.25m, "EUR"),
                        new Money(0.38m, "EUR"),
                        new Money(0.37m, "EUR")
                    });

                enumerable.Sum(m => m.Amount).Should().Be(1m);
            }

            [TestMethod]
            public void WhenDividing1EuroByGivenRatiosWhereOneIsVerySmall_ThenDivisionShouldNotLoseCents()
            {
                var euro = new Money(1.0m, "EUR");

                var enumerable = euro.SafeDivide(new[] { 200, 300, 1 }).ToList();

                enumerable.Should().NotBeNullOrEmpty()
                    .And.HaveCount(3)
                    .And.ContainItemsAssignableTo<Money>()
                    .And.ContainInOrder(new[]
                    {
                        new Money(0.40m, "EUR"),
                        new Money(0.60m, "EUR"),
                        new Money(0.0m, "EUR")
                    });

                enumerable.Sum(m => m.Amount).Should().Be(1.0m);
            }
        }
    }
}
