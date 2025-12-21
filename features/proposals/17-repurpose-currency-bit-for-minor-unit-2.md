# Proposal: Repurposing the 15th Bit in Currency Struct for MinorUnit = 2

## 1. Introduction
The `Currency` struct is a core primitive in NodaMoney, designed to be lightweight (2 bytes). It currently uses 15 bits for the 3-letter ISO-style code and 1 bit (the 15th bit) for the `IsIso4217` flag. With the adoption of V2 serialization and the requirement for unique currency codes in the registry, the `IsIso4217` flag has lost its primary utility.

This proposal suggests repurposing this 15th bit to indicate if a currency has a `MinorUnit` of exactly 2. This is the most common case in global finance (e.g., USD, EUR, GBP) and allows for significant performance optimizations in "hot paths" like rounding and `Money` initialization.

## 2. Motivation
Currently, every time a `Money` object is created or rounded using `StandardRounding`, the code must:
1. Retrieve the `CurrencyInfo` instance from the `CurrencyRegistry` (dictionary lookup).
2. Access the `MinorUnit` property.
3. Perform logic based on that value.

While dictionary lookups are fast, they are not free. In high-frequency scenarios or large batch processing, these lookups add up. By encoding the "MinorUnit is 2" information directly into the `Currency` struct, we can bypass the registry lookup for the vast majority of transactions.

## 3. Proposed Changes

### 3.1. Currency Struct (src/NodaMoney/Currency.cs)
- Rename `Iso4217BitMask` to `MinorUnit2BitMask`.
- Replace `IsIso4217` bit logic with `IsMinorUnit2` logic.
- **Bit = 1:** MinorUnit is exactly 2.
- **Bit = 0:** MinorUnit is anything else (Unknown/Lookup required).
- Update constructors to set this bit based on the provided code (or by looking up the registry during initialization).
- Keep `IsIso4217` as a property that delegates to `CurrencyInfo` for backward compatibility.
- **Decision on Visibility:** The `IsMinorUnit2` property will be `internal`.
    - *Rationale:* It is primarily a performance optimization hint for the library's internal "hot paths." Exposing it publicly could lead to a "leaky abstraction" where implementation details (how we optimize for the most common case) clutter the public API. Users who need to know the `MinorUnit` should continue to use `CurrencyInfo.GetInstance(currency).MinorUnit`, which remains the authoritative source of domain information. If future use cases show that public access provides significant value to library consumers, it can be made public later.

### 3.2. CurrencyInfo Struct (src/NodaMoney/CurrencyInfo.cs)
- Update the implicit conversion operator to `Currency` to set the `MinorUnit2BitMask` if `MinorUnit == MinorUnit.Two`.

### 3.3. Money Struct (src/NodaMoney/Money.cs)
- In the `Money` constructor, check `currency.IsMinorUnit2`.
- If true, use a fast-path for rounding that avoids calling `CurrencyInfo.GetInstance(currency)`.

### 3.4. StandardRounding (src/NodaMoney/Context/StandardRounding.cs)
- Introduce an optimized `Round` overload (or logic) that takes `Currency` and checks the bit before falling back to `CurrencyInfo`.

### 3.5. Serialization (src/NodaMoney/Currency.Serializable.cs)
- Ensure serialization remains compatible. Since `IsIso4217` was used for the `;CUSTOM` suffix in XML/Binary serialization, we will now derive this information from `CurrencyInfo.GetInstance(this).IsIso4217` when serializing, or simply stop using the suffix if V2 serialization rules allow.

## 4. Design Details

### Encoding Logic
```csharp
const ushort MinorUnit2BitMask = 1 << 15;

internal bool IsMinorUnit2 => (EncodedValue & MinorUnit2BitMask) != 0;

// Public property to maintain compatibility if needed, though mostly obsolete
public bool IsIso4217 => CurrencyInfo.GetInstance(this).IsIso4217;
```

### Performance Fast-Path Example
```csharp
// In Money constructor
if (currency.IsMinorUnit2 && context.RoundingStrategy is StandardRounding sr && !context.MaxScale.HasValue)
{
    // Fast-path for 2 decimals
    amount = decimal.Round(amount, 2, sr.Mode);
}
else
{
    // Slow-path fallback
    var currencyInfo = CurrencyInfo.GetInstance(currency);
    amount = context.RoundingStrategy.Round(amount, currencyInfo, context.MaxScale);
}
```

## 5. Impact and Performance
- **Memory:** Zero increase in memory footprint (still 2 bytes).
- **CPU:** Significant reduction in instructions for the 90%+ case (USD, EUR, etc.) by avoiding dictionary lookups and interface/virtual calls in rounding strategies.
- **Backward Compatibility:** `IsIso4217` property remains but becomes a computed property. Serialization might need a small adjustment to preserve the `;CUSTOM` suffix if strictly required for legacy interop.

## 6. Alternatives Considered
- **IsHistoric:** Useful for domain logic but doesn't offer the same performance gains in core math operations.
- **Keep IsIso4217:** Provides little value in the current architecture where registry lookups are required for most other properties anyway.

## 7. Conclusion
Repurposing the 15th bit for `MinorUnit = 2` aligns with the goal of making NodaMoney a high-performance financial library. It leverages the remaining space in the `Currency` struct to optimize the most frequent operation in the library.
