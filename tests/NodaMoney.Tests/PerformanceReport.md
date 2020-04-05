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
|        Method |     Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |---------:|---------:|---------:|------:|------:|------:|----------:|
|      FromCode | 42.42 ns | 0.176 ns | 0.165 ns |     - |     - |     - |         - |
| FromCodeBeRef | 26.27 ns | 0.112 ns | 0.105 ns |     - |     - |     - |         - |

## InitializingMoney
#### before
|                              Method |      Mean |    Error |   StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |----------:|---------:|---------:|------:|-------:|------:|------:|----------:|
|            ExplicitCurrencyAsString | 716.46 ns | 2.483 ns | 2.322 ns |  1.00 | 0.1354 |     - |     - |     429 B |
| ExplicitCurrencyAsStringAndRounding | 721.48 ns | 5.749 ns | 5.378 ns |  1.01 | 0.1354 |     - |     - |     429 B |
|            ExplicitCurrencyFromCode | 721.87 ns | 5.770 ns | 5.397 ns |  1.01 | 0.1354 |     - |     - |     429 B |
|                        HelperMethod | 762.76 ns | 3.969 ns | 3.315 ns |  1.06 | 0.1354 |     - |     - |     429 B |
|       ImplicitCurrencyByConstructor | 263.90 ns | 1.635 ns | 1.529 ns |  0.37 | 0.0124 |     - |     - |      40 B |
|           ImplicitCurrencyByCasting | 286.37 ns | 1.883 ns | 1.761 ns |  0.40 | 0.0124 |     - |     - |      40 B |
|                         Deconstruct |  63.05 ns | 0.328 ns | 0.307 ns |  0.09 |      - |     - |     - |         - |
#### after
|                              Method |      Mean |    Error |   StdDev | Ratio | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |----------:|---------:|---------:|------:|------:|------:|------:|----------:|
|            ExplicitCurrencyAsString | 102.11 ns | 0.467 ns | 0.437 ns |  1.00 |     - |     - |     - |         - |
| ExplicitCurrencyAsStringAndRounding | 101.62 ns | 0.747 ns | 0.699 ns |  1.00 |     - |     - |     - |         - |
|            ExplicitCurrencyFromCode | 101.75 ns | 0.305 ns | 0.254 ns |  1.00 |     - |     - |     - |         - |
|                        HelperMethod | 134.17 ns | 0.639 ns | 0.598 ns |  1.31 |     - |     - |     - |         - |
|       ImplicitCurrencyByConstructor | 126.04 ns | 0.607 ns | 0.568 ns |  1.23 |     - |     - |     - |         - |
|           ImplicitCurrencyByCasting | 155.17 ns | 0.992 ns | 0.928 ns |  1.52 |     - |     - |     - |         - |
|                         Deconstruct |  50.74 ns | 0.253 ns | 0.237 ns |  0.50 |     - |     - |     - |         - |


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
|                 Addition | 529.47 ns | 2.835 ns | 2.652 ns |     - |     - |     - |         - |
|              Subtraction | 525.55 ns | 2.973 ns | 2.781 ns |     - |     - |     - |         - |
|      CompareSameCurrency |  77.35 ns | 0.288 ns | 0.270 ns |     - |     - |     - |         - |
| CompareDifferentCurrency | 134.72 ns | 1.245 ns | 1.164 ns |     - |     - |     - |         - |
|            CompareAmount | 454.07 ns | 2.597 ns | 2.429 ns |     - |     - |     - |         - |
|                Increment | 726.17 ns | 3.534 ns | 2.951 ns |     - |     - |     - |         - |
|                Decrement | 729.73 ns | 4.295 ns | 3.807 ns |     - |     - |     - |         - |

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
|           Implicit | 298.0 ns | 2.30 ns | 2.15 ns | 0.0520 |     - |     - |     164 B |
| ImplicitWithFormat | 318.9 ns | 3.14 ns | 2.93 ns | 0.0520 |     - |     - |     164 B |
|           Explicit | 369.6 ns | 1.17 ns | 1.10 ns | 0.0939 |     - |     - |     296 B |
| ExplicitWithFormat | 387.8 ns | 2.46 ns | 2.18 ns | 0.0939 |     - |     - |     296 B |

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
|    Implicit | 1,102.5 ns |  5.30 ns |  4.96 ns | 0.1621 |     - |     - |     513 B |
| ImplicitTry | 1,090.2 ns | 13.29 ns | 12.43 ns | 0.1621 |     - |     - |     513 B |
|    Explicit |   620.4 ns |  3.04 ns |  2.84 ns | 0.1259 |     - |     - |     397 B |
| ExplicitTry |   614.0 ns |  5.72 ns |  5.35 ns | 0.1259 |     - |     - |     397 B |

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
|                     Method |      Mean |    Error |   StdDev |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|--------------------------- |----------:|---------:|---------:|---------:|---------:|---------:|----------:|
| CreatingOneMillionCurrency |  64.14 ms | 1.284 ms | 1.201 ms | 875.0000 | 875.0000 | 875.0000 |  38.15 MB |
|    CreatingOneMillionMoney | 134.87 ms | 0.454 ms | 0.402 ms | 750.0000 | 750.0000 | 750.0000 |  53.41 MB |
