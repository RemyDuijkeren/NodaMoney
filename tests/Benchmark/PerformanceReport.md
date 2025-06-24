## InitializingCurrency
#### before (v1.x)
| Method           |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|------------------|---------:|--------:|---------:|-------:|----------:|
| CurrencyFromCode | 443.7 ns | 8.90 ns | 14.87 ns | 0.0753 |     632 B |
#### after (v2.x)
| Method               |      Mean |     Error |    StdDev | Allocated |
|----------------------|----------:|----------:|----------:|----------:|
| CurrencyFromCode     | 15.370 ns | 0.3315 ns | 0.5352 ns |         - |
| CurrencyInfoFromCode |  7.519 ns | 0.1908 ns | 0.1692 ns |         - |

## InitializingMoney
#### before (v1.x)
| Method                        |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Allocated |
|-------------------------------|----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|
| CurrencyCode                  | 483.87 ns |  6.370 ns |  5.319 ns | 483.75 ns |  1.00 |    0.00 | 0.0753 |     632 B |
| CurrencyCodeAndRoundingMode   | 496.87 ns |  9.720 ns | 12.976 ns | 493.09 ns |  1.02 |    0.03 | 0.0753 |     632 B |
| CurrencyFromCode              | 521.25 ns | 16.493 ns | 48.630 ns | 494.71 ns |  1.16 |    0.03 | 0.0753 |     632 B |
| ExtensionMethod               | 490.89 ns |  7.103 ns |  5.931 ns | 492.29 ns |  1.01 |    0.02 | 0.0753 |     632 B |
| ImplicitCurrencyByConstructor | 114.69 ns |  2.276 ns |  2.017 ns | 115.09 ns |  0.24 |    0.00 | 0.0057 |      48 B |
| ImplicitCurrencyByCasting     | 113.58 ns |  1.579 ns |  1.477 ns | 113.67 ns |  0.23 |    0.00 | 0.0057 |      48 B |
| Deconstruct                   |  34.86 ns |  0.348 ns |  0.309 ns |  34.80 ns |  0.07 |    0.00 |      - |         - |
#### after (v2.x)
| Method                        |       Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| CurrencyCode                  | 32.4288 ns | 0.2609 ns | 0.2441 ns | 1.000 |    0.01 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 35.0707 ns | 0.2325 ns | 0.2174 ns | 1.082 |    0.01 |      - |         - |          NA |
| CurrencyFromCode              | 32.8033 ns | 0.1516 ns | 0.1344 ns | 1.012 |    0.01 |      - |         - |          NA |
| CurrencyInfoFromCode          | 32.1320 ns | 0.1910 ns | 0.1595 ns | 0.991 |    0.01 |      - |         - |          NA |
| ExtensionMethod               | 34.2212 ns | 0.4191 ns | 0.3920 ns | 1.055 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 75.4426 ns | 0.4001 ns | 0.3546 ns | 2.327 |    0.02 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 75.0124 ns | 0.3120 ns | 0.2766 ns | 2.313 |    0.02 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  0.2785 ns | 0.0068 ns | 0.0063 ns | 0.009 |    0.00 |      - |         - |          NA |
#### after MoneyContext
| Method                        |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| CurrencyCode                  | 34.101 ns | 0.6536 ns | 0.6114 ns |  1.00 |    0.02 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 43.599 ns | 0.8274 ns | 1.0465 ns |  1.28 |    0.04 |      - |         - |          NA |
| CurrencyCodeAndContext        | 40.415 ns | 0.8239 ns | 0.9488 ns |  1.19 |    0.03 |      - |         - |          NA |
| CurrencyFromCode              | 35.438 ns | 0.6304 ns | 0.5896 ns |  1.04 |    0.02 |      - |         - |          NA |
| CurrencyInfoFromCode          | 34.710 ns | 0.7216 ns | 0.8020 ns |  1.02 |    0.03 |      - |         - |          NA |
| ExtensionMethodEuro           | 33.962 ns | 0.6985 ns | 0.6192 ns |  1.00 |    0.02 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 87.120 ns | 1.3026 ns | 1.2185 ns |  2.56 |    0.06 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 81.624 ns | 1.6238 ns | 1.5189 ns |  2.39 |    0.06 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.080 ns | 0.0465 ns | 0.0435 ns |  0.03 |    0.00 |      - |         - |          NA |

## MoneyOperations
#### before (v1.x)
| Method                   |      Mean |    Error |    StdDev | Allocated |
|--------------------------|----------:|---------:|----------:|----------:|
| Addition                 | 231.00 ns | 4.470 ns |  4.181 ns |         - |
| Subtraction              | 233.42 ns | 4.665 ns |  4.581 ns |         - |
| CompareSameCurrency      |  30.09 ns | 0.518 ns |  0.485 ns |         - |
| CompareDifferentCurrency |  68.83 ns | 0.679 ns |  0.530 ns |         - |
| CompareAmount            | 212.60 ns | 4.271 ns | 10.558 ns |         - |
| Increment                | 374.80 ns | 7.507 ns | 11.907 ns |         - |
| Decrement                | 369.70 ns | 7.295 ns | 14.229 ns |         - |
#### after (v2.x)
| Method                   |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|--------------------------|----------:|----------:|----------:|-------:|----------:|
| Addition                 | 16.219 ns | 0.0810 ns | 0.0718 ns |      - |         - |
| Subtraction              | 15.777 ns | 0.0662 ns | 0.0587 ns |      - |         - |
| CompareSameCurrency      |  3.623 ns | 0.0396 ns | 0.0351 ns |      - |         - |
| CompareDifferentCurrency |  3.684 ns | 0.0314 ns | 0.0293 ns |      - |         - |
| CompareAmount            |  3.908 ns | 0.0517 ns | 0.0459 ns |      - |         - |
| Increment                | 86.789 ns | 0.8525 ns | 0.7119 ns | 0.0038 |      32 B |
| Decrement                | 86.949 ns | 0.5347 ns | 0.5001 ns | 0.0038 |      32 B |
#### after MoneyContext
| Method                      |       Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-----------------------------|-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Add                         |  40.682 ns | 0.8362 ns | 0.8213 ns |  1.00 |    0.03 |      - |         - |          NA |
| AddFastMoney                |  12.074 ns | 0.2581 ns | 0.2869 ns |  0.30 |    0.01 |      - |         - |          NA |
| Subtract                    |  41.390 ns | 0.8617 ns | 1.2630 ns |  1.02 |    0.04 |      - |         - |          NA |
| SubtractFastMoney           |   6.014 ns | 0.1338 ns | 0.2547 ns |  0.15 |    0.01 |      - |         - |          NA |
| Multiple                    |  43.001 ns | 0.5586 ns | 0.5487 ns |  1.06 |    0.02 |      - |         - |          NA |
| MultipleFastMoneyDecimal    |  30.150 ns | 0.6217 ns | 1.0388 ns |  0.74 |    0.03 |      - |         - |          NA |
| MultipleFastWholeDecimal    |  15.131 ns | 0.3312 ns | 0.4534 ns |  0.37 |    0.01 |      - |         - |          NA |
| MultipleFastMoneyLong       |   2.120 ns | 0.0515 ns | 0.0457 ns |  0.05 |    0.00 |      - |         - |          NA |
| Divide                      |  75.664 ns | 0.6646 ns | 0.5891 ns |  1.86 |    0.04 |      - |         - |          NA |
| DivideFastMoneyDecimal      |  80.678 ns | 0.9934 ns | 0.9292 ns |  1.98 |    0.04 |      - |         - |          NA |
| DivideFastMoneyWholeDecimal |  13.197 ns | 0.2873 ns | 0.3420 ns |  0.32 |    0.01 |      - |         - |          NA |
| DivideFastMoneyLong         |   1.892 ns | 0.0584 ns | 0.0488 ns |  0.05 |    0.00 |      - |         - |          NA |
| CompareSameCurrency         |   8.707 ns | 0.1937 ns | 0.1902 ns |  0.21 |    0.01 |      - |         - |          NA |
| CompareDifferentCurrency    |   5.608 ns | 0.1049 ns | 0.0982 ns |  0.14 |    0.00 |      - |         - |          NA |
| CompareAmount               |   9.267 ns | 0.0749 ns | 0.0664 ns |  0.23 |    0.00 |      - |         - |          NA |
| Increment                   | 144.549 ns | 0.5306 ns | 0.4431 ns |  3.55 |    0.07 | 0.0038 |      32 B |          NA |
| Decrement                   | 102.890 ns | 1.9648 ns | 1.9297 ns |  2.53 |    0.07 | 0.0038 |      32 B |          NA |

## MoneyFormatting
#### before (v1.x)
| Method             |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit           | 194.0 ns | 3.93 ns |  9.10 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 197.6 ns | 3.98 ns |  8.39 ns | 0.0286 |     240 B |
| Explicit           | 271.5 ns | 5.50 ns |  9.33 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 270.3 ns | 5.42 ns | 12.56 ns | 0.0525 |     440 B |
#### after (v2.x)
| Method             |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|--------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit           | 106.3 ns | 2.06 ns | 5.06 ns | 0.0468 |     392 B |
| ImplicitWithFormat | 147.5 ns | 2.66 ns | 3.07 ns | 0.0505 |     424 B |
| Explicit           | 109.1 ns | 2.14 ns | 2.93 ns | 0.0468 |     392 B |
| ExplicitWithFormat | 144.8 ns | 2.92 ns | 5.56 ns | 0.0505 |     424 B |
#### after MoneyContext
| Method             |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|--------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit           | 113.6 ns | 2.32 ns | 2.67 ns | 0.0459 |     384 B |
| ImplicitWithFormat | 153.4 ns | 3.09 ns | 4.12 ns | 0.0496 |     416 B |
| Explicit           | 114.5 ns | 2.35 ns | 3.98 ns | 0.0459 |     384 B |
| ExplicitWithFormat | 142.3 ns | 2.83 ns | 4.80 ns | 0.0496 |     416 B |

## MoneyParsing
#### before (v1.x)
| Method      |        Mean |     Error |    StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 587.14 ns | 31,817.6 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 945.70 ns | 32,079.3 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns |  62.73 ns |    860.9 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns |  21.42 ns |    871.8 ns | 0.1469 |      - |      1 KB |
#### after (v2.x)
| Method            |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|-------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit          | 427.4 ns | 7.67 ns | 7.17 ns | 0.1173 |     984 B |
| ImplicitTry       | 426.3 ns | 6.59 ns | 6.17 ns | 0.1173 |     984 B |
| Explicit          | 442.4 ns | 7.58 ns | 9.85 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 442.1 ns | 6.54 ns | 6.12 ns | 0.1173 |     984 B |
| ExplicitTry       | 431.5 ns | 6.15 ns | 5.46 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 446.3 ns | 5.94 ns | 5.55 ns | 0.1173 |     984 B |
#### after MoneyContext
| Method            |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|-------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit          | 457.6 ns | 4.67 ns | 4.37 ns | 0.1173 |     984 B |
| ImplicitTry       | 469.2 ns | 6.15 ns | 5.76 ns | 0.1173 |     984 B |
| Explicit          | 455.1 ns | 4.85 ns | 4.30 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 466.4 ns | 8.15 ns | 7.62 ns | 0.1173 |     984 B |
| ExplicitTry       | 444.4 ns | 5.91 ns | 5.24 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 467.4 ns | 7.55 ns | 7.06 ns | 0.1173 |     984 B |

## HighLoad
#### before (v1.x)
| Method           |     Mean |    Error |   StdDev |      Gen 0 | Allocated |
|------------------|---------:|---------:|---------:|-----------:|----------:|
| Create1MCurrency | 482.6 ms |  9.57 ms | 26.68 ms | 75000.0000 |    679 MB |
| Create1MMoney    | 516.8 ms | 10.33 ms | 28.46 ms | 75000.0000 |    694 MB |
#### after (v2.x)
| Method            |      Mean |     Error |    StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
|-------------------|----------:|----------:|----------:|---------:|---------:|---------:|----------:|
| Create1MCurrency  | 13.474 ms | 0.1538 ms | 0.1201 ms | 390.6250 | 390.6250 | 312.5000 |   1.91 MB |
| Create1MMoney     | 30.124 ms | 0.6020 ms | 0.7166 ms | 656.2500 | 656.2500 | 656.2500 |  22.89 MB |
| Create1MFastMoney | 42.112 ms | 0.7915 ms | 0.7017 ms | 583.3333 | 583.3333 | 583.3333 |  15.26 MB |
#### after MoneyContext
| Method            |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------|----------:|----------:|----------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 14.205 ms | 0.2033 ms | 0.1901 ms |  0.38 |    0.01 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MDecimal   |  3.686 ms | 0.0709 ms | 0.1804 ms |  0.10 |    0.01 | 597.6563 | 597.6563 | 597.6563 |  15.26 MB |        1.00 |
| Create1MMoney     | 37.325 ms | 0.7451 ms | 0.8580 ms |  1.00 |    0.03 | 571.4286 | 571.4286 | 571.4286 |  15.26 MB |        1.00 |
| Create1MFastMoney | 30.593 ms | 0.2963 ms | 0.2627 ms |  0.82 |    0.02 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 21.264 ms | 0.4239 ms | 0.4536 ms |  0.57 |    0.02 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
