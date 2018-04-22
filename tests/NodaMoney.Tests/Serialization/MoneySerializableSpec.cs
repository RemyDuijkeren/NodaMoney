using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using NodaMoney.Tests.Serialization;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Serialization.Tests.MoneySerializableSpec
{
    public class GivenIWantToDeserializeMoneyWithJavaScriptConverter
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

    public class GivenIWantToSerializeMoneyWithJsonNetSerializer
    {
        public static IEnumerable<object[]> TestData => new[]
        {
                new object[] { new Money(765.4321m, Currency.FromCode("JPY")) },
                new object[] { new Money(765.4321m, Currency.FromCode("EUR")) },
                new object[] { new Money(765.4321m, Currency.FromCode("USD")) },
                new object[] { new Money(765.4321m, Currency.FromCode("BHD")) }
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public void WhenSerializingCurrency_ThenThisShouldSucceed(Money money)
        {
            string json = JsonConvert.SerializeObject(money.Currency);
            Trace.WriteLine(json);
            var clone = JsonConvert.DeserializeObject<Currency>(json);

            clone.Should().Be(money.Currency);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void WhenSerializingMoney_ThenThisShouldSucceed(Money money)
        {
            string json = JsonConvert.SerializeObject(money);
            Trace.WriteLine(json);
            var clone = JsonConvert.DeserializeObject<Money>(json);

            clone.Should().Be(money);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void WhenSerializingArticle_ThenThisShouldSucceed(Money money)
        {
            var order = new Order
            {
                Id = 123,
                Price = money,
                Name = "Foo"
            };

            string json = JsonConvert.SerializeObject(order);
            Console.WriteLine(json);
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
            Console.WriteLine(Serialize(yen).ReadToString());

            yen.Should().Be(Clone<Money>(yen));
        }

        [Fact]
        public void WhenSerializingEuro_ThenThisShouldSucceed()
        {
            Console.WriteLine(Serialize(euro).ReadToString());

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

            Console.WriteLine(Serialize(article).ReadToString());

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
            return Deserialize<T>(Serialize(source));
        }
    }

    public class GivenIWantToSerializeMoneyWitXmlSerializer
    {
        private Money yen = new Money(765m, Currency.FromCode("JPY"));
        private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

        [Fact]
        public void WhenSerializingYen_ThenThisShouldSucceed()
        {
            Console.WriteLine(Serialize(yen).ReadToString());

            yen.Should().Be(Clone<Money>(yen));
        }

        [Fact]
        public void WhenSerializingEuro_ThenThisShouldSucceed()
        {
            Console.WriteLine(Serialize(euro).ReadToString());

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

            Console.WriteLine(Serialize(article).ReadToString());

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

        private static T Clone<T>(object source)
        {
            return Deserialize<T>(Serialize(source));
        }
    }

    public class GivenIWantToSerializeMoneyWithBinaryFormatter
    {
        private Money yen = new Money(765m, Currency.FromCode("JPY"));
        private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

        [Fact]
        public void WhenSerializingYen_ThenThisShouldSucceed()
        {
            Console.WriteLine(Serialize(yen).ReadToString());

            yen.Should().Be(Clone<Money>(yen));
        }

        [Fact]
        public void WhenSerializingEuro_ThenThisShouldSucceed()
        {
            Console.WriteLine(Serialize(euro).ReadToString());

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

            Console.WriteLine(Serialize(article).ReadToString());

            article.Price.Should().Be(Clone<Order>(article).Price);
        }

        public static Stream Serialize(object source)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            formatter.Serialize(stream, source);
            return stream;
        }

        public static T Deserialize<T>(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Position = 0L;
            return (T)formatter.Deserialize(stream);
        }

        public static T Clone<T>(object source)
        {
            return Deserialize<T>(Serialize(source));
        }
    }
}
