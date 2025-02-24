namespace NodaMoney.Tests.UnroundedMoneySpec;

public class CreateMoneyWithMinAndMaxValues
{
    [Fact]
     public void WhenMaxValue()
     {
         // Arrange

         // Act
         var money = new UnroundedMoney(decimal.MaxValue, "EUR");

         // Assert
         money.Amount.Should().Be(decimal.MaxValue);

     }

     [Fact]
     public void WhenMinValue()
     {
         // Arrange

         // Act
         var money = new UnroundedMoney(decimal.MinValue, "EUR");

         // Assert
         money.Amount.Should().Be(decimal.MinValue);
     }
}
