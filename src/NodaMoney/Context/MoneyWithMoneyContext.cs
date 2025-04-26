namespace NodaMoney.Context;

internal readonly struct MoneyWithMoneyContext
{
    internal PackedDecimal AmountWithIndex { get; }
    public decimal Amount => AmountWithIndex.Decimal;
    public CurrencyInfo Currency { get; }

    public MoneyWithMoneyContext(decimal amount, CurrencyInfo currency, MoneyContext? context = null)
    {
        // Use either provided context or current (global/thread-local)
        var effectiveContext = context ?? MoneyContext.CurrentContext;

        // Store its index
        AmountWithIndex = new PackedDecimal(amount, index: effectiveContext.Index);
        Currency = currency;
    }

    // Access the associated MoneyContext
    public MoneyContext Context => MoneyContext.Get(AmountWithIndex.Index);

    // Round based on the context rules
    public decimal Round()
    {
        // Use the rounding strategy from the context
        var roundedValue = Context.RoundingStrategy.Round(Amount, Currency, Context.MaxScale);

        // Validate precision as well (no value can exceed the Context's Precision)
        if (roundedValue.ToString().Replace(".", "").Length > Context.Precision)
        {
            throw new InvalidOperationException($"Rounded value exceeds precision defined by the context ({Context.Precision}).");
        }

        return roundedValue;
    }

    public override string ToString()
    {
        return $"{Currency.Symbol}{Amount}";
    }

    // Validate and adjust precision and scale based on this context's rules
    public MoneyWithMoneyContext ApplyPrecisionAndScale()
    {
        var maxPrecision = Context.Precision;
        var maxScale = Context.MaxScale;

        if (decimal.GetBits(Amount)[3] >> 16 > maxScale) // Check scale
        {
            throw new InvalidOperationException($"Amount exceeds allowed scale of {maxScale}.");
        }

        if (Amount.ToString().Replace(".", "").Length > maxPrecision) // Check precision
        {
            throw new InvalidOperationException($"Amount exceeds allowed precision of {maxPrecision}.");
        }

        return this;
    }

    // **MonetaryOperator/Adjuster**:
    // Just like JavaMoney uses `MonetaryOperator` for adjustments, allow custom operations to be applied to `Money`. For example, an operator to scale or convert currencies.
    public MoneyWithMoneyContext Apply(Func<decimal, decimal> adjustment)
    {
        return new MoneyWithMoneyContext(adjustment(Amount), Currency, Context);
    }

    // **Queryable Money**:
    // Allow querying context or metadata about `Money` objects, e.g.,:
    public bool IsWithinPrecisionScale()
    {
        return Amount.ToString().Replace(".", "").Length <= Context.Precision &&
               decimal.GetBits(Amount)[3] >> 16 <= Context.MaxScale;
    }

    public void MethodX()
    {
        using var context = MoneyContext.CreateScope(MoneyContext.CreateAccounting());

        // TODO: ...

        // or
        MoneyContext.ThreadContext = MoneyContext.CreateRetail();
    }
}
