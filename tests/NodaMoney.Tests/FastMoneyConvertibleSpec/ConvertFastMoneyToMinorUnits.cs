using NodaMoney.Context;

namespace NodaMoney.Tests.FastMoneyConvertibleSpec;

public class ConvertFastMoneyToMinorUnits
{
    [Fact]
    public void WhenConvertingEuroToMinorUnits_ThenItShouldBeCents()
    {
        var money = new FastMoney(765.43m, "EUR");

        money.ToMinorUnits().Should().Be(76543);
    }

    [Fact]
    public void WhenConvertingYenToMinorUnits_ThenItShouldBeYen()
    {
        var money = new FastMoney(765m, "JPY");

        money.ToMinorUnits().Should().Be(765);
    }

    [Fact]
    public void WhenConvertingMruToMinorUnits_ThenItShouldBeKhoums()
    {
        var money = new FastMoney(1.2m, "MRU"); // 1 MRU = 5 Khoums

        money.ToMinorUnits().Should().Be(6);
    }

    [Fact]
    public void WhenConvertingWithRounding_ThenItShouldUseContextRoundingStrategy()
    {
        var eur = Currency.FromCode("EUR");

        // MidpointRounding.ToEven (Default)
        var moneyToEven = new FastMoney(765.425m, eur); // 765.425 -> 765.42
        moneyToEven.ToMinorUnits().Should().Be(76542);

        // MidpointRounding.AwayFromZero
        var awayFromZeroContext = MoneyContext.Create(new MoneyContextOptions
        {
            RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero),
            Precision = 19,
            MaxScale = 4
        });

        var moneyAwayFromZero = new FastMoney(765.425m, eur, awayFromZeroContext); // 765.425 -> 765.43
        moneyAwayFromZero.ToMinorUnits().Should().Be(76543);
    }
}
