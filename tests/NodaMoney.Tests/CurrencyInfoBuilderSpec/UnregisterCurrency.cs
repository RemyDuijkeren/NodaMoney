using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.CurrencyInfoBuilderSpec;

[Collection(nameof(NoParallelization))]
public class UnregisterCurrency
{
    [Fact]
    public void WhenUnregisterIso_ShouldNotBeRegistered()
    {
        // Arrange
        CurrencyInfo exists = CurrencyInfo.FromCode("PAB");
        CurrencyInfo removed = CurrencyInfoBuilder.Unregister("PAB"); // Panamanian balboa

        // Act
        Action action = () => CurrencyInfo.FromCode("PAB");

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
        exists.Should().NotBeNull();
        exists.Should().Be(removed);
    }

    [Fact]
    public void WhenUnregisterNonIso_ShouldNotBeRegistered()
    {
        // Arrange
        var builder = new CurrencyInfoBuilder("XYZ")
        {
            EnglishName = "Xyz",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 4,
            IsIso4217 = false
        };

        builder.Register();
        Currency exists = CurrencyInfo.FromCode("XYZ"); // should work

        CurrencyInfo removed = CurrencyInfoBuilder.Unregister("XYZ");

        // Act
        Action action = () => CurrencyInfo.FromCode("XYZ");

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenCurrencyDoesNotExist_ThrowInvalidCurrencyException()
    {
        // Arrange

        // Act
        Action action = () => CurrencyInfoBuilder.Unregister("ABC");

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*is unknown currency code!");
    }

    [Fact]
    public void WhenCodeIsNull_ThrowArgumentNullException()
    {
        // Arrange

        // Act
        Action action = () => CurrencyInfoBuilder.Unregister(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCodeIsEmpty_ThrowArgumentNullException()
    {
        // Arrange

        // Act
        Action action = () => CurrencyInfoBuilder.Unregister(string.Empty);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }
}
