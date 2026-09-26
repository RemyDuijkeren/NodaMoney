namespace NodaMoney.Tests.FastMoneySpec;

public class CreateFastMoneyWithMinAndMaxValues
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
     public void WhenDecimalMaxValue_ThrowArgumentOutOfRangeException()
     {
         // Arrange

         // Act
         Action action = () => new FastMoney(decimal.MaxValue, "EUR");

         // Assert
         action.Should().Throw<ArgumentOutOfRangeException>();

     }

     [Fact]
     public void WhenDecimalMinValue_ThrowArgumentOutOfRangeException()
     {
         // Arrange

         // Act
         Action action = () => new FastMoney(decimal.MinValue, "EUR");

         // Assert
         action.Should().Throw<ArgumentOutOfRangeException>();
     }

     [Fact]
     public void WhenAmountIsExactMaxValueLong_DontThrowException()
     {
         // Arrange
         const long maxValueLong = long.MaxValue / 10_000L; // 922337203685477

         // Act
         Action action = () => new FastMoney(maxValueLong, "EUR");

         // Assert
         action.Should().NotThrow();
     }

     [Fact]
     public void WhenAmountIsExactMinValueLong_DontThrowException()
     {
         // Arrange
         const long minValueLong = long.MinValue / 10_000L; // -922337203685477

         // Act
         Action action = () => new FastMoney(minValueLong, "EUR");

         // Assert
         action.Should().NotThrow();
     }

     [Fact]
     public void WhenAmountIsAtTheTrueUpperBound_DontThrowException()
     {
         // Arrange: FastMoney's documented range tops out at 922,337,203,685,477.5807 (the OACurrency boundary),
         // half a tick above the whole-unit MaxValueLong constant.
         const decimal amount = 922337203685477.5807m;

         // Act
         Action action = () => new FastMoney(amount, "EUR");

         // Assert
         action.Should().NotThrow();
     }

     [Fact]
     public void WhenAmountIsOneTickBeyondTheTrueUpperBound_ThrowArgumentOutOfRangeException()
     {
         // Arrange
         const decimal amount = 922337203685477.5808m;

         // Act
         Action action = () => new FastMoney(amount, "EUR");

         // Assert
         action.Should().Throw<ArgumentOutOfRangeException>();
     }

     [Fact]
     public void WhenAmountIsAtTheTrueLowerBound_DontThrowException()
     {
         // Arrange
         const decimal amount = -922337203685477.5808m;

         // Act
         Action action = () => new FastMoney(amount, "EUR");

         // Assert
         action.Should().NotThrow();
     }

     [Fact]
     public void WhenAmountIsOneTickBeyondTheTrueLowerBound_ThrowArgumentOutOfRangeException()
     {
         // Arrange
         const decimal amount = -922337203685477.5809m;

         // Act
         Action action = () => new FastMoney(amount, "EUR");

         // Assert
         action.Should().Throw<ArgumentOutOfRangeException>();
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
         action.Should().Throw<InvalidCurrencyException>();
     }
}
