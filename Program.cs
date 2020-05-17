using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace SystemRandomBenchmark
{
    class Program
    {
        static void Main() => BenchmarkRunner.Run<RandomVsCsp>();
    }

    public class RandomVsCsp
    {
        private const Int32 BUFFER_LENGTH = 0x2000;
        private const Int32 SAMPLE_COUNT = 0x1000;

        private readonly Random _random = new Random(10);
        private readonly CspRandom _cspRandom = CspRandom.Create();
        private readonly Byte[] _buffer = new Byte[BUFFER_LENGTH];

        [Benchmark]
        public void RandomBytes() => _random.NextBytes(_buffer);

        [Benchmark]
        public void CspRandomBytes() => _cspRandom.NextBytes(_buffer);

        [Benchmark]
        public void RandomInt32()
        {
            for (Int32 i = 0; i < SAMPLE_COUNT; i++)
                _random.Next();
        }

        [Benchmark]
        public void CspRandomInt32()
        {
            for (Int32 i = 0; i < SAMPLE_COUNT; i++)
                _cspRandom.Next();
        }

        [Benchmark]
        public void RandomFullInt32()
        {
            for (Int32 i = 0; i < SAMPLE_COUNT; i++)
                _random.Next(Int32.MinValue, Int32.MaxValue);
        }

        [Benchmark]
        public void CspRandomFullInt32()
        {
            for (Int32 i = 0; i < SAMPLE_COUNT; i++)
                _cspRandom.Next(Int32.MinValue, Int32.MaxValue);
        }
    }
}
