using System;
using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.CurrencyBuilderSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToCreateCustomCurrency
{
    [Fact]
    public void WhenRegisterAsSimpleAsPossible_ThenShouldBeAvailableWithDefaults()
    {
        Currency result = new CurrencyBuilder("BTA", "ISO-4217").Register();

        Currency bitcoin = Currency.FromCode("BTA");
        //bitcoin.Namespace.Should().Be("ISO-4217");
        bitcoin.Symbol.Should().Be(CurrencyInfo.GenericCurrencySign); // ¤
        bitcoin.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void WhenRegisterBitCoinInIsoNamespace_ThenShouldBeAvailable()
    {
        var builder = new CurrencyBuilder("BTB", "ISO-4217")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8
        };

        Currency result = builder.Register();

        Currency bitcoin = Currency.FromCode("BTB");
        bitcoin.Symbol.Should().Be("฿");
        bitcoin.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void WhenRegisterBitCoin_ThenShouldBeAvailableByExplicitNamespace()
    {
        var builder = new CurrencyBuilder("BTC", "virtual")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8
        };

        Currency result = builder.Register();

        //Currency bitcoin = Currency.FromCode("BTC", "virtual");
        Currency bitcoin = Currency.FromCode("BTC");
        bitcoin.Symbol.Should().Be("฿");
        bitcoin.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void WhenBuildBitCoin_ThenItShouldSucceedButNotBeRegistered()
    {
        var builder = new CurrencyBuilder("BTD", "virtual")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8
        };

        Currency result = builder.Build();
        result.Symbol.Should().Be("฿");

        //Action action = () => Currency.FromCode("BTD", "virtual");
        Action action = () => Currency.FromCode("BTD");
        action.Should().Throw<InvalidCurrencyException>().WithMessage("BTD is unknown currency code!");
    }

    [Fact]
    public void WhenFromExistingCurrency_ThenThisShouldSucceed()
    {
        var builder = new CurrencyBuilder("BTE", "virtual");

        var euro = Currency.FromCode("EUR");
        builder.LoadDataFromCurrency(euro);

        var euroInfo = CurrencyInfo.FromCode("EUR");
        builder.Code.Should().Be("BTE");
        builder.Namespace.Should().Be("virtual");
        builder.EnglishName.Should().Be(euroInfo.EnglishName);
        builder.Symbol.Should().Be(euro.Symbol);
        builder.NumericCode.Should().Be(euroInfo.NumericCode);
        builder.DecimalDigits.Should().Be(euroInfo.DecimalDigits);
        builder.ValidFrom.Should().Be(euroInfo.IntroducedOn);
        builder.ValidTo.Should().Be(euroInfo.ExpiredOn);
    }

    [Fact]
    public void WhenRegisterExistingCurrency_ThenThrowException()
    {
        var builder = new CurrencyBuilder("EUR", "ISO-4217");

        var euro = Currency.FromCode("EUR");
        builder.LoadDataFromCurrency(euro);

        Action action = () => builder.Register();

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*is already registered*");
    }

    [Fact]
    public void WhenCodeIsNull_ThenThrowException()
    {
        Action action = () => new CurrencyBuilder(null, "virtual");

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCodeIsEmpty_ThenThrowException()
    {
        Action action = () => new CurrencyBuilder("", "virtual");

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenNamespaceIsNull_ThenThrowException()
    {
        Action action = () => new CurrencyBuilder("BTF", null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenNamespaceIsEmpty_ThenThrowException()
    {
        Action action = () => new CurrencyBuilder("BTG", "");

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenSymbolIsEmpty_ThenSymbolMustBeDefaultCurrencySign()
    {
        Currency result = new CurrencyBuilder("BTH", "ISO-4217")
        {
            EnglishName = "Bitcoin",
            //Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8,
        }.Register();

        Currency bitcoin = Currency.FromCode("BTH");
        bitcoin.Symbol.Should().Be(CurrencyInfo.GenericCurrencySign); // ¤
        bitcoin.Should().BeEquivalentTo(result);
    }
}

[Collection(nameof(NoParallelization))]
public class GivenIWantToUnregisterCurrency
{
    [Fact]
    public void WhenUnregisterIsoCurrency_ThenThisMustSucceed()
    {
        var money = Currency.FromCode("PEN"); // should work

        CurrencyBuilder.Unregister("PEN", "ISO-4217");
        Action action = () => Currency.FromCode("PEN");

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenUnregisterCustomCurrency_ThenThisMustSucceed()
    {
        var builder = new CurrencyBuilder("XYZ", "virtual")
        {
            EnglishName = "Xyz",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 4
        };

        builder.Register();
        //Currency xyz = Currency.FromCode("XYZ", "virtual"); // should work
        Currency xyz = Currency.FromCode("XYZ"); // should work

        CurrencyBuilder.Unregister("XYZ", "virtual");
        //Action action = () => Currency.FromCode("XYZ", "virtual");
        Action action = () => Currency.FromCode("XYZ");

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenCurrencyDoesNotExist_ThenThrowException()
    {
        Action action = () => CurrencyBuilder.Unregister("ABC", "virtual");

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*specifies a currency that is not found*");
    }

    [Fact]
    public void WhenCodeIsNull_ThenThrowException()
    {
        Action action = () => CurrencyBuilder.Unregister(null, "ISO-4217");

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCodeIsEmpty_ThenThrowException()
    {
        Action action = () => CurrencyBuilder.Unregister("", "ISO-4217");

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenNamespaceIsNull_ThenThrowException()
    {
        Action action = () => CurrencyBuilder.Unregister("EUR", null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenNamespaceIsEmpty_ThenThrowException()
    {
        Action action = () => CurrencyBuilder.Unregister("EUR", "");

        action.Should().Throw<ArgumentNullException>();
    }
}

[Collection(nameof(NoParallelization))]
public class GivenIWantToReplaceIsoCurrencyWithOwnVersion
{
    [Fact]
    public void WhenReplacingEuroWithCustom_ThenThisShouldSucceed()
    {
        // Panamanian balboa
        Currency oldEuro = CurrencyBuilder.Unregister("PAB", "ISO-4217");

        var builder = new CurrencyBuilder("PAB", "ISO-4217");
        builder.LoadDataFromCurrency(oldEuro);
        builder.EnglishName = "New Panamanian balboa";
        builder.DecimalDigits = 1;

        builder.Register();

        CurrencyInfo newEuro = CurrencyInfo.FromCode("PAB");
        newEuro.Symbol.Should().Be("B/.");
        newEuro.EnglishName.Should().Be("New Panamanian balboa");
        newEuro.DecimalDigits.Should().Be(1);
    }
}
