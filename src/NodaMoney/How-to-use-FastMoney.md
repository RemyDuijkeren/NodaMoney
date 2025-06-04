Let me compare `SqlMoney` with both `Decimal` and `SqlDecimal`:

1. **Range and Precision**:
  - `FastMoney`: ±1.0 × 10^-17 to ±9.2 × 10^17 (17 precision) (fixed 4 decimal places), like SqlMoney
  - `Money`: ±1.0 × 10^-28 to ±7.9 × 10^28 (28 digits precision)
  - `SqlDecimal`: ±1.0 × 10^-38 to ±1.0 × 10^38 (38 digits precision)

2. **Storage Size**:
  - `FastMoney`: 12 bytes (uses long internally)
  - `Money`: 16 bytes (uses decimal internally)

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
