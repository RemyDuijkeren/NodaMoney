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
|        Method |      Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |----------:|---------:|---------:|-------:|------:|------:|----------:|
|      FromCode | 100.48 ns | 0.513 ns | 0.455 ns | 0.0126 |     - |     - |      40 B |
| FromCodeBeRef |  84.08 ns | 0.473 ns | 0.395 ns | 0.0126 |     - |     - |      40 B |

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
|            ExplicitCurrencyAsString | 161.53 ns | 0.419 ns | 0.350 ns |  1.00 | 0.0126 |     - |     - |      40 B |
| ExplicitCurrencyAsStringAndRounding | 162.13 ns | 0.908 ns | 0.849 ns |  1.00 | 0.0126 |     - |     - |      40 B |
|            ExplicitCurrencyFromCode | 160.02 ns | 1.427 ns | 1.335 ns |  0.99 | 0.0126 |     - |     - |      40 B |
|                    HelperMethodEuro | 192.43 ns | 1.105 ns | 1.034 ns |  1.19 | 0.0126 |     - |     - |      40 B |
|                HelperMethodUSDollar | 185.27 ns | 1.072 ns | 1.003 ns |  1.15 | 0.0126 |     - |     - |      40 B |
|           HelperMethodPoundSterling | 185.70 ns | 0.796 ns | 0.665 ns |  1.15 | 0.0126 |     - |     - |      40 B |
|                     HelperMethodYen | 191.52 ns | 0.631 ns | 0.492 ns |  1.19 | 0.0126 |     - |     - |      40 B |
|       ImplicitCurrencyByConstructor | 178.10 ns | 0.837 ns | 0.783 ns |  1.10 | 0.0126 |     - |     - |      40 B |
|           ImplicitCurrencyByCasting | 203.81 ns | 0.851 ns | 0.796 ns |  1.26 | 0.0126 |     - |     - |      40 B |
|                         Deconstruct |  50.74 ns | 0.135 ns | 0.126 ns |  0.31 |      - |     - |     - |         - |


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
|                 Addition | 527.26 ns | 2.747 ns | 2.569 ns |     - |     - |     - |         - |
|              Subtraction | 527.67 ns | 2.006 ns | 1.876 ns |     - |     - |     - |         - |
|      CompareSameCurrency |  77.80 ns | 0.548 ns | 0.513 ns |     - |     - |     - |         - |
| CompareDifferentCurrency | 135.25 ns | 0.599 ns | 0.561 ns |     - |     - |     - |         - |
|            CompareAmount | 453.81 ns | 1.592 ns | 1.489 ns |     - |     - |     - |         - |
|                Increment | 737.24 ns | 7.947 ns | 7.434 ns |     - |     - |     - |         - |
|                Decrement | 730.24 ns | 1.991 ns | 1.862 ns |     - |     - |     - |         - |

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
|           Implicit | 299.3 ns | 0.95 ns | 0.84 ns | 0.0520 |     - |     - |     164 B |
| ImplicitWithFormat | 315.3 ns | 1.90 ns | 1.77 ns | 0.0520 |     - |     - |     164 B |
|           Explicit | 369.0 ns | 1.57 ns | 1.47 ns | 0.0939 |     - |     - |     296 B |
| ExplicitWithFormat | 388.7 ns | 1.30 ns | 1.16 ns | 0.0939 |     - |     - |     296 B |

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
|    Implicit | 1,206.5 ns |  3.29 ns |  2.74 ns | 0.1869 |     - |     - |     593 B |
| ImplicitTry | 1,215.7 ns | 10.76 ns | 10.06 ns | 0.1869 |     - |     - |     593 B |
|    Explicit |   693.0 ns |  4.83 ns |  4.52 ns | 0.1383 |     - |     - |     437 B |
| ExplicitTry |   677.4 ns |  3.34 ns |  3.13 ns | 0.1383 |     - |     - |     437 B |

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
|                     Method |     Mean |   Error |  StdDev |      Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|--------------------------- |---------:|--------:|--------:|-----------:|---------:|---------:|----------:|
| CreatingOneMillionCurrency | 119.5 ms | 0.84 ms | 0.79 ms | 13000.0000 | 400.0000 | 400.0000 |  76.35 MB |
|    CreatingOneMillionMoney | 193.4 ms | 1.20 ms | 1.12 ms | 13000.0000 | 333.3333 | 333.3333 |  91.61 MB |
