using NodaMoney.Context;

namespace NodaMoney.Tests.MoneySpec;

public class EqualsAndHashCode
{
    [Fact]
    public void Equals_ShouldBeTrue_ForSameAmountAndCurrencyAcrossDifferentContexts()
    {
        // Arrange: two different contexts leading to different scale/rounding behavior
        var contextA = MoneyContext.Create(o =>
        {
            o.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven);
            o.MaxScale = 0;
        });
        var contextB = MoneyContext.Create(o =>
        {
            o.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero);
            o.MaxScale = 4;
        });

        // Act
        var m1 = new Money(10.0m, "EUR", contextA); // 10
        var m2 = new Money(10.0000m, "EUR", contextB); // 10.0000 -> 10.0000

        // Assert
        m1.Equals(m2).Should().BeTrue();
        (m1 == m2).Should().BeTrue();
        (m1 != m2).Should().BeFalse();
        m1.GetHashCode().Should().Be(m2.GetHashCode());
    }

    [Fact]
    public void Equals_Object_Null_ShouldBeFalse()
    {
        // Arrange and Act
        var m = new Money(5m, "USD");
        object? o = null;

        // Assert
        m.Equals(o).Should().BeFalse();
    }

    [Fact]
    public void Equals_Object_SameValue_ShouldBeTrue()
    {
        // Arrange and Act
        object m1 = new Money(12.34m, "USD");
        object m2 = new Money(12.34m, "USD");

        // Assert
        m1.Equals(m2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Object_DifferentType_ShouldBeFalse()
    {
        // Arrange and Act
        var m = new Money(7m, "EUR");

        // Assert
        m.Equals(new object()).Should().BeFalse();
    }

    [Fact]
    public void Equals_SameNumericValueDifferentScale_ShouldBeTrue()
    {
        // Arrange and Act
        var a = new Money(100.0m, "EUR");
        var b = new Money(100.00m, "EUR");

        // Assert
        a.Scale.Should().NotBe(b.Scale);
        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_EqualInstances_HaveSameHashCode()
    {
        // Arrange and Act
        var a = new Money(1.23m, "GBP");
        var b = new Money(1.23m, "GBP");

        // Assert
        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentCurrency_ShouldPreferDifferentHashCode()
    {
        // Arrange and Act
        var a = new Money(50m, "USD");
        var b = new Money(50m, "EUR");

        // Assert
        // Note: unequal objects are not required to have different hash codes,
        // but our implementation uses Amount and Currency, so this should differ.
        a.Equals(b).Should().BeFalse();
        a.GetHashCode().Should().NotBe(b.GetHashCode());
    }
}
