namespace NodaMoney.Context;

// TODO: Handling of Ambiguous Currency Symbols when formatting/parsing? Add configurable policy enforcement to
// explicitly fail or resolve using a context-driven priority list.

/// <summary>Represents the financial and rounding configuration context for monetary operations.</summary>
public record MoneyContext
{
    private static byte s_lastIndex;
    private static readonly ReaderWriterLockSlim s_contextLock = new();
    private static readonly Dictionary<byte, MoneyContext> s_activeContexts = [];

    /// <summary>Thread-local index</summary>
    private static readonly AsyncLocal<byte?> s_threadLocalContext = new();

    /// <summary>Default global MoneyContext (fallback context)</summary>
    private static MoneyContext s_defaultThreadContext = new(new StandardRounding());

    /// <summary>Get the rounding strategy used for rounding monetary values in the context.</summary>
    /// <remarks>
    /// The rounding strategy determines how monetary values are rounded during operations, such as financial
    /// calculations, tax computations, or price adjustments. Examples of rounding strategies include Half-Up,
    /// Half-Even (Bankers' Rounding), or custom-defined strategies. It encapsulates the rules and logic for applying
    /// rounding, which may vary based on business context, regulatory requirements, or currency-specific needs.
    /// </remarks>
    public IRoundingStrategy RoundingStrategy { get; }

    /// <summary>Get the total number of significant digits available for numerical values in the context.</summary>
    public int Precision { get; }

    /// <summary>Get the maximum number of decimal places allowed for rounding operations within the monetary context.</summary>
    /// <remarks>This property overrides the scale in <see cref="CurrencyInfo"/>.</remarks>
    public int? MaxScale { get; }

    /// <summary>Get the default currency when none is specified for monetary operations within the context.</summary>
    /// <remarks>If not specified (null) then the current culture will be used to find the currency.</remarks>
    public CurrencyInfo? DefaultDefaultCurrency { get; }

    /// <summary>Get the metadata properties associated with the monetary context.</summary>
    public MetadataProvider Metadata { get; } = new();

    /// <summary>Efficient lookup index (1-byte reference)</summary>
    internal byte Index { get; private set; }

    private MoneyContext(IRoundingStrategy roundingStrategy, int precision = 28, int? maxScale = null, CurrencyInfo? defaultCurrency = null)
    {
        RoundingStrategy = roundingStrategy;
        Precision = precision;
        MaxScale = maxScale;
        DefaultDefaultCurrency = defaultCurrency;

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
    /// <param name="defaultCurrency">The default currency to use when none is specified for monetary operations.</param>
    /// <returns>A <see cref="MoneyContext"/> instance that matches the specified parameters.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the precision is less than or equal to zero, or when the maxScale is negative.</exception>
    /// <exception cref="ArgumentException">Thrown when the maxScale is greater than the precision.</exception>
    public static MoneyContext Create(IRoundingStrategy roundingStrategy, int precision = 28, int? maxScale = null, CurrencyInfo? defaultCurrency = null)
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
                 if (ctx.RoundingStrategy.Equals(roundingStrategy)
                    && ctx.Precision == precision
                    && ctx.MaxScale.GetValueOrDefault() == maxScale.GetValueOrDefault()
                    && ctx.DefaultDefaultCurrency == defaultCurrency)
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
        return new MoneyContext(roundingStrategy, precision, maxScale, defaultCurrency);
    }

    /// <summary>Gets or sets the default global <see cref="MoneyContext"/> instance that acts as a fallback context.</summary>
    /// <remarks>
    /// This property holds a global default monetary configuration context, used when no thread-local or specific context
    /// is defined. It can be customized by assigning a new <see cref="MoneyContext"/> instance or retrieved to use its
    /// predefined configuration. Setting this property to null will throw an <see cref="ArgumentNullException"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when the provided value is null.</exception>
    public static MoneyContext DefaultThreadContext
    {
        get => s_defaultThreadContext;
        set => s_defaultThreadContext = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Gets or sets the thread-local <see cref="MoneyContext"/> for the current execution thread.</summary>
    /// <remarks>
    /// This property allows for the configuration of a specific <see cref="MoneyContext"/> to apply
    /// within a localized scope of execution, typically for operations that require specialized monetary
    /// processing or rounding rules. If not explicitly set, the <see cref="DefaultThreadContext"/> context is used.
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
    public static MoneyContext CurrentContext => ThreadContext ?? DefaultThreadContext;

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
            foreach (MoneyContext ctx in s_activeContexts.Values)
            {
                if (ctx.RoundingStrategy.Equals(context.RoundingStrategy)
                    && ctx.Precision == context.Precision
                    && ctx.MaxScale.GetValueOrDefault() == context.MaxScale.GetValueOrDefault()
                    && ctx.DefaultDefaultCurrency == context.DefaultDefaultCurrency)
                {
                    return ctx.Index; // Return existing equivalent context index
                }
            }

            // Ensure we don't exceed the 7-bits index limit (128 contexts)
            if (s_lastIndex == 127)
            {
                throw new InvalidOperationException("Maximum number of MoneyContexts (128) reached.");
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

    /// <summary>Creates a new instance of the <see cref="MoneyContext"/> class without applying any rounding strategy.</summary>
    /// <returns>A <see cref="MoneyContext"/> instance configured with no rounding.</returns>
    internal static MoneyContext CreateNoRounding() => Create(new NoRounding());

    /// <summary>Creates a new scope for the <see cref="MoneyContext"/> based on the specified parameters, or uses an existing context if one matches.</summary>
    /// <param name="roundingStrategy">The rounding strategy to apply within the monetary context.</param>
    /// <param name="precision">The total number of significant digits for monetary calculations. Defaults to 28.</param>
    /// <param name="maxScale">The maximum number of decimal places allowed. If not specified, the default behavior is applied.</param>
    /// <returns>A disposable object that manages the lifetime of the monetary context scope.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if precision is less than or equal to zero, or if maxScale is negative.</exception>
    /// <exception cref="ArgumentException">Thrown if maxScale is greater than precision.</exception>
    public static IDisposable CreateScope(IRoundingStrategy roundingStrategy, int precision = 28, int? maxScale = null)
        => CreateScope(Create(roundingStrategy, precision, maxScale));

    /// <summary>Creates a scoped context for monetary operations with the specified configuration. When the context is disposed of, the previous context is restored.</summary>
    /// <param name="context">The <see cref="MoneyContext"/> instance representing the financial and rounding configuration to be used within the scope.</param>
    /// <returns>An <see cref="IDisposable"/> object that manages the lifecycle of the scoped context, automatically restoring the previous context upon disposal.</returns>
    public static IDisposable CreateScope(MoneyContext context)
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
