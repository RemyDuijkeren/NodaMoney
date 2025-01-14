using System;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.CurrencyInfoBuilderSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToUnregisterCurrency
{
    [Fact]
    public void WhenUnregisterIsoCurrency_ThenThisMustSucceed()
    {
        var money = Currency.FromCode("PEN"); // should work

        CurrencyInfoBuilder.Unregister("PEN");
        Action action = () => Currency.FromCode("PEN");

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenUnregisterCustomCurrency_ThenThisMustSucceed()
    {
        var builder = new CurrencyInfoBuilder("XYZ")
        {
            EnglishName = "Xyz",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 4,
            IsIso4217 = false
        };

        builder.Register();
        //Currency xyz = Currency.FromCode("XYZ", "virtual"); // should work
        Currency xyz = Currency.FromCode("XYZ"); // should work

        CurrencyInfoBuilder.Unregister("XYZ");
        //Action action = () => Currency.FromCode("XYZ", "virtual");
        Action action = () => Currency.FromCode("XYZ");

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenCurrencyDoesNotExist_ThenThrowException()
    {
        Action action = () => CurrencyInfoBuilder.Unregister("ABC");

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*is unknown currency code!");
    }

    [Fact]
    public void WhenCodeIsNull_ThenThrowException()
    {
        Action action = () => CurrencyInfoBuilder.Unregister(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCodeIsEmpty_ThenThrowException()
    {
        Action action = () => CurrencyInfoBuilder.Unregister("");

        action.Should().Throw<ArgumentNullException>();
    }
}
