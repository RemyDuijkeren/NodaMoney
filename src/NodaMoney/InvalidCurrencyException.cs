using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace NodaMoney;

/// <summary>The exception that is thrown when the <see cref="Currency"/> is invalid for the current context or object state.</summary>
[ComVisible(true)]
[Serializable]
public class InvalidCurrencyException : InvalidOperationException
{
    /// <summary>Initializes a new instance of the <see cref="InvalidCurrencyException"/> class.</summary>
    public InvalidCurrencyException() : base("The requested operation cannot be performed with the specified currency!") { }

    /// <summary>Initializes a new instance of the <see cref="InvalidCurrencyException"/> class.</summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidCurrencyException(string message) : base(message) { }

    /// <summary>Initializes a new instance of the <see cref="InvalidCurrencyException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/>
    /// parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that
    /// handles the inner exception.</param>
    public InvalidCurrencyException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>Initializes a new instance of the <see cref="InvalidCurrencyException"/> class.</summary>
    /// <param name="expected">The expected currency.</param>
    /// <param name="actual">The actual currency.</param>
    public InvalidCurrencyException(Currency expected, Currency actual)
        : this($"Currency mismatch: The requested operation expected the currency {expected.Code}, but the actual value was the currency {actual.Code}!") { }

#if !NETSTANDARD1_3 && !NET6_0_OR_GREATER
    /// <inheritdoc/>
    [Obsolete("Formatter-based serialization is obsolete and not recommended for use in modern applications.", false)]
    protected InvalidCurrencyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
}
