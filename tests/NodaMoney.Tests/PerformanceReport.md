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
| Method               |      Mean |     Error |    StdDev | Allocated |
|----------------------|----------:|----------:|----------:|----------:|
| CurrencyFromCode     | 13.950 ns | 0.0340 ns | 0.0284 ns |         - |
| CurrencyInfoFromCode |  7.906 ns | 0.0563 ns | 0.0499 ns |         - |

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
| Method                              |      Mean |    Error |   StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------------|----------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| ExplicitCurrencyAsString            | 142.29 ns | 1.862 ns | 1.742 ns |  1.00 |    0.00 |      - |         - |          NA |
| ExplicitCurrencyAsStringAndRounding | 158.65 ns | 3.182 ns | 5.047 ns |  1.14 |    0.03 |      - |         - |          NA |
| ExplicitCurrencyFromCode            | 145.45 ns | 1.175 ns | 1.042 ns |  1.02 |    0.01 |      - |         - |          NA |
| HelperMethod                        | 141.67 ns | 1.071 ns | 1.002 ns |  1.00 |    0.02 |      - |         - |          NA |
| ImplicitCurrencyByConstructor       | 202.08 ns | 2.831 ns | 2.648 ns |  1.42 |    0.02 | 0.0076 |      64 B |          NA |
| ImplicitCurrencyByCasting           | 198.08 ns | 3.056 ns | 2.858 ns |  1.39 |    0.03 | 0.0076 |      64 B |          NA |
| Deconstruct                         |  17.95 ns | 0.243 ns | 0.203 ns |  0.13 |    0.00 |      - |         - |          NA |

| Method                              |        Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------------------------------|------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| ExplicitCurrencyAsString            | 336.4189 ns | 1.4724 ns | 1.3053 ns | 1.000 |    0.00 | 0.0143 |     120 B |        1.00 |
| ExplicitCurrencyAsStringAndRounding | 342.7107 ns | 3.5120 ns | 2.9327 ns | 1.019 |    0.01 | 0.0143 |     120 B |        1.00 |
| ExplicitCurrencyFromCode            | 335.5134 ns | 2.1293 ns | 1.9918 ns | 0.998 |    0.01 | 0.0143 |     120 B |        1.00 |
| HelperMethod                        | 337.5227 ns | 2.0308 ns | 1.8002 ns | 1.003 |    0.01 | 0.0143 |     120 B |        1.00 |
| ImplicitCurrencyByConstructor       | 393.0368 ns | 2.3601 ns | 2.0922 ns | 1.168 |    0.01 | 0.0219 |     184 B |        1.53 |
| ImplicitCurrencyByCasting           | 423.9928 ns | 5.0165 ns | 4.6925 ns | 1.262 |    0.02 | 0.0219 |     184 B |        1.53 |
| Deconstruct                         |   0.2954 ns | 0.0106 ns | 0.0099 ns | 0.001 |    0.00 |      - |         - |        0.00 |

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

| Method                   | Mean       | Error     | StdDev    | Gen0   | Allocated |
|------------------------- |-----------:|----------:|----------:|-------:|----------:|
| Addition                 | 121.314 ns | 2.4425 ns | 3.2607 ns |      - |         - |
| Subtraction              | 118.499 ns | 1.4014 ns | 1.1702 ns |      - |         - |
| CompareSameCurrency      |   3.568 ns | 0.0171 ns | 0.0143 ns |      - |         - |
| CompareDifferentCurrency |   3.427 ns | 0.0157 ns | 0.0147 ns |      - |         - |
| CompareAmount            |   4.693 ns | 0.0264 ns | 0.0220 ns |      - |         - |
| Increment                | 526.314 ns | 6.8902 ns | 6.4451 ns | 0.0277 |     232 B |
| Decrement                | 517.873 ns | 3.1527 ns | 2.9491 ns | 0.0277 |     232 B |

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

| Method             | Mean      | Error    | StdDev   | Gen0   | Allocated |
|------------------- |----------:|---------:|---------:|-------:|----------:|
| Implicit           |  97.97 ns | 1.958 ns | 2.331 ns | 0.0421 |     352 B |
| ImplicitWithFormat | 102.68 ns | 1.406 ns | 1.315 ns | 0.0421 |     352 B |
| Explicit           | 142.74 ns | 2.549 ns | 2.259 ns | 0.0792 |     664 B |
| ExplicitWithFormat | 139.98 ns | 2.844 ns | 5.271 ns | 0.0792 |     664 B |

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

| Method      | Mean       | Error    | StdDev   | Gen0   | Allocated |
|------------ |-----------:|---------:|---------:|-------:|----------:|
| Implicit    | 1,063.0 ns | 15.63 ns | 14.62 ns | 0.1793 |   1.47 KB |
| ImplicitTry | 1,021.6 ns | 12.90 ns | 12.07 ns | 0.1793 |   1.47 KB |
| Explicit    |   595.7 ns |  4.58 ns |  4.29 ns | 0.1259 |   1.03 KB |
| ExplicitTry |   562.2 ns |  4.14 ns |  3.67 ns | 0.1259 |   1.03 KB |

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
#### before (v1.x)
| Method                     |     Mean |    Error |   StdDev |      Gen 0 | Allocated |
|----------------------------|---------:|---------:|---------:|-----------:|----------:|
| CreatingOneMillionCurrency | 482.6 ms |  9.57 ms | 26.68 ms | 75000.0000 |    679 MB |
| CreatingOneMillionMoney    | 516.8 ms | 10.33 ms | 28.46 ms | 75000.0000 |    694 MB |
#### after (.net8)
| Method                          | Mean       | Error     | StdDev    | Gen0       | Gen1     | Gen2     | Allocated |
|-------------------------------- |-----------:|----------:|----------:|-----------:|---------:|---------:|----------:|
| CreatingOneMillionCurrencyUnits |   2.352 ms | 0.0387 ms | 0.0380 ms |   996.0938 | 996.0938 | 996.0938 |   3.82 MB |
| CreatingOneMillionCurrency      |  23.872 ms | 0.4008 ms | 0.3749 ms |   968.7500 | 968.7500 | 968.7500 |  61.04 MB |
| CreatingOneMillionMoney         | 128.971 ms | 1.6607 ms | 1.4722 ms |   750.0000 | 750.0000 | 750.0000 |  76.29 MB |
| CreatingOneMillionMoneyUnit     | 165.396 ms | 3.2704 ms | 4.0164 ms | 13666.6667 | 333.3333 | 333.3333 | 122.07 MB |
#### after (.net8 + Currency/CurrencyInfo)
| Method                      | Mean      | Error    | StdDev   | Gen0       | Gen1      | Gen2      | Allocated |
|---------------------------- |----------:|---------:|---------:|-----------:|----------:|----------:|----------:|
| CreatingOneMillionCurrency  | 380.39 ms | 2.298 ms | 2.150 ms | 14000.0000 |         - |         - | 116.35 MB |
| CreatingOneMillionMoney     | 498.38 ms | 4.761 ms | 4.453 ms | 15000.0000 | 1000.0000 | 1000.0000 | 137.34 MB |
| CreatingOneMillionMoneyUnit |  52.37 ms | 1.037 ms | 1.153 ms |   500.0000 |  500.0000 |  500.0000 |  15.26 MB |

| Method                      | Mean      | Error    | StdDev   | Gen0     | Gen1     | Gen2     | Allocated |
|---------------------------- |----------:|---------:|---------:|---------:|---------:|---------:|----------:|
| CreatingOneMillionCurrency  |  15.17 ms | 0.099 ms | 0.092 ms | 484.3750 | 484.3750 | 484.3750 |   1.91 MB |
| CreatingOneMillionMoney     | 111.98 ms | 0.729 ms | 0.682 ms | 600.0000 | 600.0000 | 600.0000 |  22.89 MB |
| CreatingOneMillionMoneyUnit |  52.12 ms | 0.509 ms | 0.476 ms | 500.0000 | 500.0000 | 500.0000 |  15.26 MB |

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
#### after (.net8)
| Method                             | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| CreateCurrencyUnit                 | 41.62 ns | 0.840 ns | 0.785 ns |  1.00 |    0.00 | 0.0172 |     144 B |        1.00 |
| CreateCurrencyUnitNoLinq           | 10.38 ns | 0.235 ns | 0.521 ns |  0.26 |    0.01 | 0.0076 |      64 B |        0.44 |
| CreateCurrencyUnitNoLinqAndPattern | 10.72 ns | 0.270 ns | 0.782 ns |  0.28 |    0.02 | 0.0076 |      64 B |        0.44 |
