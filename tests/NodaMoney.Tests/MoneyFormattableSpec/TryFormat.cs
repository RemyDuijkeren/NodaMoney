using System.Globalization;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyFormattableSpec;

public class TryFormat
{
    [Fact]
    public void TryFormat_ShouldSuccessfullyFormat_WhenBufferIsSufficient()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<char> destination = stackalloc char[50];
        var format = "C"; // Currency format
        var provider = CultureInfo.InvariantCulture;

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("$1,234.56");
    }

    [Fact]
    public void TryFormat_ShouldFail_WhenBufferIsTooSmall()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<char> destination = stackalloc char[5]; // Smaller than needed
        var format = "C"; // Currency format
        var provider = CultureInfo.InvariantCulture;

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeFalse();
        charsWritten.Should().Be(0);
    }

    [Fact]
    public void TryFormat_ShouldRespectSpecificFormatString()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<char> destination = stackalloc char[50];
        var format = "F2"; // Fixed-point format with 2 decimals
        var provider = CultureInfo.InvariantCulture;

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("1234.56");
    }

    [Fact]
    public void TryFormat_ShouldUseCultureSettings_WhenSpecificCultureProviderIsProvided()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("EUR"));
        Span<char> destination = stackalloc char[50];
        var format = "C"; // Currency format
        var provider = new CultureInfo("fr-FR"); // French culture

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("1 234,56 €");
    }

    [Fact]
    [UseCulture("en-US")]
    public void TryFormat_ShouldFallbackToDefault_WhenProviderIsNull()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<char> destination = stackalloc char[50];
        var format = "C"; // Currency format

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format.AsSpan(), null);

        // Assert
        result.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("$1,234.56");
    }

    [Fact]
    public void TryFormat_ShouldUseDefaultFormatting_WhenFormatIsEmpty()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<char> destination = stackalloc char[50];
        var format = ReadOnlySpan<char>.Empty; // Empty format

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("$1,234.56");
    }

    [Fact]
    public void TryFormat_ShouldSucceed_WhenBufferSizeExactlyMatchesFormattedOutput()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<char> destination = stackalloc char[10]; // Exact size needed for "1234.56"
        var format = "F2"; // Fixed-point format
        var provider = CultureInfo.InvariantCulture;

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("1234.56");
    }

    [Fact]
    public void TryFormat_ShouldHandleLargeFormatStrings()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<char> destination = stackalloc char[100];
        var format = "'$'##,##0.00"; // Custom large format
        var provider = CultureInfo.InvariantCulture;

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("$1,234.56");
    }

    [Fact]
    public void TryFormat_ShouldUseCurrencyFormatting_WhenCultureAndCurrencyMismatched()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("JPY")); // Japanese Yen, no decimal places
        Span<char> destination = stackalloc char[50];
        var format = "C"; // Currency format
        var provider = new CultureInfo("en-US"); // Culture is US

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("¥1,235");
    }
}
