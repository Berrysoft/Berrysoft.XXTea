# Berrysoft.XXTea
A .NET implementation of [TEA](https://en.wikipedia.org/wiki/Tiny_Encryption_Algorithm), [XTEA](https://en.wikipedia.org/wiki/XTEA) and [XXTEA](https://en.wikipedia.org/wiki/XXTEA) algorithm.

[![Azure DevOps builds](https://strawberry-vs.visualstudio.com/Berrysoft.XXTea/_apis/build/status/Berrysoft.Berrysoft.XXTea?branch=master)](https://strawberry-vs.visualstudio.com/Berrysoft.XXTea/_build?definitionId=8)

## Usage
``` csharp
var cryptor = new XXTeaCryptor("SuperStrongKey");
var encryptedData = cryptor.EncryptString("Hello world!"); // Encrypt
var decryptedData = cryptor.DecryptString(encryptedData); // Decrypt
```

## Benchmark
The other two XXTEA implementation:
* [Gibbed.XXTEA](https://github.com/gibbed/Gibbed.XXTEA)
* [xxtea-dotnet](https://github.com/xxtea/xxtea-dotnet)

```
BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18995
Intel Core i5-7200U CPU 2.50GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host] : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
  Clr    : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.8.3921.0
  Core   : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
  CoreRT : .NET CoreRT 1.0.28208.01 @BuiltBy: dlab14-DDVSOWINAGE101 @Branch: master @Commit: 9a09d481951a9bcf7b5a736e2de43a788c758904, 64bit AOT
  Mono   : Mono 6.4.0 (Visual Studio), 64bit
```

|           Method | Runtime |     N |       Mean |     Error |    StdDev | Ratio | RatioSD | Rank |   Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|----------------- |-------- |------ |-----------:|----------:|----------:|------:|--------:|-----:|--------:|-------:|-------:|----------:|
|    GibbedEncrypt |     Clr |  1000 |   7.091 us | 0.1078 us | 0.1008 us |  1.00 |    0.00 |    2 |  1.3351 |      - |      - |    2111 B |
|    GibbedEncrypt |    Core |  1000 |   7.085 us | 0.1611 us | 0.1654 us |  1.00 |    0.03 |    2 |  1.3351 |      - |      - |    2104 B |
|    GibbedEncrypt |  CoreRT |  1000 |   6.877 us | 0.0400 us | 0.0334 us |  0.97 |    0.01 |    1 |  1.3351 |      - |      - |    2104 B |
|    GibbedEncrypt |    Mono |  1000 |  10.274 us | 0.2047 us | 0.2662 us |  1.45 |    0.04 |    3 |  0.5188 |      - |      - |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
|     XxteaEncrypt |     Clr |  1000 |  10.633 us | 0.2098 us | 0.2155 us |  1.00 |    0.00 |    2 |  1.3275 |      - |      - |    2111 B |
|     XxteaEncrypt |    Core |  1000 |  10.766 us | 0.2093 us | 0.2326 us |  1.02 |    0.04 |    2 |  1.3275 |      - |      - |    2104 B |
|     XxteaEncrypt |  CoreRT |  1000 |  10.170 us | 0.2072 us | 0.2217 us |  0.96 |    0.03 |    1 |  1.3275 |      - |      - |    2104 B |
|     XxteaEncrypt |    Mono |  1000 |  14.506 us | 0.1391 us | 0.1233 us |  1.36 |    0.02 |    3 |  0.5188 |      - |      - |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
| BerrysoftEncrypt |     Clr |  1000 |   9.416 us | 0.0901 us | 0.0842 us |  1.00 |    0.00 |    2 |  0.6561 |      - |      - |    1036 B |
| BerrysoftEncrypt |    Core |  1000 |   7.835 us | 0.1616 us | 0.2467 us |  0.84 |    0.03 |    1 |  0.6561 |      - |      - |    1032 B |
| BerrysoftEncrypt |  CoreRT |  1000 |   7.688 us | 0.1063 us | 0.0994 us |  0.82 |    0.01 |    1 |  0.6561 |      - |      - |    1032 B |
| BerrysoftEncrypt |    Mono |  1000 |  17.876 us | 0.5064 us | 0.7580 us |  1.92 |    0.11 |    3 |  0.2441 |      - |      - |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
|    GibbedDecrypt |     Clr |  1000 |   7.026 us | 0.0110 us | 0.0103 us |  1.00 |    0.00 |    1 |  1.3351 |      - |      - |    2103 B |
|    GibbedDecrypt |    Core |  1000 |   6.833 us | 0.0163 us | 0.0136 us |  0.97 |    0.00 |    1 |  1.3351 |      - |      - |    2096 B |
|    GibbedDecrypt |  CoreRT |  1000 |   6.911 us | 0.1260 us | 0.1179 us |  0.98 |    0.02 |    1 |  1.3351 |      - |      - |    2096 B |
|    GibbedDecrypt |    Mono |  1000 |  10.104 us | 0.2012 us | 0.2066 us |  1.44 |    0.03 |    2 |  0.5188 |      - |      - |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
|     XxteaDecrypt |     Clr |  1000 |  10.583 us | 0.1117 us | 0.0990 us |  1.00 |    0.00 |    3 |  1.3275 |      - |      - |    2103 B |
|     XxteaDecrypt |    Core |  1000 |  10.231 us | 0.0252 us | 0.0236 us |  0.97 |    0.01 |    2 |  1.3275 |      - |      - |    2096 B |
|     XxteaDecrypt |  CoreRT |  1000 |   9.754 us | 0.0962 us | 0.0803 us |  0.92 |    0.01 |    1 |  1.3275 |      - |      - |    2096 B |
|     XxteaDecrypt |    Mono |  1000 |  14.664 us | 0.2065 us | 0.1830 us |  1.39 |    0.02 |    4 |  0.5188 |      - |      - |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
| BerrysoftDecrypt |     Clr |  1000 |   9.854 us | 0.2050 us | 0.2441 us |  1.00 |    0.00 |    2 |  1.2970 |      - |      - |    2063 B |
| BerrysoftDecrypt |    Core |  1000 |   8.968 us | 0.1770 us | 0.1656 us |  0.91 |    0.03 |    1 |  1.2970 |      - |      - |    2056 B |
| BerrysoftDecrypt |  CoreRT |  1000 |   9.032 us | 0.1742 us | 0.2006 us |  0.92 |    0.04 |    1 |  1.2970 |      - |      - |    2056 B |
| BerrysoftDecrypt |    Mono |  1000 |  18.794 us | 0.3603 us | 0.4810 us |  1.91 |    0.09 |    3 |  0.4883 |      - |      - |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
|    GibbedEncrypt |     Clr | 10000 |  72.052 us | 1.3780 us | 1.3533 us |  1.00 |    0.00 |    2 | 12.8174 |      - |      - |   20218 B |
|    GibbedEncrypt |    Core | 10000 |  70.655 us | 1.3650 us | 1.5171 us |  0.98 |    0.03 |    2 | 12.8174 |      - |      - |   20104 B |
|    GibbedEncrypt |  CoreRT | 10000 |  68.896 us | 1.3379 us | 1.3139 us |  0.96 |    0.02 |    1 | 12.8174 |      - |      - |   20104 B |
|    GibbedEncrypt |    Mono | 10000 | 109.307 us | 1.4426 us | 1.3494 us |  1.52 |    0.02 |    3 |  1.0986 | 1.0986 | 1.0986 |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
|     XxteaEncrypt |     Clr | 10000 | 107.858 us | 2.0997 us | 2.7302 us |  1.00 |    0.00 |    2 | 12.8174 |      - |      - |   20218 B |
|     XxteaEncrypt |    Core | 10000 | 107.290 us | 2.1247 us | 1.9875 us |  1.00 |    0.04 |    2 | 12.8174 |      - |      - |   20104 B |
|     XxteaEncrypt |  CoreRT | 10000 |  97.489 us | 1.7463 us | 1.6335 us |  0.90 |    0.03 |    1 | 12.8174 |      - |      - |   20104 B |
|     XxteaEncrypt |    Mono | 10000 | 155.301 us | 3.0212 us | 3.1025 us |  1.44 |    0.06 |    3 |  0.9766 | 0.9766 | 0.9766 |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
| BerrysoftEncrypt |     Clr | 10000 |  95.920 us | 1.8380 us | 1.9667 us |  1.00 |    0.00 |    3 |  6.3477 |      - |      - |   10056 B |
| BerrysoftEncrypt |    Core | 10000 |  75.388 us | 1.1743 us | 1.0985 us |  0.79 |    0.02 |    1 |  6.3477 |      - |      - |   10032 B |
| BerrysoftEncrypt |  CoreRT | 10000 |  77.916 us | 1.6598 us | 2.6326 us |  0.82 |    0.04 |    2 |  6.3477 |      - |      - |   10032 B |
| BerrysoftEncrypt |    Mono | 10000 | 177.485 us | 3.4563 us | 4.8452 us |  1.87 |    0.07 |    4 |  0.4883 | 0.4883 | 0.4883 |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
|    GibbedDecrypt |     Clr | 10000 |  71.082 us | 1.3853 us | 1.1568 us |  1.00 |    0.00 |    1 | 12.8174 |      - |      - |   20210 B |
|    GibbedDecrypt |    Core | 10000 |  69.287 us | 1.3272 us | 1.7718 us |  0.99 |    0.02 |    1 | 12.8174 |      - |      - |   20096 B |
|    GibbedDecrypt |  CoreRT | 10000 |  68.431 us | 1.3367 us | 1.6904 us |  0.96 |    0.03 |    1 | 12.8174 |      - |      - |   20096 B |
|    GibbedDecrypt |    Mono | 10000 | 113.643 us | 2.1979 us | 3.4218 us |  1.55 |    0.05 |    2 |  1.0986 | 1.0986 | 1.0986 |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
|     XxteaDecrypt |     Clr | 10000 | 109.598 us | 2.0845 us | 2.3169 us |  1.00 |    0.00 |    3 | 12.8174 |      - |      - |   20210 B |
|     XxteaDecrypt |    Core | 10000 | 102.704 us | 1.4955 us | 1.3258 us |  0.93 |    0.02 |    2 | 12.8174 |      - |      - |   20096 B |
|     XxteaDecrypt |  CoreRT | 10000 |  99.287 us | 1.9558 us | 2.0085 us |  0.90 |    0.03 |    1 | 12.8174 |      - |      - |   20096 B |
|     XxteaDecrypt |    Mono | 10000 | 155.179 us | 3.0663 us | 2.8682 us |  1.41 |    0.04 |    4 |  0.9766 | 0.9766 | 0.9766 |         - |
|                  |         |       |            |           |           |       |         |      |         |        |        |           |
| BerrysoftDecrypt |     Clr | 10000 |  97.092 us | 1.7340 us | 1.8554 us |  1.00 |    0.00 |    2 | 12.6953 |      - |      - |   20104 B |
| BerrysoftDecrypt |    Core | 10000 |  89.577 us | 0.6699 us | 0.6266 us |  0.92 |    0.02 |    1 | 12.6953 |      - |      - |   20056 B |
| BerrysoftDecrypt |  CoreRT | 10000 |  88.484 us | 1.4652 us | 1.3705 us |  0.91 |    0.02 |    1 | 12.6953 |      - |      - |   20056 B |
| BerrysoftDecrypt |    Mono | 10000 | 190.251 us | 3.6843 us | 5.2840 us |  1.97 |    0.07 |    3 |  0.9766 | 0.9766 | 0.9766 |         - |