namespace NodaMoney.Context;

public class MoneyContextOptions
{
    public IRoundingStrategy RoundingStrategy { get; init; } = new StandardRounding();
    public int Precision { get; init; } = 28;
    public int? MaxScale { get; init; }
    public CurrencyInfo? DefaultCurrency { get; init; }
}
