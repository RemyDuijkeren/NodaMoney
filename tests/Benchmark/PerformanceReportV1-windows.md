## InitializingCurrency
#### v1
| Method           |     Mean |   Error |  Gen 0 | Allocated |
|------------------|---------:|--------:|-------:|----------:|
| CurrencyFromCode | 443.7 ns | 8.90 ns | 0.0753 |     632 B |

## InitializingMoney
#### v1
| Method                        |      Mean |     Error | Ratio |  Gen 0 | Allocated |
|-------------------------------|----------:|----------:|------:|-------:|----------:|
| CurrencyCode                  | 483.87 ns |  6.370 ns |  1.00 | 0.0753 |     632 B |
| CurrencyCodeAndRoundingMode   | 496.87 ns |  9.720 ns |  1.02 | 0.0753 |     632 B |
| CurrencyFromCode              | 521.25 ns | 16.493 ns |  1.16 | 0.0753 |     632 B |
| ExtensionMethod               | 490.89 ns |  7.103 ns |  1.01 | 0.0753 |     632 B |
| ImplicitCurrencyByConstructor | 114.69 ns |  2.276 ns |  0.24 | 0.0057 |      48 B |
| ImplicitCurrencyByCasting     | 113.58 ns |  1.579 ns |  0.23 | 0.0057 |      48 B |
| Deconstruct                   |  34.86 ns |  0.348 ns |  0.07 |      - |         - |

## MoneyEquals
#### v1
| Method           |      Mean |    Error | Allocated |
|------------------|----------:|---------:|----------:|
| NotEqualValue    |  30.09 ns | 0.518 ns |         - |
| NotEqualCurrency |  68.83 ns | 0.679 ns |         - |
| Bigger           | 212.60 ns | 4.271 ns |         - |

## MoneyOperations
#### v1
| Method           |      Mean |    Error | Allocated |
|------------------|----------:|---------:|----------:|
| Add              | 231.00 ns | 4.470 ns |         - |
| Subtract         | 233.42 ns | 4.665 ns |         - |
| Increment        | 374.80 ns | 7.507 ns |         - |
| Decrement        | 369.70 ns | 7.295 ns |         - |

## MoneyFormatting
#### v1
| Method                         |     Mean |   Error |  Gen 0 | Allocated |
|--------------------------------|---------:|--------:|-------:|----------:|
| DefaultFormat                  | 194.0 ns | 3.93 ns | 0.0286 |     240 B |
| FormatWithPrecision            | 197.6 ns | 3.98 ns | 0.0286 |     240 B |
| FormatProvider                 | 271.5 ns | 5.50 ns | 0.0525 |     440 B |
| FormatWithPrecisionAndProvider | 270.3 ns | 5.42 ns | 0.0525 |     440 B |

## MoneyParsing
#### v1
| Method      |        Mean |     Error |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns | 0.1469 |      - |      1 KB |

## HighLoad
#### v1
| Method           |     Mean |    Error |      Gen 0 | Allocated |
|------------------|---------:|---------:|-----------:|----------:|
| Create1MCurrency | 482.6 ms |  9.57 ms | 75000.0000 |    679 MB |
| Create1MMoney    | 516.8 ms | 10.33 ms | 75000.0000 |    694 MB |

// * Legends *
Mean        : Arithmetic mean of all measurements
Error       : Half of 99.9% confidence interval
StdDev      : Standard deviation of all measurements
Op/s        : Operation per second
Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
RatioSD     : Standard deviation of the ratio distribution ([Current]/[Baseline])
Gen0        : GC Generation 0 collects per 1000 operations
Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
1 ns        : 1 Nanosecond (0.000000001 sec)
