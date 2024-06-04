namespace NodaMoney;

// store currency list in 2 bits (0-3)
public enum CurrencyList : byte
{
    Iso4217 = 0,
    Iso4217Historic = 1,
    Other = 2
}
