## InitializingCurrency
#### v1
| Method           |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|------------------|---------:|--------:|---------:|-------:|----------:|
| CurrencyFromCode | 443.7 ns | 8.90 ns | 14.87 ns | 0.0753 |     632 B |
#### v2
| Method               |      Mean |     Error |    StdDev | Allocated |
|----------------------|----------:|----------:|----------:|----------:|
| CurrencyFromCode     | 14.516 ns | 0.0580 ns | 0.0453 ns |         - |
| CurrencyInfoFromCode |  8.098 ns | 0.0537 ns | 0.0503 ns |         - |
#### v2.5
| Method                  |      Mean |     Error |    StdDev | Allocated |
|-------------------------|----------:|----------:|----------:|----------:|
| CurrencyFromCode        | 16.849 ns | 0.3473 ns | 0.3079 ns |         - |
| CurrencyInfoFromCode    |  9.813 ns | 0.2050 ns | 0.1817 ns |         - |
| CurrencyInfoTryFromCode |  6.914 ns | 0.1688 ns | 0.1806 ns |         - |

## InitializingMoney
#### v1
| Method                        |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Allocated |
|-------------------------------|----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|
| CurrencyCode                  | 483.87 ns |  6.370 ns |  5.319 ns | 483.75 ns |  1.00 |    0.00 | 0.0753 |     632 B |
| CurrencyCodeAndRoundingMode   | 496.87 ns |  9.720 ns | 12.976 ns | 493.09 ns |  1.02 |    0.03 | 0.0753 |     632 B |
| CurrencyFromCode              | 521.25 ns | 16.493 ns | 48.630 ns | 494.71 ns |  1.16 |    0.03 | 0.0753 |     632 B |
| ExtensionMethod               | 490.89 ns |  7.103 ns |  5.931 ns | 492.29 ns |  1.01 |    0.02 | 0.0753 |     632 B |
| ImplicitCurrencyByConstructor | 114.69 ns |  2.276 ns |  2.017 ns | 115.09 ns |  0.24 |    0.00 | 0.0057 |      48 B |
| ImplicitCurrencyByCasting     | 113.58 ns |  1.579 ns |  1.477 ns | 113.67 ns |  0.23 |    0.00 | 0.0057 |      48 B |
| Deconstruct                   |  34.86 ns |  0.348 ns |  0.309 ns |  34.80 ns |  0.07 |    0.00 |      - |         - |
#### v2
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
#### v2.5
| Method                        |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| CurrencyCode                  | 39.391 ns | 0.7889 ns | 0.8442 ns |  1.00 |    0.03 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 45.752 ns | 0.3953 ns | 0.3504 ns |  1.16 |    0.03 |      - |         - |          NA |
| CurrencyCodeAndContext        | 43.561 ns | 0.8792 ns | 0.9029 ns |  1.11 |    0.03 |      - |         - |          NA |
| CurrencyFromCode              | 36.802 ns | 0.3324 ns | 0.3109 ns |  0.93 |    0.02 |      - |         - |          NA |
| CurrencyInfoFromCode          | 39.168 ns | 0.7141 ns | 0.9285 ns |  0.99 |    0.03 |      - |         - |          NA |
| ExtensionMethodEuro           | 38.033 ns | 0.4967 ns | 0.3878 ns |  0.97 |    0.02 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 85.986 ns | 1.3721 ns | 1.1457 ns |  2.18 |    0.05 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 85.284 ns | 1.6675 ns | 1.5598 ns |  2.17 |    0.06 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.092 ns | 0.0485 ns | 0.0476 ns |  0.03 |    0.00 |      - |         - |          NA |

## MoneyOperations
#### v1
| Method           |      Mean |    Error |    StdDev | Allocated |
|------------------|----------:|---------:|----------:|----------:|
| Add              | 231.00 ns | 4.470 ns |  4.181 ns |         - |
| Subtract         | 233.42 ns | 4.665 ns |  4.581 ns |         - |
| Equal            |  30.09 ns | 0.518 ns |  0.485 ns |         - |
| NotEqualCurrency |  68.83 ns | 0.679 ns |  0.530 ns |         - |
| Bigger           | 212.60 ns | 4.271 ns | 10.558 ns |         - |
| Increment        | 374.80 ns | 7.507 ns | 11.907 ns |         - |
| Decrement        | 369.70 ns | 7.295 ns | 14.229 ns |         - |
#### v2
| Method           |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------|----------:|----------:|----------:|-------:|----------:|
| Add              | 16.219 ns | 0.0810 ns | 0.0718 ns |      - |         - |
| Subtract         | 15.777 ns | 0.0662 ns | 0.0587 ns |      - |         - |
| Equal            |  3.623 ns | 0.0396 ns | 0.0351 ns |      - |         - |
| NotEqualCurrency |  3.684 ns | 0.0314 ns | 0.0293 ns |      - |         - |
| Bigger           |  3.908 ns | 0.0517 ns | 0.0459 ns |      - |         - |
| Increment        | 86.789 ns | 0.8525 ns | 0.7119 ns | 0.0038 |      32 B |
| Decrement        | 86.949 ns | 0.5347 ns | 0.5001 ns | 0.0038 |      32 B |
#### v2.5
| Method                      |           Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-----------------------------|---------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Add                         | **31.9243 ns** | 0.4512 ns | 0.4221 ns |  1.00 |    0.02 |      - |         - |          NA |
| AddFastMoney                |     12.8192 ns | 0.2753 ns | 0.3170 ns |  0.40 |    0.01 |      - |         - |          NA |
| Subtract                    | **31.9569 ns** | 0.6448 ns | 0.6622 ns |  1.00 |    0.02 |      - |         - |          NA |
| SubtractFastMoney           |      5.9472 ns | 0.1418 ns | 0.1742 ns |  0.19 |    0.01 |      - |         - |          NA |
| Multiple                    |     30.9518 ns | 0.3779 ns | 0.3155 ns |  0.97 |    0.02 |      - |         - |          NA |
| MultipleFastMoneyDecimal    |     29.3686 ns | 0.4207 ns | 0.3730 ns |  0.92 |    0.02 |      - |         - |          NA |
| MultipleFastWholeDecimal    |     14.7001 ns | 0.2570 ns | 0.2404 ns |  0.46 |    0.01 |      - |         - |          NA |
| MultipleFastMoneyLong       |      2.1357 ns | 0.0541 ns | 0.0480 ns |  0.07 |    0.00 |      - |         - |          NA |
| Divide                      |     86.2043 ns | 0.7083 ns | 0.6626 ns |  2.70 |    0.04 |      - |         - |          NA |
| DivideFastMoneyDecimal      |     82.4669 ns | 1.6285 ns | 1.4436 ns |  2.58 |    0.05 |      - |         - |          NA |
| DivideFastMoneyWholeDecimal |     13.1605 ns | 0.2143 ns | 0.1900 ns |  0.41 |    0.01 |      - |         - |          NA |
| DivideFastMoneyLong         |      1.9328 ns | 0.0350 ns | 0.0328 ns |  0.06 |    0.00 |      - |         - |          NA |
| Equal                       |      0.8828 ns | 0.0197 ns | 0.0175 ns |  0.03 |    0.00 |      - |         - |          NA |
| NotEqualCurrency            |      0.8225 ns | 0.0437 ns | 0.0387 ns |  0.03 |    0.00 |      - |         - |          NA |
| EqualOrBigger               |      5.5141 ns | 0.0829 ns | 0.0735 ns |  0.17 |    0.00 |      - |         - |          NA |
| Bigger                      |      5.7192 ns | 0.1241 ns | 0.1161 ns |  0.18 |    0.00 |      - |         - |          NA |
| Increment                   |     91.0277 ns | 0.8673 ns | 0.8112 ns |  2.85 |    0.04 | 0.0038 |      32 B |          NA |
| Decrement                   |     90.9336 ns | 0.8283 ns | 0.6917 ns |  2.85 |    0.04 | 0.0038 |      32 B |          NA |

## MoneyFormatting
#### v1
| Method             |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit           | 194.0 ns | 3.93 ns |  9.10 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 197.6 ns | 3.98 ns |  8.39 ns | 0.0286 |     240 B |
| Explicit           | 271.5 ns | 5.50 ns |  9.33 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 270.3 ns | 5.42 ns | 12.56 ns | 0.0525 |     440 B |
#### v2
| Method             |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|--------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit           | 106.3 ns | 2.06 ns | 5.06 ns | 0.0468 |     392 B |
| ImplicitWithFormat | 147.5 ns | 2.66 ns | 3.07 ns | 0.0505 |     424 B |
| Explicit           | 109.1 ns | 2.14 ns | 2.93 ns | 0.0468 |     392 B |
| ExplicitWithFormat | 144.8 ns | 2.92 ns | 5.56 ns | 0.0505 |     424 B |
#### v2.5
| Method             |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|--------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit           | 129.3 ns | 2.62 ns | 6.62 ns | 0.0458 |     384 B |
| ImplicitWithFormat | 160.4 ns | 3.21 ns | 6.04 ns | 0.0496 |     416 B |
| Explicit           | 120.7 ns | 2.39 ns | 4.77 ns | 0.0459 |     384 B |
| ExplicitWithFormat | 155.9 ns | 3.05 ns | 4.46 ns | 0.0496 |     416 B |

## MoneyParsing
#### v1
| Method      |        Mean |     Error |    StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 587.14 ns | 31,817.6 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 945.70 ns | 32,079.3 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns |  62.73 ns |    860.9 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns |  21.42 ns |    871.8 ns | 0.1469 |      - |      1 KB |
#### v2
| Method            |     Mean |   Error |  StdDev |   Gen0 | Allocated |
|-------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit          | 427.4 ns | 7.67 ns | 7.17 ns | 0.1173 |     984 B |
| ImplicitTry       | 426.3 ns | 6.59 ns | 6.17 ns | 0.1173 |     984 B |
| Explicit          | 442.4 ns | 7.58 ns | 9.85 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 442.1 ns | 6.54 ns | 6.12 ns | 0.1173 |     984 B |
| ExplicitTry       | 431.5 ns | 6.15 ns | 5.46 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 446.3 ns | 5.94 ns | 5.55 ns | 0.1173 |     984 B |
#### v2.5
| Method            |     Mean |    Error |   StdDev |   Gen0 | Allocated |
|-------------------|---------:|---------:|---------:|-------:|----------:|
| Implicit          | 463.3 ns |  8.00 ns | 15.79 ns | 0.1173 |     984 B |
| ImplicitTry       | 474.0 ns |  9.33 ns |  9.59 ns | 0.1173 |     984 B |
| Explicit          | 507.3 ns | 10.01 ns | 22.81 ns | 0.1173 |     984 B |
| ExplicitAsSpan    | 510.3 ns | 10.05 ns | 15.04 ns | 0.1173 |     984 B |
| ExplicitTry       | 523.9 ns | 10.17 ns | 14.59 ns | 0.1173 |     984 B |
| ExplicitTryAsSpan | 493.3 ns |  9.86 ns | 13.50 ns | 0.1173 |     984 B |

## HighLoad
#### v1
| Method           |     Mean |    Error |   StdDev |      Gen 0 | Allocated |
|------------------|---------:|---------:|---------:|-----------:|----------:|
| Create1MCurrency | 482.6 ms |  9.57 ms | 26.68 ms | 75000.0000 |    679 MB |
| Create1MMoney    | 516.8 ms | 10.33 ms | 28.46 ms | 75000.0000 |    694 MB |
#### v2
| Method            |      Mean |     Error |    StdDev |     Gen0 |     Gen1 |     Gen2 | Allocated |
|-------------------|----------:|----------:|----------:|---------:|---------:|---------:|----------:|
| Create1MCurrency  | 13.474 ms | 0.1538 ms | 0.1201 ms | 390.6250 | 390.6250 | 312.5000 |   1.91 MB |
| Create1MMoney     | 30.124 ms | 0.6020 ms | 0.7166 ms | 656.2500 | 656.2500 | 656.2500 |  22.89 MB |
| Create1MFastMoney | 42.112 ms | 0.7915 ms | 0.7017 ms | 583.3333 | 583.3333 | 583.3333 |  15.26 MB |
#### v2.5
| Method            |      Mean |     Error |    StdDev | Ratio | RatioSD |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------|----------:|----------:|----------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 13.621 ms | 0.1586 ms | 0.1325 ms |  0.36 |    0.01 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MDecimal   |  4.003 ms | 0.0919 ms | 0.2607 ms |  0.11 |    0.01 | 597.6563 | 597.6563 | 597.6563 |  15.26 MB |        1.00 |
| Create1MMoney     | 37.396 ms | 0.4565 ms | 0.4270 ms |  1.00 |    0.02 | 571.4286 | 571.4286 | 571.4286 |  15.26 MB |        1.00 |
| Create1MFastMoney | 31.652 ms | 0.3123 ms | 0.2922 ms |  0.85 |    0.01 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 23.057 ms | 0.4295 ms | 0.4596 ms |  0.62 |    0.01 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
