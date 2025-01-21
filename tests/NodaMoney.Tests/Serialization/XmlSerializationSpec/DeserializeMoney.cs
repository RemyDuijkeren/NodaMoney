using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.Serialization.XmlSerializationSpec;

public class DeserializeMoney :  XmlSerializationHelper
{
    public static IEnumerable<object[]> TestData => new[]
    {
        [new Money(765.4321m, Currency.FromCode("JPY")), """<Money Currency="JPY">765</Money>"""],
        [new Money(765.4321m, Currency.FromCode("EUR")), """<Money Currency="EUR">765.43</Money>"""],
        [new Money(765.4321m, Currency.FromCode("USD")), """<Money Currency="USD">765.43</Money>"""],
        [new Money(765.4321m, Currency.FromCode("BHD")), """<Money Currency="BHD">765.432</Money>"""],
        [new Money(765.4321m, Currency.FromCode("BTC")), """<Money Currency="BTC">765.43210000</Money>"""],
        (object[])[default(Money), """<Money Currency="XXX">0</Money>"""],
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenValidXml_ThenShouldParseToMoney(Money expectedMoney, string xml)
    {
        // Arrange

        // Act
        Money money = DeserializeFromXml<Money>(xml);

        // Assert
        money.Should().Be(expectedMoney);
    }

    [Fact]
    public void WhenValidXmlV1Format_ThenShouldParseToMoney()
    {
        // Arrange
        string xml = """<Money Amount="765.43" Currency="USD" />""";

        // Act
        Money money = DeserializeFromXml<Money>(xml);

        // Assert
        money.Should().Be(new Money(765.43m, Currency.FromCode("USD")));
    }

    [Fact]
    public void WhenInvalidXmlCurrency_ShouldThrowOnInvalidCurrency()
    {
        // Arrange
        var xml = """<Money Currency="INVALID">100.00</Money>""";

        // Act
        Action action = () => DeserializeFromXml<Money>(xml);;

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("There is an error in XML document *");
    }

    [Fact]
    public void WhenInvalidXmlAmount_ShouldThrowOnInvalidAmount()
    {
        // Arrange
        var xml = """<Money Currency="USD">INVALID</Money>""";

        // Act
        Action action = () => DeserializeFromXml<Money>(xml);

        // Assert
        action.Should().Throw<InvalidOperationException>().WithMessage("There is an error in XML document *");
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenValidNestedXml_ThenShouldParseToOrderWithTotal(Money expectedMoney, string xml)
    {
        // Arrange
        string xmlTotal = xml.Replace("Money", "Total"); // replace <Money Currency="USD">765.43</Money> to <Total Currency="USD">765.43</Total>
        string input = $$"""<Order xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>123</Id>{{xmlTotal}}<Name>Foo</Name></Order>""";

        // Act
        Order clone = DeserializeFromXml<Order>(input);

        // Assert
        var order = new Order { Id = 123, Total = expectedMoney, Name = "Foo" };
        clone.Should().BeEquivalentTo(order);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenValidNestedXml_ThenShouldParseToOrderWithNullableTotal(Money expectedMoney, string xml)
    {
        // Arrange
        string xmlTotal = xml.Replace("Money", "Total"); // replace <Money Currency="USD">765.43</Money> to <Total Currency="USD">765.43</Total>
        string input = $$"""<NullableOrder xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>123</Id>{{xmlTotal}}<Name>Foo</Name></NullableOrder>""";

        // Act
        NullableOrder clone = DeserializeFromXml<NullableOrder>(input);

        // Assert
        var order = new NullableOrder { Id = 123, Total = expectedMoney, Name = "Foo" };
        clone.Should().BeEquivalentTo(order);
    }

    [Fact]
    public void WhenValidNestedXmlSetToDefault_ThenTotalShouldBeZeroAndNoCurrency()
    {
        // Arrange
        string input = """<Order xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>123</Id><Total Currency="XXX">0</Total><Name>Foo</Name></Order>""";

        // Act
        Order clone = DeserializeFromXml<Order>(input);

        // Assert
        var order = new Order { Id = 123, Total = default, Name = "Foo" };
        clone.Should().BeEquivalentTo(order);
    }

    [Fact]
    public void WhenValidNestedXmlSetToDefault_ThenNullableTotalShouldBeNull()
    {
        // Arrange
        string input = """<NullableOrder xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Id>123</Id><Total xsi:nil="true" /><Name>Foo</Name></NullableOrder>""";

        // Act
        NullableOrder clone = DeserializeFromXml<NullableOrder>(input);

        // Assert
        var order = new NullableOrder { Id = 123, Total = default, Name = "Foo" };
        clone.Should().BeEquivalentTo(order);
    }
}
