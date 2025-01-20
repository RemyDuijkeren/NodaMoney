using System;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.CurrencyInfoBuilderSpec;

[Collection(nameof(NoParallelization))]
public class CreateCustomCurrencyInfo
{
    [Fact]
    public void WhenBuildWithDefaults_ReturnDefaultCurrencyInfo()
    {
        // Arrange
        CurrencyInfoBuilder builder = new("BTA");

        // Act
        CurrencyInfo result = builder.Build();

        // Assert
        result.Code.Should().Be("BTA");
        result.IsIso4217.Should().BeFalse();
        result.Symbol.Should().Be(CurrencyInfo.GenericCurrencySign);
        result.EnglishName.Should().BeEmpty();
        result.NumericCode.Should().Be("000");
        result.DecimalDigits.Should().Be(0);
    }

    [Fact]
    public void WhenBuild_ShouldNotBeRegistered()
    {
        // Arrange
        CurrencyInfoBuilder builder = new("BTA");
        builder.Build();

        // Act
        Action action = () => CurrencyInfo.FromCode("BTA");

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenRegister_ShouldBeRegistered()
    {
        // Arrange
        CurrencyInfoBuilder builder = new("BTZ");
        CurrencyInfo result = builder.Register();

        // Act
        CurrencyInfo ci = CurrencyInfo.FromCode("BTZ");

        // Assert
        ci.Should().BeEquivalentTo(result);
        ci.Should().Be(result);
    }

    [Fact]
    public void WhenBuildBitCoin_ShouldNotBeRegistered()
    {
        // Arrange
        var builder = new CurrencyInfoBuilder("BTD")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8
        };

        // Act
        CurrencyInfo result = builder.Build();

        // Assert
        result.Symbol.Should().Be("฿");
        result.EnglishName.Should().Be("Bitcoin");
        result.NumericCode.Should().Be("123");
        result.DecimalDigits.Should().Be(8);
        result.IsIso4217.Should().BeFalse();
        result.Code.Should().Be("BTD");

        Action action = () => CurrencyInfo.FromCode("BTD");
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenRegisterBitCoin_ShouldBeRegisteredAsDefaultToNonIso()
    {
        // Arrange
        var builder = new CurrencyInfoBuilder("BTE")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123",
            DecimalDigits = 8,
            //IsIso4217 = false, // default should be False
        };

        // Act
        CurrencyInfo result = builder.Register();

        // Assert
        CurrencyInfo bitocin = CurrencyInfo.FromCode("BTE");
        bitocin.Symbol.Should().Be("฿");
        bitocin.IsIso4217.Should().BeFalse();
        bitocin.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void WhenRegisterBitcoinAsIso_ShouldBeRegisteredAsIso()
    {
        // Arrange
        var builder = new CurrencyInfoBuilder("BTB")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123",
            DecimalDigits = 8,
            IsIso4217 = true // in iso namespace
        };

        // Act
        CurrencyInfo result = builder.Register();

        // Assert
        result.Code.Should().Be("BTB");
        result.IsIso4217.Should().BeTrue();
        result.Symbol.Should().Be("฿");
        result.EnglishName.Should().Be("Bitcoin");
        result.NumericCode.Should().Be("123");
        result.DecimalDigits.Should().Be(8);

        CurrencyInfo ci = CurrencyInfo.FromCode("BTB");
        ci.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void WhenRegisterExistingCurrency_ThrowInvalidCurrencyException()
    {
        // Arrange
        var builder = new CurrencyInfoBuilder("EUR");
        var euro = CurrencyInfo.FromCode("EUR");
        builder.LoadDataFromCurrencyInfo(euro);

        // Act
        Action action = () => builder.Register();

        // Assert
        action.Should().Throw<InvalidCurrencyException>().WithMessage("*is already registered*");
    }

    [Fact]
    public void WhenLoadFromExistingCurrency_ThenThisShouldSucceed()
    {
        var builder = new CurrencyInfoBuilder("BTE") { IsIso4217 = false };

        var euro = CurrencyInfo.FromCode("EUR");
        builder.LoadDataFromCurrencyInfo(euro);

        var euroInfo = CurrencyInfo.FromCode("EUR");
        builder.Code.Should().Be("BTE");
        builder.IsIso4217.Should().BeTrue();
        builder.EnglishName.Should().Be(euroInfo.EnglishName);
        builder.Symbol.Should().Be(euro.Symbol);
        builder.NumericCode.Should().Be(euroInfo.NumericCode);
        builder.DecimalDigits.Should().Be(euroInfo.DecimalDigits);
        builder.ValidFrom.Should().Be(euroInfo.IntroducedOn);
        builder.ValidTo.Should().Be(euroInfo.ExpiredOn);
    }

    [Fact]
    public void WhenCodeIsNull_ThrowArgumentNullException()
    {
        // Arrange

        // Act
        Action action = () => new CurrencyInfoBuilder(null) { IsIso4217 = false };

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCodeIsEmpty_ThrowArgumentNullException()
    {
        // Arrange

        // Act
        Action action = () => new CurrencyInfoBuilder("")  { IsIso4217 = false };

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenSymbolIsEmpty_SymbolMustBeDefaultCurrencySign()
    {
        // Arrange

        // Act
        Currency result = new CurrencyInfoBuilder("BTH")
        {
            EnglishName = "Bitcoin",
            //Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8,
        }.Register();

        // Assert
        Currency bitcoin = Currency.FromCode("BTH");
        bitcoin.Symbol.Should().Be(CurrencyInfo.GenericCurrencySign); // ¤
        bitcoin.Should().BeEquivalentTo(result);
    }
}
