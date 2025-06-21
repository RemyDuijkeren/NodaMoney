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
| CurrencyCode                  | 34.350 ns | 0.6490 ns | 0.5420 ns |  1.00 |    0.02 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 42.055 ns | 0.2246 ns | 0.2101 ns |  1.22 |    0.02 |      - |         - |          NA |
| CurrencyCodeAndContext        | 37.394 ns | 0.3669 ns | 0.3252 ns |  1.09 |    0.02 |      - |         - |          NA |
| CurrencyFromCode              | 33.782 ns | 0.6735 ns | 0.6300 ns |  0.98 |    0.02 |      - |         - |          NA |
| CurrencyInfoFromCode          | 33.763 ns | 0.3874 ns | 0.3624 ns |  0.98 |    0.02 |      - |         - |          NA |
| ExtensionMethodEuro           | 34.462 ns | 0.7082 ns | 0.5914 ns |  1.00 |    0.02 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 82.032 ns | 0.6266 ns | 0.5555 ns |  2.39 |    0.04 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 80.768 ns | 0.8416 ns | 0.7461 ns |  2.35 |    0.04 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.042 ns | 0.0189 ns | 0.0168 ns |  0.03 |    0.00 |      - |         - |          NA |

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
| CompareDifferentCurrency |  3.684 ns | 0.0314 ns | 0.0293 ns |      - |         - |****
| CompareAmount            |  3.908 ns | 0.0517 ns | 0.0459 ns |      - |         - |
| Increment                | 86.789 ns | 0.8525 ns | 0.7119 ns | 0.0038 |      32 B |
| Decrement                | 86.949 ns | 0.5347 ns | 0.5001 ns | 0.0038 |      32 B |
#### after MoneyContext
| Method                      |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-----------------------------|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Add                         | 18.810 ns | 0.1702 ns | 0.1592 ns |  1.00 |    0.01 |      - |         - |          NA |
| AddFastMoney                | 11.919 ns | 0.2317 ns | 0.1935 ns |  0.63 |    0.01 |      - |         - |          NA |
| AddSqlMoney                 |  4.620 ns | 0.1116 ns | 0.1285 ns |  0.25 |    0.01 |      - |         - |          NA |
| Subtract                    | 19.079 ns | 0.3416 ns | 0.3655 ns |  1.01 |    0.02 |      - |         - |          NA |
| SubtractFastMoney           |  5.830 ns | 0.0873 ns | 0.0817 ns |  0.31 |    0.00 |      - |         - |          NA |
| SubtractSqlMoney            |  4.589 ns | 0.0669 ns | 0.0626 ns |  0.24 |    0.00 |      - |         - |          NA |
| Multiple                    | 14.984 ns | 0.1805 ns | 0.1600 ns |  0.80 |    0.01 |      - |         - |          NA |
| MultipleFastMoneyDecimal    | 28.512 ns | 0.1249 ns | 0.0975 ns |  1.52 |    0.01 |      - |         - |          NA |
| MultipleFastWholeDecimal    | 14.076 ns | 0.1911 ns | 0.1694 ns |  0.75 |    0.01 |      - |         - |          NA |
| MultipleFastMoneyLong       |  2.045 ns | 0.0178 ns | 0.0166 ns |  0.11 |    0.00 |      - |         - |          NA |
| MultipleSqlMoney            | 36.845 ns | 0.2911 ns | 0.2723 ns |  1.96 |    0.02 |      - |         - |          NA |
| Divide                      | 48.496 ns | 0.2710 ns | 0.2403 ns |  2.58 |    0.02 |      - |         - |          NA |
| DivideFastMoneyDecimal      | 78.071 ns | 0.6186 ns | 0.5484 ns |  4.15 |    0.04 |      - |         - |          NA |
| DivideFastMoneyWholeDecimal | 12.769 ns | 0.1158 ns | 0.1084 ns |  0.68 |    0.01 |      - |         - |          NA |
| DivideFastMoneyLong         |  1.847 ns | 0.0209 ns | 0.0195 ns |  0.10 |    0.00 |      - |         - |          NA |
| DivideSqlMoney              | 31.660 ns | 0.2767 ns | 0.2588 ns |  1.68 |    0.02 |      - |         - |          NA |
| CompareSameCurrency         |  9.251 ns | 0.0728 ns | 0.0608 ns |  0.49 |    0.01 |      - |         - |          NA |
| CompareDifferentCurrency    |  5.467 ns | 0.0416 ns | 0.0390 ns |  0.29 |    0.00 |      - |         - |          NA |
| CompareAmount               |  8.806 ns | 0.0824 ns | 0.0730 ns |  0.47 |    0.01 |      - |         - |          NA |
| Increment                   | 71.687 ns | 0.4365 ns | 0.3869 ns |  3.81 |    0.04 | 0.0038 |      32 B |          NA |
| Decrement                   | 70.899 ns | 0.6792 ns | 0.6021 ns |  3.77 |    0.04 | 0.0038 |      32 B |          NA |

| Method                      |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-----------------------------|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Add                         | 19.255 ns | 0.3595 ns | 0.3187 ns |  1.00 |    0.02 |      - |         - |          NA |
| AddFastMoney                | 11.186 ns | 0.2283 ns | 0.1906 ns |  0.58 |    0.01 |      - |         - |          NA |
| AddSqlMoney                 |  4.515 ns | 0.0450 ns | 0.0421 ns |  0.23 |    0.00 |      - |         - |          NA |
| Subtract                    | 19.318 ns | 0.1359 ns | 0.1204 ns |  1.00 |    0.02 |      - |         - |          NA |
| SubtractFastMoney           |  5.820 ns | 0.0669 ns | 0.0626 ns |  0.30 |    0.01 |      - |         - |          NA |
| SubtractSqlMoney            |  4.560 ns | 0.0466 ns | 0.0413 ns |  0.24 |    0.00 |      - |         - |          NA |
| Multiple                    | 14.670 ns | 0.3179 ns | 0.4759 ns |  0.76 |    0.03 |      - |         - |          NA |
| MultipleFastMoneyDecimal    | 23.035 ns | 0.2813 ns | 0.2493 ns |  1.20 |    0.02 |      - |         - |          NA |
| MultipleFastWholeDecimal    |  9.656 ns | 0.0365 ns | 0.0341 ns |  0.50 |    0.01 |      - |         - |          NA |
| MultipleFastMoneyLong       |  2.083 ns | 0.0200 ns | 0.0167 ns |  0.11 |    0.00 |      - |         - |          NA |
| MultipleSqlMoney            | 37.668 ns | 0.4015 ns | 0.3559 ns |  1.96 |    0.04 |      - |         - |          NA |
| Divide                      | 49.276 ns | 0.2035 ns | 0.1804 ns |  2.56 |    0.04 |      - |         - |          NA |
| DivideFastMoneyDecimal      | 77.804 ns | 0.2935 ns | 0.2746 ns |  4.04 |    0.06 |      - |         - |          NA |
| DivideFastMoneyWholeDecimal | 12.544 ns | 0.0714 ns | 0.0668 ns |  0.65 |    0.01 |      - |         - |          NA |
| DivideFastMoneyLong         |  1.837 ns | 0.0109 ns | 0.0102 ns |  0.10 |    0.00 |      - |         - |          NA |
| DivideSqlMoney              | 30.074 ns | 0.1910 ns | 0.1595 ns |  1.56 |    0.03 |      - |         - |          NA |
| CompareSameCurrency         |  8.443 ns | 0.0895 ns | 0.0793 ns |  0.44 |    0.01 |      - |         - |          NA |
| CompareDifferentCurrency    |  5.786 ns | 0.0482 ns | 0.0427 ns |  0.30 |    0.01 |      - |         - |          NA |
| CompareAmount               |  9.002 ns | 0.0462 ns | 0.0409 ns |  0.47 |    0.01 |      - |         - |          NA |
| Increment                   | 70.073 ns | 0.6136 ns | 0.5740 ns |  3.64 |    0.06 | 0.0038 |      32 B |          NA |
| Decrement                   | 70.791 ns | 0.7494 ns | 0.6643 ns |  3.68 |    0.07 | 0.0038 |      32 B |          NA |



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
| Implicit           | 111.4 ns | 2.27 ns | 4.93 ns | 0.0458 |     384 B |
| ImplicitWithFormat | 146.5 ns | 2.99 ns | 4.09 ns | 0.0496 |     416 B |
| Explicit           | 112.9 ns | 1.60 ns | 1.50 ns | 0.0459 |     384 B |
| ExplicitWithFormat | 145.2 ns | 2.86 ns | 3.91 ns | 0.0496 |     416 B |

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
| Implicit          | 466.0 ns | 8.82 ns | 8.25 ns | 0.1173 |     984 B |
| ImplicitTry       | 465.4 ns | 9.13 ns | 8.54 ns | 0.1173 |     984 B |
| Explicit          | 460.2 ns | 8.61 ns | 8.84 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 451.5 ns | 5.51 ns | 4.89 ns | 0.1173 |     984 B |
| ExplicitTry       | 466.3 ns | 8.51 ns | 7.96 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 470.4 ns | 7.17 ns | 6.70 ns | 0.1173 |     984 B |

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
| Create1MCurrency  | 13.641 ms | 0.2538 ms | 0.2493 ms |  0.34 |    0.01 | 390.6250 | 390.6250 | 312.5000 |   1.91 MB |        0.13 |
| Create1MDecimal   |  4.547 ms | 0.1031 ms | 0.2974 ms |  0.11 |    0.01 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
| Create1MMoney     | 39.792 ms | 0.7854 ms | 1.5865 ms |  1.00 |    0.06 | 545.4545 | 545.4545 | 545.4545 |  15.26 MB |        1.00 |
| Create1MFastMoney | 32.227 ms | 0.6372 ms | 0.6818 ms |  0.81 |    0.04 | 466.6667 | 466.6667 | 466.6667 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 22.756 ms | 0.4413 ms | 0.7613 ms |  0.57 |    0.03 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
