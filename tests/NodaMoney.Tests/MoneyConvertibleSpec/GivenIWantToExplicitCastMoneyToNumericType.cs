using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.MoneyConvertibleSpec;

public class GivenIWantToExplicitCastMoneyToNumericType
{
    readonly Money _euros = new Money(10.00m, "EUR");

    [Fact]
    public void WhenExplicitCastingToDecimal_ThenCastingShouldSucceed()
    {
        var m = (decimal)_euros;

        m.Should().Be(10.00m);
    }

    [Fact]
    public void WhenExplicitCastingToDouble_ThenCastingShouldSucceed()
    {
        var d = (double)_euros;

        d.Should().Be(10.00d);
    }

    [Fact]
    public void WhenExplicitCastingToFloat_ThenCastingShouldSucceed()
    {
        var f = (float)_euros;

        f.Should().Be(10.00f);
    }

    [Fact]
    public void WhenExplicitCastingToLong_ThenCastingShouldSucceed()
    {
        var l = (long)_euros;

        l.Should().Be(10L);
    }

    [Fact]
    public void WhenExplicitCastingToByte_ThenCastingShouldSucceed()
    {
        var b = (byte)_euros;

        b.Should().Be(10);
    }

    [Fact]
    public void WhenExplicitCastingToShort_ThenCastingShouldSucceed()
    {
        var s = (short)_euros;

        s.Should().Be(10);
    }

    [Fact]
    public void WhenExplicitCastingToInt_ThenCastingShouldSucceed()
    {
        var i = (int)_euros;

        i.Should().Be(10);
    }
}
