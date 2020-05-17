using System;
using System.Security.Cryptography;

namespace SystemRandomBenchmark
{
    /// <summary>
    /// Wraps <see cref="RNGCryptoServiceProvider"/>.
    /// </summary>
    public sealed class CspRandom : Random
    {
        private const Int32 BUFFER_LENGTH = 128;

        private readonly RNGCryptoServiceProvider _rng;
        private readonly UInt32[] _buffer;
        private Int32 _index;

        public CspRandom(RNGCryptoServiceProvider rng)
        {
            _rng = rng;
            _buffer = new UInt32[BUFFER_LENGTH];
            _index = BUFFER_LENGTH;
        }

        public static CspRandom Create() => new CspRandom(new RNGCryptoServiceProvider());

        public UInt32 NextUInt32()
        {
            if (_index >= BUFFER_LENGTH)
                Regen();

            UInt32 value = _buffer[_index];
            _index += 1;
            return value;
        }

        public override Int32 Next() => Next(0, Int32.MaxValue);

        public override Int32 Next(Int32 maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"{nameof(maxValue)} must be positive.");

            return Next(0, maxValue);
        }

        public override Int32 Next(Int32 minValue, Int32 maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"{nameof(maxValue)} ({maxValue}) must be higher than or equal to {nameof(minValue)} ({minValue}).");

            return NextInclusive(minValue, maxValue - 1);
        }

        public override void NextBytes(Span<Byte> buffer) => _rng.GetBytes(buffer);

        public override void NextBytes(Byte[] buffer) => _rng.GetBytes(buffer);

        public void Dispose() => _rng.Dispose();

        /// <summary>
        /// Fills the buffer with new random bytes and sets <see cref="_index"/> to 0.
        /// </summary>
        private void Regen()
        {
            Span<Byte> buffer = System.Runtime.InteropServices.MemoryMarshal.AsBytes<UInt32>(_buffer);
            NextBytes(buffer);
            _index = 0;
        }

        private Int32 NextInclusive(Int32 minValue, Int32 maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, $"{nameof(maxValue)} ({maxValue}) must be higher than or equal to {nameof(minValue)} ({minValue}).");

            var range = unchecked((UInt32)(maxValue - minValue + 1));
            var intsToReject = range == 0 ? 0 : (UInt32.MaxValue - range + 1) % range;
            var zone = UInt32.MaxValue - intsToReject;

            while (true)
            {
                var unsigned = NextUInt32();
                if (unsigned > zone)
                    continue;

                return unchecked((Int32)(unsigned % range) + minValue);
            }
        }
    }
}
