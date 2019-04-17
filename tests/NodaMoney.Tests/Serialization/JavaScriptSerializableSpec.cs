using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Script.Serialization;
using FluentAssertions;
using Xunit;

namespace NodaMoney.Serialization.AspNet.Tests
{
    public static class JavaScriptSerializableSpec
    {
        public class GivenIWantToSerializeMoneyWithJavaScriptSerializer
        {
            public static IEnumerable<object[]> TestData => new[]
            {
                new object[] { new Money(765m, Currency.FromCode("JPY")) },
                new object[] { new Money(765.43m, Currency.FromCode("EUR")) }
            };

            [Theory]
            [MemberData(nameof(TestData))]
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

        public class GivenIWantToDeserializeMoneyWithJavaScriptSerializer
        {
            private static string CurrentCultureCode = new RegionInfo(CultureInfo.CurrentCulture.LCID).ISOCurrencySymbol;

            public static IEnumerable<object[]> ValidJsonData => new[]
            {
                new object[] { $"{{ amount: '200', currency: '{CurrentCultureCode}' }}", },
                new object[] { $"{{ amount: 200, currency: '{CurrentCultureCode}' }}" },
                new object[] { $"{{ currency: '{CurrentCultureCode}', amount: 200 }}" },
                new object[] { $"{{ currency: '{CurrentCultureCode}', amount: '200' }}" }
            };

            public static IEnumerable<object[]> InvalidJsonData => new[]
            {
                new object[] { "{ amount: '200' }" },
                new object[] { "{ amount: 200 }" },
                new object[] { $"{{ currency: '{CurrentCultureCode}' }}" },
            };

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
            [MemberData(nameof(ValidJsonData))]
            public void WhenDeserializingWithValidJSON_ThenThisShouldSucceed(string json)
            {
                var money = new Money(200, Currency.FromCode(CurrentCultureCode));

                var jsSerializer = new JavaScriptSerializer();
                jsSerializer.RegisterConverters(new JavaScriptConverter[] { new MoneyJavaScriptConverter() });

                // Console.WriteLine(json);
                var clone = jsSerializer.Deserialize<Money>(json);

                clone.Should().Be(money);
            }

            [Theory]
            [MemberData(nameof(InvalidJsonData))]
            public void WhenDeserializingWithInvalidJSON_ThenThisShouldFail(string json)
            {
                var money = new Money(200, Currency.FromCode(CurrentCultureCode));

                var jsSerializer = new JavaScriptSerializer();
                jsSerializer.RegisterConverters(new JavaScriptConverter[] { new MoneyJavaScriptConverter() });

                var exception = Record.Exception(() =>
                    jsSerializer.Deserialize<Money>(json)
                );

                exception.Should().BeOfType<ArgumentNullException>();
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

                var jsSerializer = new JavaScriptSerializer();
                jsSerializer.RegisterConverters(new JavaScriptConverter[] { new MoneyJavaScriptConverter() });

                // Console.WriteLine(json);
                var clone = jsSerializer.Deserialize<TypeWithMoneyProperty>(json);

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

                var jsSerializer = new JavaScriptSerializer();
                jsSerializer.RegisterConverters(new JavaScriptConverter[] { new MoneyJavaScriptConverter() });

                // Console.WriteLine(json);
                var clone = jsSerializer.Deserialize<TypeWithNullableMoneyProperty>(json);

                if (!json.Contains("null"))
                    clone.Cash.Should().Be(money);
                else
                    clone.Cash.Should().BeNull();
            }
        }
    }
}
