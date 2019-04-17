using System;
using System.IO;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using NodaMoney.Serialization.JsonNet;

using Formatting = Newtonsoft.Json.Formatting;

namespace NodaMoney.Tests.MoneySerializableSpec
{
    public class MoneySerializableTests
    {
        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public class GivenIWantToSerializeMoneyWithJsonNetSerializer
        {
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

            [Fact]
            public void WhenSerializingYen_ThenThisShouldSucceed()
            {
                string json = JsonConvert.SerializeObject(yen);
                Console.WriteLine(json);
                var clone = JsonConvert.DeserializeObject<Money>(json);

                clone.Should().Be(yen);
            }

            [Fact]
            public void WhenSerializingEuro_ThenThisShouldSucceed()
            {
                string json = JsonConvert.SerializeObject(euro, Formatting.None, new MoneyJsonConverter());
                Console.WriteLine(json);
                var clone = JsonConvert.DeserializeObject<Money>(json, new MoneyJsonConverter());

                clone.Should().Be(euro);
            }

            [Fact]
            public void WhenSerializingArticle_ThenThisShouldSucceed()
            {
                var article = new Article
                {
                    Id = 123,
                    Amount = Money.Euro(27.15),
                    Name = "Foo"
                };

                string json = JsonConvert.SerializeObject(article, Formatting.None, new MoneyJsonConverter());
                Console.WriteLine(json);
                var clone = JsonConvert.DeserializeObject<Article>(json, new MoneyJsonConverter());

                clone.Id.Should().Be(article.Id);
                clone.Name.Should().Be(article.Name);
                clone.Amount.Should().Be(article.Amount);
                //clone.Should().Be(article);
            }

            [Fact]
            public void WhenSerializingDefault_ThenThisShouldSucceed()
            {
                var expected = new MyType();
                var json = JsonConvert.SerializeObject(expected);
                var actual = JsonConvert.DeserializeObject<MyType>(json);
                actual.Should().NotBeNull();
                actual.Money.Should().Be(default(Money));
                actual.Money.Currency.Should().Be(expected.Money.Currency);
                actual.Money.Amount.Should().Be(expected.Money.Amount);
            }

            private class MyType
            {
                //[JsonConverter(typeof(MoneyJsonConverter))]
                public Money Money { get; set; }
            }
        }
        public class GivenIWantToSerializeMoneyWithDataContractSerializer
        {
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

            [Fact]
            public void WhenSerializingYen_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(yen)));

                yen.Should().Be(Clone<Money>(yen));
            }

            [Fact]
            public void WhenSerializingEuro_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(euro)));

                euro.Should().Be(Clone<Money>(euro));
            }

            [Fact]
            public void WhenSerializingArticle_ThenThisShouldSucceed()
            {
                var article = new Article
                {
                    Id = 123,
                    Amount = Money.Euro(27.15),
                    Name = "Foo"
                };

                Console.WriteLine(StreamToString(Serialize(article)));

                article.Amount.Should().Be(Clone<Article>(article).Amount);
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
                Console.WriteLine(StreamToString(Serialize(yen)));

                yen.Should().Be(Clone<Money>(yen));
            }

            [Fact]
            public void WhenSerializingEuro_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(euro)));

                euro.Should().Be(Clone<Money>(euro));
            }

            [Fact]
            public void WhenSerializingArticle_ThenThisShouldSucceed()
            {
                var article = new Article
                {
                    Id = 123,
                    Amount = Money.Euro(27.15),
                    Name = "Foo"
                };

                Console.WriteLine(StreamToString(Serialize(article)));

                article.Amount.Should().Be(Clone<Article>(article).Amount);
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

        [DataContract]
        public class Article
        {
            [DataMember]
            public int Id { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public Money Amount { get; set; }
        }

        //public class GivenIWantToSerializeMoneyWithBinaryFormatter
        //{
        //    private Money yen = new Money(765m, Currency.FromCode("JPY"));
        //    private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

        //    [Fact(Skip = "Not possible with PCL")]
        //    public void WhenSerializingYen_ThenThisShouldSucceed()
        //    {
        //        Console.WriteLine(StreamToString(Serialize(yen)));

        //        yen.Should().Be(Clone<Money>(yen));
        //    }

        //    [Fact(Skip = "Not possible with PCL")]
        //    public void WhenSerializingEuro_ThenThisShouldSucceed()
        //    {
        //        Console.WriteLine(StreamToString(Serialize(euro)));

        //        euro.Should().Be(Clone<Money>(euro));
        //    }

        //    public static Stream Serialize(object source)
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        Stream stream = new MemoryStream();
        //        formatter.Serialize(stream, source);
        //        return stream;
        //    }

        //    public static T Deserialize<T>(Stream stream)
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        stream.Position = 0L;
        //        return (T)formatter.Deserialize(stream);
        //    }

        //    public static T Clone<T>(object source)
        //    {
        //        return Deserialize<T>(Serialize(source));
        //    }
        //}
    }
}
