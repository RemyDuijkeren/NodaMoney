namespace NodaMoney.Tests.CurrencyInfoSpec;

public class RegisterCurrencyInfo
{
    [Fact]
    public void WhenCreateWithDefaults_ReturnDefaultWithNonIso4217AndGenericCurrencySign()
    {
        // Arrange

        // Act
        CurrencyInfo result = CurrencyInfo.Create("BTA");

        // Assert
        result.Code.Should().Be("BTA");
        result.Number.Should().Be(0);
        result.MinorUnit.Should().Be(MinorUnit.NotApplicable);
        result.EnglishName.Should().BeEmpty();
        result.Symbol.Should().Be(CurrencyInfo.GenericCurrencySign);
        result.InternationalSymbol.Should().Be(CurrencyInfo.GenericCurrencySign);
        result.AlternativeSymbols.Should().BeEmpty();
        result.IsIso4217.Should().BeFalse();
        result.ExpiredOn.Should().BeNull();
        result.IntroducedOn.Should().BeNull();
    }

    [Fact]
    public void WhenCreateCodeIsNull_ThrowArgumentNullException()
    {
        // Arrange

        // Act
        Action action = () => CurrencyInfo.Create(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("AB")]
    [InlineData("ABCD")]
    [InlineData("Abc")]
    public void WhenCreateCodeIsInvalid_ThrowArgumentException(string code)
    {
        // Arrange

        // Act
        Action action = () => CurrencyInfo.Create(code);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WhenCreateWith_ReturnCurrencyInfo()
    {
        // Arrange

        // Act
        CurrencyInfo result = CurrencyInfo.Create("BTA") with
        {
            Symbol = "$",
            Number = 666,
            InternationalSymbol = "CC$",
            MinorUnit = MinorUnit.Two,
            EnglishName = "My Custom Currency",
            IsIso4217 = true,
            AlternativeSymbols = ["cc$"],
            IntroducedOn = new DateTime(2022, 1, 1),
            ExpiredOn = new DateTime(2030, 1, 1),
        };

        // Assert
        result.Code.Should().Be("BTA");
        result.Number.Should().Be(666);
        result.MinorUnit.Should().Be(MinorUnit.Two);
        result.EnglishName.Should().Be("My Custom Currency");
        result.Symbol.Should().Be("$");
        result.InternationalSymbol.Should().Be("CC$");
        result.AlternativeSymbols.Should().BeEquivalentTo(["cc$"]);
        result.IsIso4217.Should().BeTrue();
        result.ExpiredOn.Should().Be(new DateTime(2030, 1, 1));
        result.IntroducedOn.Should().Be(new DateTime(2022, 1, 1));
    }

    [Fact]
    public void WhenCreate_ShouldNotBeRegistered()
    {
        // Arrange
        CurrencyInfo bta = CurrencyInfo.Create("BTA");

        // Act
        Action action = () => CurrencyInfo.FromCode("BTA");

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenRegister_ShouldBeRegistered()
    {
        // Arrange
        CurrencyInfo ci = CurrencyInfo.Create("BTZ");

        // Act
        CurrencyInfo.Register(ci);

        // Assert
        CurrencyInfo result = CurrencyInfo.FromCode("BTZ");
        result.Should().BeEquivalentTo(ci);
        result.Should().Be(ci);
        result.IsIso4217.Should().BeFalse();

        CurrencyInfo.Unregister("BTZ"); // cleanup
    }

    [Fact]
    public void WhenRegisterExistingCode_ThrowInvalidCurrencyException()
    {
        // Arrange
        CurrencyInfo ci = CurrencyInfo.Create("EUR");

        // Act
        Action action = () => CurrencyInfo.Register(ci);

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*is already registered*");
    }

    [Fact]
    public void WhenRegisterExistingCurrencyInfo_ThrowInvalidCurrencyException()
    {
        // Arrange
        var euro = CurrencyInfo.FromCode("EUR");

        // Act
        Action action = () => CurrencyInfo.Register(euro);

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*is already registered*");
    }

    [Fact]
    public void WhenBuildFromExistingCurrencyInfo_ThenThisShouldSucceed()
    {
        // Arrange
        var euro = CurrencyInfo.FromCode("EUR");

        // Act
        CurrencyInfo result = euro with
        {
            Code = "BTE",
            Symbol = "$",
            Number = 666,
            InternationalSymbol = "CC$",
            MinorUnit = MinorUnit.Two,
            EnglishName = "My Custom Currency",
            IsIso4217 = true,
            AlternativeSymbols = ["cc$"],
            IntroducedOn = new DateTime(2022, 1, 1),
            ExpiredOn = new DateTime(2030, 1, 1),
        };

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBe(euro);
        result.Should().NotBeEquivalentTo(euro);

        result.Code.Should().Be("BTE");
        result.Number.Should().Be(666);
        result.MinorUnit.Should().Be(MinorUnit.Two);
        result.EnglishName.Should().Be("My Custom Currency");
        result.Symbol.Should().Be("$");
        result.InternationalSymbol.Should().Be("CC$");
        result.AlternativeSymbols.Should().BeEquivalentTo(["cc$"]);
        result.IsIso4217.Should().BeTrue();
        result.ExpiredOn.Should().Be(new DateTime(2030, 1, 1));
        result.IntroducedOn.Should().Be(new DateTime(2022, 1, 1));
    }

    [Fact]
    public void WhenRegisterAfterBuildFromExistingCurrencyInfo_ShouldBeRegistered()
    {
        // Arrange
        var euro = CurrencyInfo.FromCode("EUR");
        CurrencyInfo ci = euro with
        {
            Code = "BTE",
            Symbol = "$",
            Number = 666,
            InternationalSymbol = "CC$",
            MinorUnit = MinorUnit.Two,
            EnglishName = "My Custom Currency",
            IsIso4217 = true,
            AlternativeSymbols = ["cc$"],
            IntroducedOn = new DateTime(2022, 1, 1),
            ExpiredOn = new DateTime(2030, 1, 1),
        };

        // Act
        CurrencyInfo.Register(ci);

        // Assert
        CurrencyInfo result = CurrencyInfo.FromCode("BTE");
        result.Should().BeEquivalentTo(ci);
        result.Should().Be(ci);

        CurrencyInfo.Unregister("BTE"); // cleanup
    }

    [Fact]
    public void WhenRegisterWithCodeIsNull_ThrowArgumentNullException()
    {
        // Arrange
        var created = CurrencyInfo.FromCode("EUR") with { Code = null! };

        // Act
        Action action = () => CurrencyInfo.Register(created);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("AB")]
    [InlineData("ABCD")]
    [InlineData("Abc")]
    public void WhenRegisterWithInvalidCode_ThrowArgumentException(string code)
    {
        // Arrange
        var euro = CurrencyInfo.FromCode("EUR");
        CurrencyInfo created = euro with { Code = code };

        // Act
        Action action = () => CurrencyInfo.Register(created);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1000, true)]
    [InlineData(1, false)]
    [InlineData(99, false)]
    public void WhenRegisterWithInvalidNumber_ThrowArgumentException(short number, bool isIso4217)
    {
        // Arrange
        var created = CurrencyInfo.Create("ABC") with { Number = number, IsIso4217 = isIso4217 };

        // Act
        Action action = () => CurrencyInfo.Register(created);

        // Assert
        action.Should().Throw<ArgumentException>();
    }
}
