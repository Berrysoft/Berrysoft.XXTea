using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Berrysoft.XXTea
{
    /// <summary>
    /// The base class of TEA cryptor.
    /// </summary>
    public abstract class TeaCryptorBase
    {
        private static readonly uint[] EmptyKey = new uint[4];

        private const int DefaultRound = 32;

        /// <summary>
        /// The magic number delta.
        /// </summary>
        protected const uint Delta = 0x9E3779B9;

        private uint[] ConsumeKeyInternal(ReadOnlySpan<byte> key)
        {
            if (key.Length == 0)
            {
                return EmptyKey;
            }
            else
            {
                uint[] uintKey = new uint[4];
                Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref uintKey[0]), ref Unsafe.AsRef(in key[0]), (uint)Math.Min(key.Length, 16));
                return uintKey;
            }
        }

        /// <summary>
        /// Get the fixed data length.
        /// </summary>
        /// <param name="length">The original data length.</param>
        /// <returns>The fixed data length.</returns>
        public virtual int GetFixedDataLength(int length)
        {
            if (length % 8 == 4)
            {
                return length + 4;
            }
            else
            {
                return ((length + 4) / 8 + 1) * 8;
            }
        }

        /// <summary>
        /// Get the real original data length.
        /// </summary>
        /// <param name="originalLength">The original data length.</param>
        /// <param name="fixedLength">The fixed data length.</param>
        /// <returns>The real original data length.</returns>
        protected virtual int GetOriginalDataLength(int originalLength, int fixedLength) => originalLength;

        /// <summary>
        /// Fixes the data to odd times of 4.
        /// </summary>
        /// <param name="data">The original data.</param>
        /// <returns>The fixed data.</returns>
        protected byte[] FixData(ReadOnlySpan<byte> data)
        {
            int length = GetFixedDataLength(data.Length);
            byte[] fixedData = new byte[length];
            if (data.Length > 0)
            {
                Unsafe.CopyBlock(ref fixedData[0], ref Unsafe.AsRef(in data[0]), (uint)Math.Min(length, data.Length));
            }
            return fixedData;
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="data">The fixed data.</param>
        /// <param name="key">The key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        /// <returns>The encrypted data.</returns>
        protected abstract void Encrypt(Span<uint> data, ReadOnlySpan<uint> key, int round);

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="key">The key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        /// <returns>The fixed data.</returns>
        protected abstract void Decrypt(Span<uint> data, ReadOnlySpan<uint> key, int round);

        private unsafe void EncryptInternal(Span<byte> fixedData, int originalLength, ReadOnlySpan<byte> key, int round)
        {
            fixed (byte* pfData = fixedData)
            {
                Span<uint> uintData = new Span<uint>(pfData, fixedData.Length / 4);
                AddLength(uintData, originalLength);
                if (key.Length >= 16)
                {
                    fixed (byte* pkey = key)
                    {
                        Span<uint> uintKey = new Span<uint>(pkey, 4);
                        Encrypt(uintData, uintKey, round);
                    }
                }
                else
                {
                    Encrypt(uintData, ConsumeKeyInternal(key), round);
                }
            }
        }

        /// <summary>
        /// Encrypts the data directly on the source.
        /// </summary>
        /// <param name="fixedData">The fixed data.</param>
        /// <param name="originalLength">The original data length.</param>
        /// <param name="key">The key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        public void EncryptSpan(Span<byte> fixedData, int originalLength, ReadOnlySpan<byte> key, int round = DefaultRound)
        {
            if (fixedData.Length < GetFixedDataLength(originalLength))
            {
                throw new ArgumentOutOfRangeException(nameof(fixedData));
            }
            EncryptInternal(fixedData, originalLength, key, round);
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="data">The fixed data.</param>
        /// <param name="key">The key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key, int round = DefaultRound)
        {
            byte[] fixedData = FixData(data);
            EncryptInternal(fixedData, data.Length, key, round);
            return fixedData;
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="data">The UTF-8 data string.</param>
        /// <param name="key">The key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] EncryptString(string data, string key, int round = DefaultRound) => EncryptString(data, Encoding.UTF8, key, Encoding.UTF8, round);

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="data">The data string.</param>
        /// <param name="dataEncoding">The specified encoding.</param>
        /// <param name="key">The key.</param>
        /// <param name="keyEncoding">The encoding of the key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] EncryptString(string data, Encoding dataEncoding, string key, Encoding keyEncoding, int round = DefaultRound)
        {
            byte[] fixedData = new byte[GetFixedDataLength(dataEncoding.GetByteCount(data))];
            int originalLength = dataEncoding.GetBytes(data, 0, data.Length, fixedData, 0);
            byte[] fixedKey;
            if (key.Length > 0)
            {
                fixedKey = new byte[16];
                keyEncoding.GetBytes(key, 0, Math.Min(16, key.Length), fixedKey, 0);
            }
            else
            {
                fixedKey = Array.Empty<byte>();
            }
            EncryptInternal(fixedData, originalLength, fixedKey, round);
            return fixedData;
        }

        private unsafe int DecryptInternal(Span<byte> fixedData, ReadOnlySpan<byte> key, int round)
        {
            fixed (byte* pfData = fixedData)
            {
                Span<uint> uintData = new Span<uint>(pfData, fixedData.Length / 4);
                if (key.Length >= 16)
                {
                    fixed (byte* pkey = key)
                    {
                        Span<uint> uintKey = new Span<uint>(pkey, 4);
                        Decrypt(uintData, uintKey, round);
                    }
                }
                else
                {
                    Decrypt(uintData, ConsumeKeyInternal(key), round);
                }
                return GetOriginalDataLength(GetLength(uintData), fixedData.Length);
            }
        }

        /// <summary>
        /// Decrypts the data directly on the source.
        /// </summary>
        /// <param name="fixedData">The fixed data.</param>
        /// <param name="key">The key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        /// <returns>The original data length.</returns>
        public int DecryptSpan(Span<byte> fixedData, ReadOnlySpan<byte> key, int round = DefaultRound)
        {
            if (fixedData.Length % 4 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fixedData));
            }
            return DecryptInternal(fixedData, key, round);
        }

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="key">The key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        /// <returns>The fixed data.</returns>
        public byte[] Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key, int round = DefaultRound)
        {
            if (data.Length % 4 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }
            byte[] fixedData = data.ToArray();
            int originalLength = DecryptInternal(fixedData, key, round);
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
        /// <param name="key">The key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        /// <returns>The UTF-8 data string.</returns>
        public string DecryptString(ReadOnlySpan<byte> data, string key, int round = DefaultRound) => DecryptString(data, Encoding.UTF8, key, Encoding.UTF8, round);

        /// <summary>
        /// Decrypts the data to string.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <param name="key">The key.</param>
        /// <param name="keyEncoding">The encoding of the key.</param>
        /// <param name="round">The round of crypting. No effects on XXTEA.</param>
        /// <returns>The data string.</returns>
        public string DecryptString(ReadOnlySpan<byte> data, Encoding encoding, string key, Encoding keyEncoding, int round = DefaultRound)
        {
            if (data.Length % 4 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }
            byte[] fixedData = data.ToArray();
            byte[] fixedKey;
            if (key.Length > 0)
            {
                fixedKey = new byte[16];
                keyEncoding.GetBytes(key, 0, Math.Min(16, key.Length), fixedKey, 0);
            }
            else
            {
                fixedKey = Array.Empty<byte>();
            }
            int originalLength = DecryptInternal(fixedData, fixedKey, round);
            if (originalLength == 0)
            {
                return string.Empty;
            }
            else
            {
                return encoding.GetString(fixedData, 0, originalLength);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void AddLength(Span<uint> data, int originalLength) => data[data.Length - 1] = (uint)originalLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int GetLength(Span<uint> data) => (int)data[data.Length - 1];
    }
}
