## InitializingCurrency
### v2.8-linux
| Method                  | Mean     | Error     | Op/s          | Allocated |
|------------------------ |---------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 9.613 ns | 0.2069 ns | 104,025,563.4 |         - |
| CurrencyInfoFromCode    | 4.317 ns | 0.0278 ns | 231,641,612.1 |         - |
| CurrencyInfoTryFromCode | 5.160 ns | 0.1116 ns | 193,811,049.0 |         - |
### v2.9-linux
| Method                  | Mean     | Error     | Op/s          | Allocated |
|------------------------ |---------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 4.768 ns | 0.0236 ns | 209,725,066.4 |         - |
| CurrencyInfoFromCode    | 4.408 ns | 0.0177 ns | 226,881,898.0 |         - |
| CurrencyInfoTryFromCode | 4.210 ns | 0.0096 ns | 237,543,739.4 |         - |

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
### v2.9-linux
| Method                        | Mean       | Error     | Op/s            | Ratio | Allocated | Alloc Ratio |
|------------------------------ |-----------:|----------:|----------------:|------:|----------:|------------:|
| CurrencyCode                  | 19.3065 ns | 0.0655 ns |    51,795,958.2 |  1.00 |         - |          NA |
| CurrencyCodeNeedsRounding     | 22.1643 ns | 0.0532 ns |    45,117,499.8 |  1.15 |         - |          NA |
| fCurrencyCode                 | 10.6591 ns | 0.0327 ns |    93,816,121.6 |  0.55 |         - |          NA |
| CurrencyCodeAndRoundingMode   | 23.1459 ns | 0.0179 ns |    43,204,168.9 |  1.20 |         - |          NA |
| CurrencyCodeAndContext        | 23.2087 ns | 0.0630 ns |    43,087,291.5 |  1.20 |         - |          NA |
| CurrencyFromCode              | 19.2592 ns | 0.0668 ns |    51,923,217.7 |  1.00 |         - |          NA |
| CurrencyInfoFromCode          | 19.1432 ns | 0.0598 ns |    52,237,921.1 |  0.99 |         - |          NA |
| ExtensionMethodEuro           | 19.9933 ns | 0.3017 ns |    50,016,654.9 |  1.04 |         - |          NA |
| ImplicitCurrencyByConstructor | 18.9536 ns | 0.2422 ns |    52,760,451.1 |  0.98 |         - |          NA |
| ImplicitCurrencyByCasting     | 17.3676 ns | 0.0138 ns |    57,578,508.8 |  0.90 |         - |          NA |
| Deconstruct                   |  0.8186 ns | 0.0027 ns | 1,221,617,453.8 |  0.04 |         - |          NA |

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
### v2.9-linux
| Method            | Mean      | Error     | Op/s             | Ratio | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-----------------:|------:|----------:|------------:|
| Equal             | 3.3580 ns | 0.0307 ns |    297,792,716.9 |  1.00 |         - |          NA |
| NotEqualValue     | 1.4624 ns | 0.0254 ns |    683,814,481.7 |  0.44 |         - |          NA |
| NotEqualCurrency  | 0.5413 ns | 0.0194 ns |  1,847,568,541.8 |  0.16 |         - |          NA |
| EqualOrBigger     | 4.5086 ns | 0.0190 ns |    221,798,218.4 |  1.34 |         - |          NA |
| Bigger            | 4.5918 ns | 0.0303 ns |    217,777,367.6 |  1.37 |         - |          NA |
| fEqual            | 0.0394 ns | 0.0103 ns | 25,350,120,006.2 |  0.01 |         - |          NA |
| fNotEqualValue    | 0.1383 ns | 0.0052 ns |  7,228,250,235.8 |  0.04 |         - |          NA |
| fNotEqualCurrency | 0.2307 ns | 0.0023 ns |  4,335,512,350.8 |  0.07 |         - |          NA |
| fEqualOrBigger    | 1.1878 ns | 0.0211 ns |    841,869,698.3 |  0.35 |         - |          NA |
| fBigger           | 1.2312 ns | 0.0047 ns |    812,209,340.0 |  0.37 |         - |          NA |

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
### v2.9-linux
| Method                  | Mean      | Error     | Op/s          | Ratio | Allocated | Alloc Ratio |
|------------------------ |----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 19.971 ns | 0.0534 ns |  50,072,718.9 | 16.50 |         - |          NA |
| AddMixedScale           | 20.761 ns | 0.0920 ns |  48,167,101.3 | 17.15 |         - |          NA |
| Subtract                | 21.168 ns | 0.0732 ns |  47,240,300.0 | 17.49 |         - |          NA |
| Multiple                | 19.556 ns | 0.0462 ns |  51,136,142.4 | 16.16 |         - |          NA |
| Divide                  | 59.976 ns | 0.1724 ns |  16,673,415.6 | 49.55 |         - |          NA |
| Increment               |  8.025 ns | 0.0922 ns | 124,617,806.9 |  6.63 |         - |          NA |
| Decrement               |  7.720 ns | 0.0718 ns | 129,534,340.6 |  6.38 |         - |          NA |
| Remainder               | 18.490 ns | 0.0928 ns |  54,082,394.4 | 15.28 |         - |          NA |
| fAdd                    |  1.211 ns | 0.0296 ns | 825,757,923.4 |  1.00 |         - |          NA |
| fSubtract               |  1.186 ns | 0.0036 ns | 843,171,122.2 |  0.98 |         - |          NA |
| fMultipleDec            | 16.366 ns | 0.0840 ns |  61,103,935.9 | 13.52 |         - |          NA |
| fMultipleDecWholeNumber |  7.972 ns | 0.0355 ns | 125,432,887.3 |  6.59 |         - |          NA |
| fMultipleLong           |  1.911 ns | 0.0074 ns | 523,339,752.7 |  1.58 |         - |          NA |
| fDivideDec              | 15.994 ns | 0.0250 ns |  62,522,065.8 | 13.21 |         - |          NA |
| fDivideDecWholeNumber   |  9.992 ns | 0.0340 ns | 100,082,422.3 |  8.25 |         - |          NA |
| fDivideLong             |  1.891 ns | 0.0180 ns | 528,798,404.7 |  1.56 |         - |          NA |
| fIncrement              |  2.739 ns | 0.0090 ns | 365,109,462.2 |  2.26 |         - |          NA |
| fDecrement              |  2.727 ns | 0.0044 ns | 366,650,376.8 |  2.25 |         - |          NA |
| fRemainder              |  1.726 ns | 0.0058 ns | 579,418,505.3 |  1.43 |         - |          NA |

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
### v2.9-linux
| Method                         | Mean     | Error    | Op/s         | Gen0   | Allocated |
|------------------------------- |---------:|---------:|-------------:|-------:|----------:|
| DefaultFormat                  | 60.01 ns | 0.109 ns | 16,663,727.8 | 0.0048 |      40 B |
| FormatWithPrecision            | 61.07 ns | 0.265 ns | 16,375,675.5 | 0.0048 |      40 B |
| FormatProvider                 | 89.55 ns | 0.636 ns | 11,167,261.6 | 0.0421 |     352 B |
| FormatWithPrecisionAndProvider | 92.59 ns | 1.430 ns | 10,800,632.3 | 0.0421 |     352 B |
| CompactFormat                  | 76.41 ns | 1.079 ns | 13,087,363.5 | 0.0076 |      64 B |
| GeneralFormat                  | 79.82 ns | 0.216 ns | 12,528,097.1 | 0.0057 |      48 B |
| RondTripFormat                 | 60.69 ns | 1.100 ns | 16,478,050.4 | 0.0105 |      88 B |

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
### v2.9-linux
| Method            | Mean     | Error    | Op/s         | Allocated |
|------------------ |---------:|---------:|-------------:|----------:|
| Implicit          | 64.80 ns | 0.421 ns | 15,431,939.9 |         - |
| ImplicitTry       | 74.08 ns | 0.124 ns | 13,499,201.9 |         - |
| Explicit          | 69.20 ns | 0.103 ns | 14,451,109.1 |         - |
| ExplicitAsSpan    | 67.51 ns | 0.036 ns | 14,812,896.5 |         - |
| ExplicitTry       | 74.10 ns | 0.110 ns | 13,495,023.1 |         - |
| ExplicitTryAsSpan | 77.24 ns | 0.171 ns | 12,946,726.4 |         - |

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
### v2.9-linux
| Method        | Mean      | Error     | Op/s              | Ratio  | Allocated | Alloc Ratio |
|-------------- |----------:|----------:|------------------:|-------:|----------:|------------:|
| ToDecimal     | 0.8243 ns | 0.0126 ns |   1,213,151,284.0 |  1.000 |         - |          NA |
| ToDouble      | 1.4029 ns | 0.0070 ns |     712,828,045.3 |  1.702 |         - |          NA |
| ToIn32        | 2.7476 ns | 0.0139 ns |     363,957,472.6 |  3.334 |         - |          NA |
| ToInt64       | 2.5270 ns | 0.0232 ns |     395,720,055.7 |  3.066 |         - |          NA |
| ToFastMoney   | 4.9103 ns | 0.0124 ns |     203,654,342.1 |  5.958 |         - |          NA |
| fToDecimal    | 2.1168 ns | 0.0274 ns |     472,422,083.0 |  2.568 |         - |          NA |
| fToDouble     | 2.6761 ns | 0.0301 ns |     373,679,035.1 |  3.247 |         - |          NA |
| fToIn32       | 1.9199 ns | 0.0173 ns |     520,868,521.7 |  2.330 |         - |          NA |
| fToInt64      | 1.9319 ns | 0.0191 ns |     517,630,930.7 |  2.344 |         - |          NA |
| fToMoney      | 9.2528 ns | 0.0271 ns |     108,075,354.4 | 11.227 |         - |          NA |
| fToSqlMoney   | 0.0029 ns | 0.0035 ns | 343,091,372,396.0 |  0.004 |         - |          NA |
| fFromSqlMoney | 7.0716 ns | 0.0306 ns |     141,410,103.3 |  8.581 |         - |          NA |
| fToAOCurrency | 0.0000 ns | 0.0000 ns |          Infinity |  0.000 |         - |          NA |


## HighLoad
### v2.8-linux
| Method            |      Mean |     Error |   Op/s | Ratio |     Gen0 |     Gen1 |     Gen2 | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  | 11.011 ms | 0.2096 ms |  90.82 |  0.39 | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |        0.13 |
| Create1MMoney     | 28.304 ms | 0.2866 ms |  35.33 |  1.00 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MFastMoney | 24.538 ms | 0.3367 ms |  40.75 |  0.87 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 13.097 ms | 0.2596 ms |  76.35 |  0.46 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  2.201 ms | 0.0731 ms | 454.30 |  0.08 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
### v2.9-linux
| Method            | Mean      | Error     | Op/s   | Ratio | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  |  6.759 ms | 0.1346 ms | 147.94 |  3.26 | 492.1875 | 492.1875 | 492.1875 |   1.91 MB |        0.13 |
| Create1MMoney     | 26.029 ms | 0.2448 ms |  38.42 | 12.56 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MFastMoney | 14.044 ms | 0.0936 ms |  71.20 |  6.78 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 12.860 ms | 0.0334 ms |  77.76 |  6.21 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  2.072 ms | 0.0269 ms | 482.52 |  1.00 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |

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
