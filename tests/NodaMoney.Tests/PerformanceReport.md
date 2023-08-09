``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.22000
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4515.0), X64 RyuJIT
  DefaultJob : .NET Framework 4.8 (4.8.4515.0), X64 RyuJIT


```
## InitializingCurrency
#### before
|        Method |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
|      FromCode | 611.5 ns | 7.71 ns | 7.21 ns | 0.1354 |     - |     - |     429 B |
#### after
|        Method |     Mean |    Error |   StdDev | Allocated |
|-------------- |---------:|---------:|---------:|----------:|
|      FromCode | 22.53 ns | 0.398 ns | 0.353 ns |         - |
| FromCodeBeRef | 20.27 ns | 0.339 ns | 0.283 ns |         - |
### after as class
|        Method |     Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |---------:|---------:|---------:|------:|------:|------:|----------:|
|      FromCode | 20.32 ns | 0.666 ns | 1.952 ns |     - |     - |     - |         - |
| FromCodeBeRef | 21.68 ns | 0.661 ns | 1.949 ns |     - |     - |     - |         - |

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
|                              Method |      Mean |    Error |   StdDev | Ratio | RatioSD | Allocated |
|------------------------------------ |----------:|---------:|---------:|------:|--------:|----------:|
|            ExplicitCurrencyAsString | 160.89 ns | 1.788 ns | 1.585 ns |  1.00 |    0.00 |         - |
| ExplicitCurrencyAsStringAndRounding | 163.87 ns | 1.791 ns | 1.495 ns |  1.02 |    0.01 |         - |
|            ExplicitCurrencyFromCode | 159.54 ns | 1.680 ns | 1.489 ns |  0.99 |    0.01 |         - |
|                        HelperMethod | 159.12 ns | 1.227 ns | 1.148 ns |  0.99 |    0.01 |         - |
|       ImplicitCurrencyByConstructor | 171.27 ns | 2.014 ns | 1.884 ns |  1.06 |    0.01 |         - |
|           ImplicitCurrencyByCasting | 169.77 ns | 2.456 ns | 2.297 ns |  1.05 |    0.02 |         - |
|                         Deconstruct |  18.10 ns | 0.239 ns | 0.224 ns |  0.11 |    0.00 |         - |
### after as class
|                              Method |        Mean |     Error |     StdDev |      Median | Ratio | RatioSD | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |------------:|----------:|-----------:|------------:|------:|--------:|------:|------:|------:|----------:|
|            ExplicitCurrencyAsString | 291.2011 ns | 5.7582 ns | 10.6731 ns | 292.0155 ns | 1.000 |    0.00 |     - |     - |     - |         - |
| ExplicitCurrencyAsStringAndRounding | 291.1404 ns | 5.8337 ns | 12.3053 ns | 292.7434 ns | 0.997 |    0.05 |     - |     - |     - |         - |
|            ExplicitCurrencyFromCode | 293.2023 ns | 5.8591 ns | 11.8356 ns | 293.2638 ns | 1.008 |    0.05 |     - |     - |     - |         - |
|                        HelperMethod | 295.5661 ns | 5.9172 ns | 12.3514 ns | 293.7525 ns | 1.013 |    0.04 |     - |     - |     - |         - |
|       ImplicitCurrencyByConstructor | 304.9541 ns | 6.0387 ns | 11.3421 ns | 306.0941 ns | 1.049 |    0.04 |     - |     - |     - |         - |
|           ImplicitCurrencyByCasting | 304.1174 ns | 6.0599 ns | 12.2413 ns | 307.0236 ns | 1.044 |    0.05 |     - |     - |     - |         - |
|                         Deconstruct |   0.0451 ns | 0.0231 ns |  0.0486 ns |   0.0345 ns | 0.000 |    0.00 |     - |     - |     - |         - |

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
|                   Method |      Mean |    Error |   StdDev | Allocated |
|------------------------- |----------:|---------:|---------:|----------:|
|                 Addition | 162.28 ns | 1.918 ns | 1.701 ns |         - |
|              Subtraction | 162.34 ns | 3.158 ns | 2.954 ns |         - |
|      CompareSameCurrency |  25.84 ns | 0.432 ns | 0.383 ns |         - |
| CompareDifferentCurrency |  32.52 ns | 0.661 ns | 0.734 ns |         - |
|            CompareAmount |  19.30 ns | 0.365 ns | 0.305 ns |         - |
|                Increment | 385.33 ns | 2.862 ns | 2.677 ns |         - |
|                Decrement | 379.44 ns | 2.494 ns | 2.211 ns |         - |
### after as class
|                   Method |      Mean |     Error |    StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------- |----------:|----------:|----------:|------:|------:|------:|----------:|
|                 Addition | 301.01 ns |  5.962 ns | 11.198 ns |     - |     - |     - |         - |
|              Subtraction | 299.39 ns |  5.959 ns | 10.745 ns |     - |     - |     - |         - |
|      CompareSameCurrency |  28.17 ns |  0.586 ns |  1.196 ns |     - |     - |     - |         - |
| CompareDifferentCurrency |  38.29 ns |  0.792 ns |  2.235 ns |     - |     - |     - |         - |
|            CompareAmount |  37.34 ns |  0.768 ns |  2.115 ns |     - |     - |     - |         - |
|                Increment | 674.86 ns | 13.303 ns | 24.986 ns |     - |     - |     - |         - |
|                Decrement | 700.62 ns | 13.703 ns | 15.780 ns |     - |     - |     - |         - |

## MoneyFormatting
#### before
|             Method |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
|           Implicit | 329.4 ns | 2.74 ns | 2.56 ns | 0.0520 |     - |     - |     164 B |
| ImplicitWithFormat | 346.8 ns | 3.75 ns | 3.51 ns | 0.0520 |     - |     - |     164 B |
|           Explicit | 399.8 ns | 2.49 ns | 2.33 ns | 0.0939 |     - |     - |     296 B |
| ExplicitWithFormat | 414.0 ns | 2.07 ns | 1.94 ns | 0.0939 |     - |     - |     296 B |
#### after
|             Method |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
|------------------- |---------:|--------:|--------:|-------:|----------:|
|           Implicit | 147.9 ns | 2.97 ns | 2.78 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 159.0 ns | 3.17 ns | 2.81 ns | 0.0286 |     240 B |
|           Explicit | 215.9 ns | 3.92 ns | 3.67 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 218.0 ns | 4.41 ns | 4.12 ns | 0.0525 |     440 B |
### after as class
|             Method |     Mean |   Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------- |---------:|--------:|---------:|-------:|------:|------:|----------:|
|           Implicit | 255.7 ns | 5.10 ns | 12.33 ns | 0.0420 |     - |     - |     265 B |
| ImplicitWithFormat | 265.4 ns | 5.29 ns | 14.11 ns | 0.0420 |     - |     - |     265 B |
|           Explicit | 302.1 ns | 6.03 ns | 16.41 ns | 0.0763 |     - |     - |     481 B |
| ExplicitWithFormat | 310.4 ns | 8.04 ns | 23.72 ns | 0.0763 |     - |     - |     481 B |

## MoneyParsing
#### before
|      Method |     Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------ |---------:|----------:|----------:|-------:|------:|------:|----------:|
|    Implicit | 1.360 us | 0.0101 us | 0.0095 us | 0.1869 |     - |     - |     593 B |
| ImplicitTry | 1.314 us | 0.0108 us | 0.0101 us | 0.1869 |     - |     - |     593 B |
|    Explicit | 1.255 us | 0.0092 us | 0.0082 us | 0.2613 |     - |     - |     825 B |
| ExplicitTry | 1.341 us | 0.0052 us | 0.0049 us | 0.2613 |     - |     - |     825 B |
#### after
|      Method |       Mean |    Error |   StdDev |  Gen 0 | Allocated |
|------------ |-----------:|---------:|---------:|-------:|----------:|
|    Implicit | 2,644.6 ns | 46.39 ns | 56.97 ns | 0.1526 |   1,280 B |
| ImplicitTry | 2,598.3 ns | 26.53 ns | 22.15 ns | 0.1526 |   1,280 B |
|    Explicit |   442.4 ns |  4.92 ns |  4.36 ns | 0.0715 |     600 B |
| ExplicitTry |   459.5 ns |  7.30 ns |  6.83 ns | 0.0715 |     600 B |
### after as class
|      Method |       Mean |    Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------ |-----------:|---------:|----------:|-------:|------:|------:|----------:|
|    Implicit | 1,345.5 ns | 34.66 ns | 102.19 ns | 0.1354 |     - |     - |     859 B |
| ImplicitTry | 1,395.7 ns | 43.70 ns | 128.86 ns | 0.1354 |     - |     - |     859 B |
|    Explicit |   794.9 ns | 16.03 ns |  47.28 ns | 0.1030 |     - |     - |     650 B |
| ExplicitTry |   783.0 ns | 17.38 ns |  51.23 ns | 0.1030 |     - |     - |     650 B |

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
|        Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|-------------- |---------:|---------:|---------:|-------:|----------:|
| CreateBuilder | 32.55 ns | 0.541 ns | 0.556 ns | 0.0105 |      88 B |
|         Build | 28.38 ns | 0.557 ns | 0.547 ns |      - |         - |
### after as class
|        Method |     Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |---------:|---------:|---------:|-------:|------:|------:|----------:|
| CreateBuilder | 97.77 ns | 2.649 ns | 7.810 ns | 0.0139 |     - |     - |      88 B |
|         Build | 34.83 ns | 1.019 ns | 2.972 ns | 0.0127 |     - |     - |      80 B |

## HighLoad
#### before
|                     Method |     Mean |   Error |  StdDev |       Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|--------------------------- |---------:|--------:|--------:|------------:|----------:|----------:|----------:|
| CreatingOneMillionCurrency | 665.3 ms | 3.53 ms | 3.30 ms | 137000.0000 | 1000.0000 | 1000.0000 | 458.37 MB |
|    CreatingOneMillionMoney | 788.5 ms | 6.80 ms | 6.36 ms | 137000.0000 | 1000.0000 | 1000.0000 | 473.63 MB |
#### after
|                          Method |      Mean |    Error |   StdDev |    Median |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|-------------------------------- |----------:|---------:|---------:|----------:|----------:|---------:|---------:|----------:|
| CreatingOneMillionCurrencyUnits |  24.99 ms | 0.573 ms | 1.689 ms |  25.24 ms | 8093.7500 | 468.7500 | 468.7500 |     65 MB |
|      CreatingOneMillionCurrency |  45.06 ms | 1.173 ms | 3.328 ms |  44.97 ms |  333.3333 | 333.3333 | 333.3333 |     61 MB |
|         CreatingOneMillionMoney | 152.33 ms | 3.033 ms | 6.907 ms | 149.71 ms |         - |        - |        - |     76 MB |


### after Currency as class
|                          Method |      Mean |    Error |   StdDev |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|-------------------------------- |----------:|---------:|---------:|----------:|---------:|---------:|----------:|
| CreatingOneMillionCurrencyUnits |  24.28 ms | 0.480 ms | 1.401 ms | 8093.7500 | 468.7500 | 468.7500 |     64 MB |
|      CreatingOneMillionCurrency |  18.68 ms | 0.372 ms | 1.012 ms |  312.5000 | 312.5000 | 312.5000 |      8 MB |
|         CreatingOneMillionMoney | 122.52 ms | 1.551 ms | 1.451 ms |  200.0000 | 200.0000 | 200.0000 |     23 MB |
