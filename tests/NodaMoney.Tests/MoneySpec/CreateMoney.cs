using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NodaMoney.Context;
using Xunit.Abstractions;

namespace NodaMoney.Tests.MoneySpec;

public class CreateMoney
{
    readonly ITestOutputHelper _testOutputHelper;
    readonly Currency _euro = Currency.FromCode("EUR");

    public CreateMoney(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void SizeInMemory()
    {
        // Arrange

        // Act
        int sizeOfCurrency = Marshal.SizeOf<Currency>();
        int sizeOfMoney = Marshal.SizeOf<Money>();
        int sizeOfFastMoney = Marshal.SizeOf<FastMoney>();
        int sizeOfExactMoney = Marshal.SizeOf<ExactMoney>();

        int sizeOfCurrencyUnsafe = Unsafe.SizeOf<Currency>();
        int sizeOfMoneyUnsafe = Unsafe.SizeOf<Money>();
        int sizeOfFastMoneyUnsafe = Unsafe.SizeOf<FastMoney>();
        int sizeOfExactMoneyUnsafe = Unsafe.SizeOf<ExactMoney>();

        // Assert
        _testOutputHelper.WriteLine($"Size of Currency: {sizeOfCurrency} ({sizeOfCurrencyUnsafe})");
        _testOutputHelper.WriteLine($"Size of Money: {sizeOfMoney} ({sizeOfMoneyUnsafe})");
        _testOutputHelper.WriteLine($"Size of FastMoney: {sizeOfFastMoney} ({sizeOfFastMoneyUnsafe})");
        _testOutputHelper.WriteLine($"Size of ExactMoney: {sizeOfExactMoney} ({sizeOfExactMoneyUnsafe})");

        sizeOfCurrency.Should().Be(2);
        sizeOfMoney.Should().Be(16); // was 24, but now it's 16!
        sizeOfFastMoney.Should().Be(16);
        sizeOfExactMoney.Should().Be(24);
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
        var newMoney = money with { Currency = CurrencyInfo.FromCode("USD") };

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
        var newMoney = money with { Amount = 12.34m };

        // Assert
        newMoney.Should().NotBeSameAs(money);
        newMoney.Currency.Should().Be(money.Currency);
        newMoney.Amount.Should().Be(12.34m);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 127)]
    [InlineData(12345.6789, 42)]
    [InlineData(-98765.4321, 15)]
    [InlineData(0.00000123, 5)]
    [InlineData(-0.00000123, 10)]
    public void WhenMoneyContextIndex_AddsIndexProperly(decimal input, byte index)
    {
        // Arrange
        var value = input;
        Money money = new(value, _euro, MoneyContext.CreateNoRounding());

        // Act
        var result = money with { MoneyContextIndex = index };

        // Assert
        result.MoneyContextIndex.Should().Be(index);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 127)]
    [InlineData(12345.6789, 42)]
    [InlineData(-98765.4321, 15)]
    [InlineData(0.00000123, 5)]
    [InlineData(-0.00000123, 10)]
    public void WhenDecimalWithMoneyContextIndex_AddsDecimalProperly(decimal input, byte index)
    {
        // Arrange
        var value = input;
        Money money = new(value, _euro, MoneyContext.CreateNoRounding());

        // Act
        var result = money with { MoneyContextIndex = index };

        // Assert
        result.Amount.Should().Be(input);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 127)]
    [InlineData(12345.6789, 42)]
    [InlineData(-98765.4321, 15)]
    [InlineData(0.00000123, 5)]
    [InlineData(-0.00000123, 10)]
    public void WhenCurrencyWithMoneyContextIndex_AddsCurrencyProperly(decimal input, byte index)
    {
        // Arrange
        var value = input;
        Money money = new(value, _euro, MoneyContext.CreateNoRounding());

        // Act
        var result = money with { MoneyContextIndex = index };

        // Assert
        result.Currency.Should().Be(_euro);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 127)]
    [InlineData(12345.6789, 42)]
    [InlineData(-98765.4321, 15)]
    [InlineData(0.00000123, 5)]
    [InlineData(-0.00000123, 10)]
    public void WhenCurrencyWithMoneyContextIndex_AddsCDecimalProperly(decimal input, byte index)
    {
        // Arrange
        var value = input;
        Money money = new(value, _euro, MoneyContext.CreateNoRounding());

        // Act
        var result = money with { MoneyContextIndex = index };

        // Assert
        result.Amount.Should().Be(input);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(127)]
    [InlineData(42)]
    [InlineData(15)]
    [InlineData(5)]
    [InlineData(10)]
    public void DecimalSameAsInput_WhenMaxValue(byte index)
    {
        // Arrange
        var value = Decimal.MaxValue;
        Money money = new(value, _euro, MoneyContext.CreateNoRounding());

        // Act
        var result = money with { MoneyContextIndex = index };

        // Assert
        result.Amount.Should().Be(Decimal.MaxValue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(127)]
    [InlineData(42)]
    [InlineData(15)]
    [InlineData(5)]
    [InlineData(10)]
    public void DecimalSameAsInput_WhenMinValue(byte index)
    {
        // Arrange
        var value = Decimal.MinValue;
        Money money = new(value, _euro, MoneyContext.CreateNoRounding());

        // Act
        var result = money with { MoneyContextIndex = index };

        // Assert
        result.Amount.Should().Be(Decimal.MinValue);
    }

    [Fact]
    public void ThrowsArgumentOutOfRangeException_WhenIndexIsTooLarge()
    {
        // Arrange
        var value = 0m;
        byte invalidIndex = 200; // Larger than 127
        Money money = new(value, _euro, MoneyContext.CreateNoRounding());

        // Act
        Action act = () => { var _ = money with { MoneyContextIndex = invalidIndex }; };

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("Index must be within 0 to 127*");
    }

    [Fact]
    public void ReturnsZero_WhenNoIndexAddedToDecimal()
    {
        // Arrange
        decimal value = 100m;

        // Act
        var result = new PackedDecimal(value, index: 0);

        // Assert
        result.Index.Should().Be(0);
    }

    [Fact]
    public void TweakDecimalScale()
    {
        // Arrange
        var amount = 1234.5678554439m;

        // Act
        var roundedAmount = Math.Round(amount, 2); // Rounded to 2 decimal places

        // Assert
#if NET7_0_OR_GREATER
        _testOutputHelper.WriteLine(amount.Scale.ToString());
        _testOutputHelper.WriteLine(roundedAmount.Scale.ToString());
#endif
        _testOutputHelper.WriteLine(roundedAmount.ToString());
    }
}
