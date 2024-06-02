``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.22000
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4515.0), X64 RyuJIT
  DefaultJob : .NET Framework 4.8 (4.8.4515.0), X64 RyuJIT


```
## InitializingCurrency
#### before
| Method   |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|----------|---------:|--------:|---------:|-------:|----------:|
| FromCode | 443.7 ns | 8.90 ns | 14.87 ns | 0.0753 |     632 B |
#### after
| Method        |     Mean |    Error |   StdDev | Allocated |
|---------------|---------:|---------:|---------:|----------:|
| FromCode      | 24.65 ns | 0.518 ns | 0.691 ns |         - |
| FromCodeBeRef | 25.99 ns | 0.739 ns | 2.178 ns |         - |
### after as class
| Method        |     Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------|---------:|---------:|---------:|------:|------:|------:|----------:|
| FromCode      | 20.32 ns | 0.666 ns | 1.952 ns |     - |     - |     - |         - |
| FromCodeBeRef | 21.68 ns | 0.661 ns | 1.949 ns |     - |     - |     - |         - |

## InitializingMoney
#### before
| Method                              |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Allocated |
|-------------------------------------|----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|
| ExplicitCurrencyAsString            | 483.87 ns |  6.370 ns |  5.319 ns | 483.75 ns |  1.00 |    0.00 | 0.0753 |     632 B |
| ExplicitCurrencyAsStringAndRounding | 496.87 ns |  9.720 ns | 12.976 ns | 493.09 ns |  1.02 |    0.03 | 0.0753 |     632 B |
| ExplicitCurrencyFromCode            | 521.25 ns | 16.493 ns | 48.630 ns | 494.71 ns |  1.16 |    0.03 | 0.0753 |     632 B |
| HelperMethod                        | 490.89 ns |  7.103 ns |  5.931 ns | 492.29 ns |  1.01 |    0.02 | 0.0753 |     632 B |
| ImplicitCurrencyByConstructor       | 114.69 ns |  2.276 ns |  2.017 ns | 115.09 ns |  0.24 |    0.00 | 0.0057 |      48 B |
| ImplicitCurrencyByCasting           | 113.58 ns |  1.579 ns |  1.477 ns | 113.67 ns |  0.23 |    0.00 | 0.0057 |      48 B |
| Deconstruct                         |  34.86 ns |  0.348 ns |  0.309 ns |  34.80 ns |  0.07 |    0.00 |      - |         - |
#### after
| Method                              |      Mean |    Error |   StdDev | Ratio | RatioSD | Allocated |
|-------------------------------------|----------:|---------:|---------:|------:|--------:|----------:|
| ExplicitCurrencyAsString            | 168.15 ns | 3.298 ns | 3.085 ns |  1.00 |    0.00 |         - |
| ExplicitCurrencyAsStringAndRounding | 171.95 ns | 1.516 ns | 1.344 ns |  1.02 |    0.02 |         - |
| ExplicitCurrencyFromCode            | 169.63 ns | 2.755 ns | 2.577 ns |  1.01 |    0.02 |         - |
| HelperMethod                        | 168.99 ns | 1.655 ns | 1.382 ns |  1.00 |    0.02 |         - |
| ImplicitCurrencyByConstructor       | 177.03 ns | 1.910 ns | 1.595 ns |  1.05 |    0.02 |         - |
| ImplicitCurrencyByCasting           | 183.81 ns | 1.642 ns | 1.456 ns |  1.09 |    0.02 |         - |
| Deconstruct                         |  19.46 ns | 0.390 ns | 0.383 ns |  0.12 |    0.00 |         - |
### after as class
| Method                              |        Mean |     Error |     StdDev |      Median | Ratio | RatioSD | Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------------------------|------------:|----------:|-----------:|------------:|------:|--------:|------:|------:|------:|----------:|
| ExplicitCurrencyAsString            | 291.2011 ns | 5.7582 ns | 10.6731 ns | 292.0155 ns | 1.000 |    0.00 |     - |     - |     - |         - |
| ExplicitCurrencyAsStringAndRounding | 291.1404 ns | 5.8337 ns | 12.3053 ns | 292.7434 ns | 0.997 |    0.05 |     - |     - |     - |         - |
| ExplicitCurrencyFromCode            | 293.2023 ns | 5.8591 ns | 11.8356 ns | 293.2638 ns | 1.008 |    0.05 |     - |     - |     - |         - |
| HelperMethod                        | 295.5661 ns | 5.9172 ns | 12.3514 ns | 293.7525 ns | 1.013 |    0.04 |     - |     - |     - |         - |
| ImplicitCurrencyByConstructor       | 304.9541 ns | 6.0387 ns | 11.3421 ns | 306.0941 ns | 1.049 |    0.04 |     - |     - |     - |         - |
| ImplicitCurrencyByCasting           | 304.1174 ns | 6.0599 ns | 12.2413 ns | 307.0236 ns | 1.044 |    0.05 |     - |     - |     - |         - |
| Deconstruct                         |   0.0451 ns | 0.0231 ns |  0.0486 ns |   0.0345 ns | 0.000 |    0.00 |     - |     - |     - |         - |

## MoneyOperations
#### before
| Method                   |      Mean |    Error |    StdDev | Allocated |
|--------------------------|----------:|---------:|----------:|----------:|
| Addition                 | 231.00 ns | 4.470 ns |  4.181 ns |         - |
| Subtraction              | 233.42 ns | 4.665 ns |  4.581 ns |         - |
| CompareSameCurrency      |  30.09 ns | 0.518 ns |  0.485 ns |         - |
| CompareDifferentCurrency |  68.83 ns | 0.679 ns |  0.530 ns |         - |
| CompareAmount            | 212.60 ns | 4.271 ns | 10.558 ns |         - |
| Increment                | 374.80 ns | 7.507 ns | 11.907 ns |         - |
| Decrement                | 369.70 ns | 7.295 ns | 14.229 ns |         - |
#### after
| Method                   |      Mean |    Error |   StdDev | Allocated |
|--------------------------|----------:|---------:|---------:|----------:|
| Addition                 | 170.47 ns | 0.933 ns | 0.827 ns |         - |
| Subtraction              | 168.34 ns | 2.306 ns | 2.157 ns |         - |
| CompareSameCurrency      |  25.89 ns | 0.181 ns | 0.160 ns |         - |
| CompareDifferentCurrency |  34.30 ns | 0.710 ns | 0.697 ns |         - |
| CompareAmount            |  19.88 ns | 0.307 ns | 0.287 ns |         - |
| Increment                | 386.02 ns | 3.906 ns | 3.654 ns |         - |
| Decrement                | 387.00 ns | 3.633 ns | 3.398 ns |         - |
### after as class
| Method                   |      Mean |     Error |    StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------------|----------:|----------:|----------:|------:|------:|------:|----------:|
| Addition                 | 301.01 ns |  5.962 ns | 11.198 ns |     - |     - |     - |         - |
| Subtraction              | 299.39 ns |  5.959 ns | 10.745 ns |     - |     - |     - |         - |
| CompareSameCurrency      |  28.17 ns |  0.586 ns |  1.196 ns |     - |     - |     - |         - |
| CompareDifferentCurrency |  38.29 ns |  0.792 ns |  2.235 ns |     - |     - |     - |         - |
| CompareAmount            |  37.34 ns |  0.768 ns |  2.115 ns |     - |     - |     - |         - |
| Increment                | 674.86 ns | 13.303 ns | 24.986 ns |     - |     - |     - |         - |
| Decrement                | 700.62 ns | 13.703 ns | 15.780 ns |     - |     - |     - |         - |

## MoneyFormatting
#### before
| Method             |     Mean |   Error |   StdDev |  Gen 0 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|----------:|
| Implicit           | 194.0 ns | 3.93 ns |  9.10 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 197.6 ns | 3.98 ns |  8.39 ns | 0.0286 |     240 B |
| Explicit           | 271.5 ns | 5.50 ns |  9.33 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 270.3 ns | 5.42 ns | 12.56 ns | 0.0525 |     440 B |
#### after
| Method             |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
|--------------------|---------:|--------:|--------:|-------:|----------:|
| Implicit           | 162.9 ns | 3.13 ns | 3.35 ns | 0.0286 |     240 B |
| ImplicitWithFormat | 152.2 ns | 2.36 ns | 2.09 ns | 0.0286 |     240 B |
| Explicit           | 202.8 ns | 3.22 ns | 3.01 ns | 0.0525 |     440 B |
| ExplicitWithFormat | 206.4 ns | 4.11 ns | 3.85 ns | 0.0525 |     440 B |
### after as class
| Method             |     Mean |   Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------|---------:|--------:|---------:|-------:|------:|------:|----------:|
| Implicit           | 255.7 ns | 5.10 ns | 12.33 ns | 0.0420 |     - |     - |     265 B |
| ImplicitWithFormat | 265.4 ns | 5.29 ns | 14.11 ns | 0.0420 |     - |     - |     265 B |
| Explicit           | 302.1 ns | 6.03 ns | 16.41 ns | 0.0763 |     - |     - |     481 B |
| ExplicitWithFormat | 310.4 ns | 8.04 ns | 23.72 ns | 0.0763 |     - |     - |     481 B |

## MoneyParsing
#### before
| Method      |        Mean |     Error |    StdDev |      Median |  Gen 0 |  Gen 1 | Allocated |
|-------------|------------:|----------:|----------:|------------:|-------:|-------:|----------:|
| Implicit    | 32,037.0 ns | 597.82 ns | 587.14 ns | 31,817.6 ns | 3.0823 | 0.2136 |     25 KB |
| ImplicitTry | 32,111.5 ns | 631.83 ns | 945.70 ns | 32,079.3 ns | 3.0823 | 0.2136 |     25 KB |
| Explicit    |    877.5 ns |  21.39 ns |  62.73 ns |    860.9 ns | 0.1469 |      - |      1 KB |
| ExplicitTry |    870.2 ns |  16.94 ns |  21.42 ns |    871.8 ns | 0.1469 |      - |      1 KB |
#### after
| Method      |       Mean |    Error |   StdDev |  Gen 0 | Allocated |
|-------------|-----------:|---------:|---------:|-------:|----------:|
| Implicit    | 2,640.7 ns | 40.75 ns | 36.12 ns | 0.1526 |   1,280 B |
| ImplicitTry | 2,685.3 ns | 22.01 ns | 18.38 ns | 0.1526 |   1,280 B |
| Explicit    |   448.8 ns |  5.39 ns |  4.78 ns | 0.0715 |     600 B |
| ExplicitTry |   461.9 ns |  4.32 ns |  4.04 ns | 0.0715 |     600 B |
### after as class
| Method      |       Mean |    Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------|-----------:|---------:|----------:|-------:|------:|------:|----------:|
| Implicit    | 1,345.5 ns | 34.66 ns | 102.19 ns | 0.1354 |     - |     - |     859 B |
| ImplicitTry | 1,395.7 ns | 43.70 ns | 128.86 ns | 0.1354 |     - |     - |     859 B |
| Explicit    |   794.9 ns | 16.03 ns |  47.28 ns | 0.1030 |     - |     - |     650 B |
| ExplicitTry |   783.0 ns | 17.38 ns |  51.23 ns | 0.1030 |     - |     - |     650 B |

## AddingCustomCurrency
#### before
| Method        |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|---------------|---------:|---------:|---------:|-------:|----------:|
| CreateBuilder | 25.65 ns | 0.546 ns | 0.560 ns | 0.0115 |      96 B |
| Build         | 32.48 ns | 0.487 ns | 0.380 ns |      - |         - |
#### after
| Method        |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|---------------|---------:|---------:|---------:|-------:|----------:|
| CreateBuilder | 35.99 ns | 0.543 ns | 0.454 ns | 0.0105 |      88 B |
| Build         | 31.09 ns | 0.428 ns | 0.379 ns |      - |         - |
### after as class
| Method        |     Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------|---------:|---------:|---------:|-------:|------:|------:|----------:|
| CreateBuilder | 97.77 ns | 2.649 ns | 7.810 ns | 0.0139 |     - |     - |      88 B |
| Build         | 34.83 ns | 1.019 ns | 2.972 ns | 0.0127 |     - |     - |      80 B |

## HighLoad
#### before
| Method                     |     Mean |    Error |   StdDev |      Gen 0 | Allocated |
|----------------------------|---------:|---------:|---------:|-----------:|----------:|
| CreatingOneMillionCurrency | 482.6 ms |  9.57 ms | 26.68 ms | 75000.0000 |    679 MB |
| CreatingOneMillionMoney    | 516.8 ms | 10.33 ms | 28.46 ms | 75000.0000 |    694 MB |
#### after
| Method                            |      Mean |    Error |   StdDev |      Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|-----------------------------------|----------:|---------:|---------:|-----------:|---------:|---------:|----------:|
| CreatingOneMillionCurrencyUnits   |  21.52 ms | 0.427 ms | 1.032 ms |  8093.7500 | 468.7500 | 468.7500 |     63 MB |
| CreatingOneMillionCurrency        |  50.61 ms | 1.009 ms | 1.767 ms |   272.7273 | 272.7273 | 272.7273 |     61 MB |
| CreatingOneMillionMoney           | 157.97 ms | 3.056 ms | 4.284 ms |          - |        - |        - |     76 MB |
| CreatingOneMillionMoneyUnit       | 219.60 ms | 3.809 ms | 4.386 ms | 24666.6667 |        - |        - |    221 MB |
| CreatingOneMillionMoneyUnit-Class | 208.17 ms | 2.949 ms | 2.302 ms | 24666.6667 |        - |        - |    221 MB |
### after Currency as class
| Method                          |      Mean |    Error |   StdDev |     Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|---------------------------------|----------:|---------:|---------:|----------:|---------:|---------:|----------:|
| CreatingOneMillionCurrencyUnits |  22.87 ms | 0.660 ms | 1.946 ms | 8093.7500 | 468.7500 | 468.7500 |     63 MB |
| CreatingOneMillionCurrency      |  23.19 ms | 0.460 ms | 1.000 ms |  312.5000 | 312.5000 | 312.5000 |      8 MB |
| CreatingOneMillionMoney         | 123.35 ms | 2.260 ms | 2.004 ms |         - |        - |        - |     23 MB |


## Create CurrencyUNit
#### before
| Method                             |     Mean |    Error |   StdDev | Ratio |  Gen 0 | Allocated |
|------------------------------------|---------:|---------:|---------:|------:|-------:|----------:|
| CreateCurrencyUnit                 | 78.61 ns | 1.586 ns | 3.239 ns |  1.00 | 0.0172 |     144 B |
| CreateCurrencyUnitNoLinq           | 17.47 ns | 0.329 ns | 0.503 ns |  0.22 | 0.0076 |      64 B |
| CreateCurrencyUnitNoLinqAndPattern | 18.35 ns | 0.396 ns | 0.683 ns |  0.23 | 0.0076 |      64 B |
