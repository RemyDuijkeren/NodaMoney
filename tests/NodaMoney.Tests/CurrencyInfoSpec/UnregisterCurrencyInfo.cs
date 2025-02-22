namespace NodaMoney.Tests.CurrencyInfoSpec;

public class UnregisterCurrencyInfo
{
    [Fact]
    public void WhenUnregisterIso_ShouldNotBeRegistered()
    {
        // Arrange
        CurrencyInfo exists = CurrencyInfo.FromCode("PAB");
        CurrencyInfo removed = CurrencyInfo.Unregister("PAB"); // Panamanian balboa

        // Act
        Action action = () => CurrencyInfo.FromCode("PAB");

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
        exists.Should().NotBeNull();
        exists.Should().Be(removed);

        CurrencyInfo.Register(removed); // register it back for other tests
    }

    [Fact]
    public void WhenUnregisterNonIso_ShouldNotBeRegistered()
    {
        // Arrange
        var ci = CurrencyInfo.Create("XYZ") with
        {
            EnglishName = "Xyz",
            Symbol = "฿",
            MinorUnit = MinorUnit.Two,
            IsIso4217 = false
        };

        CurrencyInfo.Register(ci);
        CurrencyInfo exists = CurrencyInfo.FromCode("XYZ"); // should work
        CurrencyInfo removed = CurrencyInfo.Unregister("XYZ");

        // Act
        Action action = () => CurrencyInfo.FromCode("XYZ");

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenCurrencyDoesNotExist_ThrowInvalidCurrencyException()
    {
        // Arrange

        // Act
        Action action = () => CurrencyInfo.Unregister("ABC");

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*is unknown currency code!");
    }

    [Fact]
    public void WhenCodeIsNull_ThrowArgumentNullException()
    {
        // Arrange

        // Act
        Action action = () => CurrencyInfo.Unregister(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCodeIsEmpty_ThrowArgumentNullException()
    {
        // Arrange

        // Act
        Action action = () => CurrencyInfo.Unregister(string.Empty);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenReplacingCurrencyWithCustom_CurrencyIsReplaced()
    {
        // Arrange
        CurrencyInfo removed = CurrencyInfo.Unregister("PAB"); // Panamanian balboa

        CurrencyInfo created = removed with
        {
            Symbol = "B/.",
            EnglishName = "New Panamanian balboa",
            MinorUnit = MinorUnit.One
        };

        CurrencyInfo.Register(created);

        // Act
        CurrencyInfo replaced = CurrencyInfo.FromCode("PAB");

        // Assert
        replaced.Symbol.Should().Be("B/.");
        replaced.EnglishName.Should().Be("New Panamanian balboa");
        replaced.MinorUnit.Should().Be(MinorUnit.One);

        replaced.Should().NotBe(removed);
        replaced.Should().NotBeEquivalentTo(removed);
        replaced.Should().Be(created);
        replaced.Should().BeEquivalentTo(created);

        CurrencyInfo.Unregister("PAB"); // cleanup
        CurrencyInfo.Register(removed); // register it back for other tests
    }
}
