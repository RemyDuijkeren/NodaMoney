namespace NodaMoney.Tests.FastMoneySpec;

public class CreateMoneyWithMinAndMaxValues
{
    [Fact]
     public void WhenMaxValue()
     {
         // Arrange

         // Act
         var money = new FastMoney(FastMoney.MaxValue, "CLF"); // 4 decimals

         // Assert
         money.Amount.Should().Be(FastMoney.MaxValue);

     }

     [Fact]
     public void WhenMinValue()
     {
         // Arrange

         // Act
         var money = new FastMoney(FastMoney.MinValue, "CLF"); // 4 decimals

         // Assert
         money.Amount.Should().Be(FastMoney.MinValue);
     }

     [Fact]
     public void WhenDecimalMaxValue_ThrowArgumentException()
     {
         // Arrange

         // Act
         Action action = () => new FastMoney(decimal.MaxValue, "EUR");

         // Assert
         action.Should().Throw<ArgumentException>();

     }

     [Fact]
     public void WhenDecimalMinValue_ThrowArgumentException()
     {
         // Arrange

         // Act
         Action action = () => new FastMoney(decimal.MinValue, "EUR");

         // Assert
         action.Should().Throw<ArgumentException>();
     }

     [Fact]
     public void WhenCurrencyHas4Decimals_DontThrowException()
     {
         // Arrange
         CurrencyInfo clf = CurrencyInfo.FromCode("CLF"); // 4 decimals

         // Act
         Action action = () => new FastMoney(1m, clf);

         // Assert
         action.Should().NotThrow();
     }

     [Fact]
     public void WhenCurrencyHasMoreThan4Decimals_ThrowException()
     {
         // Arrange
         CurrencyInfo bitcoin = CurrencyInfo.FromCode("BTC"); // 8 decimals

         // Act
         Action action = () => new FastMoney(1m, bitcoin);

         // Assert
         action.Should().Throw<ArgumentException>();
     }
}
