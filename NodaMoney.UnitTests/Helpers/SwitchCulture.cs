using System;
using System.Globalization;
using System.Threading;

namespace NodaMoney.UnitTests.Helpers
{
    /// <summary>Switch the current culture of the thread to an explicit one temporarily.</summary>
    /// <code>
    /// using (new SwitchCulture("nl-NL"))
    /// {
    ///     //Your code in the specified culture
    /// }
    /// // returns to orginal culture
    /// </code>
    internal class SwitchCulture : IDisposable
    {
        private readonly CultureInfo _original;

        public SwitchCulture(CultureInfo cultureInfo)
        {
            _original = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
        }

        public SwitchCulture(string culture) :
            this(new CultureInfo(culture))
        {
        }

        public void Dispose()
        {
            _original.ClearCachedData();
            Thread.CurrentThread.CurrentCulture = _original;
        }
    }
}