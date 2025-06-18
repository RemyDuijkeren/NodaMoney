using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using NodaMoney.Context;

namespace NodaMoney;

/// <summary>The exception that is thrown when the <see cref="Currency"/> is invalid for the current context or object state.</summary>
[ComVisible(true)]
[Serializable]
public class MoneyContextMismatchException : InvalidOperationException
{
    /// <summary>Initializes a new instance of the <see cref="MoneyContextMismatchException"/> class.</summary>
    public MoneyContextMismatchException() : base("The requested operation cannot be performed with the specified MoneyContext!") { }

    /// <summary>Initializes a new instance of the <see cref="MoneyContextMismatchException"/> class.</summary>
    /// <param name="message">The message that describes the error.</param>
    public MoneyContextMismatchException(string message) : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="MoneyContextMismatchException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/>
    /// parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that
    /// handles the inner exception.</param>
    public MoneyContextMismatchException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>Initializes a new instance of the <see cref="MoneyContextMismatchException"/> class.</summary>
    /// <param name="expected">The expected currency.</param>
    /// <param name="actual">The actual currency.</param>
    public MoneyContextMismatchException(MoneyContext expected, MoneyContext actual)
        : this($"MoneyContext mismatch: The requested operation expected the MoneyContext {expected}, but the actual value was the MoneyContext {actual}! Use the 'with' expression to align contexts before operation, e.g.: money2 with {{ Context = money1.Context }}") { }

#if !NETSTANDARD1_3 && !NET6_0_OR_GREATER
    /// <inheritdoc/>
    [Obsolete("Formatter-based serialization is obsolete and not recommended for use in modern applications.", false)]
    protected MoneyContextMismatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
}
