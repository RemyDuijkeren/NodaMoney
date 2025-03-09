namespace NodaMoney.Tests.MoneySpec;

public class CreateMoney
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
