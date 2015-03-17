using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodaMoney.UnitTests
{
    static internal class MoneySerializableTests
    {
        [TestClass]
        public class GivenIWantToSerializeMoney
        {
            private Money yen = new Money(765m, Currency.FromCode("JPY"));
            private Money euro = new Money(765.43m, Currency.FromCode("EUR"));
            private Money dollar = new Money(765.43m, Currency.FromCode("USD"));
            private Money dinar = new Money(765.432m, Currency.FromCode("BHD"));

            //[TestMethod]
            //public void WhenSerializingJavaScript_ThenThisShouldSucceed()
            //{
            //    var serializer = new JavaScriptSerializer();
            //    var serializedResult = serializer.Serialize(dollar);
            //    var deserializedResult = serializer.Deserialize<Money>(serializedResult);
            //}

            [TestMethod]
            public void WhenSerializingDataContract_ThenThisShouldSucceed()
            {
                dollar.Should().Be(CloneDataContract<Money>(dollar));
                var x = StreamToString(SerializeDataContract(dollar));
                Console.WriteLine(x);
            }

            [TestMethod]
            public void WhenSerializingXml_ThenThisShouldSucceed()
            {
                ((object)euro).Should().BeXmlSerializable();
                euro.Should().Be(CloneXml<Money>(euro));

                var x = StreamToString(SerializeDataContract(euro));
                Console.WriteLine(x);
            }

            [TestMethod]
            public void WhenSerializingBinary_ThenThisShouldSucceed()
            {
                ((object)euro).Should().BeBinarySerializable();
            }

            [TestMethod]
            public void WhenSerializingBinaryCustom_ThenThisShouldSucceed()
            {
                yen.Should().Be(CloneBinary<Money>(yen));
            }

            public static Stream SerializeBinary(object source)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                formatter.Serialize(stream, source);
                return stream;
            }

            public static T DeserializeBinary<T>(Stream stream)
            {
                IFormatter formatter = new BinaryFormatter();
                stream.Position = 0L;
                return (T)formatter.Deserialize(stream);
            }

            public static T CloneBinary<T>(object source)
            {
                return DeserializeBinary<T>(SerializeBinary(source));
            }

            public static Stream SerializeDataContract(object source)
            {
                Stream stream = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(source.GetType());
                serializer.WriteObject(stream, source);
                return stream;
            }

            public static T DeserializeDataContract<T>(Stream stream)
            {
                stream.Position = 0L;
                using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    return (T)serializer.ReadObject(reader, true);
                }
            }

            public static T CloneDataContract<T>(object source)
            {
                return DeserializeDataContract<T>(SerializeDataContract(source));
            }

            public static Stream SerializeXml(object source)
            {
                Stream stream = new MemoryStream();
                XmlSerializer xmlSerializer = new XmlSerializer(source.GetType());
                xmlSerializer.Serialize(stream, source);
                return stream;
            }

            public static T DeserializeXml<T>(Stream stream)
            {
                stream.Position = 0L;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(stream);
            }

            private static T CloneXml<T>(object source)
            {
                return DeserializeXml<T>(SerializeXml(source));
            }

            public static string StreamToString(Stream stream)
            {
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
