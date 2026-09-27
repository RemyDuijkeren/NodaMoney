using System.Collections.Generic;
using System.Linq;
using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.FastMoneyConvertibleSpec;

/// <summary>The integer conversion paths between <see cref="FastMoney"/> and <see cref="Money"/> must produce exactly
/// what the decimal round trip produces: same amount bits (value and scale), same currency, same context.</summary>
public class ConvertBetweenFastMoneyAndMoney
{
    static readonly string[] Currencies = ["EUR", "JPY", "BHD", "CLF", "MGA", "XXX"];

    static readonly long[] EdgeTicks =
    [
        0, 1, -1, 5, -5, 50, 500, 5000, 15000, 25000, 123400, 120000, 123456, -123456, 123450, 123550, 999950, -999950,
        9_999_999_999_999_999L, -9_999_999_999_999_999L, long.MaxValue, long.MaxValue - 1, long.MinValue, long.MinValue + 1,
        long.MinValue / 10000 * 10000, long.MaxValue / 10000 * 10000
    ];

    static IEnumerable<long> SampleTicks()
    {
        foreach (long ticks in EdgeTicks)
            yield return ticks;

        var random = new Random(20260927);
        for (int i = 0; i < 2000; i++)
        {
            // Mix small values (where trailing zeros and midpoints matter) with the full range.
            long ticks = (i % 3) switch
            {
                0 => random.Next(-200_000, 200_000),
                1 => NextLong(random) % 1_000_000_000_000L,
                _ => NextLong(random)
            };
            yield return ticks;
        }
    }

    // Random.NextInt64 does not exist on net48; two 32-bit draws cover the full long range.
    static long NextLong(Random random) => ((long)random.Next() << 32 | (uint)random.Next()) * (random.Next(2) == 0 ? 1 : -1);

    static IEnumerable<MoneyContext> MoneyContexts()
    {
        yield return MoneyContext.Create(o => o.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven));
        yield return MoneyContext.Create(o => o.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero));
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        yield return MoneyContext.Create(o => o.RoundingStrategy = new StandardRounding(MidpointRounding.ToNegativeInfinity));
#endif
        yield return MoneyContext.Create(o => o.RoundingStrategy = new NoRounding());
        yield return MoneyContext.Create(o => { o.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven); o.MaxScale = 0; });
        yield return MoneyContext.Create(o => { o.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven); o.MaxScale = 3; });
        yield return MoneyContext.Create(o => { o.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven); o.MaxScale = 6; });
    }

    static void ShouldBeSameMoney(Money actual, Money expected, string because)
    {
        actual.Currency.Should().Be(expected.Currency, because);
        actual.Context.Should().Be(expected.Context, because);
        decimal.GetBits(actual.Amount).Should().Equal(decimal.GetBits(expected.Amount), because);
    }

    static void ShouldBeSameFastMoney(FastMoney actual, FastMoney expected, string because)
    {
        actual.Currency.Should().Be(expected.Currency, because);
        actual.Context.Should().Be(expected.Context, because);
        decimal.GetBits(actual.Amount).Should().Equal(decimal.GetBits(expected.Amount), because);
    }

    [Fact]
    public void WhenConvertingFastMoneyToMoney_ThenResultMatchesTheDecimalPathForEveryContextAndCurrency()
    {
        foreach (MoneyContext context in MoneyContexts())
        {
            using var scope = MoneyContext.CreateScope(context);
            foreach (string code in Currencies)
            {
                Currency currency = CurrencyInfo.FromCode(code);
                foreach (long ticks in SampleTicks())
                {
                    var fast = FastMoney.FromOACurrency(ticks, currency);
                    var expected = new Money(decimal.FromOACurrency(ticks), currency, context);

                    Money actual = fast.ToMoney();

                    ShouldBeSameMoney(actual, expected, $"ticks {ticks} in {code} under {context.RoundingStrategy} max scale {context.MaxScale}");
                }
            }
        }
    }

    [Fact]
    public void WhenConvertingFastMoneyToMoneyUnderCustomRounding_ThenTheStrategyIsApplied()
    {
        var context = MoneyContext.Create(o => o.RoundingStrategy = new RecordingRoundingStrategy(12.3m));
        var fast = new FastMoney(12.3456m, "EUR");

        using var scope = MoneyContext.CreateScope(context);
        Money actual = fast.ToMoney();

        ShouldBeSameMoney(actual, new Money(12.3456m, "EUR", context), "the custom strategy decides the result");
        actual.Amount.Should().Be(12.3m);
    }

    [Fact]
    public void WhenConvertingFastMoneyToMoney_ThenTheAmbientContextIsUsedNotTheFastMoneyContext()
    {
        var fast = new FastMoney(12.3456m, "EUR"); // FastMoney context allows scale 4

        Money actual = fast.ToMoney();

        actual.Context.Should().Be(MoneyContext.CurrentContext);
        actual.Amount.Should().Be(12.35m); // rounded to the currency's two decimals by the ambient context
    }

    [Fact]
    public void WhenConvertingMoneyToFastMoney_ThenResultMatchesTheDecimalPath()
    {
        decimal[] amounts =
        [
            0m, 1m, -1m, 0.5m, 0.00005m, 0.00015m, 0.00025m, -0.00025m, 12.3m, 12.34m, 12.345m, 12.3456m, 12.34565m, 12.34575m,
            -12.34565m, 123456789.1234m, 1234567890123.4567m, 0.000000001m, 0.123456789012m, 922337203685477.5807m, -922337203685477.5807m,
            99999999999999.99999m
        ];
        var noRounding = MoneyContext.Create(o => o.RoundingStrategy = new NoRounding());

        foreach (string code in new[] { "EUR", "JPY", "BHD", "XXX" })
        {
            Currency currency = CurrencyInfo.FromCode(code);
            foreach (decimal amount in amounts)
            {
                var money = new Money(amount, currency, noRounding); // keep every scale intact
                var expected = new FastMoney(money.Amount, money.Currency);

                var actual = new FastMoney(money);

                ShouldBeSameFastMoney(actual, expected, $"amount {amount} in {code}");
            }
        }
    }

    [Theory]
    [InlineData("1000000000000000")] // fits 64 bits, but times 10^4 overflows the tick range
    [InlineData("79228162514264337593543950335")] // decimal.MaxValue, mantissa needs 96 bits
    [InlineData("-79228162514264337593543950335")]
    [InlineData("922337203685477.5808")] // one tick past long.MaxValue
    [InlineData("922337203685477.58075")] // rounds to even onto one tick past long.MaxValue
    public void WhenConvertingMoneyOutsideTheFastMoneyRange_ThenThrowArgumentOutOfRangeException(string amount)
    {
        var noRounding = MoneyContext.Create(o => o.RoundingStrategy = new NoRounding());
        var money = new Money(decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture), "EUR", noRounding);

        Action action = () => _ = new FastMoney(money);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void WhenConvertingMoneyWithUnsupportedCurrencyToFastMoney_ThenThrowInvalidCurrencyException()
    {
        var currencyInfo = CurrencyInfo.FromCode("EUR") with { Code = "ZZV", MinorUnit = MinorUnit.Five };
        CurrencyInfo.Register(currencyInfo);
        try
        {
            var money = new Money(1.23456m, currencyInfo);

            Action action = () => _ = new FastMoney(money);

            action.Should().Throw<InvalidCurrencyException>();
        }
        finally
        {
            CurrencyInfo.Unregister(currencyInfo.Code);
        }
    }
}
