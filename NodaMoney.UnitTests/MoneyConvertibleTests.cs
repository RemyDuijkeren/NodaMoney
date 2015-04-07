using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodaMoney.UnitTests
{
    static internal class MoneyConvertibleTests
    {
        [TestClass]
        public class GivenIWantToConvertMoney
        {
            readonly Money _euros = new Money(765.43m, "EUR");

            [TestMethod]
            public void WhenConvertingToDecimal_ThenThisShouldSucceed()
            {
                var result = Money.ToDecimal(_euros);

                result.Should().Be(765.43m);                
            }

            [TestMethod]
            public void WhenConvertingToDouble_ThenThisShouldSucceed()
            {
                var result = Money.ToDouble(_euros);

                result.Should().BeApproximately(765.43d, 0.001d);
            }

            [TestMethod]
            public void WhenConvertingToSingle_ThenThisShouldSucceed()
            {
                var result = Money.ToSingle(_euros);

                result.Should().BeApproximately(765.43f, 0.001f);
            }
        }
    }
}