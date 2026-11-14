namespace NodaMoney.Tests.MoneyConvertibleSpec;

public class ConvertMoneyToNumericType
{
    readonly Money _euros = new Money(765.43m, "EUR");

    [Fact]
    public void WhenConvertingToDecimal_ThenThisShouldSucceed()
    {
        var result = _euros.ToDecimal();

        result.Should().Be(765.43m);
    }

    [Fact]
    public void WhenConvertingToDouble_ThenThisShouldSucceed()
    {
        var result = _euros.ToDouble();

        result.Should().BeApproximately(765.43d, 0.001d);
    }

    [Fact]
    public void WhenConvertingSixteenSignificantDigitsToDouble_ThenTheResultShouldFollowTheRuntime()
    {
        // From .NET 11 the runtime rounds decimal to double correctly. Earlier runtimes lose the 16th digit here.
        var money = new Money(9895985413.759941m, "XXX");

        var result = money.ToDouble();

        result.Should().Be(Environment.Version.Major >= 11 ? 9895985413.759941d : 9895985413.75994d);
    }

    [Fact]
    public void WhenConvertingFifteenSignificantDigitsToDouble_ThenTheResultShouldBeTheNearestDoubleOnEveryRuntime()
    {
        var money = new Money(98959854137.5994m, "XXX");

        var result = money.ToDouble();

        result.Should().Be(98959854137.5994d);
    }

    [Fact]
    public void WhenConvertingToLong_ThenThisShouldSucceed()
    {
        var result = _euros.ToInt64();

        result.Should().Be(765);
    }

    [Fact]
    public void WhenConvertingToInt_ThenThisShouldSucceed()
    {
        var result = _euros.ToInt32();

        result.Should().Be(765);
    }
}
