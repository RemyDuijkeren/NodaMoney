## InitializingCurrency
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
#### v2.6
| Method                  |      Mean |     Error |          Op/s | Allocated |
|-------------------------|----------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 12.229 ns | 0.1748 ns |  81,774,812.1 |         - |
| CurrencyInfoFromCode    |  6.611 ns | 0.1740 ns | 151,264,622.9 |         - |
| CurrencyInfoTryFromCode |  6.052 ns | 0.1095 ns | 165,230,528.0 |         - |


## InitializingMoney
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
#### v2.6
| Method                        |      Mean |     Error |          Op/s | Ratio |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------|----------:|----------:|--------------:|------:|-------:|----------:|------------:|
| CurrencyCode                  | 28.564 ns | 0.5243 ns |  35,008,927.4 |  1.00 |      - |         - |          NA |
| fCurrencyCode                 | 22.776 ns | 0.2386 ns |  43,905,942.1 |  0.80 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 30.179 ns | 0.6230 ns |  33,135,089.8 |  1.06 |      - |         - |          NA |
| CurrencyCodeAndContext        | 29.527 ns | 0.5400 ns |  33,867,685.3 |  1.03 |      - |         - |          NA |
| CurrencyFromCode              | 28.356 ns | 0.5939 ns |  35,265,913.1 |  0.99 |      - |         - |          NA |
| CurrencyInfoFromCode          | 28.415 ns | 0.5931 ns |  35,193,217.2 |  1.00 |      - |         - |          NA |
| ExtensionMethodEuro           | 28.590 ns | 0.3620 ns |  34,976,875.1 |  1.00 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 73.518 ns | 1.5096 ns |  13,602,182.8 |  2.57 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 74.238 ns | 1.4504 ns |  13,470,267.0 |  2.60 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  1.108 ns | 0.0485 ns | 902,338,512.8 |  0.04 |      - |         - |          NA |

## MoneyEquals
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
#### v2.6
| Method            |      Mean |     Error |            Op/s | Ratio | Allocated | Alloc Ratio |
|-------------------|----------:|----------:|----------------:|------:|----------:|------------:|
| Equal             | 3.8190 ns | 0.0306 ns |   261,849,894.9 |  1.00 |         - |          NA |
| NotEqualValue     | 2.6528 ns | 0.0537 ns |   376,954,591.2 |  0.69 |         - |          NA |
| NotEqualCurrency  | 0.6307 ns | 0.0418 ns | 1,585,540,447.8 |  0.17 |         - |          NA |
| EqualOrBigger     | 5.4350 ns | 0.0854 ns |   183,992,476.8 |  1.42 |         - |          NA |
| Bigger            | 5.3413 ns | 0.0658 ns |   187,219,787.6 |  1.40 |         - |          NA |
| fEqual            | 0.1653 ns | 0.0191 ns | 6,051,048,225.4 |  0.04 |         - |          NA |
| fNotEqualValue    | 0.2646 ns | 0.0245 ns | 3,779,863,337.6 |  0.07 |         - |          NA |
| fNotEqualCurrency | 0.2029 ns | 0.0098 ns | 4,928,646,686.2 |  0.05 |         - |          NA |
| fEqualOrBigger    | 1.2687 ns | 0.0217 ns |   788,201,886.0 |  0.33 |         - |          NA |
| fBigger           | 1.2771 ns | 0.0190 ns |   783,006,682.2 |  0.33 |         - |          NA |

## MoneyOperations
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
#### v2.6
| Method                  |      Mean |     Error |          Op/s | Ratio | Allocated | Alloc Ratio |
|-------------------------|----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 28.195 ns | 0.5836 ns |  35,466,906.4 | 14.90 |         - |          NA |
| Subtract                | 30.374 ns | 0.3741 ns |  32,922,933.9 | 16.05 |         - |          NA |
| Multiple                | 29.046 ns | 0.3349 ns |  34,427,988.1 | 15.35 |         - |          NA |
| Divide                  | 70.748 ns | 0.9017 ns |  14,134,735.2 | 37.39 |         - |          NA |
| Increment               |  9.157 ns | 0.1865 ns | 109,206,346.3 |  4.84 |         - |          NA |
| Decrement               |  9.338 ns | 0.2062 ns | 107,087,451.5 |  4.94 |         - |          NA |
| Remainder               | 20.345 ns | 0.1780 ns |  49,152,774.0 | 10.75 |         - |          NA |
| fAdd                    |  1.893 ns | 0.0566 ns | 528,159,297.6 |  1.00 |         - |          NA |
| fSubtract               |  1.737 ns | 0.0504 ns | 575,731,435.0 |  0.92 |         - |          NA |
| fMultipleDec            | 29.450 ns | 1.6689 ns |  33,955,903.5 | 15.57 |         - |          NA |
| fMultipleDecWholeNumber |  9.521 ns | 0.1498 ns | 105,031,040.1 |  5.03 |         - |          NA |
| fMultipleLong           |  2.131 ns | 0.0690 ns | 469,277,233.0 |  1.13 |         - |          NA |
| fDivideDec              | 70.444 ns | 1.1502 ns |  14,195,762.2 | 37.23 |         - |          NA |
| fDivideDecWholeNumber   | 10.886 ns | 0.2397 ns |  91,858,580.4 |  5.75 |         - |          NA |
| fDivideLong             |  2.024 ns | 0.0540 ns | 493,960,294.3 |  1.07 |         - |          NA |
| fIncrement              |  5.215 ns | 0.1268 ns | 191,744,815.9 |  2.76 |         - |          NA |
| fDecrement              |  4.627 ns | 0.1185 ns | 216,144,432.2 |  2.45 |         - |          NA |
| fRemainder              |  1.935 ns | 0.0278 ns | 516,849,107.9 |  1.02 |         - |          NA |

## MoneyFormatting
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
#### v2.6
| Method                         |     Mean |   Error |        Op/s |   Gen0 | Allocated |
|--------------------------------|---------:|--------:|------------:|-------:|----------:|
| DefaultFormat                  | 115.0 ns | 2.30 ns | 8,698,600.7 | 0.0458 |     384 B |
| FormatWithPrecision            | 188.9 ns | 1.70 ns | 5,294,420.3 | 0.0572 |     480 B |
| FormatProvider                 | 106.0 ns | 2.13 ns | 9,432,282.7 | 0.0459 |     384 B |
| FormatWithPrecisionAndProvider | 193.5 ns | 1.88 ns | 5,168,147.6 | 0.0572 |     480 B |
| CompactFormat                  |       NA |      NA |          NA |     NA |        NA |
| GeneralFormat                  | 166.4 ns | 3.39 ns | 6,009,638.1 | 0.0842 |     704 B |
| RondTripFormat                 | 117.0 ns | 2.36 ns | 8,543,902.3 | 0.0516 |     432 B |

## MoneyParsing
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
#### v2.6
| Method            |     Mean |   Error |        Op/s |   Gen0 | Allocated |
|-------------------|---------:|--------:|------------:|-------:|----------:|
| Implicit          | 155.8 ns | 1.68 ns | 6,419,477.6 | 0.0191 |     160 B |
| ImplicitTry       | 184.7 ns | 3.47 ns | 5,412,967.7 | 0.0191 |     160 B |
| Explicit          | 159.3 ns | 1.22 ns | 6,279,071.2 | 0.0191 |     160 B |
| ExplicitAsSpan    | 181.6 ns | 0.66 ns | 5,507,450.0 | 0.0191 |     160 B |
| ExplicitTry       | 167.6 ns | 2.89 ns | 5,967,989.4 | 0.0191 |     160 B |
| ExplicitTryAsSpan | 165.3 ns | 1.93 ns | 6,051,351.3 | 0.0191 |     160 B |

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
#### v2.6
| Method        |       Mean |     Error |            Op/s |  Ratio | Allocated | Alloc Ratio |
|---------------|-----------:|----------:|----------------:|-------:|----------:|------------:|
| ToDecimal     |  0.8550 ns | 0.0398 ns | 1,169,551,722.0 |  1.006 |         - |          NA |
| ToDouble      |  2.0422 ns | 0.0442 ns |   489,673,883.1 |  2.402 |         - |          NA |
| ToIn32        | 19.2384 ns | 0.4046 ns |    51,979,438.5 | 22.626 |         - |          NA |
| ToInt64       | 18.5800 ns | 0.3984 ns |    53,821,252.8 | 21.851 |         - |          NA |
| ToFastMoney   | 20.8188 ns | 0.3086 ns |    48,033,589.0 | 24.484 |         - |          NA |
| fToDecimal    |  2.8545 ns | 0.0530 ns |   350,329,702.1 |  3.357 |         - |          NA |
| fToDouble     |  3.8613 ns | 0.0965 ns |   258,977,131.2 |  4.541 |         - |          NA |
| fToIn32       |  4.9999 ns | 0.0778 ns |   200,002,357.8 |  5.880 |         - |          NA |
| fToInt64      |  4.1874 ns | 0.1085 ns |   238,813,758.2 |  4.925 |         - |          NA |
| fToMoney      | 20.8108 ns | 0.1066 ns |    48,052,077.6 | 24.475 |         - |          NA |
| fToSqlMoney   | 17.1244 ns | 0.2703 ns |    58,396,200.2 | 20.139 |         - |          NA |
| fToAOCurrency |  0.0000 ns | 0.0000 ns |        Infinity |  0.000 |         - |          NA |

## HighLoad
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
#### v2.6
| Method            |      Mean |     Error |   Op/s | Ratio |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|-------------------|----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 12.686 ms | 0.2494 ms |  78.83 |  0.43 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 29.750 ms | 0.2796 ms |  33.61 |  1.00 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MFastMoney | 21.997 ms | 0.4164 ms |  45.46 |  0.74 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 17.601 ms | 0.3084 ms |  56.81 |  0.59 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  3.438 ms | 0.1078 ms | 290.87 |  0.12 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |

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
