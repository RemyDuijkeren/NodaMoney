using System;
using System.IO;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Globalization;
using NodaMoney.Tests.Serialization;

namespace NodaMoney.Serialization.Tests.MoneySerializableSpec
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

        public class GivenIWantToDeserializeMoneyWithJavaScriptConverter
        {
            private static string CurrentCultureCode = new RegionInfo(CultureInfo.CurrentCulture.LCID).ISOCurrencySymbol;

            public static IEnumerable<object[]> ValidNestedJsonData => new[]
            {
                new object[] { $"{{ cash: {{ amount: '200', currency: '{CurrentCultureCode}' }} }}", },
                new object[] { $"{{ cash: {{ amount: 200, currency: '{CurrentCultureCode}' }} }}" },
                new object[] { $"{{ cash: {{ currency: '{CurrentCultureCode}', amount: 200 }} }}" },
                new object[] { $"{{ cash: {{ currency: '{CurrentCultureCode}', amount: '200' }} }}" }
            };

            public static IEnumerable<object[]> ValidNestedNullableJsonData => new[]
            {
                new object[] { $"{{ cash: {{ amount: '200', currency: '{CurrentCultureCode}' }} }}", },
                new object[] { $"{{ cash: {{ amount: 200, currency: '{CurrentCultureCode}' }} }}" },
                new object[] { $"{{ cash: {{ currency: '{CurrentCultureCode}', amount: 200 }} }}" },
                new object[] { $"{{ cash: {{ currency: '{CurrentCultureCode}', amount: '200' }} }}" },
                new object[] { $"{{ cash: null }}" },
            };

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

                var exception = Record.Exception(() =>
                    JsonConvert.DeserializeObject<Money>(json)
                );

                exception.Should().BeOfType<SerializationException>();
            }

            private class TypeWithMoneyProperty
            {
                public Money Cash { get; set; }
            }

            [Theory]
            [MemberData(nameof(ValidNestedJsonData))]
            public void WhenDeserializingWithNested_ThenThisShouldSucceed(string json)
            {
                var money = new Money(200, Currency.FromCode(CurrentCultureCode));

                // Console.WriteLine(json);
                var clone = JsonConvert.DeserializeObject<TypeWithMoneyProperty>(json);

                clone.Cash.Should().Be(money);
            }

            private class TypeWithNullableMoneyProperty
            {
                public Money? Cash { get; set; }
            }

            [Theory]
            [MemberData(nameof(ValidNestedNullableJsonData))]
            public void WhenDeserializingWithNestedNullable_ThenThisShouldSucceed(string json)
            {
                var money = new Money(200, Currency.FromCode(CurrentCultureCode));

                // Console.WriteLine(json);
                var clone = JsonConvert.DeserializeObject<TypeWithNullableMoneyProperty>(json);

                if (!json.Contains("null"))
                    clone.Cash.Should().Be(money);
                else
                    clone.Cash.Should().BeNull();
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
                var article = new Order
                {
                    Id = 123,
                    Price = money,
                    Name = "Foo"
                };

                string json = JsonConvert.SerializeObject(article);
                Console.WriteLine(json);
                var clone = JsonConvert.DeserializeObject<Order>(json);

                clone.Price.Should().Be(money);
            }

            //[Theory]
            //[MemberData("TestData")]
            //public void WhenSerializing_ThenThisShouldSucceed(Money money)
            //{
            //    JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            //    {
            //        Converters = new List<JsonConverter> { new MoneyJsonConverter() }
            //    };

            //    var json = JsonConvert.SerializeObject(money);
            //    // Console.WriteLine(json);
            //    var clone = JsonConvert.DeserializeObject<Money>(json);

            //    clone.Should().Be(money);
            //}

            //[Fact]
            //public void WhenSerializingEuroWithExplicitConverter_ThenThisShouldSucceed()
            //{
            //    string json = JsonConvert.SerializeObject(euro, Formatting.None, new MoneyJsonConverter());
            //    Console.WriteLine(json);
            //    var clone = JsonConvert.DeserializeObject<Money>(json, new MoneyJsonConverter());

            //    clone.Should().Be(euro);
            //}

            //[Fact]
            //public void WhenSerializingArticleWithExplicitConverter_ThenThisShouldSucceed()
            //{
            //    var article = new Article
            //    {
            //        Id = 123,
            //        Amount = Money.Euro(27.15),
            //        Name = "Foo"
            //    };

            //    string json = JsonConvert.SerializeObject(article, Formatting.None, new MoneyJsonConverter());
            //    Console.WriteLine(json);
            //    var clone = JsonConvert.DeserializeObject<Article>(json, new MoneyJsonConverter());

            //    clone.Id.Should().Be(article.Id);
            //    clone.Name.Should().Be(article.Name);
            //    clone.Amount.Should().Be(article.Amount);
            //    //clone.Should().Be(article);
            //}
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
                var article = new Order
                {
                    Id = 123,
                    Price = Money.Euro(27.15),
                    Name = "Foo"
                };

                Console.WriteLine(StreamToString(Serialize(article)));

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
                var article = new Order
                {
                    Id = 123,
                    Price = Money.Euro(27.15),
                    Name = "Foo"
                };

                Console.WriteLine(StreamToString(Serialize(article)));

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
                var article = new Order
                {
                    Id = 123,
                    Price = Money.Euro(27.15),
                    Name = "Foo"
                };

                Console.WriteLine(StreamToString(Serialize(article)));

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
}
