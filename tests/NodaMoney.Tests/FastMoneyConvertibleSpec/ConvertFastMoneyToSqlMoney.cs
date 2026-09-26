using System.Data.SqlTypes;

namespace NodaMoney.Tests.FastMoneyConvertibleSpec;

public class ConvertFastMoneyToSqlMoney
{
    static readonly Currency Eur = CurrencyInfo.FromCode("EUR");

    [Fact]
    public void WhenConvertingEuroToSqlMoney_ThenValueShouldMatch()
    {
        var money = new FastMoney(765.43m, Eur);

        var sqlMoney = money.ToSqlMoney();

        sqlMoney.Should().Be(new SqlMoney(765.43m));
    }

    [Fact]
    public void WhenConvertingSqlMoneyBackToFastMoney_ThenAmountShouldMatch()
    {
        var sqlMoney = new SqlMoney(765.43m);

        var money = FastMoney.FromSqlMoney(sqlMoney, Eur);

        money.Should().Be(new FastMoney(765.43m, Eur));
    }

    [Fact]
    public void WhenConvertingSqlMoneyNullToFastMoney_ThenResultShouldBeNull()
    {
        var money = FastMoney.FromSqlMoney(SqlMoney.Null, Eur);

        money.Should().BeNull();
    }

    [Fact]
    public void WhenConvertingNegativeEuroToSqlMoneyAndBack_ThenValueShouldRoundTrip()
    {
        var money = new FastMoney(-765.43m, Eur);

        var sqlMoney = money.ToSqlMoney();
        var roundTripped = FastMoney.FromSqlMoney(sqlMoney, Eur);

        sqlMoney.Should().Be(new SqlMoney(-765.43m));
        roundTripped.Should().Be(money);
    }

    [Fact]
    public void WhenConvertingTickLevelMaximumToSqlMoneyAndBack_ThenTicksShouldRoundTrip()
    {
        var money = FastMoney.FromOACurrency(long.MaxValue, Eur);

        var sqlMoney = money.ToSqlMoney();
        var roundTripped = FastMoney.FromSqlMoney(sqlMoney, Eur);

        roundTripped.Should().Be(money);
        roundTripped!.Value.ToOACurrency().Should().Be(long.MaxValue);
    }

    [Fact]
    public void WhenConvertingTickLevelMinimumToSqlMoneyAndBack_ThenTicksShouldRoundTrip()
    {
        var money = FastMoney.FromOACurrency(long.MinValue, Eur);

        var sqlMoney = money.ToSqlMoney();
        var roundTripped = FastMoney.FromSqlMoney(sqlMoney, Eur);

        roundTripped.Should().Be(money);
        roundTripped!.Value.ToOACurrency().Should().Be(long.MinValue);
    }
}
