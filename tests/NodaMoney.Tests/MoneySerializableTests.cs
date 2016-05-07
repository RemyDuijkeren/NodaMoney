using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NodaMoney.Serialization.AspNet;
using NodaMoney.Serialization.JsonNet;

using Formatting = Newtonsoft.Json.Formatting;

namespace NodaMoney.Tests
{
    internal static class MoneySerializableTests
    {
        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [TestClass]
        public class GivenIWantToSerializeMoneyWithJsonNetSerializer
        {
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

            [TestMethod]
            public void WhenSerializingYen_ThenThisShouldSucceed()
            {
                string json = JsonConvert.SerializeObject(yen);
                Console.WriteLine(json);
                var clone = JsonConvert.DeserializeObject<Money>(json);

                clone.Should().Be(yen);
            }

            [TestMethod]
            public void WhenSerializingEuro_ThenThisShouldSucceed()
            {
                string json = JsonConvert.SerializeObject(euro, Formatting.None, new MoneyJsonConverter());
                Console.WriteLine(json);
                var clone = JsonConvert.DeserializeObject<Money>(json, new MoneyJsonConverter());

                clone.Should().Be(euro);
            }
        }

        [TestClass]
        public class GivenIWantToSerializeMoneyWithJavaScriptSerializer
        {
            private readonly JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));


            [TestMethod]
            public void WhenSerializingYen_ThenThisShouldSucceed()
            {
                javaScriptSerializer.RegisterConverters(new JavaScriptConverter[] { new MoneyJavaScriptConverter() });

                var json = javaScriptSerializer.Serialize(yen);
                Console.WriteLine(json);
                var clone = javaScriptSerializer.Deserialize<Money>(json);

                clone.Should().Be(yen);
            }

            [TestMethod]
            public void WhenSerializingEuro_ThenThisShouldSucceed()
            {
                javaScriptSerializer.RegisterConverters(new JavaScriptConverter[] { new MoneyJavaScriptConverter() });

                var json = javaScriptSerializer.Serialize(euro);
                Console.WriteLine(json);
                var clone = javaScriptSerializer.Deserialize<Money>(json);

                clone.Should().Be(euro);
            }
        }

        [TestClass]
        public class GivenIWantToSerializeMoneyWithDataContractJsonSerializer
        {
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

            [TestMethod]
            public void WhenSerializingYen_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(yen)));

                yen.Should().Be(Clone<Money>(yen));
            }

            [TestMethod]
            public void WhenSerializingEuro_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(euro)));

                euro.Should().Be(Clone<Money>(euro));
            }

            public static Stream Serialize(object source)
            {
                Stream stream = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(source.GetType());
                serializer.WriteObject(stream, source);
                return stream;

                //MemoryStream stream = new MemoryStream();
                //DataContractJsonSerializer serializer = new DataContractJsonSerializer(source.GetType());
                //serializer.WriteObject(stream, source);
                //string json = Encoding.UTF8.GetString(stream.ToArray());
                //stream.Close();

                //return json;
            }

            public static T Deserialize<T>(Stream stream)
            {
                stream.Position = 0L;
                using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    return (T)serializer.ReadObject(reader, true);
                }

                //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                //MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
                //return (T)serializer.ReadObject(stream);
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

	    [TestClass]
        public class GivenIWantToSerializeMoneyWithDataContractSerializer
        {
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

            [TestMethod]
            public void WhenSerializingYen_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(yen)));

                yen.Should().Be(Clone<Money>(yen));
            }

            [TestMethod]
            public void WhenSerializingEuro_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(euro)));

                euro.Should().Be(Clone<Money>(euro));
            }

	        [TestMethod]
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
                DataContractSerializer serializer = new DataContractSerializer(source.GetType());
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

        [TestClass]
        public class GivenIWantToSerializeMoneyWitXmlSerializer
        {
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

            [TestMethod]
            public void WhenSerializingYen_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(yen)));

                yen.Should().Be(Clone<Money>(yen));
            }

            [TestMethod]
            public void WhenSerializingEuro_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(euro)));

                euro.Should().Be(Clone<Money>(euro));
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
        }

        [TestClass][Ignore]
        public class GivenIWantToSerializeMoneyWithBinaryFormatter
        {
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));

            [TestMethod]
            public void WhenSerializingYen_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(yen)));

                yen.Should().Be(Clone<Money>(yen));
            }

            [TestMethod]
            public void WhenSerializingEuro_ThenThisShouldSucceed()
            {
                Console.WriteLine(StreamToString(Serialize(euro)));

                euro.Should().Be(Clone<Money>(euro));
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
