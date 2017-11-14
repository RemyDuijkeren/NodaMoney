using System.Collections.Generic;
using System.Globalization;
using System.Web.Script.Serialization;
using FluentAssertions;
using Newtonsoft.Json;
using NodaMoney.Serialization.JsonNet;
using Xunit;

namespace NodaMoney.Serialization.AspNet.Tests
{
    public static class MoneySerializableTests
    {
        public class GivenIWantToSerializeMoneyWithJavaScriptSerializer
        {
            public static IEnumerable<object[]> TestData => new[]
            {
                new object[] { new Money(765m, Currency.FromCode("JPY")) },
                new object[] { new Money(765.43m, Currency.FromCode("EUR")) }
            };

            [Theory]
            [MemberData("TestData")]
            public void WhenSerializing_ThenThisShouldSucceed(Money money)
            {
                var jsSerializer = new JavaScriptSerializer();
                jsSerializer.RegisterConverters(new JavaScriptConverter[] { new MoneyJavaScriptConverter() });

                var json = jsSerializer.Serialize(money);
                // Console.WriteLine(json);
                var clone = jsSerializer.Deserialize<Money>(json);

                clone.Should().Be(money);
            }
        }

        public class GivenIWantToSerializeMoneyWithJavaScriptConverter
        {
            public static IEnumerable<object[]> TestData => new[]
            {
                new object[] { new Money(765m, Currency.FromCode("JPY")) },
                new object[] { new Money(765.43m, Currency.FromCode("EUR")) }
            };

            [Theory]
            [MemberData("TestData")]
            public void WhenSerializing_ThenThisShouldSucceed(Money money)
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new MoneyJsonConverter() }
                };

                var json = JsonConvert.SerializeObject(money);
                // Console.WriteLine(json);
                var clone = JsonConvert.DeserializeObject<Money>(json);

                clone.Should().Be(money);
            }
        }

        public class GivenIWantToDeserializeMoneyWithJavaScriptSerializer
        {
            private static string CurrentCultureCode = new RegionInfo(CultureInfo.CurrentCulture.LCID).ISOCurrencySymbol;

            public static IEnumerable<object[]> TestData => new[]
            {
                new object[] { $"{{ amount: '200', currency: '{CurrentCultureCode}' }}", },
                new object[] { $"{{ amount: 200, currency: '{CurrentCultureCode}' }}" },
                new object[] { $"{{ currency: '{CurrentCultureCode}', amount: 200 }}" },
                new object[] { "{ amount: '200' }" },
                new object[] { "{ amount: 200 }" }
            };

            [Theory]
            [MemberData("TestData")]
            public void WhenDeserializing_ThenThisShouldSucceed(string json)
            {
                var money = new Money(200, Currency.FromCode(CurrentCultureCode));

                var jsSerializer = new JavaScriptSerializer();
                jsSerializer.RegisterConverters(new JavaScriptConverter[] { new MoneyJavaScriptConverter() });

                // Console.WriteLine(json);
                var clone = jsSerializer.Deserialize<Money>(json);

                clone.Should().Be(money);
            }

            public class GivenIWantToDeserializeMoneyWithJavaScriptConverter
            {
                private static string CurrentCultureCode = new RegionInfo(CultureInfo.CurrentCulture.LCID).ISOCurrencySymbol;

                public static IEnumerable<object[]> TestData => new[]
                {
                    new object[] { $"{{ amount: '200', currency: '{CurrentCultureCode}' }}", },
                    new object[] { $"{{ amount: 200, currency: '{CurrentCultureCode}' }}" },
                    new object[] { $"{{ currency: '{CurrentCultureCode}', amount: 200 }}" },
                    new object[] { "{ amount: '200' }" },
                    new object[] { "{ amount: 200 }" }
                };

                [Theory]
                [MemberData("TestData")]
                public void WhenDeserializing_ThenThisShouldSucceed(string json)
                {
                    var money = new Money(200, Currency.FromCode(CurrentCultureCode));

                    JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new MoneyJsonConverter() }
                    };

                    // Console.WriteLine(json);
                    var clone = JsonConvert.DeserializeObject<Money>(json);

                    clone.Should().Be(money);
                }
            }
        }
    }
}