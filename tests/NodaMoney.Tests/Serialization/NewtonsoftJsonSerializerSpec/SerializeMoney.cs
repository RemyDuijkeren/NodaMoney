using System.Collections.Generic;
using Newtonsoft.Json;

namespace NodaMoney.Tests.Serialization.NewtonsoftJsonSerializerSpec;

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

    [Fact]
    public void WhenNullableOrderWithTotalIsNull_ThenThisShouldSucceed()
    {
        // Arrange
        //var order = new NullableOrder { Id = 123, Name = "Foo", Total = null };
        var order = new NullableOrder { Id = 123, Name = "Foo" };
        string expected = $$"""{"Id":123,"Total":null,"Name":"Foo"}""";

        // Act
        string json = JsonConvert.SerializeObject(order);

        // Assert
        json.Should().Be(expected);

        // var clone = JsonConvert.DeserializeObject<NullableOrder>(json);
        // clone.Should().BeEquivalentTo(order);
        // clone.Total.Should().BeNull();
    }
}
