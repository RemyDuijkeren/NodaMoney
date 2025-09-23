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
