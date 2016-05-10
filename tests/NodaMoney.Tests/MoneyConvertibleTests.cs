using FluentAssertions;
using Xunit;

namespace NodaMoney.Tests
{
    public class MoneyConvertibleTests
    {
        public class GivenIWantToConvertMoney
        {
            readonly Money _euros = new Money(765.43m, "EUR");

            [Fact]
            public void WhenConvertingToDecimal_ThenThisShouldSucceed()
            {
                var result = Money.ToDecimal(_euros);

                result.Should().Be(765.43m);                
            }

            [Fact]
            public void WhenConvertingToDouble_ThenThisShouldSucceed()
            {
                var result = Money.ToDouble(_euros);

                result.Should().BeApproximately(765.43d, 0.001d);
            }

            [Fact]
            public void WhenConvertingToSingle_ThenThisShouldSucceed()
            {
                var result = Money.ToSingle(_euros);

                result.Should().BeApproximately(765.43f, 0.001f);
            }
        }
    }
}