using NodaMoney.Context;

namespace NodaMoney.Tests.MoneySpec;

public class ScaleAndPrecision
{
    [Fact]
    public void ZeroAmount_ShouldHaveScale0_AndPrecision1()
    {
        // Arrange / Act
        var mUsd = new Money(0m, "USD");
        var mEur = new Money(0m, "EUR");

        // Assert
        mUsd.Amount.Should().Be(0m);
        mUsd.Scale.Should().Be(0);
        mUsd.Precision.Should().Be(1);

        mEur.Amount.Should().Be(0m);
        mEur.Scale.Should().Be(0);
        mEur.Precision.Should().Be(1);
    }

    [Fact]
    public void NegativeAmount_WithMaxScale_RoundsAndSetsScale()
    {
        // Arrange
        var amount = -123.45678m;
        var context = MoneyContext.Create(o =>
        {
            o.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero);
            o.MaxScale = 3;
        });

        // Act
        var money = new Money(amount, "USD", context);

        // Assert
        money.Amount.Should().Be(-123.457m, because: "Rounded to 3 decimals away from zero");
        money.Scale.Should().Be(3);
    }

    [Fact]
    public void Precision_ShouldReflectDigits_AfterRounding_WithPotentiallyTrimmedTrailingZeros()
    {
        // Arrange
        var amount = 1.23m; // fewer decimals than MaxScale
        var context = MoneyContext.Create(o =>
        {
            o.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven);
            o.MaxScale = 4;
        });

        // Act
        var money = new Money(amount, "EUR", context);

        // Assert
        // decimal.Round may not pad trailing zeros; the value remains 1.23 with scale typically 2.
        money.Amount.Should().Be(1.23m);
        money.Scale.Should().BeInRange(2, 4, because: "decimal scale may retain original scale while respecting MaxScale upper bound");
        money.Precision.Should().Be(3, because: "1.23 has 3 significant digits (123)");
    }

    [Fact]
    public void Scale_ShouldReflectInput()
    {
        // Arrange
        var amount0 = 10m;
        var amount1 = 10.0m;
        var amount2 = 10.00m;
        var amount3 = 10.000m;

        // Act
        var money0 = new Money(amount0, "BHD");
        var money1 = new Money(amount1, "BHD");
        var money2 = new Money(amount2, "BHD");
        var money3 = new Money(amount3, "BHD");

        // Assert
        money0.Scale.Should().Be(0, because: "decimal scale should retain original scale");
        money0.Precision.Should().Be(2, because: "10 has 2 significant digits (10)");

        money1.Scale.Should().Be(1, because: "decimal scale should retain original scale");
        money1.Precision.Should().Be(3, because: "10.0 has 3 significant digits (100)");

        money2.Scale.Should().Be(2, because: "decimal scale should retain original scale");
        money2.Precision.Should().Be(4, because: "10.000 has 4 significant digits (10000)");

        money3.Scale.Should().Be(3, because: "decimal scale should retain original scale");
        money3.Precision.Should().Be(5, because: "10.000 has 5 significant digits (10000)");
    }

    [Fact]
    public void Scale_ShouldReflectInput_TrimWhenBiggerThenCurrencyDigits()
    {
        // Arrange
        var amount = 10.0000m;

        // Act
        var money = new Money(amount, "BHD"); // dinar has 3 digits, so should trim trailing zeros

        // Assert
        money.Scale.Should().Be(3, because: "decimal scale should retain original scale while respecting currency decimal digits");
        money.Precision.Should().Be(5, because: "10.000 has 5 significant digits (10000)");
    }

    [Fact]
    public void MaxScale28_With28FractionalDigits_ShouldKeepScaleAndPrecision28()
    {
        // Arrange: 28 fractional digits
        var amount = 0.1234567890123456789012345678m; // 28 decimal places
        var context = MoneyContext.Create(o =>
        {
            o.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven);
            o.MaxScale = 28;
        });

        // Act
        var money = new Money(amount, "USD", context);

        // Assert
        money.Amount.Should().Be(amount);
        money.Scale.Should().Be(28);
        money.Precision.Should().Be(28);
    }

    [Fact]
    public void CurrencyWithZeroDecimals_DefaultContext_ShouldRoundToIntegerAndScale0()
    {
        // Arrange
        var amount = 1234.56m;

        // Act
        var jpy = new Money(amount, "JPY"); // JPY has 0 decimal digits by default

        // Assert
        jpy.Scale.Should().Be(0);
        jpy.Amount.Should().Be(1235m, because: "JPY uses 0 decimals and rounds to nearest integer");
        jpy.Precision.Should().Be(4, because: "1235 has 4 significant digits");
    }
}
