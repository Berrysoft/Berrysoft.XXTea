using System;
using System.Text;

namespace Berrysoft.XXTea
{
    /// <summary>
    /// Represents a cryptor with XTEA algorithm.
    /// </summary>
    public sealed class XTeaCryptor : TeaCryptorBase
    {
        private const int DefaultRound = 32;

        /// <inhertidoc/>
        public XTeaCryptor() : base() { }
        /// <inhertidoc/>
        public XTeaCryptor(byte[] key) : base(key) { }
        /// <inhertidoc/>
        public XTeaCryptor(string key) : base(key) { }
        /// <inhertidoc/>
        public XTeaCryptor(string key, Encoding encoding) : base(key, encoding) { }

        private static void EncryptInternal(ref uint v0, ref uint v1, ReadOnlySpan<uint> key, int n)
        {
            uint sum = 0;
            unchecked
            {
                while (n-- > 0)
                {
                    v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[(int)(sum & 3)]);
                    sum += Delta;
                    v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(int)((sum >> 11) & 3)]);
                }
            }
        }

        private static void DecryptInternal(ref uint v0, ref uint v1, ReadOnlySpan<uint> key, int n)
        {
            uint sum = unchecked((uint)n * Delta);
            unchecked
            {
                while (n-- > 0)
                {
                    v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(int)((sum >> 11) & 3)]);
                    sum -= Delta;
                    v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[(int)(sum & 3)]);
                }
            }
        }

        private void Encrypt(Span<uint> data, int round)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                EncryptInternal(ref data[i], ref data[i + 1], UInt32Key, round);
            }
        }

        /// <inhertidoc/>
        protected override void Encrypt(Span<uint> data) => Encrypt(data, DefaultRound);

        private void Decrypt(Span<uint> data, int round)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                DecryptInternal(ref data[i], ref data[i + 1], UInt32Key, round);
            }
        }

        /// <inhertidoc/>
        protected override void Decrypt(Span<uint> data) => Decrypt(data, DefaultRound);

        private unsafe void EncryptInternal(Span<byte> fixedData, int originalLength, int round)
        {
            fixed (byte* pfData = fixedData)
            {
                Span<uint> uintData = new Span<uint>(pfData, fixedData.Length / 4);
                AddLength(uintData, originalLength);
                Encrypt(uintData, round);
            }
        }

        /// <summary>
        /// Encrypts the data directly on the source.
        /// </summary>
        /// <param name="fixedData">The fixed data.</param>
        /// <param name="originalLength">The original data length.</param>
        /// <param name="round">The round of loop.</param>
        public void EncryptSpan(Span<byte> fixedData, int originalLength, int round)
        {
            if (fixedData.Length < GetFixedDataLength(originalLength))
            {
                throw new ArgumentOutOfRangeException(nameof(fixedData));
            }
            EncryptInternal(fixedData, originalLength, round);
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="data">The fixed data.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] Encrypt(ReadOnlySpan<byte> data, int round)
        {
            byte[] fixedData = FixData(data);
            EncryptInternal(fixedData, data.Length, round);
            return fixedData;
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="data">The UTF-8 data string.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] EncryptString(string data, int round) => EncryptString(data, Encoding.UTF8, round);

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="data">The data string.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] EncryptString(string data, Encoding encoding, int round)
        {
            byte[] fixedData = new byte[GetFixedDataLength(encoding.GetByteCount(data))];
            int originalLength = encoding.GetBytes(data, 0, data.Length, fixedData, 0);
            EncryptInternal(fixedData, originalLength, round);
            return fixedData;
        }

        private unsafe int DecryptInternal(Span<byte> fixedData, int round)
        {
            fixed (byte* pfData = fixedData)
            {
                Span<uint> uintData = new Span<uint>(pfData, fixedData.Length / 4);
                Decrypt(uintData, round);
                return GetOriginalDataLength(GetLength(uintData), fixedData.Length);
            }
        }

        /// <summary>
        /// Decrypts the data directly on the source.
        /// </summary>
        /// <param name="fixedData">The fixed data.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The original data length.</returns>
        public int DecryptSpan(Span<byte> fixedData, int round)
        {
            if (fixedData.Length % 4 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fixedData));
            }
            return DecryptInternal(fixedData, round);
        }

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The fixed data.</returns>
        public byte[] Decrypt(ReadOnlySpan<byte> data, int round)
        {
            if (data.Length % 4 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }
            byte[] fixedData = data.ToArray();
            int originalLength = DecryptInternal(fixedData, round);
            if (originalLength == 0)
            {
                return Array.Empty<byte>();
            }
            else
            {
                return fixedData.AsSpan().Slice(0, originalLength).ToArray();
            }
        }

        /// <summary>
        /// Decrypts the data to string.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The UTF-8 data string.</returns>
        public string DecryptString(ReadOnlySpan<byte> data, int round) => DecryptString(data, Encoding.UTF8, round);

        /// <summary>
        /// Decrypts the data to string.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The data string.</returns>
        public string DecryptString(ReadOnlySpan<byte> data, Encoding encoding, int round)
        {
            if (data.Length % 4 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }
            byte[] fixedData = data.ToArray();
            int originalLength = DecryptInternal(fixedData, round);
            if (originalLength == 0)
            {
                return string.Empty;
            }
            else
            {
                return encoding.GetString(fixedData, 0, originalLength);
            }
        }
    }
}
