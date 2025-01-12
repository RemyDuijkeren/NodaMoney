using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.Serialization.XmlSerializationSpec;

// old format v1 : <Money Amount="765.43" Currency="USD" />
// new format v2: <money currency="USD">765.43</money>.
// Last option is better because it separates the currency and amount into two distinct parts: an attribute and the text content.
// This can make it easier to parse programmatically, as you can directly access the currency and amount without needing to split a string.
// However, it is slightly more complex in terms of XML structure.
// In terms of XML best practices, attributes are generally used to provide additional information about an element, while the
// text content of the element is the primary data. In this case, the amount of money could be considered the primary data,
// and the currency could be considered additional information, which would suggest that option 2 is more in line with XML best practices.

public class GivenIWantToSerializeMoney : XmlSerializationHelper
{
    public static IEnumerable<object[]> TestData => new[]
    {
        [new Money(765.4321m, Currency.FromCode("JPY")), """<Money Currency="JPY">765</Money>"""],
        [new Money(765.4321m, Currency.FromCode("EUR")), """<Money Currency="EUR">765.43</Money>"""],
        [new Money(765.4321m, Currency.FromCode("USD")), """<Money Currency="USD">765.43</Money>"""],
        [new Money(765.4321m, Currency.FromCode("BHD")), """<Money Currency="BHD">765.432</Money>"""],
        [new Money(765.4321m, Currency.FromCode("BTC")), """<Money Currency="BTC">765.43210000</Money>"""],
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

public class GivenIWantToDeserializeMoney :  XmlSerializationHelper
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

public class GivenIWantToSerializeCurrencyWithXmlSerializer : XmlSerializationHelper
{
    readonly Currency _yen = Currency.FromCode("JPY");
    readonly Currency _euro = Currency.FromCode("EUR");
    readonly Currency _bitcoin = Currency.FromCode("BTC");

    [Fact]
    public void WhenSerializingYen_ThenThisShouldSucceed()
    {
        //Console.WriteLine(StreamToString(Serialize(yen)));
        var xml = SerializeToXml(_yen);

        _yen.Should().Be(Clone<Currency>(_yen));
    }

    [Fact]
    public void WhenSerializingEuro_ThenThisShouldSucceed()
    {
        //Console.WriteLine(StreamToString(Serialize(euro)));
        var xml = SerializeToXml(_euro);

        _euro.Should().Be(Clone<Currency>(_euro));
    }

    [Fact]
    public void WhenSerializingBitcoin_ThenThisShouldSucceed()
    {
        //Console.WriteLine(StreamToString(Serialize(euro)));
        var xml = SerializeToXml(_bitcoin);

        _bitcoin.Should().Be(Clone<Currency>(_bitcoin));
    }
}

public class XmlSerializationHelper
{
    public static string SerializeToXml(object source)
    {
        var settings = new XmlWriterSettings
        {
            Indent = false,
            OmitXmlDeclaration = true
        };
        using var stream = new StringWriter();
        using var writer = XmlWriter.Create(stream, settings);

        var xmlSerializer = new XmlSerializer(source.GetType());
        xmlSerializer.Serialize(writer, source);

        return stream.ToString();
    }

    public static T DeserializeFromXml<T>(string xml)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        using var stream = new StringReader(xml);
        return (T)xmlSerializer.Deserialize(stream);
    }

    public static T Clone<T>(object source) => DeserializeFromXml<T>(SerializeToXml(source));
}
