using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace NodaMoney.Tests.MoneySpec;

public class CreateMoney
{
    readonly ITestOutputHelper _testOutputHelper;

    public CreateMoney(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void SizeInMemory()
    {
        // Arrange


        // Act
        _testOutputHelper.WriteLine($"Size of Money: {Marshal.SizeOf<Money>()}");
        _testOutputHelper.WriteLine($"Size of FastMoney: {Marshal.SizeOf<FastMoney>()}");
        _testOutputHelper.WriteLine($"Size of ExactMoney: {Marshal.SizeOf<ExactMoney>()}");
        _testOutputHelper.WriteLine($"Size of Currency: {Marshal.SizeOf<Currency>()}");


        // Assert
    }


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

     [Fact]
     public void WithDifferentCurrency()
     {
         // Arrange
         Money money = new Money(123456789.1234567890m, "EUR");

         // Act
         var newMoney = money with { Currency = CurrencyInfo.FromCode("USD")};

         // Assert
         newMoney.Should().NotBeSameAs(money);
         newMoney.Currency.Should().Be((Currency)CurrencyInfo.FromCode("USD"));
         newMoney.Amount.Should().Be(money.Amount);
     }

     [Fact]
     public void WithDifferentAmount()
     {
         // Arrange
         Money money = new Money(123456789.1234567890m, "EUR");

         // Act
         var newMoney = money with { Amount = 12.34m};

         // Assert
         newMoney.Should().NotBeSameAs(money);
         newMoney.Currency.Should().Be(money.Currency);
         newMoney.Amount.Should().Be(12.34m);
     }
}
