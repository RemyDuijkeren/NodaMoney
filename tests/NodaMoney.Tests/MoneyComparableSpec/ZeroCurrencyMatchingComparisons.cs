using NodaMoney.Context;

namespace NodaMoney.Tests.MoneyComparableSpec;

public class ZeroCurrencyMatchingComparisons
{
    [Fact]
    public void RelaxedMode_DifferentCurrency_WithZeroOnRight_AllowsComparison_AndGreaterThan()
    {
        // Arrange
        MoneyContext relaxed = MoneyContext.Create(o => o.EnforceZeroCurrencyMatching = false);
        Money tenEur = new(10m, "EUR", relaxed);
        Money zeroUsd = new(0m, "USD", relaxed);

        // Act + Assert
        tenEur.CompareTo(zeroUsd).Should().Be(1);
        Money.Compare(tenEur, zeroUsd).Should().Be(1);
        (tenEur > zeroUsd).Should().BeTrue();
        (tenEur >= zeroUsd).Should().BeTrue();
        (tenEur < zeroUsd).Should().BeFalse();
        (tenEur <= zeroUsd).Should().BeFalse();
    }

    [Fact]
    public void RelaxedMode_DifferentCurrency_WithZeroOnLeft_AllowsComparison_AndLessThan()
    {
        // Arrange
        MoneyContext relaxed = MoneyContext.Create(o => o.EnforceZeroCurrencyMatching = false);
        Money zeroEur = new(0m, "EUR", relaxed);
        Money tenUsd = new(10m, "USD", relaxed);

        // Act + Assert
        zeroEur.CompareTo(tenUsd).Should().Be(-1);
        Money.Compare(zeroEur, tenUsd).Should().Be(-1);
        (zeroEur < tenUsd).Should().BeTrue();
        (zeroEur <= tenUsd).Should().BeTrue();
        (zeroEur > tenUsd).Should().BeFalse();
        (zeroEur >= tenUsd).Should().BeFalse();
    }

    [Fact]
    public void RelaxedMode_DifferentCurrency_BothZero_AllowsComparison_AndEquals()
    {
        // Arrange
        MoneyContext relaxed = MoneyContext.Create(o => o.EnforceZeroCurrencyMatching = false);
        Money zeroEur = new(0m, "EUR", relaxed);
        Money zeroUsd = new(0m, "USD", relaxed);

        // Act + Assert
        zeroEur.CompareTo(zeroUsd).Should().Be(0);
        Money.Compare(zeroEur, zeroUsd).Should().Be(0);
        (zeroEur <= zeroUsd).Should().BeTrue();
        (zeroEur >= zeroUsd).Should().BeTrue();
        (zeroEur < zeroUsd).Should().BeFalse();
        (zeroEur > zeroUsd).Should().BeFalse();
    }

    [Fact]
    public void RelaxedMode_DifferentCurrency_NonZeroVsNonZero_Throws()
    {
        // Arrange
        MoneyContext relaxed = MoneyContext.Create(o => o.EnforceZeroCurrencyMatching = false);
        Money tenEur = new(10m, "EUR", relaxed);
        Money tenUsd = new(10m, "USD", relaxed);

        // Act
        Action act1 = () => tenEur.CompareTo(tenUsd);
        Action act2 = () => { var _ = tenEur < tenUsd; };

        // Assert
        act1.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
        act2.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }

    [Fact]
    public void EnforcedMode_DifferentCurrency_WithZeroOnEitherSide_Throws()
    {
        // Arrange
        MoneyContext enforced = MoneyContext.Create(o => o.EnforceZeroCurrencyMatching = true);
        Money tenEur = new(10m, "EUR", enforced);
        Money zeroUsd = new(0m, "USD", enforced);
        Money zeroEur = new(0m, "EUR", enforced);
        Money tenUsd = new(10m, "USD", enforced);

        // Act
        Action act1 = () => tenEur.CompareTo(zeroUsd);
        Action act2 = () => zeroEur.CompareTo(tenUsd);
        Action act3 = () => { var _ = tenEur > zeroUsd; };
        Action act4 = () => { var _ = zeroEur < tenUsd; };

        // Assert
        act1.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
        act2.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
        act3.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
        act4.Should().Throw<InvalidCurrencyException>().WithMessage("Currency mismatch*");
    }
}
