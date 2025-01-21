using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.Serialization.SystemTextJsonSerializationSpec;

public class SerializeMoney
{
    public static IEnumerable<object[]> TestData => new[]
    {
        [new Money(765.4321m, CurrencyInfo.FromCode("JPY")), "JPY", "JPY 765"],
        [new Money(765.4321m, CurrencyInfo.FromCode("EUR")), "EUR", "EUR 765.43"],
        [new Money(765.4321m, CurrencyInfo.FromCode("USD")), "USD", "USD 765.43"],
        [new Money(765.4321m, CurrencyInfo.FromCode("BHD")), "BHD", "BHD 765.432"],
        [new Money(765.43214321m, CurrencyInfo.FromCode("BTC")), "BTC", "BTC 765.43214321"],
        (object[])[default(Money), "XXX", "XXX 0"],
        //new object[] { default(Money?), "\"\"", "\"\"" }
    };

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
    public void WhenOrderWithTotal_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        // Arrange
        var order = new Order { Id = 123, Name = "Foo", Total = money };
        string expected = $$"""{"Id":123,"Total":"{{expectedMoney}}","Name":"Foo"}""";

        // Act
        string json = JsonSerializer.Serialize(order);

        // Assert
        json.Should().Be(expected);

        var clone = JsonSerializer.Deserialize<Order>(json);
        clone.Should().BeEquivalentTo(order);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenNullableOrderWithTotal_ThenThisShouldSucceed(Money money, string expectedCurrency, string expectedMoney)
    {
        // Arrange
        var order = new NullableOrder() { Id = 123, Name = "Foo", Total = money };
        string expected = $$"""{"Id":123,"Total":"{{expectedMoney}}","Name":"Foo"}""";

        // Act
        string json = JsonSerializer.Serialize(order);

        // Assert
        json.Should().Be(expected);

        var clone = JsonSerializer.Deserialize<NullableOrder>(json);
        clone.Should().BeEquivalentTo(order);
        clone.Total.Should().Be(money);
    }

    [Fact]
    public void WhenNullableOrderWithTotalIsNull_ThenThisShouldSucceed()
    {
        // Arrange
        //var order = new NullableOrder { Id = 123, Name = "Foo", Total = null };
        var order = new NullableOrder { Id = 123, Name = "Foo" };
        string expected = $$"""{"Id":123,"Total":null,"Name":"Foo"}""";

        // Act
        string json = JsonSerializer.Serialize(order);

        // Assert
        json.Should().Be(expected);

        var clone = JsonSerializer.Deserialize<NullableOrder>(json);
        clone.Should().BeEquivalentTo(order);
        clone.Total.Should().BeNull();
    }
}
