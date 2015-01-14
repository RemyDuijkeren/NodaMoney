using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodaMoney.UnitTests
{
    static internal class MoneyFourMostUsedCurrenciesTests
    {
        [TestClass]
        public class GivenIWantMoneyInOneOfTheFourMostUsedCurrenciesInTheWorld
        {
            [TestMethod]
            public void WhenEurosIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal
                var euros = Money.Euro(10.00m);

                euros.Currency.Should().Be(Currency.FromCode("EUR"));
                euros.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenEurosIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                var euros = Money.Euro(10.00);

                euros.Currency.Should().Be(Currency.FromCode("EUR"));
                euros.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenEurosIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                var euros = Money.Euro(10L);

                euros.Currency.Should().Be(Currency.FromCode("EUR"));
                euros.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenDollarsIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal (other integral types are implicitly converted to decimal)
                var dollars = Money.USDollar(10.00m);

                dollars.Currency.Should().Be(Currency.FromCode("USD"));
                dollars.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenDollarsIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                var dollars = Money.USDollar(10.00);

                dollars.Currency.Should().Be(Currency.FromCode("USD"));
                dollars.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenDollarsIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                var dollars = Money.USDollar(10L);

                dollars.Currency.Should().Be(Currency.FromCode("USD"));
                dollars.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenYensIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal (other integral types are implicitly converted to decimal)
                var yens = Money.Yen(10.00m);

                yens.Should().NotBeNull();
                yens.Currency.Should().Be(Currency.FromCode("JPY"));
                yens.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenYensIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                var yens = Money.Yen(10.00);

                yens.Should().NotBeNull();
                yens.Currency.Should().Be(Currency.FromCode("JPY"));
                yens.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenYensIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                var yens = Money.Yen(10L);

                yens.Should().NotBeNull();
                yens.Currency.Should().Be(Currency.FromCode("JPY"));
                yens.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenPondsIsDecimal_ThenCreatingShouldSucceed()
            {
                //from decimal (other integral types are implicitly converted to decimal)
                var pounds = Money.PoundSterling(10.00m);

                pounds.Should().NotBeNull();
                pounds.Currency.Should().Be(Currency.FromCode("GBP"));
                pounds.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenPondsIsDouble_ThenCreatingShouldSucceed()
            {
                //from double (float is implicitly converted to double)
                var pounds = Money.PoundSterling(10.00);

                pounds.Should().NotBeNull();
                pounds.Currency.Should().Be(Currency.FromCode("GBP"));
                pounds.Amount.Should().Be(10.00m);
            }

            [TestMethod]
            public void WhenPondsIsLong_ThenCreatingShouldSucceed()
            {
                //from long (byte, short and int are implicitly converted to long)
                var pounds = Money.PoundSterling(10L);

                pounds.Should().NotBeNull();
                pounds.Currency.Should().Be(Currency.FromCode("GBP"));
                pounds.Amount.Should().Be(10.00m);
            }
        }
    }
}