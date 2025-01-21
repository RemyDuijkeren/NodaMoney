using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests.Serialization.XmlSerializationSpec;

public class SerializeCurrencyWithXmlSerializer : XmlSerializationHelper
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
