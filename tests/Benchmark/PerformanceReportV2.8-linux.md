## InitializingCurrency
### v2.8-linux
| Method                  | Mean     | Error     | Op/s          | Allocated |
|------------------------ |---------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 9.613 ns | 0.2069 ns | 104,025,563.4 |         - |
| CurrencyInfoFromCode    | 4.317 ns | 0.0278 ns | 231,641,612.1 |         - |
| CurrencyInfoTryFromCode | 5.160 ns | 0.1116 ns | 193,811,049.0 |         - |

## InitializingMoney
### v2.8-linux
| Method                        | Mean       | Error     | Op/s            | Ratio | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |-----------:|----------:|----------------:|------:|-------:|----------:|------------:|
| CurrencyCode                  | 21.4583 ns | 0.0750 ns |    46,602,078.4 |  1.00 |      - |         - |          NA |
| fCurrencyCode                 | 22.8582 ns | 0.1077 ns |    43,747,945.3 |  1.07 |      - |         - |          NA |
| CurrencyCodeAndRoundingMode   | 24.0014 ns | 0.1075 ns |    41,664,206.5 |  1.12 |      - |         - |          NA |
| CurrencyCodeAndContext        | 23.1415 ns | 0.0588 ns |    43,212,356.5 |  1.08 |      - |         - |          NA |
| CurrencyFromCode              | 30.3961 ns | 0.4545 ns |    32,898,979.2 |  1.42 |      - |         - |          NA |
| CurrencyInfoFromCode          | 22.9432 ns | 0.0611 ns |    43,585,982.6 |  1.07 |      - |         - |          NA |
| ExtensionMethodEuro           | 21.8405 ns | 0.4381 ns |    45,786,422.3 |  1.02 |      - |         - |          NA |
| ImplicitCurrencyByConstructor | 58.0552 ns | 1.1317 ns |    17,224,972.9 |  2.71 | 0.0038 |      32 B |          NA |
| ImplicitCurrencyByCasting     | 58.3372 ns | 1.0287 ns |    17,141,720.4 |  2.72 | 0.0038 |      32 B |          NA |
| Deconstruct                   |  0.8444 ns | 0.0161 ns | 1,184,282,504.8 |  0.04 |      - |         - |          NA |

## MoneyEquals
### v2.8-linux
| Method            | Mean      | Error     | Op/s             | Ratio | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-----------------:|------:|----------:|------------:|
| Equal             | 3.1839 ns | 0.0143 ns |    314,084,871.8 | 1.000 |         - |          NA |
| NotEqualValue     | 2.3576 ns | 0.0132 ns |    424,162,041.4 | 0.740 |         - |          NA |
| NotEqualCurrency  | 0.5437 ns | 0.0091 ns |  1,839,141,541.6 | 0.171 |         - |          NA |
| EqualOrBigger     | 4.2969 ns | 0.0110 ns |    232,725,344.0 | 1.350 |         - |          NA |
| Bigger            | 4.3224 ns | 0.0189 ns |    231,354,110.3 | 1.358 |         - |          NA |
| fEqual            | 0.0170 ns | 0.0011 ns | 58,725,089,032.5 | 0.005 |         - |          NA | <-! was | 0.1306 ns | 0.0316 ns | 7,658,922,507.6 |  0.03 |
| fNotEqualValue    | 0.1414 ns | 0.0013 ns |  7,074,557,074.3 | 0.044 |         - |          NA |
| fNotEqualCurrency | 0.2437 ns | 0.0059 ns |  4,103,250,835.2 | 0.077 |         - |          NA |
| fEqualOrBigger    | 1.1686 ns | 0.0023 ns |    855,733,939.5 | 0.367 |         - |          NA |
| fBigger           | 1.1820 ns | 0.0112 ns |    845,993,899.9 | 0.371 |         - |          NA |

## MoneyOperations
### v2.8-linux
| Method                  | Mean      | Error     | Op/s          | Ratio | Allocated | Alloc Ratio |
|------------------------ |----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 20.689 ns | 0.3419 ns |  48,333,857.1 | 15.81 |         - |          NA |
| Subtract                | 20.123 ns | 0.0814 ns |  49,693,933.0 | 15.38 |         - |          NA |
| Multiple                | 18.819 ns | 0.3971 ns |  53,138,659.5 | 14.38 |         - |          NA |
| Divide                  | 61.429 ns | 0.8173 ns |  16,278,848.1 | 46.94 |         - |          NA |
| Increment               |  9.589 ns | 0.1794 ns | 104,283,243.7 |  7.33 |         - |          NA |
| Decrement               |  9.581 ns | 0.0612 ns | 104,378,673.5 |  7.32 |         - |          NA |
| Remainder               | 18.224 ns | 0.0914 ns |  54,871,257.8 | 13.92 |         - |          NA |
| fAdd                    |  1.309 ns | 0.0130 ns | 763,999,716.2 |  1.00 |         - |          NA |
| fSubtract               |  1.198 ns | 0.0079 ns | 834,544,475.5 |  0.92 |         - |          NA |
| fMultipleDec            | 15.800 ns | 0.1004 ns |  63,289,385.3 | 12.07 |         - |          NA |
| fMultipleDecWholeNumber |  7.692 ns | 0.0166 ns | 130,010,288.7 |  5.88 |         - |          NA |
| fMultipleLong           |  2.254 ns | 0.0124 ns | 443,601,922.4 |  1.72 |         - |          NA |
| fDivideDec              | 50.625 ns | 0.3219 ns |  19,752,911.9 | 38.68 |         - |          NA |
| fDivideDecWholeNumber   |  8.713 ns | 0.1105 ns | 114,777,109.5 |  6.66 |         - |          NA |
| fDivideLong             |  2.294 ns | 0.0387 ns | 435,944,300.4 |  1.75 |         - |          NA |
| fIncrement              |  5.160 ns | 0.0120 ns | 193,802,443.1 |  3.94 |         - |          NA |
| fDecrement              |  5.196 ns | 0.0139 ns | 192,461,021.5 |  3.97 |         - |          NA |
| fRemainder              |  1.693 ns | 0.0101 ns | 590,631,414.4 |  1.29 |         - |          NA |

## MoneyFormatting
### v2.8-linux
| Method                         | Mean      | Error    | Op/s         | Gen0   | Allocated |
|------------------------------- |----------:|---------:|-------------:|-------:|----------:|
| DefaultFormat                  |  87.87 ns | 1.146 ns | 11,380,769.4 | 0.0459 |     384 B |
| FormatWithPrecision            | 105.24 ns | 0.607 ns |  9,501,677.7 | 0.0497 |     416 B |
| FormatProvider                 |  98.27 ns | 1.214 ns | 10,176,139.4 | 0.0459 |     384 B |
| FormatWithPrecisionAndProvider | 104.95 ns | 0.527 ns |  9,528,659.7 | 0.0497 |     416 B |
| CompactFormat                  | 105.48 ns | 1.087 ns |  9,480,609.3 | 0.0488 |     408 B |
| GeneralFormat                  | 133.23 ns | 0.967 ns |  7,505,636.1 | 0.0842 |     704 B |
| RondTripFormat                 |  88.45 ns | 0.992 ns | 11,305,598.0 | 0.0516 |     432 B |

## MoneyParsing
### v2.8-linux
| Method            | Mean     | Error   | Op/s        | Gen0   | Allocated |
|------------------ |---------:|--------:|------------:|-------:|----------:|
| Implicit          | 101.7 ns | 0.39 ns | 9,832,950.5 | 0.0191 |     160 B |
| ImplicitTry       | 113.3 ns | 0.27 ns | 8,827,052.4 | 0.0191 |     160 B |
| Explicit          | 113.0 ns | 0.80 ns | 8,846,720.3 | 0.0191 |     160 B |
| ExplicitAsSpan    | 102.3 ns | 0.79 ns | 9,773,753.7 | 0.0191 |     160 B |
| ExplicitTry       | 136.5 ns | 1.69 ns | 7,327,137.6 | 0.0191 |     160 B |
| ExplicitTryAsSpan | 129.2 ns | 1.35 ns | 7,739,427.9 | 0.0191 |     160 B |

## MoneyConversion
### v2.8-linux
| Method        | Mean       | Error     | Op/s                | Ratio  | Allocated | Alloc Ratio |
|-------------- |-----------:|----------:|--------------------:|-------:|----------:|------------:|
| ToDecimal     |  0.8353 ns | 0.0329 ns |     1,197,107,646.2 |  1.001 |         - |          NA |
| ToDouble      |  1.4372 ns | 0.0480 ns |       695,777,823.0 |  1.723 |         - |          NA |
| ToIn32        | 28.3997 ns | 0.3253 ns |        35,211,601.8 | 34.040 |         - |          NA |
| ToInt64       | 28.1164 ns | 0.2131 ns |        35,566,481.7 | 33.701 |         - |          NA |
| ToFastMoney   | 10.4538 ns | 0.1555 ns |        95,658,628.8 | 12.530 |         - |          NA |
| fToDecimal    |  2.0411 ns | 0.0458 ns |       489,927,445.8 |  2.447 |         - |          NA |
| fToDouble     |  2.6046 ns | 0.0573 ns |       383,931,295.1 |  3.122 |         - |          NA |
| fToIn32       |  3.3082 ns | 0.0492 ns |       302,279,471.6 |  3.965 |         - |          NA |
| fToInt64      |  3.3568 ns | 0.0479 ns |       297,904,763.5 |  4.023 |         - |          NA |
| fToMoney      | 17.1920 ns | 0.3528 ns |        58,166,648.2 | 20.607 |         - |          NA |
| fToSqlMoney   | 12.9546 ns | 0.1840 ns |        77,192,950.7 | 15.528 |         - |          NA |
| fToAOCurrency |  0.0007 ns | 0.0019 ns | 1,526,008,389,284.6 |  0.001 |         - |          NA |

## HighLoad
### v2.8-linux
| Method            |      Mean |     Error |   Op/s | Ratio |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 11.011 ms | 0.2096 ms |  90.82 |  0.39 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 28.304 ms | 0.2866 ms |  35.33 |  1.00 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MFastMoney | 24.538 ms | 0.3367 ms |  40.75 |  0.87 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 13.097 ms | 0.2596 ms |  76.35 |  0.46 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  2.201 ms | 0.0731 ms | 454.30 |  0.08 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |

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
