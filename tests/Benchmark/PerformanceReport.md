## InitializingCurrency
#### before (v1.x)
| Method               |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|----------------------|---------:|--------:|---------:|-------:|----------:|
| FromCode             | 443.7 ns | 8.90 ns | 14.87 ns | 0.0753 |     632 B |
#### after
| Method               |      Mean |     Error |    StdDev | Allocated |
|----------------------|----------:|----------:|----------:|----------:|
| CurrencyFromCode     | 14.529 ns | 0.1348 ns | 0.1125 ns |         - |
| CurrencyInfoFromCode |  7.251 ns | 0.0714 ns | 0.0668 ns |         - |

## InitializingMoney
#### before (v1.x)
| Method                          |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Allocated |
|---------------------------------|----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|
| ExplicitCurrencyCode            | 483.87 ns |  6.370 ns |  5.319 ns | 483.75 ns |  1.00 |    0.00 | 0.0753 |     632 B |
| ExplicitCurrencyCodeAndRounding | 496.87 ns |  9.720 ns | 12.976 ns | 493.09 ns |  1.02 |    0.03 | 0.0753 |     632 B |
| ExplicitCurrencyFromCode        | 521.25 ns | 16.493 ns | 48.630 ns | 494.71 ns |  1.16 |    0.03 | 0.0753 |     632 B |
| ExtensionMethod                 | 490.89 ns |  7.103 ns |  5.931 ns | 492.29 ns |  1.01 |    0.02 | 0.0753 |     632 B |
| ImplicitCurrencyByConstructor   | 114.69 ns |  2.276 ns |  2.017 ns | 115.09 ns |  0.24 |    0.00 | 0.0057 |      48 B |
| ImplicitCurrencyByCasting       | 113.58 ns |  1.579 ns |  1.477 ns | 113.67 ns |  0.23 |    0.00 | 0.0057 |      48 B |
| Deconstruct                     |  34.86 ns |  0.348 ns |  0.309 ns |  34.80 ns |  0.07 |    0.00 |      - |         - |
#### after
| Method                          |       Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|---------------------------------|-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| ExplicitCurrencyCodeA           | 32.4288 ns | 0.2609 ns | 0.2441 ns | 1.000 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyCodeAndRounding | 35.0707 ns | 0.2325 ns | 0.2174 ns | 1.082 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyFromCode        | 32.8033 ns | 0.1516 ns | 0.1344 ns | 1.012 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyInfoFromCode    | 32.1320 ns | 0.1910 ns | 0.1595 ns | 0.991 |    0.01 |      - |         - |          NA |
| ExtensionMethod                 | 34.2212 ns | 0.4191 ns | 0.3920 ns | 1.055 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor   | 75.4426 ns | 0.4001 ns | 0.3546 ns | 2.327 |    0.02 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting       | 75.0124 ns | 0.3120 ns | 0.2766 ns | 2.313 |    0.02 | 0.0038 |      32 B |          NA |
| Deconstruct                     |  0.2785 ns | 0.0068 ns | 0.0063 ns | 0.009 |    0.00 |      - |         - |          NA |
#### after PackedDecimal and MoneyContext
| Method                          |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|---------------------------------|----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| ExplicitCurrencyCodeA           | 35.269 ns | 0.8674 ns | 2.4608 ns | 34.335 ns |  1.00 |    0.10 |      - |         - |          NA |
| ExplicitCurrencyCodeAndRounding | 78.217 ns | 1.5908 ns | 2.7013 ns | 77.258 ns |  2.23 |    0.17 | 0.0029 |      24 B |          NA |
| ExplicitCurrencyFromCode        | 33.601 ns | 0.6858 ns | 0.6415 ns | 33.484 ns |  0.96 |    0.07 |      - |         - |          NA |
| ExplicitCurrencyInfoFromCode    | 34.462 ns | 0.7035 ns | 0.8640 ns | 34.279 ns |  0.98 |    0.07 |      - |         - |          NA |
| ExtensionMethod                 | 34.365 ns | 0.6393 ns | 0.7611 ns | 34.307 ns |  0.98 |    0.07 |      - |         - |          NA |
| ImplicitCurrencyByConstructor   | 79.728 ns | 1.4831 ns | 1.5231 ns | 79.306 ns |  2.27 |    0.16 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting       | 80.892 ns | 1.6322 ns | 1.7464 ns | 80.499 ns |  2.30 |    0.16 | 0.0038 |      32 B |          NA |
| Deconstruct                     |  1.054 ns | 0.0408 ns | 0.0381 ns |  1.048 ns |  0.03 |    0.00 |      - |         - |          NA |

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
#### after
| Method                   |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|--------------------------|----------:|----------:|----------:|-------:|----------:|
| Addition                 | 16.219 ns | 0.0810 ns | 0.0718 ns |      - |         - |
| Subtraction              | 15.777 ns | 0.0662 ns | 0.0587 ns |      - |         - |
| CompareSameCurrency      |  3.623 ns | 0.0396 ns | 0.0351 ns |      - |         - |
| CompareDifferentCurrency |  3.684 ns | 0.0314 ns | 0.0293 ns |      - |         - |
| CompareAmount            |  3.908 ns | 0.0517 ns | 0.0459 ns |      - |         - |
| Increment                | 86.789 ns | 0.8525 ns | 0.7119 ns | 0.0038 |      32 B |
| Decrement                | 86.949 ns | 0.5347 ns | 0.5001 ns | 0.0038 |      32 B |
#### after PackedDecimal and MoneyContext
| Method                   |       Mean |     Error |    StdDev |   Gen0 | Allocated |
|--------------------------|-----------:|----------:|----------:|-------:|----------:|
| Addition                 |  27.445 ns | 0.4171 ns | 0.3697 ns |      - |         - |
| Subtraction              |  26.682 ns | 0.4386 ns | 0.3888 ns |      - |         - |
| CompareSameCurrency      |   8.436 ns | 0.1747 ns | 0.1549 ns |      - |         - |
| CompareDifferentCurrency |   5.328 ns | 0.1371 ns | 0.1347 ns |      - |         - |
| CompareAmount            |   8.581 ns | 0.0929 ns | 0.0869 ns |      - |         - |
| Increment                |  97.553 ns | 1.8742 ns | 2.0054 ns | 0.0038 |      32 B |
| Decrement                | 102.372 ns | 0.3815 ns | 0.3382 ns | 0.0038 |      32 B |

## MoneyFormatting
#### before (v1.x)
| Method             |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit           | 194.0 ns | 3.93 ns |  9.10 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 197.6 ns | 3.98 ns |  8.39 ns | 0.0286 |     240 B |
| Explicit           | 271.5 ns | 5.50 ns |  9.33 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 270.3 ns | 5.42 ns | 12.56 ns | 0.0525 |     440 B |
#### after
| Method             |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|--------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit           | 106.3 ns | 2.06 ns | 5.06 ns | 0.0468 |     392 B |
| ImplicitWithFormat | 147.5 ns | 2.66 ns | 3.07 ns | 0.0505 |     424 B |
| Explicit           | 109.1 ns | 2.14 ns | 2.93 ns | 0.0468 |     392 B |
| ExplicitWithFormat | 144.8 ns | 2.92 ns | 5.56 ns | 0.0505 |     424 B |
#### after PackedDecimal and MoneyContext
| Method             |     Mean |   Error |  StdDev |   Median |   Gen0 | Allocated |
|--------------------|---------:|--------:|--------:|---------:|-------:|----------:|
| Implicit           | 128.1 ns | 2.76 ns | 7.83 ns | 125.4 ns | 0.0458 |     384 B |
| ImplicitWithFormat | 153.9 ns | 3.01 ns | 2.95 ns | 154.0 ns | 0.0496 |     416 B |
| Explicit           | 123.7 ns | 2.18 ns | 3.87 ns | 123.0 ns | 0.0458 |     384 B |
| ExplicitWithFormat | 157.1 ns | 3.18 ns | 5.48 ns | 155.2 ns | 0.0496 |     416 B |

## MoneyParsing
#### before (v1.x)
| Method      |        Mean |     Error |    StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 587.14 ns | 31,817.6 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 945.70 ns | 32,079.3 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns |  62.73 ns |    860.9 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns |  21.42 ns |    871.8 ns | 0.1469 |      - |      1 KB |
#### after
| Method            |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|-------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit          | 427.4 ns | 7.67 ns | 7.17 ns | 0.1173 |     984 B |
| ImplicitTry       | 426.3 ns | 6.59 ns | 6.17 ns | 0.1173 |     984 B |
| Explicit          | 442.4 ns | 7.58 ns | 9.85 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 442.1 ns | 6.54 ns | 6.12 ns | 0.1173 |     984 B |
| ExplicitTry       | 431.5 ns | 6.15 ns | 5.46 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 446.3 ns | 5.94 ns | 5.55 ns | 0.1173 |     984 B |
#### after PackedDecimal and MoneyContext
| Method            |     Mean |    Error |   StdDev |   Gen0 | Allocated |
|-------------------|---------:|---------:|---------:|-------:|----------:|
| Implicit          | 481.9 ns |  9.48 ns | 12.32 ns | 0.1173 |     984 B |
| ImplicitTry       | 472.5 ns |  9.12 ns |  8.96 ns | 0.1173 |     984 B |
| Explicit          | 493.1 ns |  9.69 ns |  8.09 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 504.9 ns | 10.09 ns | 15.10 ns | 0.1173 |     984 B |
| ExplicitTry       | 489.8 ns |  4.45 ns |  3.72 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 486.1 ns |  9.44 ns | 12.28 ns | 0.1173 |     984 B |

## HighLoad
#### before (v1.x)
| Method           |     Mean |    Error |   StdDev |      Gen 0 | Allocated |
|------------------|---------:|---------:|---------:|-----------:|----------:|
| Create1MCurrency | 482.6 ms |  9.57 ms | 26.68 ms | 75000.0000 |    679 MB |
| Create1MMoney    | 516.8 ms | 10.33 ms | 28.46 ms | 75000.0000 |    694 MB |
#### after
| Method            |      Mean |     Error |    StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
|-------------------|----------:|----------:|----------:|---------:|---------:|---------:|----------:|
| Create1MCurrency  | 13.474 ms | 0.1538 ms | 0.1201 ms | 390.6250 | 390.6250 | 312.5000 |   1.91 MB |
| Create1MMoney     | 30.124 ms | 0.6020 ms | 0.7166 ms | 656.2500 | 656.2500 | 656.2500 |  22.89 MB |
| Create1MFastMoney | 42.112 ms | 0.7915 ms | 0.7017 ms | 583.3333 | 583.3333 | 583.3333 |  15.26 MB |
#### after PackedDecimal and MoneyContext
| Method                |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-----------------------|----------:|----------:|----------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency      | 14.128 ms | 0.2566 ms | 0.4217 ms |  2.80 |    0.26 | 437.5000 | 437.5000 | 281.2500 |   1.91 MB |        0.13 |
| Create1MDecimal       |  5.091 ms | 0.1639 ms | 0.4728 ms |  1.01 |    0.13 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
| Create1MPackedDecimal | 11.743 ms | 0.2143 ms | 0.2005 ms |  2.33 |    0.21 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
| Create1MMoney         | 38.100 ms | 0.6791 ms | 0.6353 ms |  7.54 |    0.67 | 571.4286 | 571.4286 | 571.4286 |  15.26 MB |        1.00 |
| Create1MFastMoney     | 44.415 ms | 0.8609 ms | 1.0887 ms |  8.79 |    0.79 | 583.3333 | 583.3333 | 583.3333 |  15.26 MB |        1.00 |
