``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i5-4690K CPU 3.50GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4121.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8 (4.8.4121.0), X86 LegacyJIT


```
## InitializingCurrency
#### before
|        Method |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
|      FromCode | 611.5 ns | 7.71 ns | 7.21 ns | 0.1354 |     - |     - |     429 B |
#### after
|        Method |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
|      FromCode | 121.6 ns | 1.19 ns | 1.11 ns | 0.0126 |     - |     - |      40 B |
| FromCodeBeRef | 102.8 ns | 0.63 ns | 0.59 ns | 0.0126 |     - |     - |      40 B |

## InitializingMoney
#### before
|                              Method |      Mean |    Error |   StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |----------:|---------:|---------:|------:|-------:|------:|------:|----------:|
|            ExplicitCurrencyAsString | 716.46 ns | 2.483 ns | 2.322 ns |  1.00 | 0.1354 |     - |     - |     429 B |
| ExplicitCurrencyAsStringAndRounding | 721.48 ns | 5.749 ns | 5.378 ns |  1.01 | 0.1354 |     - |     - |     429 B |
|            ExplicitCurrencyFromCode | 721.87 ns | 5.770 ns | 5.397 ns |  1.01 | 0.1354 |     - |     - |     429 B |
|                    HelperMethodEuro | 762.76 ns | 3.969 ns | 3.315 ns |  1.06 | 0.1354 |     - |     - |     429 B |
|                HelperMethodUSDollar | 756.89 ns | 3.969 ns | 3.713 ns |  1.06 | 0.1354 |     - |     - |     429 B |
|           HelperMethodPoundSterling | 754.99 ns | 5.144 ns | 4.812 ns |  1.05 | 0.1354 |     - |     - |     429 B |
|                     HelperMethodYen | 742.89 ns | 4.413 ns | 4.128 ns |  1.04 | 0.1354 |     - |     - |     429 B |
|       ImplicitCurrencyByConstructor | 263.90 ns | 1.635 ns | 1.529 ns |  0.37 | 0.0124 |     - |     - |      40 B |
|           ImplicitCurrencyByCasting | 286.37 ns | 1.883 ns | 1.761 ns |  0.40 | 0.0124 |     - |     - |      40 B |
|                         Deconstruct |  63.05 ns | 0.328 ns | 0.307 ns |  0.09 |      - |     - |     - |         - |
#### after
|                              Method |      Mean |    Error |   StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |----------:|---------:|---------:|------:|-------:|------:|------:|----------:|
|            ExplicitCurrencyAsString | 190.49 ns | 0.846 ns | 0.706 ns |  1.00 | 0.0126 |     - |     - |      40 B |
| ExplicitCurrencyAsStringAndRounding | 184.70 ns | 1.050 ns | 0.982 ns |  0.97 | 0.0126 |     - |     - |      40 B |
|            ExplicitCurrencyFromCode | 191.36 ns | 1.206 ns | 1.128 ns |  1.00 | 0.0126 |     - |     - |      40 B |
|                    HelperMethodEuro | 228.97 ns | 1.485 ns | 1.389 ns |  1.20 | 0.0126 |     - |     - |      40 B |
|                HelperMethodUSDollar | 225.82 ns | 0.759 ns | 0.634 ns |  1.19 | 0.0126 |     - |     - |      40 B |
|           HelperMethodPoundSterling | 221.59 ns | 0.886 ns | 0.828 ns |  1.16 | 0.0126 |     - |     - |      40 B |
|                     HelperMethodYen | 229.38 ns | 1.652 ns | 1.465 ns |  1.20 | 0.0126 |     - |     - |      40 B |
|       ImplicitCurrencyByConstructor | 190.87 ns | 0.893 ns | 0.791 ns |  1.00 | 0.0126 |     - |     - |      40 B |
|           ImplicitCurrencyByCasting | 222.92 ns | 1.839 ns | 1.720 ns |  1.17 | 0.0126 |     - |     - |      40 B |
|                         Deconstruct |  63.20 ns | 0.538 ns | 0.503 ns |  0.33 |      - |     - |     - |         - |


## MoneyOperations
#### before
|                   Method |      Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------- |----------:|---------:|---------:|------:|------:|------:|----------:|
|                 Addition | 589.40 ns | 2.720 ns | 2.544 ns |     - |     - |     - |         - |
|              Subtraction | 590.93 ns | 3.519 ns | 3.291 ns |     - |     - |     - |         - |
|      CompareSameCurrency |  87.50 ns | 0.342 ns | 0.320 ns |     - |     - |     - |         - |
| CompareDifferentCurrency | 158.99 ns | 0.624 ns | 0.584 ns |     - |     - |     - |         - |
|            CompareAmount | 474.49 ns | 1.128 ns | 1.000 ns |     - |     - |     - |         - |
|                Increment | 855.93 ns | 2.963 ns | 2.771 ns |     - |     - |     - |         - |
|                Decrement | 848.59 ns | 4.146 ns | 3.878 ns |     - |     - |     - |         - |
#### after
|                   Method |      Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------- |----------:|---------:|---------:|------:|------:|------:|----------:|
|                 Addition | 559.68 ns | 3.602 ns | 3.369 ns |     - |     - |     - |         - |
|              Subtraction | 560.67 ns | 2.530 ns | 2.243 ns |     - |     - |     - |         - |
|      CompareSameCurrency |  87.17 ns | 0.331 ns | 0.309 ns |     - |     - |     - |         - |
| CompareDifferentCurrency | 160.07 ns | 0.771 ns | 0.683 ns |     - |     - |     - |         - |
|            CompareAmount | 463.21 ns | 1.968 ns | 1.841 ns |     - |     - |     - |         - |
|                Increment | 797.14 ns | 3.444 ns | 3.222 ns |     - |     - |     - |         - |
|                Decrement | 790.35 ns | 2.992 ns | 2.798 ns |     - |     - |     - |         - |

## MoneyFormatting
#### before
|             Method |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
|           Implicit | 329.4 ns | 2.74 ns | 2.56 ns | 0.0520 |     - |     - |     164 B |
| ImplicitWithFormat | 346.8 ns | 3.75 ns | 3.51 ns | 0.0520 |     - |     - |     164 B |
|           Explicit | 399.8 ns | 2.49 ns | 2.33 ns | 0.0939 |     - |     - |     296 B |
| ExplicitWithFormat | 414.0 ns | 2.07 ns | 1.94 ns | 0.0939 |     - |     - |     296 B |
#### after
|             Method |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
|           Implicit | 301.7 ns | 1.89 ns | 1.68 ns | 0.0520 |     - |     - |     164 B |
| ImplicitWithFormat | 320.3 ns | 2.39 ns | 2.24 ns | 0.0520 |     - |     - |     164 B |
|           Explicit | 374.8 ns | 2.86 ns | 2.68 ns | 0.0939 |     - |     - |     296 B |
| ExplicitWithFormat | 391.1 ns | 1.61 ns | 1.50 ns | 0.0939 |     - |     - |     296 B |

## MoneyParsing
#### before
|      Method |     Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------ |---------:|----------:|----------:|-------:|------:|------:|----------:|
|    Implicit | 1.360 us | 0.0101 us | 0.0095 us | 0.1869 |     - |     - |     593 B |
| ImplicitTry | 1.314 us | 0.0108 us | 0.0101 us | 0.1869 |     - |     - |     593 B |
|    Explicit | 1.255 us | 0.0092 us | 0.0082 us | 0.2613 |     - |     - |     825 B |
| ExplicitTry | 1.341 us | 0.0052 us | 0.0049 us | 0.2613 |     - |     - |     825 B |
#### after
|      Method |       Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------ |-----------:|---------:|---------:|-------:|------:|------:|----------:|
|    Implicit | 1,237.1 ns | 11.94 ns | 11.17 ns | 0.1869 |     - |     - |     593 B |
| ImplicitTry | 1,254.3 ns |  7.99 ns |  7.09 ns | 0.1869 |     - |     - |     593 B |
|    Explicit |   721.7 ns |  4.41 ns |  4.12 ns | 0.1383 |     - |     - |     437 B |
| ExplicitTry |   717.4 ns |  3.94 ns |  3.49 ns | 0.1383 |     - |     - |     437 B |

## AddingCustomCurrency
#### before
|        Method |      Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |----------:|---------:|---------:|-------:|------:|------:|----------:|
| CreateBuilder |  22.81 ns | 0.382 ns | 0.357 ns | 0.0191 |     - |     - |      60 B |
|         Build |  73.28 ns | 0.523 ns | 0.489 ns |      - |     - |     - |         - |
|      Register |        NA |       NA |       NA |      - |     - |     - |         - |
|    Unregister |        NA |       NA |       NA |      - |     - |     - |         - |
|       Replace | 601.07 ns | 4.807 ns | 4.496 ns | 0.0668 |     - |     - |     212 B |

Benchmarks with issues:
  AddingCustomCurrencyBenchmarks.Register: DefaultJob
  AddingCustomCurrencyBenchmarks.Unregister: DefaultJob
#### after
|        Method |             Mean |            Error |           StdDev |    Gen 0 |    Gen 1 |    Gen 2 |  Allocated |
|-------------- |-----------------:|-----------------:|-----------------:|---------:|---------:|---------:|-----------:|
| CreateBuilder |         22.83 ns |         0.151 ns |         0.134 ns |   0.0191 |        - |        - |       60 B |
|         Build |         70.03 ns |         0.360 ns |         0.337 ns |        - |        - |        - |          - |
|       Replace | 23,254,957.65 ns | 2,227,991.312 ns | 6,569,282.653 ns | 151.7334 | 151.3672 | 151.3672 | 64979652 B |

## HighLoad
#### before
|                     Method |     Mean |   Error |  StdDev |       Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|--------------------------- |---------:|--------:|--------:|------------:|----------:|----------:|----------:|
| CreatingOneMillionCurrency | 665.3 ms | 3.53 ms | 3.30 ms | 137000.0000 | 1000.0000 | 1000.0000 | 458.37 MB |
|    CreatingOneMillionMoney | 788.5 ms | 6.80 ms | 6.36 ms | 137000.0000 | 1000.0000 | 1000.0000 | 473.63 MB |
#### after
|                     Method |     Mean |   Error |  StdDev |       Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|--------------------------- |---------:|--------:|--------:|------------:|----------:|----------:|----------:|
| CreatingOneMillionCurrency | 146.6 ms | 1.94 ms | 1.82 ms |  12750.0000 |  250.0000 |  250.0000 |   87.8 MB |
|    CreatingOneMillionMoney | 228.4 ms | 2.31 ms | 2.16 ms |  13000.0000 |  333.3333 |  333.3333 | 103.06 MB |
