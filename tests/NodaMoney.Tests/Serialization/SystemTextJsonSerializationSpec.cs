using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.Serialization.SystemTextJsonSerializationSpec;

public class GivenIWantToSerializeMoney
{
    public static IEnumerable<object[]> TestData => new[]
    {
        [new Money(765.4321m, Currency.FromCode("JPY")), "JPY", "JPY 765"],
        [new Money(765.4321m, Currency.FromCode("EUR")), "EUR", "EUR 765.43"],
        [new Money(765.4321m, Currency.FromCode("USD")), "USD", "USD 765.43"],
        [new Money(765.4321m, Currency.FromCode("BHD")), "BHD", "BHD 765.432"],
        [new Money(765.4321m, Currency.FromCode("BTC")), "BTC", "BTC 765.43210000"],
        (object[])[default(Money), "XXX", "XXX 0"],
        //new object[] { default(Money?), "\"\"", "\"\"" }
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingCurrency_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        // Arrange

        // Act
        string json = JsonSerializer.Serialize(money.Currency);

        // Assert
        json.Should().Be($"\"{expectedCurrency}\"");

        //var clone = System.Text.Json.JsonSerializer.Deserialize<Currency>(json);

        //clone.Should().Be(money.Currency);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenOnlyMoney_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        string json = JsonSerializer.Serialize(money);

        json.Should().Be($"\"{expectedMoney}\"");

        var clone = JsonSerializer.Deserialize<Money>(json);
        clone.Should().Be(money);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenOrderWithPriceAndNullableDiscount_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        // Arrange
        var order = new Order { Id = 123, Name = "Foo", Total = money };
        string expected = $$"""{"Id":123,"Name":"Foo","Total":"{{expectedMoney}}","Discount":null}""";

        // Act
        string json = JsonSerializer.Serialize(order);

        // Assert
        json.Should().Be(expected);

        var clone = JsonSerializer.Deserialize<Order>(json);
        clone.Should().BeEquivalentTo(order);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenNullableOrder_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        // Arrange
        var order = new NullableOrder() { Id = 123, Name = "Foo", Total = money };
        string expected = $$"""{"Id":123,"Name":"Foo","Total":"{{expectedMoney}}""";

        // Act
        string json = JsonSerializer.Serialize(order);

        // Assert
        json.Should().Be(expected);

        var clone = JsonSerializer.Deserialize<Order>(json);
        clone.Should().BeEquivalentTo(order);
    }
}

public class GivenIWantToDeserializeMoney
{
    [Theory]
    [ClassData(typeof(ValidJsonV1TestData))]
    public void WhenDeserializing_ThenThisShouldSucceed(string json, Money expected)
    {
        var clone = JsonSerializer.Deserialize<Money>(json);

        clone.Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(InvalidJsonV1TestData))]
    public void WhenDeserializingWithInvalidJSON_ThenThisShouldFail(string json)
    {
        Action action = () => JsonSerializer.Deserialize<Money>(json);

        action.Should().Throw<JsonException>().WithMessage("*property*");
    }

    [Theory]
    [ClassData(typeof(NestedJsonV1TestData))]
    public void WhenDeserializingWithNested_ThenThisShouldSucceed(string json, Order expected)
    {
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

        var clone = JsonSerializer.Deserialize<Order>(json, options);

        clone.Should().BeEquivalentTo(expected);
    }
}

public class GivenIWantToSerializeCurrency
{
    [Theory]
    [InlineData("EUR")]
    [InlineData("JPY")]
    [InlineData("CZK")]
    [InlineData("BTC")]
    public void WhenSerializingCurrency_ThenThisShouldSucceed(string code)
    {
        var currency = Currency.FromCode(code);

        string json = JsonSerializer.Serialize(currency);
        var clone = JsonSerializer.Deserialize<Currency>(json);

        clone.Should().Be(currency);
    }
}
