using System.Globalization;
using System.Text;
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
        var format = "c"; // Currency format (local)
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
        var format = "c"; // Currency format (local)
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
        var format = "c"; // Currency format (local)
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
        var format = "c"; // Currency format (local)

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
        var format = "c"; // Currency format (local)
        var provider = new CultureInfo("en-US"); // Culture is US

        // Act
        bool result = money.TryFormat(destination, out int charsWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("¥1,235");
    }

    [Fact]
    public void TryFormatByte_ShouldSuccessfullyFormatUtf8_WhenBufferIsSufficient()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<byte> destination = stackalloc byte[50];
        var format = "c"; // Currency format (local)
        var provider = CultureInfo.InvariantCulture;

        // Act
        bool result = money.TryFormat(destination, out int bytesWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        bytesWritten.Should().BeGreaterThan(0);

        // Convert output back to string and validate
        string formattedResult = Encoding.UTF8.GetString(destination.Slice(0, bytesWritten).ToArray());
        formattedResult.Should().Be("$1,234.56");
    }

    [Fact]
    public void TryFormatByte_ShouldFail_WhenBufferIsTooSmall()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<byte> destination = stackalloc byte[5]; // Smaller than needed
        var format = "c";
        var provider = CultureInfo.InvariantCulture;

        // Act
        bool result = money.TryFormat(destination, out int bytesWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeFalse();
        bytesWritten.Should().Be(0);
    }

    [Fact]
    public void TryFormatByte_ShouldRespectFormatString_ForUtf8Output()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<byte> destination = stackalloc byte[50];
        var format = "F2"; // Fixed-point format with 2 decimals
        var provider = CultureInfo.InvariantCulture;

        // Act
        bool result = money.TryFormat(destination, out int bytesWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        bytesWritten.Should().BeGreaterThan(0);

        // Convert output back to string and validate
        string formattedResult = Encoding.UTF8.GetString(destination.Slice(0, bytesWritten).ToArray());
        formattedResult.Should().Be("1234.56");
    }

    [Fact]
    public void TryFormatByte_ShouldRespectCulture_ForUtf8Output()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("EUR"));
        Span<byte> destination = stackalloc byte[50];
        var format = "c"; // Currency format (local)
        var provider = new CultureInfo("fr-FR"); // French culture

        // Act
        bool result = money.TryFormat(destination, out int bytesWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        bytesWritten.Should().BeGreaterThan(0);

        // Convert output back to string and validate
        string formattedResult = Encoding.UTF8.GetString(destination.Slice(0, bytesWritten).ToArray());
        formattedResult.Should().Be("1 234,56 €");
    }

    [Fact]
    [UseCulture("en-US")]
    public void TryFormatByte_ShouldFallbackToDefaultCulture_WhenProviderIsNull()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<byte> destination = stackalloc byte[50];
        var format = "c"; // Currency format (local)

        // Act
        bool result = money.TryFormat(destination, out int bytesWritten, format.AsSpan(), null);

        // Assert
        result.Should().BeTrue();
        bytesWritten.Should().BeGreaterThan(0);

        // Convert output back to string and validate
        string formattedResult = Encoding.UTF8.GetString(destination.Slice(0, bytesWritten).ToArray());
        formattedResult.Should().Be("$1,234.56");
    }

    [Fact]
    public void TryFormatByte_ShouldHandleEmptyFormatString()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        Span<byte> destination = stackalloc byte[100]; // Buffer size is larger than necessary
        var format = ReadOnlySpan<char>.Empty; // No format specified

        // Act
        bool result = money.TryFormat(destination, out int bytesWritten, format, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeTrue();
        bytesWritten.Should().BeGreaterThan(0);

        // Validate the UTF-8 output
        string formattedResult = Encoding.UTF8.GetString(destination.Slice(0, bytesWritten).ToArray());
        formattedResult.Should().Be("$1,234.56");
    }

    [Fact]
    public void TryFormatByte_ShouldSucceed_WhenBufferSizeExactlyMatchesUtf8Output()
    {
        // Arrange
        var money = new Money(1234.56m, Currency.FromCode("USD"));
        string expected = "$1,234.56";
        var utf8ExpectedLength = Encoding.UTF8.GetByteCount(expected);
        Span<byte> destination = stackalloc byte[utf8ExpectedLength]; // Exact size of the needed UTF-8 bytes
        var format = "c";
        var provider = CultureInfo.InvariantCulture;

        // Act
        bool result = money.TryFormat(destination, out int bytesWritten, format.AsSpan(), provider);

        // Assert
        result.Should().BeTrue();
        bytesWritten.Should().Be(utf8ExpectedLength);

        // Validate the UTF-8 output
        string formattedResult = Encoding.UTF8.GetString(destination.Slice(0, bytesWritten).ToArray());
        formattedResult.Should().Be(expected);
    }
}
