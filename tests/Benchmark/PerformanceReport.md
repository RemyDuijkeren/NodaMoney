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
| CurrencyCode                  | 38.254 ns | 0.5405 ns | 0.5056 ns |  26,140,954.9 |  1.00 |    0.02 |      - |         - |          NA |
| fCurrencyCode                 | 33.994 ns | 0.5416 ns | 0.4522 ns |  29,416,951.4 |  0.89 |    0.02 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 44.330 ns | 0.3970 ns | 0.3520 ns |  22,557,911.1 |  1.16 |    0.02 |      - |         - |          NA |
| CurrencyCodeAndContext        | 42.655 ns | 0.2374 ns | 0.2221 ns |  23,443,999.0 |  1.12 |    0.02 |      - |         - |          NA |
| CurrencyFromCode              | 37.422 ns | 0.2613 ns | 0.2316 ns |  26,722,192.3 |  0.98 |    0.01 |      - |         - |          NA |
| CurrencyInfoFromCode          | 37.151 ns | 0.2870 ns | 0.2544 ns |  26,917,085.2 |  0.97 |    0.01 |      - |         - |          NA |
| ExtensionMethodEuro           | 37.006 ns | 0.4006 ns | 0.3128 ns |  27,022,575.9 |  0.97 |    0.01 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 84.376 ns | 1.5994 ns | 1.5708 ns |  11,851,753.2 |  2.21 |    0.05 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 82.875 ns | 1.6222 ns | 1.4381 ns |  12,066,352.9 |  2.17 |    0.05 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.040 ns | 0.0248 ns | 0.0232 ns | 961,394,287.4 |  0.03 |    0.00 |      - |         - |          NA |

## MoneyOperations
#### v1
| Method           |      Mean |    Error |    StdDev | Allocated |
|------------------|----------:|---------:|----------:|----------:|
| Add              | 231.00 ns | 4.470 ns |  4.181 ns |         - |
| Subtract         | 233.42 ns | 4.665 ns |  4.581 ns |         - |
| NotEqualValue    |  30.09 ns | 0.518 ns |  0.485 ns |         - |
| NotEqualCurrency |  68.83 ns | 0.679 ns |  0.530 ns |         - |
| Bigger           | 212.60 ns | 4.271 ns | 10.558 ns |         - |
| Increment        | 374.80 ns | 7.507 ns | 11.907 ns |         - |
| Decrement        | 369.70 ns | 7.295 ns | 14.229 ns |         - |
#### v2
| Method           |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|------------------|----------:|----------:|----------:|-------:|----------:|
| Add              | 16.219 ns | 0.0810 ns | 0.0718 ns |      - |         - |
| Subtract         | 15.777 ns | 0.0662 ns | 0.0587 ns |      - |         - |
| NotEqualValue    |  3.623 ns | 0.0396 ns | 0.0351 ns |      - |         - |
| NotEqualCurrency |  3.684 ns | 0.0314 ns | 0.0293 ns |      - |         - |
| Bigger           |  3.908 ns | 0.0517 ns | 0.0459 ns |      - |         - |
| Increment        | 86.789 ns | 0.8525 ns | 0.7119 ns | 0.0038 |      32 B |
| Decrement        | 86.949 ns | 0.5347 ns | 0.5001 ns | 0.0038 |      32 B |
#### v2.5
| Method               |       Mean |     Error |    StdDev |             Op/s | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|----------------------|-----------:|----------:|----------:|-----------------:|------:|--------:|-------:|----------:|------------:|
| Add                  | 26.5215 ns | 0.3496 ns | 0.2919 ns |     37,705,290.2 | 1.000 |    0.01 |      - |         - |          NA |
| Subtract             | 26.4855 ns | 0.1146 ns | 0.1072 ns |     37,756,505.8 | 0.999 |    0.01 |      - |         - |          NA |
| Multiple             | 29.8811 ns | 0.2236 ns | 0.1982 ns |     33,465,942.8 | 1.127 |    0.01 |      - |         - |          NA |
| Divide               | 83.5724 ns | 0.4332 ns | 0.4052 ns |     11,965,674.7 | 3.151 |    0.04 |      - |         - |          NA |
| Equal                |  4.4347 ns | 0.0419 ns | 0.0372 ns |    225,493,890.9 | 0.167 |    0.00 |      - |         - |          NA |
| NotEqualValue        |  3.3539 ns | 0.0168 ns | 0.0140 ns |    298,157,104.8 | 0.126 |    0.00 |      - |         - |          NA |
| NotEqualCurrency     |  1.7678 ns | 0.0272 ns | 0.0255 ns |    565,671,447.8 | 0.067 |    0.00 |      - |         - |          NA |
| EqualOrBigger        |  5.5061 ns | 0.0455 ns | 0.0426 ns |    181,618,084.0 | 0.208 |    0.00 |      - |         - |          NA |
| Bigger               |  5.6735 ns | 0.0334 ns | 0.0312 ns |    176,258,517.0 | 0.214 |    0.00 |      - |         - |          NA |
| Increment            | 86.3937 ns | 0.4241 ns | 0.3760 ns |     11,574,919.6 | 3.258 |    0.04 | 0.0038 |      32 B |          NA |
| Decrement            | 87.7657 ns | 1.2121 ns | 1.1338 ns |     11,393,972.1 | 3.310 |    0.05 | 0.0038 |      32 B |          NA |
| fAdd                 |  1.4766 ns | 0.0307 ns | 0.0272 ns |    677,248,261.3 | 0.056 |    0.00 |      - |         - |          NA |
| fSubtract            |  1.4512 ns | 0.0167 ns | 0.0139 ns |    689,079,906.8 | 0.055 |    0.00 |      - |         - |          NA |
| fMultiple            | 28.7180 ns | 0.3424 ns | 0.3202 ns |     34,821,318.4 | 1.083 |    0.02 |      - |         - |          NA |
| fMultipleWholeNumber | 14.5920 ns | 0.1750 ns | 0.1637 ns |     68,530,561.6 | 0.550 |    0.01 |      - |         - |          NA |
| fMultipleLong        |  2.2465 ns | 0.0408 ns | 0.0362 ns |    445,130,643.7 | 0.085 |    0.00 |      - |         - |          NA |
| fDivide              | 79.3698 ns | 1.4841 ns | 1.3156 ns |     12,599,243.5 | 2.993 |    0.06 |      - |         - |          NA |
| fDivideWholeNumber   | 12.6539 ns | 0.0695 ns | 0.0616 ns |     79,026,720.6 | 0.477 |    0.01 |      - |         - |          NA |
| fDivideLong          |  1.8899 ns | 0.0251 ns | 0.0209 ns |    529,132,961.1 | 0.071 |    0.00 |      - |         - |          NA |
| fEqual               |  0.0277 ns | 0.0071 ns | 0.0063 ns | 36,152,888,587.9 | 0.001 |    0.00 |      - |         - |          NA |
| fNotEqualValue       |  0.2832 ns | 0.0213 ns | 0.0189 ns |  3,531,009,898.8 | 0.011 |    0.00 |      - |         - |          NA |
| fNotEqualCurrency    |  0.2210 ns | 0.0111 ns | 0.0098 ns |  4,525,350,293.5 | 0.008 |    0.00 |      - |         - |          NA |
| fEqualOrBigger       | 16.5950 ns | 0.3039 ns | 0.2842 ns |     60,259,109.8 | 0.626 |    0.01 |      - |         - |          NA |
| fBigger              | 16.8814 ns | 0.2098 ns | 0.1860 ns |     59,236,859.9 | 0.637 |    0.01 |      - |         - |          NA |

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
| Method            |     Mean |   Error |   StdDev |   Median |        Op/s |   Gen0 | Allocated |
|-------------------|---------:|--------:|---------:|---------:|------------:|-------:|----------:|
| Implicit          | 185.6 ns | 3.82 ns | 10.96 ns | 181.3 ns | 5,386,586.2 | 0.0191 |     160 B |
| ImplicitTry       | 189.7 ns | 3.79 ns |  5.79 ns | 188.6 ns | 5,270,498.0 | 0.0191 |     160 B |
| Explicit          | 193.2 ns | 3.84 ns |  4.57 ns | 192.3 ns | 5,176,120.3 | 0.0191 |     160 B |
| ExplicitAsSpan    | 191.4 ns | 2.19 ns |  1.94 ns | 191.3 ns | 5,223,378.5 | 0.0191 |     160 B |
| ExplicitTry       | 202.4 ns | 2.89 ns |  2.41 ns | 202.7 ns | 4,939,987.4 | 0.0191 |     160 B |
| ExplicitTryAsSpan | 193.3 ns | 1.90 ns |  1.77 ns | 193.7 ns | 5,173,897.1 | 0.0191 |     160 B |

## MoneyConversion
### v2.5
| Method        |       Mean |     Error |    StdDev |     Median |             Op/s | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------|-----------:|----------:|----------:|-----------:|-----------------:|------:|--------:|----------:|------------:|
| ToDecimal     |  0.8455 ns | 0.0438 ns | 0.0669 ns |  0.8215 ns |  1,182,679,904.5 |  1.01 |    0.11 |         - |          NA |
| ToDouble      |  2.9635 ns | 0.0807 ns | 0.0755 ns |  2.9493 ns |    337,439,639.7 |  3.52 |    0.27 |         - |          NA |
| ToIn32        | 22.7336 ns | 0.4754 ns | 0.7810 ns | 22.4914 ns |     43,987,805.9 | 27.04 |    2.17 |         - |          NA |
| ToInt64       | 22.9894 ns | 0.3967 ns | 0.4245 ns | 22.8428 ns |     43,498,290.9 | 27.34 |    2.05 |         - |          NA |
| ToFastMoney   | 28.6825 ns | 0.5323 ns | 0.5228 ns | 28.5345 ns |     34,864,447.5 | 34.11 |    2.55 |         - |          NA |
| fToDecimal    |  3.0329 ns | 0.0609 ns | 0.0570 ns |  3.0172 ns |    329,718,318.2 |  3.61 |    0.27 |         - |          NA |
| fToDouble     | 16.4829 ns | 0.1799 ns | 0.1682 ns | 16.4417 ns |     60,668,843.6 | 19.60 |    1.44 |         - |          NA |
| fToIn32       |  7.0331 ns | 0.1635 ns | 0.2126 ns |  6.9731 ns |    142,184,665.4 |  8.37 |    0.66 |         - |          NA |
| fToInt64      |  6.7446 ns | 0.1190 ns | 0.1055 ns |  6.7221 ns |    148,266,932.8 |  8.02 |    0.60 |         - |          NA |
| fToMoney      | 22.0964 ns | 0.1660 ns | 0.1386 ns | 22.0542 ns |     45,256,149.1 | 26.28 |    1.92 |         - |          NA |
| fToSqlMoney   | 22.7936 ns | 0.2174 ns | 0.2033 ns | 22.7529 ns |     43,871,886.1 | 27.11 |    1.98 |         - |          NA |
| fToAOCurrency |  0.0119 ns | 0.0167 ns | 0.0156 ns |  0.0000 ns | 84,114,152,018.4 |  0.01 |    0.02 |         - |          NA |

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
