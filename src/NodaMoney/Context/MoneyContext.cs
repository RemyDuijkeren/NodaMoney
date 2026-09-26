using System.Diagnostics;
using System.Runtime.CompilerServices;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace NodaMoney.Context;

// TODO: Handling of Ambiguous Currency Symbols when formatting/parsing? Add configurable policy enforcement to
// explicitly fail or resolve using a context-driven priority list.
// Handle zero currency check? Strict vs Relaxed. Make this an option? See also #107

/// <summary>Identifies which rounding implementation a <see cref="MoneyContext"/> carries, precomputed so hot paths
/// can switch on a byte instead of type-testing the <see cref="IRoundingStrategy"/> instance.</summary>
internal enum RoundingKind : byte
{
    /// <summary>The context uses <see cref="Context.NoRounding"/>; the amount is never rounded.</summary>
    None = 0,

    /// <summary>The context uses the sealed <see cref="Context.StandardRounding"/>.</summary>
    Standard = 1,

    /// <summary>The context uses a custom <see cref="IRoundingStrategy"/> implementation.</summary>
    Custom = 2
}

/// <summary>Represents the financial and rounding configuration context for monetary operations.</summary>
public sealed record MoneyContext
{
    private static readonly object s_lock = new();
    private static readonly MoneyContext?[] s_activeContexts = new MoneyContext?[128];
#if NET8_0_OR_GREATER // In .NET 8 or higher, we use FrozenDictionary for optimal immutability and performance
    private static FrozenDictionary<string, byte> s_namedContexts = new Dictionary<string, byte>(6, StringComparer.OrdinalIgnoreCase).ToFrozenDictionary();
#else // In .NET Standard 2.0, we use Dictionary with ReaderWriterLockSlim for thread safety
    private static readonly ReaderWriterLockSlim s_contextLock = new();
    private static readonly Dictionary<string, byte> s_namedContexts = new(6, StringComparer.OrdinalIgnoreCase);
#endif

    private static readonly AsyncLocal<MoneyContext?> s_threadLocalContext = new();
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

    /// <summary>Gets the precomputed rounding kind for <see cref="RoundingStrategy"/>, used by hot paths to avoid a type test.</summary>
    internal RoundingKind Kind { get; }

    /// <summary>Gets the <see cref="MidpointRounding"/> mode of <see cref="RoundingStrategy"/> when <see cref="Kind"/> is
    /// <see cref="RoundingKind.Standard"/>. Undefined for any other kind.</summary>
    internal MidpointRounding Mode { get; }

    /// <summary>Get the total number of significant digits available for numerical values in the context.</summary>
    public int Precision => Options.Precision;

    /// <summary>Get the maximum number of decimal places allowed for rounding operations within the monetary context.</summary>
    /// <remarks>This property overrides the scale in <see cref="CurrencyInfo"/>.</remarks>
    public int? MaxScale => Options.MaxScale;

    /// <summary>Get the default currency when none is specified for monetary operations within the context.</summary>
    public CurrencyInfo? DefaultCurrency => Options.DefaultCurrency;

    /// <summary>Gets the value indicating whether zero amounts should require matching currency validation.</summary>
    /// <remarks>
    /// When set to <c>true</c>, zero monetary amounts will be subject to currency matching rules, which can enforce stricter validation
    /// in scenarios where currency consistency is critical, even for zero values. When set to <c>false</c>, zero amounts are exempt from
    /// currency matching, allowing more relaxed validation for such cases. By default, it is <c>false</c>.
    /// </remarks>
    public bool EnforceZeroCurrencyMatching => Options.EnforceZeroCurrencyMatching;

    /// <summary>Provides a predefined <see cref="MoneyContext"/> instance with no rounding strategy applied.</summary>
    internal static MoneyContext NoRounding { get; }

    /// <summary>Provides a predefined monetary context for <see cref="FastMoney"/></summary>
    /// <remarks>
    /// This context has the following default values:
    /// <list type="bullet">
    ///   <item><description><see cref="RoundingStrategy"/> = <see cref="StandardRounding"/> with <see cref="MidpointRounding.ToEven"/></description></item>
    ///   <item><description><see cref="Precision"/> = 19</description></item>
    ///   <item><description><see cref="MaxScale"/> = 4</description></item>
    /// </list>
    /// This aligns with the default precision and scale for <see cref="FastMoney"/>, which is 19 digits and 4 decimal places and
    /// the internal rounding that decimal.ToOACurrency() does.
    /// </remarks>
    internal static MoneyContext FastMoney { get; }

    static MoneyContext()
    {
        // Pre-initialize contexts for all standard rounding modes. Create contexts in the exact same order as the
        // MidpointRounding enum values! This ensures that their indices align with the enum values for fast lookup.

        var toEvenContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.ToEven) });
        Debug.Assert(toEvenContext.Index == (byte)MidpointRounding.ToEven, $"Index of ToEven context should be 0, but is {toEvenContext.Index}");
        s_defaultThreadContext = toEvenContext; // Set default context to ToEven

        var awayFromZeroContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.AwayFromZero) });
        Debug.Assert(awayFromZeroContext.Index == (byte)MidpointRounding.AwayFromZero, $"Index of AwayFromZero context should be 1, but is {awayFromZeroContext.Index}");

#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        var toZeroContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.ToZero) });
        Debug.Assert(toZeroContext.Index == (byte)MidpointRounding.ToZero, $"Index of ToZero context should be 2, but is {toZeroContext.Index}");

        var toNegInfContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.ToNegativeInfinity) });
        Debug.Assert(toNegInfContext.Index == (byte)MidpointRounding.ToNegativeInfinity, $"Index of ToNegativeInfinity context should be 3, but is {toNegInfContext.Index}");

        var toPosInfContext = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.ToPositiveInfinity) });
        Debug.Assert(toPosInfContext.Index == (byte)MidpointRounding.ToPositiveInfinity, $"Index of ToPositiveInfinity context should be 4, but is {toPosInfContext.Index}");
#endif
        FastMoney = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new StandardRounding(MidpointRounding.ToEven), Precision = 19, MaxScale = 4 });
        NoRounding = new MoneyContext(new MoneyContextOptions { RoundingStrategy = new NoRounding() });
    }

    private MoneyContext(MoneyContextOptions options)
    {
        Debug.Assert(options is not null, $"{nameof(options)} must not be null");
        Options = options!;

        // Precompute the rounding kind so hot paths can switch on a byte instead of a type test.
        switch (Options.RoundingStrategy)
        {
            case Context.NoRounding:
                Kind = RoundingKind.None;
                break;
            case Context.StandardRounding standard:
                Kind = RoundingKind.Standard;
                Mode = standard.Mode;
                break;
            default:
                Kind = RoundingKind.Custom;
                break;
        }

        // Automatically register this context
        Index = RegisterContext(this);
    }

    public static MoneyContext Create(MoneyContextOptions options, string? name = null)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (options.Precision <= 0) throw new ArgumentOutOfRangeException(nameof(options.Precision), "Precision must be positive");
        if (options.MaxScale < 0) throw new ArgumentOutOfRangeException(nameof(options.MaxScale), "MaxScale cannot be negative");
        if (options.MaxScale > options.Precision) throw new ArgumentException("MaxScale cannot be greater than precision");

        // Look for an equivalent context in the array
        foreach (MoneyContext? ctx in s_activeContexts)
        {
            if (ctx?.Options.Equals(options) == true)
            {
                AddNamedContext(ctx);
                return ctx; // Return existing equivalent context
            }
        }

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
        // This is a fast path that avoids creating a new context for standard rounding modes
        var context = Volatile.Read(ref s_activeContexts[(byte)mode]);
        if (context is not null) return context;
        // Fallback for any future rounding modes that might be added in the future.
        return Create(new MoneyContextOptions { RoundingStrategy = new StandardRounding(mode) });
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
        get => s_threadLocalContext.Value;
        set => s_threadLocalContext.Value = value;
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
        var context = Volatile.Read(ref s_activeContexts[index]);
        if (context is not null) return context;
        throw new ArgumentException($"Invalid MoneyContext index: {index}");
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
        lock (s_lock)
        {
            foreach (MoneyContext? ctx in s_activeContexts)
            {
                if (ctx?.Options.Equals(context.Options) == true)
                {
                    return ctx.Index; // Return existing equivalent context index
                }
            }

            var newIndex = MoneyContextIndex.New(); // Max index is 127 (128 contexts)
            Volatile.Write(ref s_activeContexts[newIndex], context);
            return newIndex; // Return new index
        }
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

    private sealed class ContextScope(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }
}
