using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.Serialization.DataContractSerializerSpec;

public class GivenIWantToSerializeMoney
{
    private Money yen = new Money(765m, Currency.FromCode("JPY"));
    private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

    [Fact]
    public void WhenSerializingYen_ThenThisShouldSucceed()
    {
        yen.Should().Be(Clone<Money>(yen));
    }

    [Fact]
    public void WhenSerializingEuro_ThenThisShouldSucceed()
    {
        euro.Should().Be(Clone<Money>(euro));
    }

    [Fact]
    public void WhenSerializingArticle_ThenThisShouldSucceed()
    {
        var article = new Order
        {
            Id = 123,
            Total = Money.Euro(27.15),
            Name = "Foo"
        };

        article.Total.Should().Be(Clone<Order>(article).Total);
    }

    public static Stream Serialize(object source)
    {
        Stream stream = new MemoryStream();
        var serializer = new DataContractSerializer(source.GetType());
        serializer.WriteObject(stream, source);
        return stream;
    }

    public static T Deserialize<T>(Stream stream)
    {
        stream.Position = 0L;
        using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
        {
            var serializer = new DataContractSerializer(typeof(T));
            return (T)serializer.ReadObject(reader, true);
        }
    }

    private static T Clone<T>(object source)
    {
        // Console.WriteLine(Serialize(source).ToString());
        return Deserialize<T>(Serialize(source));
    }
}
