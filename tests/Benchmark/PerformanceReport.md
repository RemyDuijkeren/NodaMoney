## InitializingCurrency
#### v1
| Method           |     Mean |   Error |  Gen 0 | Allocated |
|------------------|---------:|--------:|-------:|----------:|
| CurrencyFromCode | 443.7 ns | 8.90 ns | 0.0753 |     632 B |
#### v2
| Method               |      Mean |     Error | Allocated |
|----------------------|----------:|----------:|----------:|
| CurrencyFromCode     | 14.516 ns | 0.0580 ns |         - |
| CurrencyInfoFromCode |  8.098 ns | 0.0537 ns |         - |
#### v2.5
| Method                  |      Mean |     Error |          Op/s | Allocated |
|-------------------------|----------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 16.178 ns | 0.2721 ns |  61,810,657.5 |         - |
| CurrencyInfoFromCode    |  9.293 ns | 0.0985 ns | 107,608,674.6 |         - |
| CurrencyInfoTryFromCode |  6.471 ns | 0.0487 ns | 154,534,339.0 |         - |

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
#### v2
| Method                        |       Mean |     Error | Ratio |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|-----------:|----------:|------:|-------:|----------:|------------:|
| CurrencyCode                  | 32.4288 ns | 0.2609 ns | 1.000 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 35.0707 ns | 0.2325 ns | 1.082 |      - |         - |          NA |
| CurrencyFromCode              | 32.8033 ns | 0.1516 ns | 1.012 |      - |         - |          NA |
| CurrencyInfoFromCode          | 32.1320 ns | 0.1910 ns | 0.991 |      - |         - |          NA |
| ExtensionMethod               | 34.2212 ns | 0.4191 ns | 1.055 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 75.4426 ns | 0.4001 ns | 2.327 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 75.0124 ns | 0.3120 ns | 2.313 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  0.2785 ns | 0.0068 ns | 0.009 |      - |         - |          NA |
#### v2.5
| Method                        |      Mean |     Error |          Op/s | Ratio |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|----------:|----------:|--------------:|------:|-------:|----------:|------------:|
| CurrencyCode                  | 38.254 ns | 0.5405 ns |  26,140,954.9 |  1.00 |      - |         - |          NA |
| fCurrencyCode                 | 33.994 ns | 0.5416 ns |  29,416,951.4 |  0.89 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 44.330 ns | 0.3970 ns |  22,557,911.1 |  1.16 |      - |         - |          NA |
| CurrencyCodeAndContext        | 42.655 ns | 0.2374 ns |  23,443,999.0 |  1.12 |      - |         - |          NA |
| CurrencyFromCode              | 37.422 ns | 0.2613 ns |  26,722,192.3 |  0.98 |      - |         - |          NA |
| CurrencyInfoFromCode          | 37.151 ns | 0.2870 ns |  26,917,085.2 |  0.97 |      - |         - |          NA |
| ExtensionMethodEuro           | 37.006 ns | 0.4006 ns |  27,022,575.9 |  0.97 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 84.376 ns | 1.5994 ns |  11,851,753.2 |  2.21 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 82.875 ns | 1.6222 ns |  12,066,352.9 |  2.17 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.040 ns | 0.0248 ns | 961,394,287.4 |  0.03 |      - |         - |          NA |

## MoneyEquals
#### v1
| Method           |      Mean |    Error | Allocated |
|------------------|----------:|---------:|----------:|
| NotEqualValue    |  30.09 ns | 0.518 ns |         - |
| NotEqualCurrency |  68.83 ns | 0.679 ns |         - |
| Bigger           | 212.60 ns | 4.271 ns |         - |
#### v2
| Method           |      Mean |     Error |   Gen0 | Allocated |
|------------------|----------:|----------:|-------:|----------:|
| NotEqualValue    |  3.623 ns | 0.0396 ns |      - |         - |
| NotEqualCurrency |  3.684 ns | 0.0314 ns |      - |         - |
| Bigger           |  3.908 ns | 0.0517 ns |      - |         - |
#### v2.5
| Method            |      Mean |     Error |            Op/s | Ratio | Allocated | Alloc Ratio |
|-------------------|----------:|----------:|----------------:|------:|----------:|------------:|
| Equal             | 4.4893 ns | 0.0446 ns |   222,753,932.0 |  1.00 |         - |          NA |
| NotEqualValue     | 3.5042 ns | 0.0959 ns |   285,373,664.3 |  0.78 |         - |          NA |
| NotEqualCurrency  | 1.7181 ns | 0.0622 ns |   582,046,685.4 |  0.38 |         - |          NA |
| EqualOrBigger     | 5.1530 ns | 0.0307 ns |   194,060,251.2 |  1.15 |         - |          NA |
| Bigger            | 5.3321 ns | 0.0461 ns |   187,544,525.5 |  1.19 |         - |          NA |
| fEqual            | 0.1510 ns | 0.0110 ns | 6,624,626,124.1 |  0.03 |         - |          NA |
| fNotEqualValue    | 0.3422 ns | 0.0279 ns | 2,922,441,245.4 |  0.08 |         - |          NA |
| fNotEqualCurrency | 0.2224 ns | 0.0187 ns | 4,496,291,443.1 |  0.05 |         - |          NA |
| fEqualOrBigger    | 1.2799 ns | 0.0207 ns |   781,291,453.2 |  0.29 |         - |          NA |
| fBigger           | 1.3996 ns | 0.0577 ns |   714,509,324.0 |  0.31 |         - |          NA |

## MoneyOperations
#### v1
| Method           |      Mean |    Error | Allocated |
|------------------|----------:|---------:|----------:|
| Add              | 231.00 ns | 4.470 ns |         - |
| Subtract         | 233.42 ns | 4.665 ns |         - |
| Increment        | 374.80 ns | 7.507 ns |         - |
| Decrement        | 369.70 ns | 7.295 ns |         - |
#### v2
| Method           |      Mean |     Error |   Gen0 | Allocated |
|------------------|----------:|----------:|-------:|----------:|
| Add              | 16.219 ns | 0.0810 ns |      - |         - |
| Subtract         | 15.777 ns | 0.0662 ns |      - |         - |
| Increment        | 86.789 ns | 0.8525 ns | 0.0038 |      32 B |
| Decrement        | 86.949 ns | 0.5347 ns | 0.0038 |      32 B |
#### v2.5
| Method                  |      Mean |     Error |          Op/s | Ratio | Allocated | Alloc Ratio |
|-------------------------|----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 26.968 ns | 0.2871 ns |  37,080,694.8 | 18.28 |         - |          NA |
| Subtract                | 25.921 ns | 0.1544 ns |  38,579,474.4 | 17.57 |         - |          NA |
| Multiple                | 30.849 ns | 0.4080 ns |  32,416,301.6 | 20.92 |         - |          NA |
| Divide                  | 83.995 ns | 0.4120 ns |  11,905,409.6 | 56.95 |         - |          NA |
| Increment               | 16.407 ns | 0.0855 ns |  60,950,309.8 | 11.12 |         - |          NA |
| Decrement               | 19.793 ns | 0.3293 ns |  50,521,853.8 | 13.42 |         - |          NA |
| Remainder               | 21.764 ns | 0.1380 ns |  45,948,316.5 | 14.76 |         - |          NA |
| fAdd                    |  1.475 ns | 0.0188 ns | 677,926,754.0 |  1.00 |         - |          NA |
| fSubtract               |  1.927 ns | 0.0433 ns | 518,833,833.6 |  1.31 |         - |          NA |
| fMultipleDec            | 28.538 ns | 0.5040 ns |  35,041,170.3 | 19.35 |         - |          NA |
| fMultipleDecWholeNumber | 14.430 ns | 0.1620 ns |  69,301,660.6 |  9.78 |         - |          NA |
| fMultipleLong           |  2.204 ns | 0.0308 ns | 453,750,585.1 |  1.49 |         - |          NA |
| fDivideDec              | 79.456 ns | 0.7799 ns |  12,585,508.5 | 53.87 |         - |          NA |
| fDivideDecWholeNumber   | 12.855 ns | 0.0787 ns |  77,791,781.7 |  8.72 |         - |          NA |
| fDivideLong             |  1.789 ns | 0.0158 ns | 558,992,014.8 |  1.21 |         - |          NA |
| fIncrement              |  6.150 ns | 0.1163 ns | 162,607,271.5 |  4.17 |         - |          NA |
| fDecrement              |  6.125 ns | 0.0587 ns | 163,260,592.0 |  4.15 |         - |          NA |
| fRemainder              |  1.963 ns | 0.0291 ns | 509,443,667.2 |  1.33 |         - |          NA |

## MoneyFormatting
#### v1
| Method                         |     Mean |   Error |  Gen 0 | Allocated |
|--------------------------------|---------:|--------:|-------:|----------:|
| DefaultFormat                  | 194.0 ns | 3.93 ns | 0.0286 |     240 B |
| FormatWithPrecision            | 197.6 ns | 3.98 ns | 0.0286 |     240 B |
| FormatProvider                 | 271.5 ns | 5.50 ns | 0.0525 |     440 B |
| FormatWithPrecisionAndProvider | 270.3 ns | 5.42 ns | 0.0525 |     440 B |
#### v2
| Method                         |     Mean |   Error |   Gen0 | Allocated |
|--------------------------------|---------:|--------:|-------:|----------:|
| DefaultFormat                  | 106.3 ns | 2.06 ns | 0.0468 |     392 B |
| FormatWithPrecision            | 147.5 ns | 2.66 ns | 0.0505 |     424 B |
| FormatProvider                 | 109.1 ns | 2.14 ns | 0.0468 |     392 B |
| FormatWithPrecisionAndProvider | 144.8 ns | 2.92 ns | 0.0505 |     424 B |
#### v2.5
| Method                         |     Mean |   Error |        Op/s |   Gen0 | Allocated |
|--------------------------------|---------:|--------:|------------:|-------:|----------:|
| DefaultFormat                  | 121.1 ns | 2.41 ns | 8,259,783.1 | 0.0458 |     384 B |
| FormatWithPrecision            | 161.3 ns | 3.12 ns | 6,199,592.9 | 0.0496 |     416 B |
| FormatProvider                 | 113.2 ns | 2.31 ns | 8,834,277.4 | 0.0459 |     384 B |
| FormatWithPrecisionAndProvider | 147.0 ns | 2.90 ns | 6,803,103.3 | 0.0496 |     416 B |
| CompactFormat                  | 230.4 ns | 4.06 ns | 4,340,834.0 | 0.0572 |     480 B |
| GeneralFormat                  | 170.2 ns | 3.46 ns | 5,876,783.1 | 0.0842 |     704 B |
| RondTripFormat                 | 118.1 ns | 2.42 ns | 8,465,497.5 | 0.0515 |     432 B |

## MoneyParsing
#### v1
| Method      |        Mean |     Error |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns | 0.1469 |      - |      1 KB |
#### v2
| Method            |     Mean |   Error |   Gen0 | Allocated |
|-------------------|---------:|--------:|-------:|----------:|
| Implicit          | 427.4 ns | 7.67 ns | 0.1173 |     984 B |
| ImplicitTry       | 426.3 ns | 6.59 ns | 0.1173 |     984 B |
| Explicit          | 442.4 ns | 7.58 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 442.1 ns | 6.54 ns | 0.1173 |     984 B |
| ExplicitTry       | 431.5 ns | 6.15 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 446.3 ns | 5.94 ns | 0.1173 |     984 B |
#### v2.5
| Method            |     Mean |   Error |        Op/s |   Gen0 | Allocated |
|-------------------|---------:|--------:|------------:|-------:|----------:|
| Implicit          | 185.6 ns | 3.82 ns | 5,386,586.2 | 0.0191 |     160 B |
| ImplicitTry       | 189.7 ns | 3.79 ns | 5,270,498.0 | 0.0191 |     160 B |
| Explicit          | 193.2 ns | 3.84 ns | 5,176,120.3 | 0.0191 |     160 B |
| ExplicitAsSpan    | 191.4 ns | 2.19 ns | 5,223,378.5 | 0.0191 |     160 B |
| ExplicitTry       | 202.4 ns | 2.89 ns | 4,939,987.4 | 0.0191 |     160 B |
| ExplicitTryAsSpan | 193.3 ns | 1.90 ns | 5,173,897.1 | 0.0191 |     160 B |

## MoneyConversion
### v2.5
| Method        |       Mean |     Error |             Op/s | Ratio | Allocated | Alloc Ratio |
|---------------|-----------:|----------:|-----------------:|------:|----------:|------------:|
| ToDecimal     |  0.8455 ns | 0.0438 ns |  1,182,679,904.5 |  1.01 |         - |          NA |
| ToDouble      |  2.9635 ns | 0.0807 ns |    337,439,639.7 |  3.52 |         - |          NA |
| ToIn32        | 22.7336 ns | 0.4754 ns |     43,987,805.9 | 27.04 |         - |          NA |
| ToInt64       | 22.9894 ns | 0.3967 ns |     43,498,290.9 | 27.34 |         - |          NA |
| ToFastMoney   | 28.6825 ns | 0.5323 ns |     34,864,447.5 | 34.11 |         - |          NA |
| fToDecimal    |  3.0329 ns | 0.0609 ns |    329,718,318.2 |  3.61 |         - |          NA |
| fToDouble     | 16.4829 ns | 0.1799 ns |     60,668,843.6 | 19.60 |         - |          NA |
| fToIn32       |  7.0331 ns | 0.1635 ns |    142,184,665.4 |  8.37 |         - |          NA |
| fToInt64      |  6.7446 ns | 0.1190 ns |    148,266,932.8 |  8.02 |         - |          NA |
| fToMoney      | 22.0964 ns | 0.1660 ns |     45,256,149.1 | 26.28 |         - |          NA |
| fToSqlMoney   | 22.7936 ns | 0.2174 ns |     43,871,886.1 | 27.11 |         - |          NA |
| fToAOCurrency |  0.0119 ns | 0.0167 ns | 84,114,152,018.4 |  0.01 |         - |          NA |

## HighLoad
#### v1
| Method           |     Mean |    Error |      Gen 0 | Allocated |
|------------------|---------:|---------:|-----------:|----------:|
| Create1MCurrency | 482.6 ms |  9.57 ms | 75000.0000 |    679 MB |
| Create1MMoney    | 516.8 ms | 10.33 ms | 75000.0000 |    694 MB |
#### v2
| Method            |      Mean |     Error |     Gen0 |     Gen1 |     Gen2 | Allocated |
|-------------------|----------:|----------:|---------:|---------:|---------:|----------:|
| Create1MCurrency  | 13.474 ms | 0.1538 ms | 390.6250 | 390.6250 | 312.5000 |   1.91 MB |
| Create1MMoney     | 30.124 ms | 0.6020 ms | 656.2500 | 656.2500 | 656.2500 |  22.89 MB |
| Create1MFastMoney | 42.112 ms | 0.7915 ms | 583.3333 | 583.3333 | 583.3333 |  15.26 MB |
#### v2.5
| Method            |      Mean |     Error |    Op/s | Ratio |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------|----------:|----------:|--------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 13.953 ms | 0.2772 ms |   71.67 |  0.38 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 36.576 ms | 0.6902 ms |   27.34 |  1.00 | 571.4286 | 571.4286 | 571.4286 |  15.26 MB |        1.00 |
| Create1MFastMoney | 30.137 ms | 0.3957 ms |   33.18 |  0.82 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 20.931 ms | 0.4076 ms |   47.78 |  0.57 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
| Create1MDecimal   |  4.050 ms | 0.0805 ms |  246.91 |  0.11 | 597.6563 | 597.6563 | 597.6563 |  15.26 MB |        1.00 |

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
