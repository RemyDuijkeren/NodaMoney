using System.Globalization;

namespace NodaMoney;

/// <summary>Extension methods for globalization types to ease access to NodaMoney types.</summary>
public static class GlobalizationExtensions
{
    extension(RegionInfo region)
    {
        /// <summary>Gets the <see cref="CurrencyInfo"/> for the region.</summary>
        /// <returns>The <see cref="CurrencyInfo"/> for the region.</returns>
        /// <exception cref="ArgumentNullException">The value of 'region' cannot be null.</exception>
        public CurrencyInfo GetCurrencyInfo() => CurrencyInfo.GetInstance(region);
    }

    extension(CultureInfo culture)
    {
        /// <summary>Gets the <see cref="CurrencyInfo"/> for the culture.</summary>
        /// <returns>The <see cref="CurrencyInfo"/> for the culture.</returns>
        /// <exception cref="ArgumentNullException">The value of 'culture' cannot be null.</exception>
        /// <exception cref="ArgumentException">
        /// Culture is a neutral culture, from which no region information can be extracted -or-
        /// The 'code' is an unknown ISO 4217 currency code.
        /// </exception>
        public CurrencyInfo GetCurrencyInfo() => CurrencyInfo.FromCulture(culture);
    }
}
