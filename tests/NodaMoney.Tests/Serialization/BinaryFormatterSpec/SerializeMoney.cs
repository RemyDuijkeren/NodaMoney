using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NodaMoney.Tests.Serialization.BinaryFormatterSpec;

public class SerializeMoney
{
    private Money yen = new Money(765m, Currency.FromCode("JPY"));
    private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

#if NET8_0_OR_GREATER
    [Fact(Skip = "Not supported in .NET 8.0 or greater. See https://aka.ms/binaryformatter")]
#else
        [Fact]
#endif
    public void WhenSerializingYen_ThenThisShouldSucceed()
    {
        yen.Should().Be(Clone<Money>(yen));
    }

#if NET8_0_OR_GREATER
    [Fact(Skip = "Not supported in .NET 8.0 or greater. See https://aka.ms/binaryformatter")]
#else
        [Fact]
#endif
    public void WhenSerializingEuro_ThenThisShouldSucceed()
    {
        euro.Should().Be(Clone<Money>(euro));
    }

#if NET8_0_OR_GREATER
    [Fact(Skip = "Not supported in .NET 8.0 or greater. See https://aka.ms/binaryformatter")]
#else
        [Fact]
#endif
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

    [Obsolete("Obsolete")]
    public static Stream Serialize(object source)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        formatter.Serialize(stream, source);
        return stream;
    }

    [Obsolete("Obsolete")]
    public static T Deserialize<T>(Stream stream)
    {
        IFormatter formatter = new BinaryFormatter();
        stream.Position = 0L;
        return (T)formatter.Deserialize(stream);
    }

    public static T Clone<T>(object source)
    {
        // Console.WriteLine(Serialize(source).ToString());
        return Deserialize<T>(Serialize(source));
    }
}
