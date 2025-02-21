namespace NodaMoney.Tests.CurrencyInfoSpec;

public class CreateAsFormatProvider
{
    [Fact]
    public void WhenCreateCurrencyInfo_ShouldBeFormatProvider()
    {
        // Act
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("EUR");

        // Assert
        currencyInfo.Should().BeAssignableTo<IFormatProvider>();
    }
}
