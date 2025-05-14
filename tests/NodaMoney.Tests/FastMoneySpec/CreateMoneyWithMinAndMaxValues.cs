namespace NodaMoney.Tests.FastMoneySpec;

public class CreateMoneyWithMinAndMaxValues
{
    [Fact]
     public void WhenMaxValue()
     {
         // Arrange
         Currency eur = CurrencyInfo.FromCode("EUR");

         // Act
         var result = FastMoney.MinValue with { Currency = eur };

         // Assert
         result.Amount.Should().Be(long.MinValue / 10_000L);
         result.Currency.Should().Be(eur);
     }

     [Fact]
     public void WhenMinValue()
     {
         // Arrange
         Currency eur = CurrencyInfo.FromCode("EUR");

         // Act
         var result = FastMoney.MinValue with { Currency = eur };

         // Assert
         result.Amount.Should().Be(long.MinValue / 10_000L);
         result.Currency.Should().Be(eur);
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
