namespace NodaMoney.Context;

public class MoneyContextOptions
{
    public IRoundingStrategy RoundingStrategy { get; init; } = new StandardRounding();
    public int Precision { get; init; }
    public int? MaxScale { get; init; } = 28;
    public CurrencyInfo? DefaultCurrency { get; init; }
}
