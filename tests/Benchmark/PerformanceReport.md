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
| Method                  |      Mean |     Error |    StdDev |          Op/s | Allocated |
|-------------------------|----------:|----------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 16.178 ns | 0.2721 ns | 0.2412 ns |  61,810,657.5 |         - |
| CurrencyInfoFromCode    |  9.293 ns | 0.0985 ns | 0.0873 ns | 107,608,674.6 |         - |
| CurrencyInfoTryFromCode |  6.471 ns | 0.0487 ns | 0.0456 ns | 154,534,339.0 |         - |

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
| Method                        |      Mean |     Error |    StdDev |          Op/s | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|----------:|----------:|----------:|--------------:|------:|--------:|-------:|----------:|------------:|
| CurrencyCode                  | 37.646 ns | 0.3923 ns | 0.3276 ns |  26,562,901.4 |  1.00 |    0.01 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 44.154 ns | 0.4830 ns | 0.4282 ns |  22,648,133.6 |  1.17 |    0.01 |      - |         - |          NA |
| CurrencyCodeAndContext        | 41.593 ns | 0.5863 ns | 0.5485 ns |  24,042,661.1 |  1.10 |    0.02 |      - |         - |          NA |
| CurrencyFromCode              | 37.623 ns | 0.1818 ns | 0.1611 ns |  26,579,744.2 |  1.00 |    0.01 |      - |         - |          NA |
| CurrencyInfoFromCode          | 38.475 ns | 0.1707 ns | 0.1513 ns |  25,990,782.1 |  1.02 |    0.01 |      - |         - |          NA |
| ExtensionMethodEuro           | 37.693 ns | 0.4837 ns | 0.4288 ns |  26,529,951.0 |  1.00 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 84.934 ns | 1.0002 ns | 0.8352 ns |  11,773,846.6 |  2.26 |    0.03 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 81.604 ns | 0.4188 ns | 0.3917 ns |  12,254,245.1 |  2.17 |    0.02 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.060 ns | 0.0259 ns | 0.0242 ns | 943,520,894.5 |  0.03 |    0.00 |      - |         - |          NA |

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
| Method                      |      Mean |     Error |    StdDev |          Op/s | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-----------------------------|----------:|----------:|----------:|--------------:|------:|--------:|-------:|----------:|------------:|
| Add                         | 26.730 ns | 0.4042 ns | 0.3583 ns |  37,410,832.2 |  1.00 |    0.02 |      - |         - |          NA |
| AddFastMoney                | 11.440 ns | 0.2238 ns | 0.1869 ns |  87,414,421.9 |  0.43 |    0.01 |      - |         - |          NA |
| Subtract                    | 26.428 ns | 0.1206 ns | 0.1007 ns |  37,839,081.6 |  0.99 |    0.01 |      - |         - |          NA |
| SubtractFastMoney           |  5.868 ns | 0.0755 ns | 0.0669 ns | 170,430,275.8 |  0.22 |    0.00 |      - |         - |          NA |
| Multiple                    | 32.960 ns | 0.2225 ns | 0.1972 ns |  30,340,060.4 |  1.23 |    0.02 |      - |         - |          NA |
| MultipleFastMoneyDecimal    | 28.330 ns | 0.2286 ns | 0.2026 ns |  35,297,951.6 |  1.06 |    0.02 |      - |         - |          NA |
| MultipleFastWholeDecimal    | 14.785 ns | 0.1376 ns | 0.1287 ns |  67,637,647.0 |  0.55 |    0.01 |      - |         - |          NA |
| MultipleFastMoneyLong       |  2.083 ns | 0.0350 ns | 0.0310 ns | 480,070,346.5 |  0.08 |    0.00 |      - |         - |          NA |
| Divide                      | 83.825 ns | 0.3771 ns | 0.3149 ns |  11,929,609.5 |  3.14 |    0.04 |      - |         - |          NA |
| DivideFastMoneyDecimal      | 78.831 ns | 0.4703 ns | 0.4169 ns |  12,685,296.2 |  2.95 |    0.04 |      - |         - |          NA |
| DivideFastMoneyWholeDecimal | 13.265 ns | 0.2866 ns | 0.3727 ns |  75,385,506.6 |  0.50 |    0.02 |      - |         - |          NA |
| DivideFastMoneyLong         |  1.984 ns | 0.0629 ns | 0.1432 ns | 504,005,460.5 |  0.07 |    0.01 |      - |         - |          NA |
| Equal                       |  3.619 ns | 0.0531 ns | 0.0444 ns | 276,314,723.4 |  0.14 |    0.00 |      - |         - |          NA |
| NotEqualCurrency            |  1.657 ns | 0.0266 ns | 0.0249 ns | 603,660,650.8 |  0.06 |    0.00 |      - |         - |          NA |
| EqualOrBigger               |  5.650 ns | 0.1022 ns | 0.0956 ns | 176,989,154.9 |  0.21 |    0.00 |      - |         - |          NA |
| Bigger                      |  5.582 ns | 0.0589 ns | 0.0522 ns | 179,156,947.4 |  0.21 |    0.00 |      - |         - |          NA |
| Increment                   | 87.358 ns | 1.0496 ns | 0.9304 ns |  11,447,121.2 |  3.27 |    0.05 | 0.0038 |      32 B |          NA |
| Decrement                   | 86.520 ns | 0.8442 ns | 0.7049 ns |  11,557,991.8 |  3.24 |    0.05 | 0.0038 |      32 B |          NA |

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
| Method             |     Mean |   Error |  StdDev |        Op/s |   Gen0 | Allocated |
|--------------------|---------:|--------:|--------:|------------:|-------:|----------:|
| Implicit           | 115.5 ns | 2.36 ns | 4.88 ns | 8,657,442.3 | 0.0459 |     384 B |
| ImplicitWithFormat | 144.3 ns | 2.81 ns | 3.66 ns | 6,931,054.6 | 0.0496 |     416 B |
| Explicit           | 119.0 ns | 2.42 ns | 4.89 ns | 8,405,527.7 | 0.0459 |     384 B |
| ExplicitWithFormat | 149.4 ns | 3.01 ns | 2.95 ns | 6,695,496.3 | 0.0496 |     416 B |

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
| Method            |     Mean |   Error |  StdDev |        Op/s |   Gen0 | Allocated |
|-------------------|---------:|--------:|--------:|------------:|-------:|----------:|
| Implicit          | 383.8 ns | 6.06 ns | 5.37 ns | 2,605,394.7 | 0.0801 |     672 B |
| ImplicitTry       | 370.7 ns | 6.75 ns | 6.31 ns | 2,697,790.3 | 0.0801 |     672 B |
| Explicit          | 395.9 ns | 5.53 ns | 4.62 ns | 2,525,750.5 | 0.0801 |     672 B |
| ExplicitAsSpan    | 412.3 ns | 8.10 ns | 7.96 ns | 2,425,162.2 | 0.0801 |     672 B |
| ExplicitTry       | 421.2 ns | 8.38 ns | 8.61 ns | 2,374,036.0 | 0.0801 |     672 B |
| ExplicitTryAsSpan | 391.4 ns | 7.21 ns | 7.08 ns | 2,554,645.2 | 0.0801 |     672 B |

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
| Method            |      Mean |     Error |    StdDev |    Op/s | Ratio | RatioSD |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------|----------:|----------:|----------:|--------:|------:|--------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 13.953 ms | 0.2772 ms | 0.2593 ms |   71.67 |  0.38 |    0.01 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 36.576 ms | 0.6902 ms | 0.6456 ms |   27.34 |  1.00 |    0.02 | 571.4286 | 571.4286 | 571.4286 |  15.26 MB |        1.00 |
| Create1MFastMoney | 30.137 ms | 0.3957 ms | 0.3701 ms |   33.18 |  0.82 |    0.02 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 20.931 ms | 0.4076 ms | 0.4185 ms |   47.78 |  0.57 |    0.01 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
| Create1MDecimal   |  4.050 ms | 0.0805 ms | 0.2122 ms |  246.91 |  0.11 |    0.01 | 597.6563 | 597.6563 | 597.6563 |  15.26 MB |        1.00 |
