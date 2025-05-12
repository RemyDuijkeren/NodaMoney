using System.Runtime.Serialization;
using Newtonsoft.Json;
using NodaMoney.Context;

namespace NodaMoney.Tests.Serialization.NewtonsoftJsonSerializerSpec;

public class DeserializeMoney
{
    [Theory(Skip = "This test is not working. Can't fix this without Newtonsoft dependency.")]
    [ClassData(typeof(ValidJsonV1TestData))]
    public void WhenDeserializingV1_ThenThisShouldSucceed(string json, Money expected)
    {
        var clone = JsonConvert.DeserializeObject<Money>(json);

        clone.Should().Be(expected);
    }

    [Theory(Skip = "This test is not working. Can't fix this without Newtonsoft dependency.")]
    [ClassData(typeof(InvalidJsonV1TestData))]
    public void WhenDeserializingWithInvalidJSONV1_ThenThisShouldFail(string json)
    {
        Action action = () => JsonConvert.DeserializeObject<Money>(json);

        action.Should().Throw<SerializationException>();
    }

    [Theory(Skip = "This test is not working. Can't fix this without Newtonsoft dependency.")]
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
    public void WhenDesirializingV2_ShouldBeOk()
    {
        // Arrange
        string json = "\"EUR 123.456\"";
        var expected = new Money(123.456m, CurrencyInfo.FromCode("EUR"), MoneyContext.CreateNoRounding());

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

    [Fact]
    public void WhenNullableOrderWithTotalIsNull_ThenThisShouldSucceed()
    {
        // Arrange
        //var order = new NullableOrder { Id = 123, Name = "Foo", Total = null };
        var order = new NullableOrder { Id = 123, Name = "Foo" };
        string json = $$"""{"Id":123,"Total":null,"Name":"Foo"}""";

        // Act
        var deserialized = JsonConvert.DeserializeObject<NullableOrder>(json);

        // Assert
        deserialized.Should().BeEquivalentTo(order);
        deserialized.Total.Should().BeNull();
    }
}
