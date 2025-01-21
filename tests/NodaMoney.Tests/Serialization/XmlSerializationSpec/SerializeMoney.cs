using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.Serialization.XmlSerializationSpec;

public class SerializeMoney : XmlSerializationHelper
{
    public static IEnumerable<object[]> TestData => new[]
    {
        [new Money(765.4321m, CurrencyInfo.FromCode("JPY")), """<Money Currency="JPY">765</Money>"""],
        [new Money(765.4321m, CurrencyInfo.FromCode("EUR")), """<Money Currency="EUR">765.43</Money>"""],
        [new Money(765.4321m, CurrencyInfo.FromCode("USD")), """<Money Currency="USD">765.43</Money>"""],
        [new Money(765.4321m, CurrencyInfo.FromCode("BHD")), """<Money Currency="BHD">765.432</Money>"""],
        [new Money(765.43214321m, CurrencyInfo.FromCode("BTC")), """<Money Currency="BTC">765.43214321</Money>"""],
        (object[])[default(Money), """<Money Currency="XXX">0</Money>"""]
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenMoney_ThenShouldProduceValidXml(Money money, string expectedXml)
    {
        // Arrange

        // Act
        var xml = SerializeToXml(money);

        // Assert
        xml.Should().Be($"{expectedXml}");
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenObjectWithNestedMoney_ThenShouldProduceValidXml(Money money, string expectedXml)
    {
        // Arrange
        var order = new Order { Id = 123, Total = money, Name = "Foo" };

        // Act
        var xml = SerializeToXml(order);

        // Assert
        string xmlTotal = expectedXml.Replace("Money", "Total"); // replace <Money Currency="USD">765.43</Money> to <Total Currency="USD">765.43</Total>
        xml.Should().Be($$"""<Order xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>123</Id>{{xmlTotal}}<Name>Foo</Name></Order>""");
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenObjectWithNestedNullableMoney_ThenShouldProduceValidXml(Money money, string expectedXml)
    {
        // Arrange
        var order = new NullableOrder { Id = 123, Total = money, Name = "Foo" };

        // Act
        var xml = SerializeToXml(order);

        // Assert
        string xmlTotal = expectedXml.Replace("Money", "Total"); // replace <Money Currency="USD">765.43</Money> to <Total Currency="USD">765.43</Total>
        xml.Should().Be($$"""<NullableOrder xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>123</Id>{{xmlTotal}}<Name>Foo</Name></NullableOrder>""");
    }

    [Fact]
    public void WhenObjectWithNestedMoneySetToDefault_ThenTotalShouldBeZeroWithNoCurrency()
    {
        // Arrange
        var order = new Order { Id = 123, Total = default, Name = "Foo" };

        // Act
        var xml = SerializeToXml(order);

        // Assert
        string xmlTotal = """<Total Currency="XXX">0</Total>""";
        xml.Should().Be($$"""<Order xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>123</Id>{{xmlTotal}}<Name>Foo</Name></Order>""");
    }

    [Fact]
    public void WhenObjectWithNestedNullableMoneySetToDefault_ThenTotalShouldBeNull()
    {
        // Arrange
        var order = new NullableOrder { Id = 123, Total = default, Name = "Foo" };

        // Act
        var xml = SerializeToXml(order);

        // Assert
        string xmlTotal = """<Total xsi:nil="true" />""";
        xml.Should().Be($$"""<NullableOrder xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>123</Id>{{xmlTotal}}<Name>Foo</Name></NullableOrder>""");
    }
}
