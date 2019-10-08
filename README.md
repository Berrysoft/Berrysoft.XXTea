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

|           Method |    Job | Runtime | Toolchain |     N |       Mean |     Error |    StdDev | Ratio | RatioSD | Rank |   Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|----------------- |------- |-------- |---------- |------ |-----------:|----------:|----------:|------:|--------:|-----:|--------:|-------:|-------:|----------:|
|    GibbedEncrypt |    Clr |     Clr |   Default |  1000 |   6.995 us | 0.0241 us | 0.0201 us |  1.00 |    0.00 |    2 |  1.3351 |      - |      - |    2111 B |
|    GibbedEncrypt |   Core |    Core |   Default |  1000 |   6.778 us | 0.0390 us | 0.0346 us |  0.97 |    0.01 |    1 |  1.3351 |      - |      - |    2104 B |
|    GibbedEncrypt | CoreRT |  CoreRT |   Default |  1000 |   6.951 us | 0.1350 us | 0.1386 us |  1.00 |    0.02 |    2 |  1.3351 |      - |      - |    2104 B |
|    GibbedEncrypt |   Mono |    Mono |    net472 |  1000 |   9.734 us | 0.0195 us | 0.0182 us |  1.39 |    0.00 |    3 |  0.5188 |      - |      - |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
|     XxteaEncrypt |    Clr |     Clr |   Default |  1000 |  10.230 us | 0.0238 us | 0.0199 us |  1.00 |    0.00 |    2 |  1.3275 |      - |      - |    2111 B |
|     XxteaEncrypt |   Core |    Core |   Default |  1000 |  10.167 us | 0.0265 us | 0.0221 us |  0.99 |    0.00 |    2 |  1.3275 |      - |      - |    2104 B |
|     XxteaEncrypt | CoreRT |  CoreRT |   Default |  1000 |   9.899 us | 0.2329 us | 0.2682 us |  0.98 |    0.03 |    1 |  1.3275 |      - |      - |    2104 B |
|     XxteaEncrypt |   Mono |    Mono |    net472 |  1000 |  14.007 us | 0.0429 us | 0.0380 us |  1.37 |    0.00 |    3 |  0.5188 |      - |      - |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
| BerrysoftEncrypt |    Clr |     Clr |   Default |  1000 |   9.283 us | 0.0670 us | 0.0626 us |  1.00 |    0.00 |    3 |  0.6714 |      - |      - |    1076 B |
| BerrysoftEncrypt |   Core |    Core |   Default |  1000 |   7.683 us | 0.1532 us | 0.1764 us |  0.83 |    0.02 |    2 |  0.6790 |      - |      - |    1072 B |
| BerrysoftEncrypt | CoreRT |  CoreRT |   Default |  1000 |   7.455 us | 0.0133 us | 0.0118 us |  0.80 |    0.01 |    1 |  0.6790 |      - |      - |    1072 B |
| BerrysoftEncrypt |   Mono |    Mono |    net472 |  1000 |  17.069 us | 0.0530 us | 0.0496 us |  1.84 |    0.01 |    4 |  0.2441 |      - |      - |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
|    GibbedDecrypt |    Clr |     Clr |   Default |  1000 |   6.892 us | 0.0257 us | 0.0241 us |  1.00 |    0.00 |    2 |  1.3351 |      - |      - |    2103 B |
|    GibbedDecrypt |   Core |    Core |   Default |  1000 |   6.908 us | 0.1154 us | 0.1023 us |  1.00 |    0.02 |    2 |  1.3351 |      - |      - |    2096 B |
|    GibbedDecrypt | CoreRT |  CoreRT |   Default |  1000 |   6.649 us | 0.0122 us | 0.0095 us |  0.96 |    0.00 |    1 |  1.3351 |      - |      - |    2096 B |
|    GibbedDecrypt |   Mono |    Mono |    net472 |  1000 |   9.871 us | 0.0221 us | 0.0185 us |  1.43 |    0.00 |    3 |  0.5188 |      - |      - |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
|     XxteaDecrypt |    Clr |     Clr |   Default |  1000 |  10.619 us | 0.1673 us | 0.1565 us |  1.00 |    0.00 |    2 |  1.3275 |      - |      - |    2103 B |
|     XxteaDecrypt |   Core |    Core |   Default |  1000 |  10.518 us | 0.1785 us | 0.1394 us |  0.99 |    0.02 |    2 |  1.3275 |      - |      - |    2096 B |
|     XxteaDecrypt | CoreRT |  CoreRT |   Default |  1000 |   9.836 us | 0.1911 us | 0.2417 us |  0.93 |    0.03 |    1 |  1.3275 |      - |      - |    2096 B |
|     XxteaDecrypt |   Mono |    Mono |    net472 |  1000 |  14.078 us | 0.0577 us | 0.0482 us |  1.33 |    0.02 |    3 |  0.5188 |      - |      - |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
| BerrysoftDecrypt |    Clr |     Clr |   Default |  1000 |   9.625 us | 0.1639 us | 0.1533 us |  1.00 |    0.00 |    3 |  1.3275 |      - |      - |    2103 B |
| BerrysoftDecrypt |   Core |    Core |   Default |  1000 |   8.818 us | 0.0401 us | 0.0355 us |  0.92 |    0.01 |    2 |  1.3275 |      - |      - |    2096 B |
| BerrysoftDecrypt | CoreRT |  CoreRT |   Default |  1000 |   8.674 us | 0.0172 us | 0.0134 us |  0.90 |    0.02 |    1 |  1.3275 |      - |      - |    2096 B |
| BerrysoftDecrypt |   Mono |    Mono |    net472 |  1000 |  17.733 us | 0.0551 us | 0.0515 us |  1.84 |    0.03 |    4 |  0.5188 |      - |      - |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
|    GibbedEncrypt |    Clr |     Clr |   Default | 10000 |  68.390 us | 0.0967 us | 0.0905 us |  1.00 |    0.00 |    3 | 12.8174 |      - |      - |   20218 B |
|    GibbedEncrypt |   Core |    Core |   Default | 10000 |  66.538 us | 0.2048 us | 0.1915 us |  0.97 |    0.00 |    2 | 12.8174 |      - |      - |   20104 B |
|    GibbedEncrypt | CoreRT |  CoreRT |   Default | 10000 |  65.131 us | 0.1119 us | 0.0992 us |  0.95 |    0.00 |    1 | 12.8174 |      - |      - |   20104 B |
|    GibbedEncrypt |   Mono |    Mono |    net472 | 10000 | 109.043 us | 1.6829 us | 1.4918 us |  1.59 |    0.02 |    4 |  1.0986 | 1.0986 | 1.0986 |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
|     XxteaEncrypt |    Clr |     Clr |   Default | 10000 | 100.887 us | 0.1345 us | 0.1258 us |  1.00 |    0.00 |    2 | 12.8174 |      - |      - |   20218 B |
|     XxteaEncrypt |   Core |    Core |   Default | 10000 | 100.612 us | 0.2021 us | 0.1791 us |  1.00 |    0.00 |    2 | 12.8174 |      - |      - |   20104 B |
|     XxteaEncrypt | CoreRT |  CoreRT |   Default | 10000 |  94.945 us | 0.2189 us | 0.2047 us |  0.94 |    0.00 |    1 | 12.8174 |      - |      - |   20104 B |
|     XxteaEncrypt |   Mono |    Mono |    net472 | 10000 | 151.036 us | 1.7843 us | 1.6690 us |  1.50 |    0.02 |    3 |  0.9766 | 0.9766 | 0.9766 |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
| BerrysoftEncrypt |    Clr |     Clr |   Default | 10000 |  91.067 us | 0.4200 us | 0.3929 us |  1.00 |    0.00 |    2 |  6.3477 |      - |      - |   10109 B |
| BerrysoftEncrypt |   Core |    Core |   Default | 10000 |  74.246 us | 0.2258 us | 0.2112 us |  0.82 |    0.00 |    1 |  6.3477 |      - |      - |   10072 B |
| BerrysoftEncrypt | CoreRT |  CoreRT |   Default | 10000 |  73.458 us | 0.1589 us | 0.1409 us |  0.81 |    0.00 |    1 |  6.3477 |      - |      - |   10072 B |
| BerrysoftEncrypt |   Mono |    Mono |    net472 | 10000 | 173.560 us | 0.4953 us | 0.4136 us |  1.91 |    0.01 |    3 |  0.4883 | 0.4883 | 0.4883 |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
|    GibbedDecrypt |    Clr |     Clr |   Default | 10000 |  67.841 us | 0.9013 us | 0.7990 us |  1.00 |    0.00 |    3 | 12.8174 |      - |      - |   20210 B |
|    GibbedDecrypt |   Core |    Core |   Default | 10000 |  66.554 us | 0.1855 us | 0.1644 us |  0.98 |    0.01 |    2 | 12.8174 |      - |      - |   20096 B |
|    GibbedDecrypt | CoreRT |  CoreRT |   Default | 10000 |  65.176 us | 0.1278 us | 0.1196 us |  0.96 |    0.01 |    1 | 12.8174 |      - |      - |   20096 B |
|    GibbedDecrypt |   Mono |    Mono |    net472 | 10000 | 107.411 us | 0.4441 us | 0.4154 us |  1.58 |    0.02 |    4 |  1.0986 | 1.0986 | 1.0986 |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
|     XxteaDecrypt |    Clr |     Clr |   Default | 10000 | 102.839 us | 0.2675 us | 0.2371 us |  1.00 |    0.00 |    2 | 12.8174 |      - |      - |   20210 B |
|     XxteaDecrypt |   Core |    Core |   Default | 10000 | 103.779 us | 2.0129 us | 2.9505 us |  1.02 |    0.03 |    2 | 12.8174 |      - |      - |   20096 B |
|     XxteaDecrypt | CoreRT |  CoreRT |   Default | 10000 |  94.272 us | 0.1695 us | 0.1586 us |  0.92 |    0.00 |    1 | 12.8174 |      - |      - |   20096 B |
|     XxteaDecrypt |   Mono |    Mono |    net472 | 10000 | 149.103 us | 0.4284 us | 0.3797 us |  1.45 |    0.00 |    3 |  0.9766 | 0.9766 | 0.9766 |         - |
|                  |        |         |           |       |            |           |           |       |         |      |         |        |        |           |
| BerrysoftDecrypt |    Clr |     Clr |   Default | 10000 |  93.325 us | 0.2291 us | 0.2143 us |  1.00 |    0.00 |    3 | 12.8174 |      - |      - |   20210 B |
| BerrysoftDecrypt |   Core |    Core |   Default | 10000 |  86.865 us | 0.1386 us | 0.1157 us |  0.93 |    0.00 |    2 | 12.8174 |      - |      - |   20096 B |
| BerrysoftDecrypt | CoreRT |  CoreRT |   Default | 10000 |  85.499 us | 0.2259 us | 0.2113 us |  0.92 |    0.00 |    1 | 12.8174 |      - |      - |   20096 B |
| BerrysoftDecrypt |   Mono |    Mono |    net472 | 10000 | 183.836 us | 0.9328 us | 0.8269 us |  1.97 |    0.01 |    4 |  0.9766 | 0.9766 | 0.9766 |         - |
