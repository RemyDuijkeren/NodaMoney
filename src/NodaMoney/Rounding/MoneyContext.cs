namespace NodaMoney.Rounding;

// TODO: Or MonetaryContext (or RoundingContext), MonetaryConfiguration (or MoneyRules, MonetaryRules)?

/// <summary>Represents the financial and rounding configuration context for monetary operations.</summary>
internal record MoneyContext
{
    private static byte s_lastIndex;
    private static readonly ReaderWriterLockSlim s_contextLock = new();
    private static readonly Dictionary<byte, MoneyContext> s_activeContexts = [];

    /// <summary>Thread-local index</summary>
    private static readonly AsyncLocal<byte?> s_threadLocalContext = new();

    /// <summary>Default global MoneyContext (fallback context)</summary>
    private static MoneyContext s_defaultGlobalContext = new(new DefaultRounding());

    /// <summary>Get the rounding strategy used for rounding monetary values in the context.</summary>
    /// <remarks>
    /// The rounding strategy determines how monetary values are rounded during operations, such as financial
    /// calculations, tax computations, or price adjustments. Examples of rounding strategies include Half-Up,
    /// Half-Even (Bankers' Rounding), or custom-defined strategies. It encapsulates the rules and logic for applying
    /// rounding, which may vary based on business context, regulatory requirements, or currency-specific needs.
    /// </remarks>
    public IRoundingStrategy RoundingStrategy { get; } // or StandardRoundingStrategy and CashRoundingStrategy?

    /// <summary>Get the total number of significant digits available for numerical values in the context.</summary>
    public int Precision { get; }

    /// <summary>Get the maximum number of decimal places allowed for rounding operations within the monetary context.</summary>
    /// <remarks>This property overrides the scale in <see cref="CurrencyInfo"/>.</remarks>
    public int? MaxScale { get; }

    /// <summary>Indicates whether cash rounding is applied in the monetary context.</summary>
    /// <remarks>This property reflects if rounding rules are tailored for cash handling, typically used to accommodate for physical currency denominations.</remarks>
    public bool CashRounding { get; } // TODO: CashRoundingStrategy? Or store in Metadata?

    /// <summary>Get the metadata properties associated with the monetary context.</summary>
    public MetadataProvider Metadata { get; } = new();

    /// <summary>Efficient lookup index (1-byte reference)</summary>
    internal byte Index { get; }

    private MoneyContext(IRoundingStrategy roundingStrategy, int precision = 28, int? maxScale = null, bool cashRounding = false)
    {
        RoundingStrategy = roundingStrategy;
        Precision = precision;
        MaxScale = maxScale;
        CashRounding = cashRounding;

        // Automatically register this context
        Index = RegisterContext(this);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MoneyContext"/> class or retrieves an existing instance if an
    /// equivalent context is already registered.
    /// </summary>
    /// <param name="roundingStrategy">The rounding strategy to be applied in monetary calculations.</param>
    /// <param name="precision">The total number of significant digits for monetary values. Defaults to 28.</param>
    /// <param name="maxScale">The maximum number of digits to the right of the decimal point. Overrides the scale in <see cref="CurrencyInfo"/></param>
    /// <param name="cashRounding">Indicates whether cash rounding is applied in the monetary context.</param>
    /// <returns>A <see cref="MoneyContext"/> instance that matches the specified parameters.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the precision is less than or equal to zero, or when the maxScale is negative.</exception>
    /// <exception cref="ArgumentException">Thrown when the maxScale is greater than the precision.</exception>
    public static MoneyContext Create(IRoundingStrategy roundingStrategy, int precision = 28, int? maxScale = null, bool cashRounding = false)
    {
        if (precision <= 0) throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be positive");
        if (maxScale < 0) throw new ArgumentOutOfRangeException(nameof(maxScale), "MaxScale cannot be negative");
        if (maxScale > precision) throw new ArgumentException("MaxScale cannot be greater than precision");

        s_contextLock.EnterReadLock();
        try
        {
            // Look for an equivalent context in the dictionary
            foreach (MoneyContext ctx in s_activeContexts.Values)
            {
                if (ctx.RoundingStrategy == roundingStrategy &&
                    ctx.Precision == precision &&
                    ctx.MaxScale == maxScale &&
                    ctx.CashRounding == cashRounding)
                {
                    return ctx; // Return existing equivalent context
                }
            }
        }
        finally
        {
            s_contextLock.ExitReadLock();
        }

        // Create and register a new context if no match is found
        return new MoneyContext(roundingStrategy, precision, maxScale);
    }

    /// <summary>Gets or sets the default global <see cref="MoneyContext"/> instance that acts as a fallback context.</summary>
    /// <remarks>
    /// This property holds a global default monetary configuration context, used when no thread-local or specific context
    /// is defined. It can be customized by assigning a new <see cref="MoneyContext"/> instance or retrieved to use its
    /// predefined configuration. Setting this property to null will throw an <see cref="ArgumentNullException"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the provided value is null.</exception>
    public static MoneyContext DefaultGlobal
    {
        get => s_defaultGlobalContext;
        set => s_defaultGlobalContext = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets or sets the thread-local <see cref="MoneyContext"/> for the current execution thread.</summary>
    /// <remarks>
    /// This property allows for the configuration of a specific <see cref="MoneyContext"/> to apply
    /// within a localized scope of execution, typically for operations that require specialized monetary
    /// processing or rounding rules. If not explicitly set, the <see cref="DefaultGlobal"/> context is used.
    /// </remarks>
    public static MoneyContext? ThreadContext
    {
        get => s_threadLocalContext.Value.HasValue ? Get(s_threadLocalContext.Value.Value) : null;
        set => s_threadLocalContext.Value = value?.Index;
    }

    /// <summary>
    /// Gets the current monetary context used for financial and rounding operations,
    /// defaulting to a thread-local context if set, or otherwise to the global default context.
    /// </summary>
    public static MoneyContext Current => ThreadContext ?? DefaultGlobal;

    /// <summary>Retrieves an existing <see cref="MoneyContext"/> instance based on the specified index.</summary>
    /// <param name="index">The unique index identifying the desired <see cref="MoneyContext"/> instance.</param>
    /// <returns>The <see cref="MoneyContext"/> instance corresponding to the specified index.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided index does not correspond to a valid or existing <see cref="MoneyContext"/> instance.</exception>
    internal static MoneyContext Get(byte index)
    {
        s_contextLock.EnterReadLock(); // Allow parallel reads
        try
        {
            if (!s_activeContexts.TryGetValue(index, out var context))
            {
                throw new ArgumentException($"Invalid MoneyContext index: {index}");
            }
            return context;
        }
        finally
        {
            s_contextLock.ExitReadLock();
        }
    }

    private static byte RegisterContext(MoneyContext context)
    {
        s_contextLock.EnterWriteLock(); // Ensure write exclusivity
        try
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
        finally
        {
            s_contextLock.ExitWriteLock();
        }
    }

    // Specific factory methods for common configurations
    public static MoneyContext CreateDefault() => Create(new DefaultRounding());
    public static MoneyContext CreateNoRounding() => Create(new NoRoundingStrategy());
    public static MoneyContext CreateRetail() => Create(new HalfUpRounding(), maxScale: 2);
    public static MoneyContext CreateAccounting() => Create(new HalfEvenRounding(), maxScale: 4);

    /// <summary>
    /// Temporarily sets the thread-local monetary context to the specified <see cref="MoneyContext"/> and restores
    /// the previous context when disposed.
    /// </summary>
    /// <param name="context">The <see cref="MoneyContext"/> to use as the thread-local context.</param>
    /// <returns>An <see cref="IDisposable"/> instance that restores the previous thread-local monetary context upon disposal.</returns>
    public static IDisposable UseContext(MoneyContext context)
    {
        MoneyContext? previous = ThreadContext;
        ThreadContext = context;
        return new ContextScope(() => ThreadContext = previous);
    }

    private class ContextScope(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }
}

internal readonly struct MoneyV2
{
    public decimal Amount { get; }
    public CurrencyInfo Currency { get; }
    private readonly byte _moneyContextIndex;

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

    public void MethodX()
    {
        using var context = MoneyContext.UseContext(MoneyContext.CreateAccounting());

        // TODO: ...
    }
}
