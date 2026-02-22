# Globalization Extensions

Proposal to add extension methods to `CultureInfo` and `RegionInfo` for easy access to `CurrencyInfo`.

## Why it matters
In .NET applications, `CultureInfo` is the primary way to handle globalization, and `RegionInfo` is where currency data is traditionally stored. Providing direct access to `NodaMoney.CurrencyInfo` from these types improves developer experience and discoverability.

## Proposal
Implement `GetCurrencyInfo()` as an extension member on both `CultureInfo` and `RegionInfo`.

- `RegionInfo.GetCurrencyInfo()`: Returns the official ISO currency information for the region.
- `CultureInfo.GetCurrencyInfo()`: A convenience method that retrieves the `CurrencyInfo` for the culture's region. It handles `InvariantCulture` by returning `CurrencyInfo.NoCurrency`.

To keep the initial implementation focused, we will only implement `GetCurrencyInfo()` at this stage. Additional features like `GetCurrency()` or `CurrencyUsage` selection will be handled as future overloads, ensuring a clear upgrade path for regions with multiple currencies.

## API sketches

Using the new extension method syntax in .NET 10:

```csharp
using System.Globalization;

namespace NodaMoney;

public static class GlobalizationExtensions
{
    extension(RegionInfo region)
    {
        /// <summary>Gets the CurrencyInfo for the region.</summary>
        // The default 'Official' value ensures current behavior remains standard.
        public CurrencyInfo GetCurrencyInfo()
        {
            return CurrencyInfo.GetInstance(region);
        }
    }

    extension(CultureInfo culture)
    {
        /// <summary>Gets the CurrencyInfo for the culture.</summary>
        public CurrencyInfo GetCurrencyInfo()
        {
            if (Equals(culture, CultureInfo.InvariantCulture))
                return CurrencyInfo.NoCurrency;

            return CurrencyInfo.GetInstance(new RegionInfo(culture.Name));
        }
    }
}
```

## Future Upgrade Path: Alternative Currencies

Real-world scenarios often involve countries that use multiple currencies. For example, Panama (PA) uses the Balboa (PAB) officially, but the US Dollar (USD) is the most used currency for paper money. El Salvador (SV) uses both the US Dollar (USD) and Bitcoin (BTC) as legal tender.

To handle these cases, we plan to extend the API in future iterations with a `CurrencyUsage` enum and additional overloads.

### Proposed CurrencyUsage Enum
```csharp
public enum CurrencyUsage
{
    Official,   // The ISO-4217 standard currency for the region
    MostUsed,   // The currency most commonly used in daily transactions
    Alternative // Other currencies with legal tender status or wide usage
}
```

### Future API Sketches

```csharp
public static class GlobalizationExtensions
{
    extension(RegionInfo region)
    {
        // Overload to specify usage
        public CurrencyInfo GetCurrencyInfo(CurrencyUsage usage)
        {
            if (usage == CurrencyUsage.MostUsed)
            {
                // Future logic: Check specialized mapping for countries like Panama (USD)
            }
            return CurrencyInfo.GetInstance(region);
        }

        // Returns all currencies associated with the region
        public IEnumerable<CurrencyInfo> GetCurrencies()
        {
             // Logic to look up mapping in a new internal data structure
        }
    }
}
```

## Risks / considerations
- `RegionInfo` creation from a culture name is relatively fast but should be used judiciously in hot paths.
- Proper handling of the `InvariantCulture` and neutral cultures is required (already addressed by returning `NoCurrency` for `InvariantCulture`).
