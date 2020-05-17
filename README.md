# Summary

Compares the performance of .NET Core's two built-in random number generators, `System.Random` (Random) and `System.Security.Cryptography.RNGCryptoServiceProvider` (CSP).

When sampling sampling half the range of Int32, `Random` is 28% faster than `CSP`. However, when
sampling the full range of Int32, `Random` is 89% *slower* than `CSP`. Filling a buffer with `NextBytes`,
it appears `Random` is incredibly slow - **22** times slower than `CSP`.

So while the common wisdom is that `Random` should be used over `CSP` due to speed and convenience,
only the latter is a valid reason. `CSP` is much faster than `Random` whenever you're filling a buffer,
or simply want negative numbers.

Note: "half" range refers to [0, 2^31-1), while "full" range includes negative numbers; [-2^31, 2^31-1).
This distinction is made since `Random` takes one sample for half ranges, and two samples for full ranges.

## Data

|             Method |      Mean |     Error |    StdDev |
|------------------- |----------:|----------:|----------:|
|        RandomBytes | 47.752 us | 0.2442 us | 0.2164 us |
|     CspRandomBytes |  2.143 us | 0.0065 us | 0.0054 us |
|        RandomInt32 | 27.068 us | 0.0987 us | 0.0875 us |
|     CspRandomInt32 | 37.815 us | 0.1337 us | 0.1250 us |
|    RandomFullInt32 | 71.698 us | 0.2888 us | 0.2701 us |
| CspRandomFullInt32 | 37.839 us | 0.1744 us | 0.1632 us |

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18362.720 (1903/May2019Update/19H1)
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET Core SDK=3.1.300-preview-015135
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
