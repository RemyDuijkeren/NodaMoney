using System.Runtime.CompilerServices;

namespace NodaMoney.Context;

internal record CashDenominationRounding : IRoundingStrategy
{
    public CashDenominationRounding(decimal decimals)
    {
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal Round(decimal amount, CurrencyInfo currencyInfo, int? decimals) => throw new NotImplementedException();
}
