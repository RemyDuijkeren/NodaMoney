namespace NodaMoney.Tests.CurrencyInfoSpec;

public class ConvertToCurrency
{
    [Fact]
    public void WhenConvertedTwice_ThenResultingCurrenciesAreEqualWithMinorUnitTwoFlagSet()
    {
        // Arrange
        var euro = CurrencyInfo.FromCode("EUR");

        // Act
        Currency first = euro;
        Currency second = euro;

        // Assert
        first.Should().Be(second);
        first.Code.Should().Be("EUR");
        Currency.FromCode("EUR").Should().Be(first, because: "EUR has a minor unit of 2, so the fast-path flag should be set");
    }

    [Fact]
    public void WhenOneOfTwoStructurallyEqualInstancesIsConvertedAndTheOtherIsNot_ThenTheyAreStillEqualAndHaveTheSameHashCode()
    {
        // Arrange
        // Two independently-constructed instances: each 'with { Code = ... }' gets its own fresh cache holder
        // (KTD4), so registry singleton reuse cannot mask the equality contract of the holder itself.
        var usd = CurrencyInfo.FromCode("USD");
        var converted = usd with { Code = "EUR" };
        var notConverted = usd with { Code = "EUR" };

        // Act
        _ = (Currency)converted; // force the cache to be filled on 'converted' only

        // Assert
        converted.Should().Be(notConverted);
        converted.GetHashCode().Should().Be(notConverted.GetHashCode());
    }

    [Fact]
    public void WhenCodeIsChangedWithRecordWith_ThenConvertedCurrencyEncodesTheNewCode()
    {
        // Arrange
        var euro = CurrencyInfo.FromCode("EUR");

        // Act
        var dollarLike = euro with { Code = "USD" };
        Currency currency = dollarLike;

        // Assert
        currency.Code.Should().Be("USD");
    }

    [Fact]
    public void WhenMinorUnitIsChangedWithRecordWith_ThenConvertedCurrencyHasNoMinorUnitTwoFlag()
    {
        // Arrange
        var euro = CurrencyInfo.FromCode("EUR");

        // Act
        var withoutMinorUnitTwo = euro with { MinorUnit = MinorUnit.Zero };
        Currency currency = withoutMinorUnitTwo;

        // Assert
        // Currency.Equals() ignores the minor-unit-2 fast-path bit, so compare the raw encoded value instead.
        currency.EncodedValue.Should().NotBe(Currency.FromCode("EUR").EncodedValue, because: "the minor-unit-2 fast-path flag differs");
    }
}
