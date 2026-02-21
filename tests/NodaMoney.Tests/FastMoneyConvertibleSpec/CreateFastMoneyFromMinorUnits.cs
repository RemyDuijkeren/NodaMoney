namespace NodaMoney.Tests.FastMoneyConvertibleSpec;

public class CreateFastMoneyFromMinorUnits
{
    [Fact]
    public void WhenCreatingEuroFromMinorUnits_ThenItShouldBeCorrectAmount()
    {
        var money = FastMoney.FromMinorUnits(76543, Currency.FromCode("EUR"));

        money.Amount.Should().Be(765.43m);
        money.Currency.Code.Should().Be("EUR");
    }

    [Fact]
    public void WhenCreatingYenFromMinorUnits_ThenItShouldBeCorrectAmount()
    {
        var money = FastMoney.FromMinorUnits(765, Currency.FromCode("JPY"));

        money.Amount.Should().Be(765m);
        money.Currency.Code.Should().Be("JPY");
    }

    [Fact]
    public void WhenCreatingMruFromMinorUnits_ThenItShouldBeCorrectAmount()
    {
        var money = FastMoney.FromMinorUnits(6, Currency.FromCode("MRU")); // 6 Khoums = 1.2 MRU

        money.Amount.Should().Be(1.2m);
        money.Currency.Code.Should().Be("MRU");
    }
}
