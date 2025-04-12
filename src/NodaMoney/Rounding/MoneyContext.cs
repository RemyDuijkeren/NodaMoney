namespace NodaMoney.Rounding;

// MonetaryContext (or RoundingContext), MonetaryConfiguration (or MoneyRules, MonetaryRules)
internal record MoneyContext
{
    // Static dictionary to hold all active contexts and enforce deduplication
    private static readonly Dictionary<byte, MoneyContext> s_activeContexts = new();
    private static readonly AsyncLocal<byte?> s_threadLocalContext = new(); // Thread-local index
    private static byte s_lastIndex = 0;

    // Default global MoneyContext (fallback context)
    private static MoneyContext s_defaultGlobalContext = new(new DefaultRounding());

    // Configuration properties (set via private constructor)
    public IRoundingStrategy RoundingStrategy { get; }
    // public IRoundingStrategy StandardRoundingStrategy { get; }
    // public IRoundingStrategy CashRoundingStrategy { get; }
    public int Precision { get; }
    public int MaxScale { get; }

    // attach metadata to rounding operations. This could include:
    // - Details like maximum/minimum allowed scale.
    // - Reasons for rounding (e.g., legal rules, financial calculations).
    // - Region- or jurisdiction-specific adjustments.
    public Dictionary<string, string> Attributes { get; }
    public bool CashRounding { get; }
    // or
    // Access metadata for the context
    public MetadataProvider Metadata { get; } = new();

    // Private constructor to prevent direct instantiation
    private MoneyContext(IRoundingStrategy roundingStrategy, int precision = 28, int maxScale = 2)
    {
        RoundingStrategy = roundingStrategy;
        Precision = precision;
        MaxScale = maxScale;

        // Automatically register this context
        _index = RegisterContext(this);
    }

    // Efficient lookup index (1-byte reference)
    private readonly byte _index;

    internal byte Index => _index;

    // Factory method to enforce deduplication and controlled creation
    public static MoneyContext Create(IRoundingStrategy roundingStrategy, int precision = 28, int maxScale = 2)
    {
        // Look for an equivalent context in the dictionary
        foreach (var kvp in s_activeContexts)
        {
            if (kvp.Value.RoundingStrategy == roundingStrategy &&
                kvp.Value.Precision == precision &&
                kvp.Value.MaxScale == maxScale)
            {
                return kvp.Value; // Return existing equivalent context
            }
        }

        // Create and register a new context if no match is found
        return new MoneyContext(roundingStrategy, precision, maxScale);
    }

    // Retrieve MoneyContext by its 1-byte index
    internal static MoneyContext Get(byte index)
    {
        if (!s_activeContexts.TryGetValue(index, out var context))
        {
            throw new ArgumentException($"Invalid MoneyContext index: {index}");
        }

        return context;
    }

    private static byte RegisterContext(MoneyContext context)
    {
        // Ensure we don't exceed the 1-byte index limit (256 contexts)
        if (s_lastIndex == 255)
        {
            throw new InvalidOperationException("Maximum number of MoneyContexts (256) reached.");
        }

        var newIndex = ++s_lastIndex;
        s_activeContexts[newIndex] = context;
        return newIndex;
    }

    // Global Default Context Getter/Setter
    public static MoneyContext DefaultGlobal
    {
        get => s_defaultGlobalContext;
        set => s_defaultGlobalContext = value ?? throw new ArgumentNullException(nameof(value));
    }

    // Thread-local Context Getter/Setter
    public static MoneyContext? ThreadContext
    {
        get => s_threadLocalContext.Value.HasValue ? Get(s_threadLocalContext.Value.Value) : null;
        set => s_threadLocalContext.Value = value?._index;
    }

    public static MoneyContext Current => ThreadContext ?? DefaultGlobal;

    // Specific factory methods for common configurations
    public static MoneyContext CreateDefault() => Create(new DefaultRounding());
    public static MoneyContext CreateNoRounding() => Create(new NoRoundingStrategy());
    public static MoneyContext CreateRetail() => Create(new HalfUpRounding(), maxScale: 2);
    public static MoneyContext CreateAccounting() => Create(new HalfEvenRounding(), maxScale: 4);
}

internal readonly struct MoneyV2
{
    public decimal Amount { get; }
    public CurrencyInfo Currency { get; }
    private readonly byte _moneyContextIndex; // 1-byte reference to MoneyContext

    public MoneyV2(decimal amount, CurrencyInfo currency, MoneyContext? context = null)
    {
        Amount = amount;
        Currency = currency;

        // Use either provided context or current (global/thread-local)
        var effectiveContext = context ?? MoneyContext.Current;

        // Store its index
        _moneyContextIndex = effectiveContext.Index;
    }

    // Access the associated MoneyContext
    public MoneyContext Context => MoneyContext.Get(_moneyContextIndex);

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
    public MoneyV2 ApplyPrecisionAndScale()
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
    public MoneyV2 Apply(Func<decimal, decimal> adjustment)
    {
        return new MoneyV2(adjustment(Amount), Currency, Context);
    }

    // **Queryable Money**:
    // Allow querying context or metadata about `Money` objects, e.g.,:
    public bool IsWithinPrecisionScale()
    {
        return Amount.ToString().Replace(".", "").Length <= Context.Precision &&
               decimal.GetBits(Amount)[3] >> 16 <= Context.MaxScale;
    }
}
