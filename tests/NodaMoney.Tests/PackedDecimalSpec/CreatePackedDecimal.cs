using NodaMoney.Rounding;
using Xunit.Abstractions;

namespace NodaMoney.Tests.PackedDecimalSpec;

public class CreatePackedDecimal
{
    readonly ITestOutputHelper _testOutputHelper;

    public CreatePackedDecimal(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 127)]
    [InlineData(12345.6789, 42)]
    [InlineData(-98765.4321, 15)]
    [InlineData(0.00000123, 5)]
    [InlineData(-0.00000123, 10)]
    public void AddsIndexProperly_WhenIndexIsValid(decimal input, byte index)
    {
        // Arrange
        var value = input;

        // Act
        PackedDecimal packedDecimal = new(value, index: index);

        // Assert
        packedDecimal.Index.Should().Be(index);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 127)]
    [InlineData(12345.6789, 42)]
    [InlineData(-98765.4321, 15)]
    [InlineData(0.00000123, 5)]
    [InlineData(-0.00000123, 10)]
    public void DecimalSameAsInput_WhenIndexIsValid(decimal input, byte index)
    {
        // Arrange
        var value = input;

        // Act
        PackedDecimal packedDecimal = new(value, index: index);

        // Assert
        packedDecimal.Decimal.Should().Be(input);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 127)]
    [InlineData(12345.6789, 42)]
    [InlineData(-98765.4321, 15)]
    [InlineData(0.00000123, 5)]
    [InlineData(-0.00000123, 10)]
    public void AddsCurrencyProperly_WhenCurrencyIsValid(decimal input, byte index)
    {
        // Arrange
        var eur = Currency.FromCode("EUR");
        var value = input;

        // Act
        PackedDecimal packedDecimal = new(value, eur, index);

        // Assert
        packedDecimal.Currency.EncodedValue.Should().Be(eur.EncodedValue);
        packedDecimal.Currency.Should().Be(eur);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 127)]
    [InlineData(12345.6789, 42)]
    [InlineData(-98765.4321, 15)]
    [InlineData(0.00000123, 5)]
    [InlineData(-0.00000123, 10)]
    public void DecimalSameAsInput_WhenCurrencyIsValid(decimal input, byte index)
    {
        // Arrange
        var eur = Currency.FromCode("EUR");
        var value = input;

        // Act
        PackedDecimal packedDecimal = new(value, eur, index);

        // Assert
        packedDecimal.Decimal.Should().Be(input);
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

        // Act
        PackedDecimal packedDecimal = new(value, index: index);

        // Assert
        packedDecimal.Decimal.Should().Be(Decimal.MaxValue);
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

        // Act
        PackedDecimal packedDecimal = new(value, index: index);

        // Assert
        packedDecimal.Decimal.Should().Be(Decimal.MinValue);
    }

    [Fact]
    public void ThrowsArgumentOutOfRangeException_WhenIndexIsTooLarge()
    {
        // Arrange
        var value = 0m;
        byte invalidIndex = 200; // Larger than 127

        // Act
        Action act = () => new PackedDecimal(value, index: invalidIndex);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
           .WithMessage("Index must be between 0 and 127. (Parameter 'index')");
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
