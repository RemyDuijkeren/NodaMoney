#if NET7_0_OR_GREATER
using System.Text;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyParsableSpec;

public class ParseUtf8
{
    static T Parse<T>(ReadOnlySpan<byte> utf8Text) where T : IUtf8SpanParsable<T> => T.Parse(utf8Text, null);

    static bool TryParse<T>(ReadOnlySpan<byte> utf8Text, out T result) where T : IUtf8SpanParsable<T> =>
        T.TryParse(utf8Text, null, out result!);

    [Fact]
    [UseCulture("en-US")]
    public void WhenParsingUtf8Text_ShouldReturnMoney()
    {
        Parse<Money>("$765.43"u8).Should().Be(new Money(765.43m, "USD"));
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenTryParsingUtf8Text_ShouldReturnMoney()
    {
        TryParse("$765.43"u8, out Money money).Should().BeTrue();

        money.Should().Be(new Money(765.43m, "USD"));
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenTryParsingInvalidUtf8Text_ShouldReturnFalse()
    {
        TryParse("not money"u8, out Money _).Should().BeFalse();
    }

    [Fact]
    [UseCulture("en-US")]
    public void WhenParsingUtf8TextLargerThanTheStack_ShouldNotOverflowTheStack()
    {
        // 8 MB of leading whitespace: as UTF-16 on the stack that would be 16 MB, more than any thread's stack.
        byte[] utf8Text = Encoding.UTF8.GetBytes(new string(' ', 8_000_000) + "$765.43");

        Parse<Money>(utf8Text).Should().Be(new Money(765.43m, "USD"));
        TryParse(utf8Text, out Money money).Should().BeTrue();
        money.Should().Be(new Money(765.43m, "USD"));
    }
}
#endif
