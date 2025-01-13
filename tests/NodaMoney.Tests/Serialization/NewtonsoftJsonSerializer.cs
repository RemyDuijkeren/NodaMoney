using System;
using System.Runtime.Serialization;
using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NodaMoney.Tests.Serialization.NewtonsoftJsonSerializerSpec;

public class GivenIWantToSerializeMoney
{
    public static IEnumerable<object[]> TestData => new[]
    {
        [new Money(765.4321m, CurrencyInfo.FromCode("JPY")), "JPY", "JPY 765"],
        [new Money(765.4321m, CurrencyInfo.FromCode("EUR")), "EUR", "EUR 765.43"],
        [new Money(765.4321m, CurrencyInfo.FromCode("USD")), "USD", "USD 765.43"],
        [new Money(765.4321m, CurrencyInfo.FromCode("BHD")), "BHD", "BHD 765.432"],
        [new Money(765.4321m, CurrencyInfo.FromCode("BTC")), "BTC", "BTC 765.43210000"],
        (object[])[default(Money), "XXX", "XXX 0"],
        //new object[] { default(Money?), "\"\"", "\"\"" }
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingCurrency_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        // Arrange

        // Act
        string json = JsonConvert.SerializeObject(money.Currency);

        // Assert
        json.Should().Be($"\"{expectedCurrency}\"");

        // var clone = JsonConvert.DeserializeObject<Currency>(json);
        // clone.Should().Be(money.Currency);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenOnlyMoney_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        // Arrange

        // Act
        string json = JsonConvert.SerializeObject(money);

        // Assert
        json.Should().Be($"\"{expectedMoney}\"");

        // var clone = JsonConvert.DeserializeObject<Money>(json);
        // clone.Should().Be(money);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenOrder_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        // Arrange
        var order = new Order { Id = 123, Name = "Foo", Total = money };
        string expected = $$"""
                            {"Id":123,"Total":"{{expectedMoney}}","Name":"Foo"}
                            """;

        // Act
        string json = JsonConvert.SerializeObject(order);

        // Assert
        json.Should().Be(expected);

        // var clone = JsonConvert.DeserializeObject<Order>(json);
        // clone.Should().BeEquivalentTo(order);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenNullableOrder_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        // Arrange
        var order = new NullableOrder() { Id = 123, Name = "Foo", Total = money };
        string expected = $$"""{"Id":123,"Total":"{{expectedMoney}}","Name":"Foo"}""";

        // Act
        string json = JsonConvert.SerializeObject(order);

        // Assert
        json.Should().Be(expected);

        // var clone = JsonConvert.DeserializeObject<Order>(json);
        // clone.Should().BeEquivalentTo(order);
    }
}

public class GivenIWantToDeserializeMoney
{
    [Theory(Skip = "This test is not working.")]
    [ClassData(typeof(ValidJsonV1TestData))]
    public void WhenDeserializingV1_ThenThisShouldSucceed(string json, Money expected)
    {
        var clone = JsonConvert.DeserializeObject<Money>(json);

        clone.Should().Be(expected);
    }

    [Theory(Skip = "This test is not working.")]
    [ClassData(typeof(InvalidJsonV1TestData))]
    public void WhenDeserializingWithInvalidJSONV1_ThenThisShouldFail(string json)
    {
        Action action = () => JsonConvert.DeserializeObject<Money>(json);

        action.Should().Throw<SerializationException>();
    }

    [Theory(Skip = "This test is not working.")]
    [ClassData(typeof(NestedJsonV1TestData))]
    public void WhenDeserializingWithNestedV1_ThenThisShouldSucceed(string json, Order expected)
    {
        var clone = JsonConvert.DeserializeObject<Order>(json);

        clone.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(ValidJsonV2TestData))]
    public void WhenDeserializingV2_ThenThisShouldSucceed(string json, Money expected)
    {
        var clone = JsonConvert.DeserializeObject<Money>(json);

        clone.Should().Be(expected);
    }

    [Fact]
    public void WhenDesializingV2_ShouldBeOk()
    {
        // Arrange
        string json = "\"EUR 123.456\"";
        var expected = new Money(123.456m, CurrencyInfo.FromCode("EUR"));

        // Act
        var clone = JsonConvert.DeserializeObject<Money>(json);

        // Assert
        clone.Should().Be(expected);
    }


    [Theory]
    [ClassData(typeof(InvalidJsonV2TestData))]
    public void WhenDeserializingWithInvalidJSONV2_ThenThisShouldFail(string json)
    {
        Action action = () => JsonConvert.DeserializeObject<Money>(json);

        action.Should().Throw<JsonException>();
    }

    [Theory]
    [ClassData(typeof(NestedJsonV2TestData))]
    public void WhenDeserializingWithNestedV2_ThenThisShouldSucceed(string json, Order expected)
    {
        var clone = JsonConvert.DeserializeObject<Order>(json);

        clone.Should().BeEquivalentTo(expected);
    }
}

public class GivenIWantToSerializeCurrency
{
    [Theory]
    [InlineData("JPY")]
    [InlineData("EUR")]
    [InlineData("USD")]
    [InlineData("BHD")]
    [InlineData("BTC")]
    [InlineData("XXX")]
    public void WhenSerializingCurrency_ThenThisShouldSucceed(string code)
    {
        // Arrange
        var currency = Currency.FromCode(code);

        // Act
        string json = JsonConvert.SerializeObject(currency);

        // Assert
        json.Should().Be($"\"{code}\"");

        var clone = JsonConvert.DeserializeObject<Currency>(json);
        clone.Should().Be(currency);
    }
}
