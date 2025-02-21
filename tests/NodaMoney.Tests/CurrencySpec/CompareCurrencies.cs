namespace NodaMoney.Tests.CurrencySpec;

public class CompareCurrencies
{
    private Currency _euro1 = CurrencyInfo.FromCode("EUR");

    private Currency _euro2 = CurrencyInfo.FromCode("EUR");

    private Currency _dollar = CurrencyInfo.FromCode("USD");

    [Fact]
    public void WhenComparingEquality_ThenCurrencyShouldBeEqual()
    {
        // Compare using Equal()
        _euro1.Should().Be(_euro2);
        _euro1.Should().NotBe(_dollar);
        _euro1.Should().NotBeNull();
        _euro1.Should().NotBe(new object(), "comparing Currency to a different object should fail!");
    }

    [Fact]
    public void WhenComparingStaticEquality_ThenCurrencyShouldBeEqual()
    {
        // Compare using static Equal()
        Currency.Equals(_euro1, _euro2).Should().BeTrue();
        Currency.Equals(_euro1, _dollar).Should().BeFalse();
    }

    [Fact]
    public void WhenComparingWithEqualityOperator_ThenCurrencyShouldBeEqual()
    {
        // Compare using Euality operators
        (_euro1 == _euro2).Should().BeTrue();
        (_euro1 != _dollar).Should().BeTrue();
    }

    [Fact]
    public void WhenComparingHashCodes_ThenCurrencyShouldBeEqual()
    {
        // Compare using GetHashCode()
        _euro1.GetHashCode().Should().Be(_euro2.GetHashCode());
        _euro1.GetHashCode().Should().NotBe(_dollar.GetHashCode());
    }
}
