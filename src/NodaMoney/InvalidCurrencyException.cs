using System;
using System.Runtime.InteropServices;

namespace NodaMoney
{
    /// <summary>The exception that is thrown when the <see cref="Currency"/> is invalid for the current context or object state.</summary>
    [ComVisible(true)]
    public class InvalidCurrencyException : InvalidOperationException
    {
        /// <summary>Initializes a new instance of the <see cref="InvalidCurrencyException"/> class.</summary>
        public InvalidCurrencyException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="InvalidCurrencyException"/> class.</summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidCurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="InvalidCurrencyException"/> class.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/>
        /// parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that
        /// handles the inner exception.</param>
        public InvalidCurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}