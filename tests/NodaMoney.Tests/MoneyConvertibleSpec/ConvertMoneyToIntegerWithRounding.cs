using NodaMoney.Context;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.MoneyConvertibleSpec;

/// <summary>Covers U6: <see cref="Money.ToInt32()"/> and <see cref="Money.ToInt64()"/> round on the integer mantissa
/// when it fits in 64 bits, producing the same result as the decimal path for every <see cref="MidpointRounding"/>
/// mode.</summary>
[Collection(nameof(NoParallelization))]
public class ConvertMoneyToIntegerWithRounding
{
    [Fact]
    public void ToInt32_NonTieAmount_ReturnsTruncatedRoundedValue()
    {
        Money money = new(765.43m, "EUR");

        money.ToInt32().Should().Be(765);
    }

    [Fact]
    public void ToInt32_TieUnderToEven_RoundsToNearestEven()
    {
        Money moneyRoundsUp = new(765.50m, "EUR");
        Money moneyRoundsDown = new(764.50m, "EUR");

        moneyRoundsUp.ToInt32().Should().Be(766);
        moneyRoundsDown.ToInt32().Should().Be(764);
    }

    [Fact]
    public void ToInt32_NegativeTie_DiffersBetweenToEvenAndAwayFromZero()
    {
        MoneyContext toEven = MoneyContext.Create(options => options.RoundingStrategy = new StandardRounding(MidpointRounding.ToEven));
        MoneyContext awayFromZero = MoneyContext.Create(options => options.RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero));
        Money moneyToEven = new Money(-2.5m, "EUR", MoneyContext.NoRounding) with { Context = toEven };
        Money moneyAwayFromZero = new Money(-2.5m, "EUR", MoneyContext.NoRounding) with { Context = awayFromZero };

        moneyToEven.ToInt32().Should().Be(-2);
        moneyAwayFromZero.ToInt32().Should().Be(-3);
    }

#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
    [Theory]
    [InlineData(2.5, MidpointRounding.ToZero, 2)]
    [InlineData(2.5, MidpointRounding.ToNegativeInfinity, 2)]
    [InlineData(2.5, MidpointRounding.ToPositiveInfinity, 3)]
    [InlineData(-2.5, MidpointRounding.ToZero, -2)]
    [InlineData(-2.5, MidpointRounding.ToNegativeInfinity, -3)]
    [InlineData(-2.5, MidpointRounding.ToPositiveInfinity, -2)]
    public void ToInt32_TieUnderDirectionalMode_RoundsTowardTheNamedDirection(double amount, MidpointRounding mode, int expected)
    {
        MoneyContext context = MoneyContext.Create(options => options.RoundingStrategy = new StandardRounding(mode));
        Money money = new Money((decimal)amount, "EUR", MoneyContext.NoRounding) with { Context = context };

        money.ToInt32().Should().Be(expected);
    }
#endif

    [Fact]
    public void ToInt32_AboveIntMaxValue_ThrowsOverflowException()
    {
        Money money = new(3_000_000_000m, "EUR");

        Action act = () => money.ToInt32();

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void ToInt64_AboveIntMaxValue_Succeeds()
    {
        Money money = new(3_000_000_000m, "EUR");

        money.ToInt64().Should().Be(3_000_000_000L);
    }

    [Fact]
    public void ToInt64_MantissaAboveSixtyFourBitsUnderNoRounding_MatchesDecimalPath()
    {
        // 24 significant digits at scale 1; the 96-bit mantissa cannot fit in 64 bits (_high is nonzero).
        Money money = new(12345678901234567890123.5m, "EUR", MoneyContext.NoRounding);

        Action act = () => money.ToInt64();

        act.Should().Throw<OverflowException>(because: "the decimal path also overflows a long for this magnitude");
    }

    [Fact]
    public void ToInt64_ScaleAboveEighteenUnderNoRounding_MatchesDecimalPath()
    {
        Money money = new(0.1234567890123456789m, "EUR", MoneyContext.NoRounding); // scale 19

        money.ToInt64().Should().Be(0L);
    }

    [Fact]
    public void ToInt32_CustomRoundingStrategy_IsCalledWithZeroDecimals()
    {
        var strategy = new RecordingRoundingStrategy(7m);
        MoneyContext customContext = MoneyContext.Create(options => options.RoundingStrategy = strategy);
        Money money = new Money(10.6m, "EUR") with { Context = customContext };

        int result = money.ToInt32();

        strategy.CallCount.Should().Be(1);
        strategy.LastDecimals.Should().Be(0);
        result.Should().Be(7);
    }

    [Fact]
    public void ToInt32_MgaOneFifthMinorUnit_MatchesPinnedPreChangeResult()
    {
        // MGA (MinorUnit.OneFifth) is excluded from the fast path (KD3); it must keep taking the decimal path.
        // Expected value pinned by running this test against the pre-change code.
        Money money = new(10.3m, "MGA");

        money.ToInt32().Should().Be(10);
    }

    public static TheoryData<decimal, MidpointRounding> FastPathMatchesDecimalPathData()
    {
        decimal[] amounts =
        [
            0m,
            100m,
            -100m,
            765.43m,
            765.5m,
            764.50m,
            -2.5m,
            3.5m,
            -3.5m,
            12.345m,
            -12.345m,
            1.23455m,
            100000.123456m,
            -100000.123456m,
            0.500000m,
            -0.500000m
        ];

        MidpointRounding[] modes =
        [
            MidpointRounding.ToEven,
            MidpointRounding.AwayFromZero,
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            MidpointRounding.ToZero,
            MidpointRounding.ToNegativeInfinity,
            MidpointRounding.ToPositiveInfinity
#endif
        ];

        var data = new TheoryData<decimal, MidpointRounding>();
        foreach (decimal amount in amounts)
        foreach (MidpointRounding mode in modes)
            data.Add(amount, mode);

        return data;
    }

    [Theory]
    [MemberData(nameof(FastPathMatchesDecimalPathData))]
    public void ToInt32AndToInt64_FastPath_MatchesDecimalRoundReference(decimal amount, MidpointRounding mode)
    {
        MoneyContext context = MoneyContext.Create(options => options.RoundingStrategy = new StandardRounding(mode));
        Money money = new Money(amount, "EUR", MoneyContext.NoRounding) with { Context = context };

        decimal reference = decimal.Round(amount, 0, mode);

        money.ToInt64().Should().Be((long)reference);
        money.ToInt32().Should().Be((int)reference);
    }
}
