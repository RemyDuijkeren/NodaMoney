using System.IO;
using System.Xml.Serialization;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace NodaMoney.Tests.Serialization;

public class GivenIWantToSerializeCurrencyWithXmlSerializer
{
    private Currency yen = Currency.FromCode("JPY");

    private Currency euro = Currency.FromCode("EUR");

    [Fact]
    public void WhenSerializingYen_ThenThisShouldSucceed()
    {
        //Console.WriteLine(StreamToString(Serialize(yen)));
        StreamToString(Serialize(yen));

        yen.Should().Be(Clone<Currency>(yen));
    }

    [Fact]
    public void WhenSerializingEuro_ThenThisShouldSucceed()
    {
        //Console.WriteLine(StreamToString(Serialize(euro)));
        StreamToString(Serialize(euro));

        euro.Should().Be(Clone<Currency>(euro));
    }

    public static Stream Serialize(object source)
    {
        Stream stream = new MemoryStream();
        XmlSerializer xmlSerializer = new XmlSerializer(source.GetType());
        xmlSerializer.Serialize(stream, source);
        return stream;
    }

    public static T Deserialize<T>(Stream stream)
    {
        stream.Position = 0L;
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        return (T)xmlSerializer.Deserialize(stream);
    }

    private static T Clone<T>(object source)
    {
        return Deserialize<T>(Serialize(source));
    }

    public static string StreamToString(Stream stream)
    {
        stream.Position = 0;
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}

public class GivenIWantToSerializeCurrencyWithNewtownsoftJson
{
    [Theory]
    [InlineData("EUR")]
    [InlineData("JPY")]
    [InlineData("CZK")]
    public void WhenSerializingCurrency_ThenThisShouldSucceed(string code)
    {
        var currency = Currency.FromCode(code);

        string json = JsonConvert.SerializeObject(currency);
        var clone = JsonConvert.DeserializeObject<Currency>(json);

        clone.Should().Be(currency);
    }
}
