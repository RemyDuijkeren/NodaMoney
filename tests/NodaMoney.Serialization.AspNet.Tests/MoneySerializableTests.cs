using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using FluentAssertions;
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
                Console.WriteLine(json);
                var clone = jsSerializer.Deserialize<Money>(json);

                clone.Should().Be(money);
            }
        }
    }
}
