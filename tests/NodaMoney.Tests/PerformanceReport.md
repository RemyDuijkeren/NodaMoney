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
|        Method |     Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |---------:|---------:|---------:|------:|------:|------:|----------:|
|      FromCode | 14.68 ns | 0.313 ns | 0.396 ns |     - |     - |     - |         - |
| FromCodeBeRef | 16.54 ns | 0.644 ns | 1.897 ns |     - |     - |     - |         - |
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
|                              Method |        Mean |     Error |     StdDev |      Median | Ratio | RatioSD | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------------ |------------:|----------:|-----------:|------------:|------:|--------:|------:|------:|------:|----------:|
|            ExplicitCurrencyAsString | 272.0372 ns | 3.8016 ns |  3.3700 ns | 271.9266 ns | 1.000 |    0.00 |     - |     - |     - |         - |
| ExplicitCurrencyAsStringAndRounding | 281.6815 ns | 5.6151 ns | 13.7739 ns | 280.5965 ns | 1.087 |    0.06 |     - |     - |     - |         - |
|            ExplicitCurrencyFromCode | 286.8482 ns | 5.6987 ns | 10.1293 ns | 286.2630 ns | 1.042 |    0.04 |     - |     - |     - |         - |
|                        HelperMethod | 305.7742 ns | 6.0623 ns | 12.3836 ns | 303.9115 ns | 1.110 |    0.05 |     - |     - |     - |         - |
|       ImplicitCurrencyByConstructor | 301.9183 ns | 6.0293 ns | 17.2992 ns | 298.9093 ns | 1.128 |    0.06 |     - |     - |     - |         - |
|           ImplicitCurrencyByCasting | 290.5713 ns | 5.5149 ns |  5.4163 ns | 289.4430 ns | 1.066 |    0.03 |     - |     - |     - |         - |
|                         Deconstruct |   0.0005 ns | 0.0019 ns |  0.0018 ns |   0.0000 ns | 0.000 |    0.00 |     - |     - |     - |         - |
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
|                   Method |      Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------- |----------:|---------:|---------:|------:|------:|------:|----------:|
|                 Addition | 313.06 ns | 4.307 ns | 4.029 ns |     - |     - |     - |         - |
|              Subtraction | 312.72 ns | 4.825 ns | 4.029 ns |     - |     - |     - |         - |
|      CompareSameCurrency |  54.52 ns | 0.762 ns | 0.675 ns |     - |     - |     - |         - |
| CompareDifferentCurrency |  58.88 ns | 1.091 ns | 1.021 ns |     - |     - |     - |         - |
|            CompareAmount |  57.70 ns | 1.177 ns | 1.571 ns |     - |     - |     - |         - |
|                Increment | 689.64 ns | 9.313 ns | 8.256 ns |     - |     - |     - |         - |
|                Decrement | 680.19 ns | 9.031 ns | 8.448 ns |     - |     - |     - |         - |
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
|             Method |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
|           Implicit | 187.0 ns | 3.61 ns | 4.02 ns | 0.0420 |     - |     - |     265 B |
| ImplicitWithFormat | 206.8 ns | 3.87 ns | 3.62 ns | 0.0420 |     - |     - |     265 B |
|           Explicit | 233.9 ns | 3.54 ns | 3.32 ns | 0.0763 |     - |     - |     481 B |
| ExplicitWithFormat | 245.8 ns | 4.82 ns | 4.73 ns | 0.0763 |     - |     - |     481 B |
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
|      Method |       Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------ |-----------:|---------:|---------:|-------:|------:|------:|----------:|
|    Implicit | 1,139.8 ns | 22.56 ns | 58.65 ns | 0.1354 |     - |     - |     859 B |
| ImplicitTry | 1,119.2 ns | 22.26 ns | 19.74 ns | 0.1354 |     - |     - |     859 B |
|    Explicit |   684.3 ns | 12.26 ns | 11.47 ns | 0.1030 |     - |     - |     650 B |
| ExplicitTry |   760.5 ns | 20.91 ns | 61.66 ns | 0.1030 |     - |     - |     650 B |
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
|        Method |     Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |---------:|---------:|---------:|-------:|------:|------:|----------:|
| CreateBuilder | 94.72 ns | 2.983 ns | 8.796 ns | 0.0139 |     - |     - |      88 B |
|         Build | 56.46 ns | 1.258 ns | 3.708 ns |      - |     - |     - |         - |
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
|                     Method |      Mean |    Error |   StdDev |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|--------------------------- |----------:|---------:|---------:|---------:|---------:|---------:|----------:|
| CreatingOneMillionCurrency |  46.95 ms | 0.995 ms | 2.918 ms | 307.6923 | 307.6923 | 307.6923 |  61.04 MB |
|    CreatingOneMillionMoney | 241.55 ms | 4.731 ms | 9.665 ms | 333.3333 | 333.3333 | 333.3333 |  76.29 MB |
### after as class
|                     Method |      Mean |    Error |   StdDev |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|--------------------------- |----------:|---------:|---------:|---------:|---------:|---------:|----------:|
| CreatingOneMillionCurrency |  26.03 ms | 0.517 ms | 1.278 ms | 343.7500 | 343.7500 | 343.7500 |   7.63 MB |
|    CreatingOneMillionMoney | 230.95 ms | 4.577 ms | 8.597 ms | 333.3333 | 333.3333 | 333.3333 |  22.89 MB |