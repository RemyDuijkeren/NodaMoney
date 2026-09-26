_Environment: BenchmarkDotNet v0.15.8, Linux Omarchy AMD Ryzen 7 8845HS w/ Radeon 780M Graphics 1.09GHz, 1 CPU, 16 logical and 8 physical cores .NET SDK 10.0.401_

## InitializingCurrency
### v2.9-linux
| Method                  | Mean     | Error     | Op/s          | Allocated |
|------------------------ |---------:|----------:|--------------:|----------:|
| CurrencyFromCode        | 4.792 ns | 0.0113 ns | 208,682,339.3 |         - |
| CurrencyInfoFromCode    | 4.440 ns | 0.0252 ns | 225,209,268.8 |         - |
| CurrencyInfoTryFromCode | 4.155 ns | 0.0221 ns | 240,646,461.3 |         - |

## InitializingMoney
### v2.9-linux
| Method                        | Mean       | Error     | Op/s            | Ratio | Allocated | Alloc Ratio |
|------------------------------ |-----------:|----------:|----------------:|------:|----------:|------------:|
| CurrencyCode                  | 19.4154 ns | 0.0829 ns |    51,505,380.7 |  1.00 |         - |          NA |
| CurrencyCodeNeedsRounding     | 21.5546 ns | 0.0286 ns |    46,393,714.1 |  1.11 |         - |          NA |
| fCurrencyCode                 | 12.6826 ns | 0.0257 ns |    78,848,217.2 |  0.65 |         - |          NA |
| CurrencyCodeAndRoundingMode   | 21.3125 ns | 0.0192 ns |    46,920,755.1 |  1.10 |         - |          NA |
| CurrencyCodeAndContext        | 28.7442 ns | 0.0741 ns |    34,789,631.8 |  1.48 |         - |          NA |
| CurrencyFromCode              | 19.1410 ns | 0.0308 ns |    52,243,941.5 |  0.99 |         - |          NA |
| CurrencyInfoFromCode          | 19.5165 ns | 0.0164 ns |    51,238,614.2 |  1.01 |         - |          NA |
| ExtensionMethodEuro           | 19.2789 ns | 0.0610 ns |    51,870,225.0 |  0.99 |         - |          NA |
| ImplicitCurrencyByConstructor | 15.6066 ns | 0.0272 ns |    64,075,288.7 |  0.80 |         - |          NA |
| ImplicitCurrencyByCasting     | 15.5554 ns | 0.0268 ns |    64,286,154.4 |  0.80 |         - |          NA |
| Deconstruct                   |  0.8231 ns | 0.0060 ns | 1,214,937,912.5 |  0.04 |         - |          NA |

## MoneyEquals
### v2.9-linux
| Method            | Mean      | Error     | Op/s             | Ratio | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-----------------:|------:|----------:|------------:|
| Equal             | 3.2436 ns | 0.0135 ns |    308,304,086.1 | 1.000 |         - |          NA |
| NotEqualValue     | 2.3685 ns | 0.0083 ns |    422,207,661.3 | 0.730 |         - |          NA |
| NotEqualCurrency  | 0.5863 ns | 0.0074 ns |  1,705,614,214.5 | 0.181 |         - |          NA |
| EqualOrBigger     | 4.3151 ns | 0.0083 ns |    231,745,086.2 | 1.330 |         - |          NA |
| Bigger            | 4.4053 ns | 0.0150 ns |    227,001,013.5 | 1.358 |         - |          NA |
| fEqual            | 0.0119 ns | 0.0073 ns | 83,716,980,420.1 | 0.004 |         - |          NA |
| fNotEqualValue    | 0.2305 ns | 0.0022 ns |  4,338,980,914.1 | 0.071 |         - |          NA |
| fNotEqualCurrency | 0.0722 ns | 0.0065 ns | 13,854,914,584.2 | 0.022 |         - |          NA |
| fEqualOrBigger    | 1.1807 ns | 0.0093 ns |    846,980,785.9 | 0.364 |         - |          NA |
| fBigger           | 1.1849 ns | 0.0052 ns |    843,920,005.7 | 0.365 |         - |          NA |

## MoneyOperations
### v2.9-linux
| Method                  | Mean      | Error     | Op/s          | Ratio | Allocated | Alloc Ratio |
|------------------------ |----------:|----------:|--------------:|------:|----------:|------------:|
| Add                     | 16.404 ns | 0.0936 ns |  60,960,929.6 | 13.69 |         - |          NA |
| AddMixedScale           | 17.611 ns | 0.1031 ns |  56,783,495.9 | 14.70 |         - |          NA |
| Subtract                | 17.184 ns | 0.1270 ns |  58,192,692.8 | 14.34 |         - |          NA |
| Multiple                | 19.092 ns | 0.1085 ns |  52,377,791.4 | 15.94 |         - |          NA |
| Divide                  | 63.358 ns | 0.6121 ns |  15,783,370.4 | 52.89 |         - |          NA |
| Increment               |  7.643 ns | 0.1098 ns | 130,830,863.8 |  6.38 |         - |          NA |
| Decrement               |  9.034 ns | 0.0611 ns | 110,691,143.5 |  7.54 |         - |          NA |
| Remainder               | 17.780 ns | 0.0718 ns |  56,242,694.7 | 14.84 |         - |          NA |
| fAdd                    |  1.198 ns | 0.0119 ns | 834,658,443.4 |  1.00 |         - |          NA |
| fSubtract               |  1.192 ns | 0.0108 ns | 838,972,962.1 |  0.99 |         - |          NA |
| fMultipleDec            | 16.397 ns | 0.0625 ns |  60,985,315.4 | 13.69 |         - |          NA |
| fMultipleDecWholeNumber |  8.024 ns | 0.0831 ns | 124,625,460.8 |  6.70 |         - |          NA |
| fMultipleLong           |  2.322 ns | 0.0131 ns | 430,630,750.5 |  1.94 |         - |          NA |
| fDivideDec              | 16.216 ns | 0.0796 ns |  61,667,290.3 | 13.54 |         - |          NA |
| fDivideDecWholeNumber   | 10.012 ns | 0.0807 ns |  99,877,835.2 |  8.36 |         - |          NA |
| fDivideLong             |  2.300 ns | 0.0268 ns | 434,725,822.4 |  1.92 |         - |          NA |
| fIncrement              |  2.751 ns | 0.0272 ns | 363,456,105.1 |  2.30 |         - |          NA |
| fDecrement              |  2.779 ns | 0.0178 ns | 359,827,914.2 |  2.32 |         - |          NA |
| fRemainder              |  1.785 ns | 0.0106 ns | 560,288,361.7 |  1.49 |         - |          NA |

## MoneyFormatting
### v2.9-linux
| Method                         | Mean     | Error    | Op/s         | Gen0   | Allocated |
|------------------------------- |---------:|---------:|-------------:|-------:|----------:|
| DefaultFormat                  | 62.91 ns | 0.376 ns | 15,895,971.7 | 0.0086 |      72 B |
| FormatWithPrecision            | 60.98 ns | 0.340 ns | 16,398,266.6 | 0.0086 |      72 B |
| FormatProvider                 | 89.79 ns | 0.533 ns | 11,136,906.8 | 0.0459 |     384 B |
| FormatWithPrecisionAndProvider | 86.03 ns | 0.379 ns | 11,623,271.1 | 0.0459 |     384 B |
| CompactFormat                  | 77.61 ns | 0.149 ns | 12,884,186.9 | 0.0114 |      96 B |
| GeneralFormat                  | 78.06 ns | 1.575 ns | 12,810,420.0 | 0.0095 |      80 B |
| RondTripFormat                 | 59.46 ns | 0.286 ns | 16,818,076.8 | 0.0143 |     120 B |

## MoneyParsing
### v2.9-linux
| Method            | Mean     | Error    | Op/s         | Gen0   | Allocated |
|------------------ |---------:|---------:|-------------:|-------:|----------:|
| Implicit          | 72.55 ns | 0.558 ns | 13,783,684.7 | 0.0029 |      24 B |
| ImplicitTry       | 77.14 ns | 0.526 ns | 12,964,156.7 | 0.0029 |      24 B |
| Explicit          | 77.62 ns | 0.360 ns | 12,882,958.1 | 0.0029 |      24 B |
| ExplicitAsSpan    | 74.00 ns | 0.318 ns | 13,514,164.5 | 0.0029 |      24 B |
| ExplicitTry       | 89.35 ns | 0.466 ns | 11,192,107.9 | 0.0029 |      24 B |
| ExplicitTryAsSpan | 90.91 ns | 0.588 ns | 10,999,539.6 | 0.0029 |      24 B |

## MoneyConversion
### v2.9-linux
| Method        | Mean       | Error     | Op/s              | Ratio  | Allocated | Alloc Ratio |
|-------------- |-----------:|----------:|------------------:|-------:|----------:|------------:|
| ToDecimal     |  1.1641 ns | 0.0099 ns |     859,034,697.7 |  1.000 |         - |          NA |
| ToDouble      |  1.4292 ns | 0.0242 ns |     699,697,678.8 |  1.228 |         - |          NA |
| ToIn32        |  2.7147 ns | 0.0328 ns |     368,360,810.6 |  2.332 |         - |          NA |
| ToInt64       |  3.0470 ns | 0.0118 ns |     328,193,997.8 |  2.618 |         - |          NA |
| ToFastMoney   |  8.9505 ns | 0.0686 ns |     111,725,773.1 |  7.689 |         - |          NA |
| fToDecimal    |  2.0866 ns | 0.0131 ns |     479,244,928.1 |  1.793 |         - |          NA |
| fToDouble     |  2.6582 ns | 0.0170 ns |     376,200,773.9 |  2.284 |         - |          NA |
| fToIn32       |  1.9591 ns | 0.0157 ns |     510,428,900.2 |  1.683 |         - |          NA |
| fToInt64      |  1.8370 ns | 0.0105 ns |     544,364,732.2 |  1.578 |         - |          NA |
| fToMoney      | 16.9695 ns | 0.1587 ns |      58,929,328.7 | 14.578 |         - |          NA |
| fToSqlMoney   |  0.0028 ns | 0.0029 ns | 357,947,286,111.6 |  0.002 |         - |          NA |
| fFromSqlMoney |  6.9464 ns | 0.0247 ns |     143,959,060.4 |  5.967 |         - |          NA |
| fToAOCurrency |  0.0017 ns | 0.0036 ns | 595,481,760,012.2 |  0.001 |         - |          NA |

## HighLoad
### v2.9-linux
| Method            | Mean      | Error     | Op/s   | Ratio | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
|------------------ |----------:|----------:|-------:|------:|---------:|---------:|---------:|----------:|------------:|
| Create1MCurrency  |  6.545 ms | 0.0355 ms | 152.79 |  0.24 | 492.1875 | 492.1875 | 492.1875 |   1.91 MB |        0.13 |
| Create1MMoney     | 26.783 ms | 0.0379 ms |  37.34 |  1.00 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MFastMoney | 16.226 ms | 0.1022 ms |  61.63 |  0.61 | 500.0000 | 500.0000 | 500.0000 |  11.44 MB |        0.75 |
| Create1MSqlMoney  | 12.640 ms | 0.0362 ms |  79.11 |  0.47 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |
| Create1MDecimal   |  1.926 ms | 0.0182 ms | 519.26 |  0.07 | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |        1.00 |

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
