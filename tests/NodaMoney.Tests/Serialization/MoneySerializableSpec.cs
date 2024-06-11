using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace NodaMoney.Tests.Serialization.MoneySerializableSpec;

public class GivenIWantToDeserializeMoneyWithSystemTextJsonSerializer
{
    [Theory]
    [ClassData(typeof(ValidJsonTestData))]
    public void WhenDeserializing_ThenThisShouldSucceed(string json, Money expected)
    {
            var clone = System.Text.Json.JsonSerializer.Deserialize<Money>(json);

            clone.Should().Be(expected);
        }

    [Theory]
    [ClassData(typeof(InvalidJsonTestData))]
    public void WhenDeserializingWithInvalidJSON_ThenThisShouldFail(string json)
    {
            Action action = () => System.Text.Json.JsonSerializer.Deserialize<Money>(json);

            action.Should().Throw<SerializationException>().WithMessage("Member '*");
        }

    [Theory]
    [ClassData(typeof(NestedJsonTestData))]
    public void WhenDeserializingWithNested_ThenThisShouldSucceed(string json, Order expected)
    {
            var clone = System.Text.Json.JsonSerializer.Deserialize<Order>(json);

            clone.Should().BeEquivalentTo(expected);
            clone.Discount.Should().BeNull();
        }
}

// TODO: Write convertors, see https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-8-0
public class GivenIWantToSerializeMoneyWithSystemTextJsonSerializer
{
    public static IEnumerable<object[]> TestData => new[]
        {
                new object[] { new Money(765.4321m, Currency.FromCode("JPY")) },
                new object[] { new Money(765.4321m, Currency.FromCode("EUR")) },
                new object[] { new Money(765.4321m, Currency.FromCode("USD")) },
                new object[] { new Money(765.4321m, Currency.FromCode("BHD")) },
                new object[] { default(Money) },
                new object[] { default(Money?) }
        };

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingCurrency_ThenThisShouldSucceed(Money money)
    {
            string json = System.Text.Json.JsonSerializer.Serialize(money.Currency);
            Console.WriteLine(json);
            var clone = System.Text.Json.JsonSerializer.Deserialize<Currency>(json);

            clone.Should().Be(money.Currency);
        }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingMoney_ThenThisShouldSucceed(Money money)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(money);
        // Console.WriteLine(json);
        var clone = System.Text.Json.JsonSerializer.Deserialize<Money>(json);

        clone.Should().Be(money);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingArticleWithPrice_ThenThisShouldSucceed(Money money)
    {
            var order = new Order
            {
                Id = 123,
                Name = "Foo",
                Price = money
            };

            string json = System.Text.Json.JsonSerializer.Serialize(order);
            // Console.WriteLine(json);
            var clone = System.Text.Json.JsonSerializer.Deserialize<Order>(json);

            clone.Should().BeEquivalentTo(order);
        }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingArticleWithPriceAndDiscount_ThenThisShouldSucceed(Money money)
    {
            var order = new Order
            {
                Id = 123,
                Name = "Foo",
                Price = money,
                Discount = money
            };

            string json = System.Text.Json.JsonSerializer.Serialize(order);
            // Console.WriteLine(json);
            var clone = System.Text.Json.JsonSerializer.Deserialize<Order>(json);

            clone.Should().BeEquivalentTo(order);
        }
}

public class GivenIWantToDeserializeMoneyWithNewtonsoftJsonSerializer
{
    [Theory]
    [ClassData(typeof(ValidJsonTestData))]
    public void WhenDeserializing_ThenThisShouldSucceed(string json, Money expected)
    {
            var clone = JsonConvert.DeserializeObject<Money>(json);

            clone.Should().Be(expected);
        }

    [Theory]
    [ClassData(typeof(InvalidJsonTestData))]
    public void WhenDeserializingWithInvalidJSON_ThenThisShouldFail(string json)
    {
            Action action = () => JsonConvert.DeserializeObject<Money>(json);

            action.Should().Throw<SerializationException>().WithMessage("Member '*");
        }

    [Theory]
    [ClassData(typeof(NestedJsonTestData))]
    public void WhenDeserializingWithNested_ThenThisShouldSucceed(string json, Order expected)
    {
            var clone = JsonConvert.DeserializeObject<Order>(json);

            clone.Should().BeEquivalentTo(expected);
            clone.Discount.Should().BeNull();
        }
}

public class GivenIWantToSerializeMoneyWithNewtonsoftJsonSerializer
{
    public static IEnumerable<object[]> TestData => new[]
        {
                new object[] { new Money(765.4321m, Currency.FromCode("JPY")) },
                new object[] { new Money(765.4321m, Currency.FromCode("EUR")) },
                new object[] { new Money(765.4321m, Currency.FromCode("USD")) },
                new object[] { new Money(765.4321m, Currency.FromCode("BHD")) },
                new object[] { default(Money) },
                new object[] { default(Money?) }
        };

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingCurrency_ThenThisShouldSucceed(Money money)
    {
            string json = JsonConvert.SerializeObject(money.Currency);
            //Console.WriteLine(json);
            var clone = JsonConvert.DeserializeObject<Currency>(json);

            clone.Should().Be(money.Currency);
        }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingMoney_ThenThisShouldSucceed(Money money)
    {
            string json = JsonConvert.SerializeObject(money);
            // Console.WriteLine(json);
            var clone = JsonConvert.DeserializeObject<Money>(json);

            clone.Should().Be(money);
        }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingArticleWithPrice_ThenThisShouldSucceed(Money money)
    {
            var order = new Order
            {
                Id = 123,
                Name = "Foo",
                Price = money
            };

            string json = JsonConvert.SerializeObject(order);
            // Console.WriteLine(json);
            var clone = JsonConvert.DeserializeObject<Order>(json);

            clone.Should().BeEquivalentTo(order);
        }

    [Theory]
    [MemberData(nameof(TestData))]
    public void WhenSerializingArticleWithPriceAndDiscount_ThenThisShouldSucceed(Money money)
    {
            var order = new Order
            {
                Id = 123,
                Name = "Foo",
                Price = money,
                Discount = money
            };

            string json = JsonConvert.SerializeObject(order);
            // Console.WriteLine(json);
            var clone = JsonConvert.DeserializeObject<Order>(json);

            clone.Should().BeEquivalentTo(order);
        }
}

public class GivenIWantToSerializeMoneyWithDataContractSerializer
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
                Price = Money.Euro(27.15),
                Name = "Foo"
            };

            article.Price.Should().Be(Clone<Order>(article).Price);
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

public class GivenIWantToSerializeMoneyWithXmlSerializer
{
    private Money yen = new Money(765m, Currency.FromCode("JPY"));
    private Money euro = new Money(765.43m, Currency.FromCode("EUR"));
    private Money bitcoin = new Money(765.43m, Currency.FromCode("BTC"));

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
    public void WhenSerializingBitcoin_ThenThisShouldSucceed()
    {
        bitcoin.Should().Be(Clone<Money>(bitcoin));
    }

    [Fact]
    public void WhenSerializingArticle_ThenThisShouldSucceed()
    {
            var article = new Order
            {
                Id = 123,
                Price = Money.Euro(27.15),
                Name = "Foo"
            };

            article.Price.Should().Be(Clone<Order>(article).Price);
        }

    public static Stream Serialize(object source)
    {
            Stream stream = new MemoryStream();
            var xmlSerializer = new XmlSerializer(source.GetType());
            xmlSerializer.Serialize(stream, source);
            return stream;
        }

    public static T Deserialize<T>(Stream stream)
    {
            stream.Position = 0L;
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(stream);
        }

    public static string StreamToString(Stream stream)
    {
        stream.Position = 0;
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    private static T Clone<T>(object source)
    {
        Console.WriteLine(StreamToString(Serialize(source)));
        return Deserialize<T>(Serialize(source));
    }
}

public class GivenIWantToSerializeMoneyWithBinaryFormatter
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
                Price = Money.Euro(27.15),
                Name = "Foo"
            };

            article.Price.Should().Be(Clone<Order>(article).Price);
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
