using System;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.CurrencyInfoSpec;

public class CreateAsFormatProvider
{
    [Fact]
    public void IsIFormatProvider_When_CreateCurrencyInfo()
    {
        // Act
        CurrencyInfo currencyInfo = CurrencyInfo.FromCode("EUR");

        // Assert
        currencyInfo.Should().BeAssignableTo<IFormatProvider>();
    }

    [Fact]
    public void METHOD()
    {
        // Arrange


        // Act

        // Assert
    }

}
