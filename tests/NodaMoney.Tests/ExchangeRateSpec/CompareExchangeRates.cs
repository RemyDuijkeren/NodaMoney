using System.Collections.Generic;
using NodaMoney.Exchange;

namespace NodaMoney.Tests.ExchangeRateSpec;

public class CompareExchangeRates
{
    public static IEnumerable<object[]> TestData => new[]
    {
        new object[] { new ExchangeRate("EUR", "USD", 1.2591), new ExchangeRate("EUR", "USD", 1.2591), true },
        new object[] { new ExchangeRate("EUR", "USD", 0.0), new ExchangeRate("EUR", "USD", 0.0), true },
        new object[] { new ExchangeRate("EUR", "USD", 1.2591), new ExchangeRate("EUR", "USD", 1.600), false },
        new object[] { new ExchangeRate("EUR", "USD", 1.2591), new ExchangeRate("EUR", "AFN", 1.2591), false },
        new object[] { new ExchangeRate("AFN", "USD", 1.2591), new ExchangeRate("EUR", "USD", 1.2591), false }
    };

    [Theory][MemberData(nameof(TestData))]
    public void WhenTheAreEqual_ThenComparingShouldBeTrueOtherwiseFalse(ExchangeRate fx1, ExchangeRate fx2, bool areEqual)
    {
        if (areEqual)
            fx1.Should().Be(fx2);
        else
            fx1.Should().NotBe(fx2);

        if (areEqual)
            fx1.GetHashCode().Should().Be(fx2.GetHashCode()); //using GetHashCode()
        else
            fx1.GetHashCode().Should().NotBe(fx2.GetHashCode()); //using GetHashCode()

        fx1.Equals(fx2).Should().Be(areEqual); //using Equal()
        ExchangeRate.Equals(fx1, fx2).Should().Be(areEqual); //using static Equals()
        (fx1 == fx2).Should().Be(areEqual); //using Equality operators
        (fx1 != fx2).Should().Be(!areEqual); //using Equality operators
    }
}
