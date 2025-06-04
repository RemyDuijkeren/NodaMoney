## InitializingCurrency
#### before (v1.x)
| Method           |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|------------------|---------:|--------:|---------:|-------:|----------:|
| CurrencyFromCode | 443.7 ns | 8.90 ns | 14.87 ns | 0.0753 |     632 B |
#### after (v2.x)
| Method               |      Mean |     Error |    StdDev | Allocated |
|----------------------|----------:|----------:|----------:|----------:|
| CurrencyFromCode     | 14.787 ns | 0.0481 ns | 0.0427 ns |         - |
| CurrencyInfoFromCode |  7.272 ns | 0.0670 ns | 0.0627 ns |         - |

## InitializingMoney
#### before (v1.x)
| Method                              |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Allocated |
|-------------------------------------|----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|
| ExplicitCurrencyCodeDefaultRounding | 483.87 ns |  6.370 ns |  5.319 ns | 483.75 ns |  1.00 |    0.00 | 0.0753 |     632 B |
| ExplicitCurrencyCodeAndRounding     | 496.87 ns |  9.720 ns | 12.976 ns | 493.09 ns |  1.02 |    0.03 | 0.0753 |     632 B |
| ExplicitCurrencyFromCode            | 521.25 ns | 16.493 ns | 48.630 ns | 494.71 ns |  1.16 |    0.03 | 0.0753 |     632 B |
| ExtensionMethod                     | 490.89 ns |  7.103 ns |  5.931 ns | 492.29 ns |  1.01 |    0.02 | 0.0753 |     632 B |
| ImplicitCurrencyByConstructor       | 114.69 ns |  2.276 ns |  2.017 ns | 115.09 ns |  0.24 |    0.00 | 0.0057 |      48 B |
| ImplicitCurrencyByCasting           | 113.58 ns |  1.579 ns |  1.477 ns | 113.67 ns |  0.23 |    0.00 | 0.0057 |      48 B |
| Deconstruct                         |  34.86 ns |  0.348 ns |  0.309 ns |  34.80 ns |  0.07 |    0.00 |      - |         - |
#### after (v2.x)
| Method                              |       Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------------|-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| ExplicitCurrencyCodeDefaultRounding | 32.4288 ns | 0.2609 ns | 0.2441 ns | 1.000 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyCodeAndRounding     | 35.0707 ns | 0.2325 ns | 0.2174 ns | 1.082 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyFromCode            | 32.8033 ns | 0.1516 ns | 0.1344 ns | 1.012 |    0.01 |      - |         - |          NA |
| ExplicitCurrencyInfoFromCode        | 32.1320 ns | 0.1910 ns | 0.1595 ns | 0.991 |    0.01 |      - |         - |          NA |
| ExtensionMethod                     | 34.2212 ns | 0.4191 ns | 0.3920 ns | 1.055 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor       | 75.4426 ns | 0.4001 ns | 0.3546 ns | 2.327 |    0.02 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting           | 75.0124 ns | 0.3120 ns | 0.2766 ns | 2.313 |    0.02 | 0.0038 |      32 B |          NA |
| Deconstruct                         |  0.2785 ns | 0.0068 ns | 0.0063 ns | 0.009 |    0.00 |      - |         - |          NA |
#### after MoneyContext
| Method                        |          Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|--------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| CurrencyCodeDefaultRounding   |     34.113 ns | 0.2930 ns | 0.2597 ns |  1.00 |    0.01 |      - |         - |          NA |
| CurrencyCodeAndRounding       | **76.983 ns** | 0.8848 ns | 0.7388 ns |  2.26 |    0.03 | 0.0114 |      96 B |          NA |
| CurrencyCodeAndContext        |     40.134 ns | 0.5557 ns | 0.4640 ns |  1.18 |    0.02 |      - |         - |          NA |
| CurrencyFromCode              |     35.851 ns | 0.2022 ns | 0.1793 ns |  1.05 |    0.01 |      - |         - |          NA |
| CurrencyInfoFromCode          |     33.617 ns | 0.1626 ns | 0.1521 ns |  0.99 |    0.01 |      - |         - |          NA |
| ExtensionMethodEuro           |     34.023 ns | 0.3055 ns | 0.2551 ns |  1.00 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor |     81.072 ns | 1.5320 ns | 1.4330 ns |  2.38 |    0.04 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     |     80.194 ns | 0.3839 ns | 0.3591 ns |  2.35 |    0.02 | 0.0038 |      32 B |          NA |
| Deconstruct                   |      1.163 ns | 0.0417 ns | 0.0348 ns |  0.03 |    0.00 |      - |         - |          NA |


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
| Method                   |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|--------------------------|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Addition                 | 16.607 ns | 0.2187 ns | 0.2046 ns |  1.00 |    0.02 |      - |         - |          NA |
| AdditionFastMoney        |  5.431 ns | 0.0977 ns | 0.1337 ns |  0.33 |    0.01 |      - |         - |          NA |
| AdditionSqlMoney         |  4.627 ns | 0.1068 ns | 0.1049 ns |  0.28 |    0.01 |      - |         - |          NA |
| Subtraction              | 16.281 ns | 0.0725 ns | 0.0606 ns |  0.98 |    0.01 |      - |         - |          NA |
| SubtractionFastMoney     |  5.290 ns | 0.0336 ns | 0.0262 ns |  0.32 |    0.00 |      - |         - |          NA |
| SubtractionSqlMoney      |  4.586 ns | 0.0434 ns | 0.0339 ns |  0.28 |    0.00 |      - |         - |          NA |
| CompareSameCurrency      |  8.196 ns | 0.0494 ns | 0.0438 ns |  0.49 |    0.01 |      - |         - |          NA |
| CompareDifferentCurrency |  5.364 ns | 0.0904 ns | 0.0801 ns |  0.32 |    0.01 |      - |         - |          NA |
| CompareAmount            |  9.011 ns | 0.0746 ns | 0.0662 ns |  0.54 |    0.01 |      - |         - |          NA |
| Increment                | 70.491 ns | 0.4234 ns | 0.3961 ns |  4.25 |    0.06 | 0.0038 |      32 B |          NA |
| Decrement                | 77.344 ns | 0.5503 ns | 0.5148 ns |  4.66 |    0.06 | 0.0038 |      32 B |          NA |

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
| Method              |       Mean |     Error |    StdDev |     Gen0 |   Allocated |
| ------------------- | ---------: | --------: | --------: | -------: | ----------: |
| Implicit            |   106.8 ns |   2.07 ns |   2.12 ns |   0.0459 |       384 B |
| ImplicitWithFormat  |   137.7 ns |   2.07 ns |   1.94 ns |   0.0496 |       416 B |
| Explicit            |   105.8 ns |   2.16 ns |   4.36 ns |   0.0459 |       384 B |
| ExplicitWithFormat  |   133.0 ns |   1.77 ns |   1.65 ns |   0.0496 |       416 B |

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
| Method            |     Mean |   Error |   StdDev |   Gen0 | Allocated |
|-------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit          | 412.9 ns | 6.01 ns |  5.62 ns | 0.1173 |     984 B |
| ImplicitTry       | 433.0 ns | 8.60 ns | 16.35 ns | 0.1173 |     984 B |
| Explicit          | 439.7 ns | 7.32 ns |  6.85 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 438.2 ns | 5.15 ns |  4.56 ns | 0.1173 |     984 B |
| ExplicitTry       | 468.4 ns | 8.98 ns |  9.61 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 445.3 ns | 8.29 ns |  7.75 ns | 0.1173 |     984 B |

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
| Method            |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------|----------:|----------:|----------:|----------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 13.526 ms | 0.1908 ms | 0.1785 ms | 13.455 ms |  0.37 |    0.01 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MDecimal   |  3.596 ms | 0.0721 ms | 0.2114 ms |  3.530 ms |  0.10 |    0.01 | 597.6563 | 597.6563 | 597.6563 |  15.26 MB |        1.00 |
| Create1MMoney     | 36.177 ms | 0.2327 ms | 0.1943 ms | 36.124 ms |  1.00 |    0.01 | 571.4286 | 571.4286 | 571.4286 |  15.26 MB |        1.00 |
| Create1MFastMoney | 35.573 ms | 0.5687 ms | 0.5320 ms | 35.348 ms |  0.98 |    0.02 | 466.6667 | 466.6667 | 466.6667 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 21.038 ms | 0.2447 ms | 0.2043 ms | 20.984 ms |  0.58 |    0.01 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
