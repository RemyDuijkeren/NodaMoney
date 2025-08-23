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
| CurrencyCode                  | 37.781 ns | 0.6040 ns | 0.5650 ns |  1.00 |    0.02 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 44.718 ns | 0.2399 ns | 0.2003 ns |  1.18 |    0.02 |      - |         - |          NA |
| CurrencyCodeAndContext        | 43.372 ns | 0.6128 ns | 0.5117 ns |  1.15 |    0.02 |      - |         - |          NA |
| CurrencyFromCode              | 38.638 ns | 0.8005 ns | 0.7488 ns |  1.02 |    0.02 |      - |         - |          NA |
| CurrencyInfoFromCode          | 38.666 ns | 0.5743 ns | 0.5372 ns |  1.02 |    0.02 |      - |         - |          NA |
| ExtensionMethodEuro           | 37.896 ns | 0.2896 ns | 0.2709 ns |  1.00 |    0.02 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 82.937 ns | 0.5015 ns | 0.4445 ns |  2.20 |    0.03 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 83.013 ns | 0.8628 ns | 0.7648 ns |  2.20 |    0.04 | 0.0038 |      32 B |          NA |
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
| Method                      |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-----------------------------|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| Add                         | 26.500 ns | 0.0840 ns | 0.0744 ns |  1.00 |    0.00 |      - |         - |          NA |
| AddFastMoney                | 11.744 ns | 0.2611 ns | 0.4065 ns |  0.44 |    0.02 |      - |         - |          NA |
| Subtract                    | 26.694 ns | 0.1328 ns | 0.1109 ns |  1.01 |    0.00 |      - |         - |          NA |
| SubtractFastMoney           |  6.004 ns | 0.0956 ns | 0.0894 ns |  0.23 |    0.00 |      - |         - |          NA |
| Multiple                    | 29.890 ns | 0.2835 ns | 0.2652 ns |  1.13 |    0.01 |      - |         - |          NA |
| MultipleFastMoneyDecimal    | 29.255 ns | 0.3047 ns | 0.2545 ns |  1.10 |    0.01 |      - |         - |          NA |
| MultipleFastWholeDecimal    | 14.316 ns | 0.1455 ns | 0.1290 ns |  0.54 |    0.00 |      - |         - |          NA |
| MultipleFastMoneyLong       |  2.116 ns | 0.0691 ns | 0.0710 ns |  0.08 |    0.00 |      - |         - |          NA |
| Divide                      | 83.462 ns | 0.4867 ns | 0.4553 ns |  3.15 |    0.02 |      - |         - |          NA |
| DivideFastMoneyDecimal      | 79.419 ns | 0.5305 ns | 0.4430 ns |  3.00 |    0.02 |      - |         - |          NA |
| DivideFastMoneyWholeDecimal | 12.891 ns | 0.1048 ns | 0.0980 ns |  0.49 |    0.00 |      - |         - |          NA |
| DivideFastMoneyLong         |  1.861 ns | 0.0206 ns | 0.0193 ns |  0.07 |    0.00 |      - |         - |          NA |
| Equal                       |  3.621 ns | 0.0270 ns | 0.0253 ns |  0.14 |    0.00 |      - |         - |          NA |
| NotEqualCurrency            |  1.646 ns | 0.0215 ns | 0.0201 ns |  0.06 |    0.00 |      - |         - |          NA |
| EqualOrBigger               |  5.481 ns | 0.0398 ns | 0.0372 ns |  0.21 |    0.00 |      - |         - |          NA |
| Bigger                      |  5.571 ns | 0.0378 ns | 0.0353 ns |  0.21 |    0.00 |      - |         - |          NA |
| Increment                   | 85.797 ns | 0.4201 ns | 0.3724 ns |  3.24 |    0.02 | 0.0038 |      32 B |          NA |
| Decrement                   | 86.363 ns | 0.5180 ns | 0.4592 ns |  3.26 |    0.02 | 0.0038 |      32 B |          NA |

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
| Create1MCurrency  | 14.436 ms | 0.2765 ms | 0.2958 ms |  0.39 |    0.01 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MDecimal   |  4.145 ms | 0.1471 ms | 0.4314 ms |  0.11 |    0.01 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
| Create1MMoney     | 37.208 ms | 0.7428 ms | 0.7295 ms |  1.00 |    0.03 | 571.4286 | 571.4286 | 571.4286 |  15.26 MB |        1.00 |
| Create1MFastMoney | 31.586 ms | 0.5131 ms | 0.4799 ms |  0.85 |    0.02 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 21.695 ms | 0.4137 ms | 0.3869 ms |  0.58 |    0.01 | 593.7500 | 593.7500 | 593.7500 |  15.26 MB |        1.00 |
