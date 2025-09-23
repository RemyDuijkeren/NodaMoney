Let me compare `SqlMoney` with both `Decimal` and `SqlDecimal`:

1. **Range and Precision**:
  - `FastMoney`:  ±1.0 × 10^-17 to ±9.2 × 10^17 (17 digits precision) (fixed 4 decimal places), like SqlMoney
  - `Money`:      ±1.0 × 10^-28 to ±7.9 × 10^28 (28 digits precision)
  - `SqlDecimal`: ±1.0 × 10^-38 to ±1.0 × 10^38 (38 digits precision)

2. **Storage Size**:
  - `FastMoney`: 12 bytes (uses long internally)
  - `Money`:    16 bytes (uses decimal internally)

3. **Performance**:
  - `FastMoney` is fastest (uses native CPU integer operations), so far we see 3x faster adding/subtracting
  - `Money` is second
  - `SqlDecimal` is slowest due to its internal complexity

4. **Use Cases**:
  - `FastMoney`: Best for:
    - High-performance financial calculations
    - When you need exactly 4 decimal places
    - When working with currency values in a standard range
  - `Money`: Best for:
    - General-purpose decimal calculations with rounding
    - Rounding is required
    - When you need a balance between precision and performance
  - `SqlDecimal`: Best for:
    - When you need precise control over precision and scale
    - Database operations that require exact decimal precision
    - Complex financial calculations that require high precision

The key consideration for choosing between these types often comes down to:
1. If you need exact database type matching → `SqlDecimal`
2. If you need maximum performance with standard currency precision → `FastMoney`
3. If you need general-purpose decimal calculations → `Money`


// ## Performance Benefits of Using Long vs Decimal
// The more significant advantage of `FastMoney` is using `long` for calculations instead of `decimal`:
// 1. **Faster Arithmetic Operations**: Integer operations on `long` are much faster than `decimal` operations:
// - Addition/subtraction with `long` is typically 3-10x faster than with `decimal`
// - Multiplication/division with `long` is typically 5-20x faster than with `decimal`
//
// 2. **Simpler CPU Instructions**: `long` operations use native CPU instructions, while `decimal` requires complex emulation as it's not directly supported by CPUs.
//
// 3. **SIMD Potential**: Operations on `long` values can potentially be vectorized with SIMD instructions, which isn't possible with `decimal`.
//
// 4. **Reduced Method Call Overhead**: Your `Add` method in `FastMoney` directly manipulates the `long` value without converting to `decimal` and back:
//     ``` csharp
// public FastMoney Add(FastMoney other)
// {
//     EnsureSameCurrency(this, other);
//     long totalAmount = checked(OACurrencyAmount + other.OACurrencyAmount);
//     return this with { OACurrencyAmount = totalAmount };
// }
// ```
// This is much more efficient than the equivalent operation with `decimal`.
//
// ## Trade-offs and Limitations
// 1. **Precision**: `FastMoney` is limited to 4 decimal places (scaled by 10,000), while `Money` can support up to 28 decimal places.
// 2. **Range**: `FastMoney` has a smaller range (-922,337,203,685,477.5808 to 922,337,203,685,477.5807) compared to `decimal`.
// 3. **Memory Alignment**: A 12-byte structure isn't optimally aligned for 64-bit systems, but the calculation benefits likely outweigh this.
// 4. **No Rounding**: FastMoney is not doing any internal rounding. For Display convert it to Money type.
//
// ## Recommendation
// 1. **Keep the 12-byte Size**: The performance benefits of using `long` for calculations are significant enough to justify using `FastMoney`, even if it's only 4 bytes smaller than `Money`.
//
// TODO: Benchmark if this all is really true! => Yes, Add/Subtract is 16x faster!
// FastMoney is very similar as SqlMoney but with the benefit of storing the currency.
// SqlMoney (8bytes long + 1byte bool = 9 bytes but 16bytes with padding) https://learn.microsoft.com/en-us/dotnet/api/system.data.sqltypes.sqlmoney?view=net-9.0
// Internal Currency type https://referencesource.microsoft.com/#mscorlib/system/currency.cs
