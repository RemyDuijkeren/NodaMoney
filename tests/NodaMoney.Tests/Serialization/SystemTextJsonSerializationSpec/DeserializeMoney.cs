using System.Text.Json;

namespace NodaMoney.Tests.Serialization.SystemTextJsonSerializationSpec;

public class DeserializeMoney
{
    [Theory]
    [ClassData(typeof(ValidJsonV1TestData))]
    public void WhenDeserializingV1_ThenThisShouldSucceed(string json, Money expected)
    {
        var clone = JsonSerializer.Deserialize<Money>(json);

        clone.Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(ValidJsonV2TestData))]
    public void WhenDeserializingV2_ThenThisShouldSucceed(string json, Money expected)
    {
        var clone = JsonSerializer.Deserialize<Money>(json);

        clone.Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(InvalidJsonV1TestData))]
    public void WhenDeserializingWithInvalidJSONV1_ThenThisShouldFail(string json)
    {
        Action action = () => JsonSerializer.Deserialize<Money>(json);

        action.Should().Throw<JsonException>().WithMessage("*property*");
    }

    [Theory]
    [ClassData(typeof(InvalidJsonV2TestData))]
    public void WhenDeserializingWithInvalidJSONV2_ThenThisShouldFail(string json)
    {
        Action action = () => JsonSerializer.Deserialize<Money>(json);

        action.Should().Throw<JsonException>().WithMessage("*invalid*");
    }

    [Theory]
    [ClassData(typeof(NestedJsonV1TestData))]
    public void WhenDeserializingWithNestedV1_ThenThisShouldSucceed(string json, Order expected)
    {
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        var clone = JsonSerializer.Deserialize<Order>(json, options);

        clone.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(NestedJsonV2TestData))]
    public void WhenDeserializingWithNestedV2_ThenThisShouldSucceed(string json, Order expected)
    {
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        var clone = JsonSerializer.Deserialize<Order>(json, options);

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
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        var deserialized = JsonSerializer.Deserialize<NullableOrder>(json, options);

        // Assert
        deserialized.Should().BeEquivalentTo(order);
        deserialized.Total.Should().BeNull();
    }
}
