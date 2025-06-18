using System.Diagnostics;
using System.Runtime.CompilerServices;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace NodaMoney.Context;

// TODO: Handling of Ambiguous Currency Symbols when formatting/parsing? Add configurable policy enforcement to
// explicitly fail or resolve using a context-driven priority list.
// Handle zero currency check? Strict vs Relaxed. Make this an option? See also #107

/// <summary>Represents the financial and rounding configuration context for monetary operations.</summary>
public sealed class MoneyContext
{
#if NET8_0_OR_GREATER // In .NET 8 or higher, we use FrozenDictionary for optimal immutability and performance
    static readonly object s_lock = new();
    private static FrozenDictionary<byte, MoneyContext> s_activeContexts = new Dictionary<byte, MoneyContext>(6).ToFrozenDictionary();
    private static FrozenDictionary<string, byte> s_namedContexts = new Dictionary<string, byte>(6, StringComparer.OrdinalIgnoreCase).ToFrozenDictionary();
#else // In .NET Standard 2.0, we use Dictionary with ReaderWriterLockSlim for thread safety
    private static readonly ReaderWriterLockSlim s_contextLock = new();
    private static readonly Dictionary<byte, MoneyContext> s_activeContexts = [];
    private static readonly Dictionary<string, byte> s_namedContexts = new(6, StringComparer.OrdinalIgnoreCase);
#endif

    private static readonly AsyncLocal<MoneyContextIndex?> s_threadLocalContext = new();
    private static MoneyContext s_defaultThreadContext;
    private MoneyContextOptions Options { get; }

    /// <summary>Efficient lookup index (1-byte reference)</summary>
    internal MoneyContextIndex Index { get; }

    /// <summary>Get the rounding strategy used for rounding monetary values in the context.</summary>
    /// <remarks>
    /// The rounding strategy determines how monetary values are rounded during operations, such as financial
    /// calculations, tax computations, or price adjustments. Examples of rounding strategies include Half-Up,
    /// Half-Even (Bankers' Rounding), or custom-defined strategies. It encapsulates the rules and logic for applying
    /// rounding, which may vary based on business context, regulatory requirements, or currency-specific needs.
    /// </remarks>
    public IRoundingStrategy RoundingStrategy => Options.RoundingStrategy;

    /// <summary>Get the total number of significant digits available for numerical values in the context.</summary>
    public int Precision => Options.Precision;

    /// <summary>Get the maximum number of decimal places allowed for rounding operations within the monetary context.</summary>
    /// <remarks>This property overrides the scale in <see cref="CurrencyInfo"/>.</remarks>
    public int? MaxScale => Options.MaxScale;

    /// <summary>Get the default currency when none is specified for monetary operations within the context.</summary>
    /// <remarks>If not specified (null) then the current culture will be used to find the currency.</remarks>
    public CurrencyInfo? DefaultCurrency => Options.DefaultCurrency;

    /// <summary>Provides a predefined <see cref="MoneyContext"/> instance with no rounding strategy applied.</summary>
    internal static MoneyContext NoRounding => Create(new MoneyContextOptions { RoundingStrategy = new NoRounding() });

    static MoneyContext()
    {
        // Pre-initialize contexts for all standard rounding modes. Create contexts in the exact same order as the
        // MidpointRounding enum values. This ensures that their indices align with the enum values for fast lookup.

        var toEvenContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.ToEven) });
        Trace.Assert(toEvenContext.Index == (byte)MidpointRounding.ToEven, $"Index of ToEven context should be 0, but is {toEvenContext.Index}");
        s_defaultThreadContext = toEvenContext; // Set default context to ToEven

        var awayFromZeroContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero) });
        Trace.Assert(awayFromZeroContext.Index == (byte)MidpointRounding.AwayFromZero, $"Index of AwayFromZero context should be 1, but is {awayFromZeroContext.Index}");

#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        var toZeroContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.ToZero) });
        Trace.Assert(toZeroContext.Index == (byte)MidpointRounding.ToZero, $"Index of ToZero context should be 2, but is {toZeroContext.Index}");

        var toNegInfContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.ToNegativeInfinity) });
        Trace.Assert(toNegInfContext.Index == (byte)MidpointRounding.ToNegativeInfinity, $"Index of ToNegativeInfinity context should be 3, but is {toNegInfContext.Index}");

        var toPosInfContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.ToPositiveInfinity) });
        Trace.Assert(toPosInfContext.Index == (byte)MidpointRounding.ToPositiveInfinity, $"Index of ToPositiveInfinity context should be 4, but is {toPosInfContext.Index}");
#endif
    }

    private MoneyContext(MoneyContextOptions options)
    {
        Trace.Assert(options is not null, $"{nameof(options)} must not be null");
        Options = options!;

        // Automatically register this context
        Index = RegisterContext(this);
    }

    public static MoneyContext Create(MoneyContextOptions options, string? name = null)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (options.Precision <= 0) throw new ArgumentOutOfRangeException(nameof(options.Precision), "Precision must be positive");
        if (options.MaxScale < 0) throw new ArgumentOutOfRangeException(nameof(options.MaxScale), "MaxScale cannot be negative");
        if (options.MaxScale > options.Precision) throw new ArgumentException("MaxScale cannot be greater than precision");

#if NET8_0_OR_GREATER
        // Look for an equivalent context in the dictionary
        foreach (MoneyContext ctx in s_activeContexts.Values)
        {
            if (ctx.Options.Equals(options))
            {
                AddNamedContext(ctx);
                return ctx; // Return existing equivalent context
            }
        }
#else
        s_contextLock.EnterReadLock();
        try
        {
            // Look for an equivalent context in the dictionary
            foreach (MoneyContext ctx in s_activeContexts.Values)
            {
                if (ctx.Options.Equals(options))
                {
                    AddNamedContext(ctx);
                    return ctx; // Return existing equivalent context
                }
            }
        }
        finally
        {
            s_contextLock.ExitReadLock();
        }
#endif

        // Create and register a new context if no match is found
        MoneyContext context = new(options);
        AddNamedContext(context);
        return context;

        void AddNamedContext(MoneyContext ctx)
        {
            if (string.IsNullOrEmpty(name)) return;
#if NET8_0_OR_GREATER
            lock (s_lock)
            {
                var mutableDictionary = s_namedContexts.ToDictionary();
                mutableDictionary[name] = ctx.Index;
                s_namedContexts = mutableDictionary.ToFrozenDictionary();
            }
#else
            s_contextLock.EnterWriteLock();
            try
            {
                s_namedContexts[name!] = ctx.Index;
            }
            finally
            {
                s_contextLock.ExitWriteLock();
            }
#endif
        }
    }

    public static MoneyContext Create(Action<MoneyContextOptions> configureOptions, string? name = null)
    {
        var options = new MoneyContextOptions();
        configureOptions(options);
        return Create(options, name);
    }

    /// <summary>Fast path for creating a new instance of the <see cref="MoneyContext"/> class with a standard rounding mode.</summary>
    /// <param name="mode">The <see cref="MidpointRounding"/> mode to be applied for monetary calculations.</param>
    /// <returns>A <see cref="MoneyContext"/> instance corresponding to the specified rounding mode.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static MoneyContext Create(MidpointRounding mode)
    {
#if NET8_0_OR_GREATER
        // This is a fast path that avoids creating a new context for standard rounding modes
        if (s_activeContexts.TryGetValue((byte)mode, out var context)) return context;
        // Fallback for any future rounding modes that might be added in the future.
        return Create(new MoneyContextOptions { RoundingStrategy = new StandardRounding(mode) });
#else
        s_contextLock.EnterReadLock(); // Allow parallel reads
        try
        {
            // This is a fast path that avoids creating a new context for standard rounding modes
            if (s_activeContexts.TryGetValue((MoneyContextIndex)(byte)mode, out var context)) return context;
            // Fallback for any future rounding modes that might be added in the future.
            return Create(new MoneyContextOptions { RoundingStrategy = new StandardRounding(mode) });
        }
        finally
        {
            s_contextLock.ExitReadLock();
        }
#endif
    }

    public static MoneyContext CreateAndSetDefault(MoneyContextOptions options, string? name = null)
    {
        MoneyContext context = Create(options, name);
        DefaultThreadContext = context;
        return context;
    }

    public static MoneyContext CreateAndSetDefault(Action<MoneyContextOptions> configureOptions, string? name = null)
    {
        var options = new MoneyContextOptions();
        configureOptions(options);
        return CreateAndSetDefault(options, name);
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static MoneyContext Get(byte index)
    {
#if NET8_0_OR_GREATER
        if (s_activeContexts.TryGetValue(index, out var context)) return context;
        throw new ArgumentException($"Invalid MoneyContext index: {index}");
#else
        s_contextLock.EnterReadLock(); // Allow parallel reads
        try
        {
            if (s_activeContexts.TryGetValue(index, out var context)) return context;
            throw new ArgumentException($"Invalid MoneyContext index: {index}");
        }
        finally
        {
            s_contextLock.ExitReadLock();
        }
#endif
    }

    /// <summary>Retrieves a <see cref="MoneyContext"/> instance by its registered name, if available.</summary>
    /// <param name="name">The name of the registered <see cref="MoneyContext"/> to retrieve.</param>
    /// <returns>The <see cref="MoneyContext"/> instance corresponding to the given name, or null if no matching context is found.</returns>
    /// <exception cref="ArgumentException">Thrown if the provided name is null or empty.</exception>
    public static MoneyContext? Get(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Context name cannot be null or empty", nameof(name));

#if NET8_0_OR_GREATER
        return s_namedContexts.TryGetValue(name, out var moneyContextIndex)
            ? Get(moneyContextIndex)
            : null;
#else
        s_contextLock.EnterReadLock();
        try
        {
            return s_namedContexts.TryGetValue(name, out var moneyContextIndex)
                ? Get(moneyContextIndex)
                : null;
        }
        finally
        {
            s_contextLock.ExitReadLock();
        }
#endif
    }

    private static MoneyContextIndex RegisterContext(MoneyContext context)
    {
#if NET8_0_OR_GREATER
        lock (s_lock)
        {
            foreach (MoneyContext ctx in s_activeContexts.Values)
            {
                if (ctx.Options.Equals(context.Options))
                {
                    return ctx.Index; // Return existing equivalent context index
                }
            }

            var newIndex = MoneyContextIndex.New(); // Max index is 127 (128 contexts)

            var mutableDictionary = s_activeContexts.ToDictionary();
            mutableDictionary[newIndex] = context;
            s_activeContexts = mutableDictionary.ToFrozenDictionary();

            return newIndex; // Return new index
        }
#else
        s_contextLock.EnterWriteLock(); // Ensure write exclusivity
        try
        {
            foreach (MoneyContext ctx in s_activeContexts.Values)
            {
                if (ctx.Options.Equals(context.Options))
                {
                    return ctx.Index; // Return existing equivalent context index
                }
            }

            var newIndex = MoneyContextIndex.New(); // Max index is 127 (128 contexts)
            s_activeContexts[newIndex] = context;
            return newIndex;
        }
        finally
        {
            s_contextLock.ExitWriteLock();
        }
#endif
    }

    /// <summary>Creates a scoped context for monetary operations with the specified configuration. When the context is disposed of, the previous context is restored.</summary>
    /// <param name="context">The <see cref="MoneyContext"/> instance representing the financial and rounding configuration to be used within the scope.</param>
    /// <returns>An <see cref="IDisposable"/> object that manages the lifecycle of the scoped context, automatically restoring the previous context upon disposal.</returns>
    public static IDisposable CreateScope(MoneyContext context)
    {
        MoneyContext? previous = ThreadContext;
        ThreadContext = context;
        return new ContextScope(() => ThreadContext = previous);
    }

    /// <summary>Creates a new execution scope for a monetary calculation context using the specified configuration options.</summary>
    /// <param name="configureOptions">An action to configure the <see cref="MoneyContextOptions"/> for the scope.</param>
    /// <returns>A disposable object that restores the previous context when disposed.</returns>
    public static IDisposable CreateScope(Action<MoneyContextOptions> configureOptions)
        => CreateScope(Create(configureOptions));

    /// <summary>Creates a scope for the specified monetary context options, which temporarily overrides the current thread's monetary context.</summary>
    /// <param name="options">The <see cref="MoneyContextOptions"/> defining the configuration for the new monetary context within the created scope.</param>
    /// <returns>An <see cref="IDisposable"/> object that, when disposed, reverts the thread's monetary context to its previous state.</returns>
    public static IDisposable CreateScope(MoneyContextOptions options)
        => CreateScope(Create(options));

    public static IDisposable CreateScope(string name)
        => CreateScope(Get(name) ?? throw new ArgumentException($"No context with name '{name}' found", nameof(name)));

    private sealed class ContextScope : IDisposable
    {
        private readonly Action _onDispose;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ContextScope(Action onDispose) => _onDispose = onDispose;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => _onDispose();
    }
}
