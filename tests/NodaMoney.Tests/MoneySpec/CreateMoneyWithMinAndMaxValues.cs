using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneySpec;

public class CreateMoneyWithMinAndMaxValues
{
    [Fact]
     public void WhenMaxValue()
     {
         // Arrange

         // Act
         var money = new Money(decimal.MaxValue, "EUR");

         // Assert
         money.Amount.Should().Be(decimal.MaxValue);

     }

     [Fact]
     public void WhenMinValue()
     {
         // Arrange

         // Act
         var money = new Money(decimal.MinValue, "EUR");

         // Assert
         money.Amount.Should().Be(decimal.MinValue);
     }
}
