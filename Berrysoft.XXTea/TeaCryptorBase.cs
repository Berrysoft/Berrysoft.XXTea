using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace Berrysoft.XXTea
{
    /// <summary>
    /// The base class of TEA cryptor.
    /// </summary>
    public abstract class TeaCryptorBase
    {
        private static readonly ImmutableArray<uint> EmptyKey = ImmutableArray.Create<uint>(0, 0, 0, 0);

        /// <summary>
        /// The magic number delta.
        /// </summary>
        protected const uint Delta = 0x9E3779B9;

        /// <summary>
        /// The 128-bit key.
        /// </summary>
        public ImmutableArray<uint> UInt32Key { get; private set; }

        /// <summary>
        /// Initializes a new instance of cryptor.
        /// </summary>
        protected TeaCryptorBase() : this(Array.Empty<byte>()) { }

        /// <summary>
        /// Initializes a new instance of cryptor with key.
        /// </summary>
        /// <param name="key">The key.</param>
        protected TeaCryptorBase(byte[] key) => ConsumeKey(key);

        /// <summary>
        /// Initializes a new instance of cryptor with key string.
        /// </summary>
        /// <param name="key">The UTF-8 key string.</param>
        protected TeaCryptorBase(string key) : this(key, Encoding.UTF8) { }

        /// <summary>
        /// Initializes a new instance of cryptor with key string and its encoding.
        /// </summary>
        /// <param name="key">The key string.</param>
        /// <param name="encoding">The specified encoding.</param>
        protected TeaCryptorBase(string key, Encoding encoding) : this(encoding.GetBytes(key)) { }

        /// <summary>
        /// Change the key to another one.
        /// </summary>
        /// <param name="key">The key.</param>
        public void ConsumeKey(byte[] key)
        {
            if (key.Length == 0)
            {
                UInt32Key = EmptyKey;
            }
            else
            {
                uint[] uintKey = new uint[4];
                Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref uintKey[0]), ref key[0], (uint)Math.Min(key.Length, 16));
                UInt32Key = ImmutableArray.Create(uintKey);
            }
        }

        /// <summary>
        /// Change the key to another one.
        /// </summary>
        /// <param name="key">The UTF-8 key string.</param>
        public void ConsumeKey(string key) => ConsumeKey(key, Encoding.UTF8);

        /// <summary>
        /// Change the key to another one.
        /// </summary>
        /// <param name="key">The key string.</param>
        /// <param name="encoding">The specified encoding.</param>
        public void ConsumeKey(string key, Encoding encoding) => ConsumeKey(encoding.GetBytes(key));

        /// <summary>
        /// Get the fixed data length.
        /// </summary>
        /// <param name="length">The original data length.</param>
        /// <returns>The fixed data length.</returns>
        protected virtual int GetFixedDataLength(int length)
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
        /// <returns>The encrypted data.</returns>
        protected abstract void Encrypt(Span<uint> data);

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <returns>The fixed data.</returns>
        protected abstract void Decrypt(Span<uint> data);

        private unsafe void EncryptInternal(byte[] fixedData, int originalLength)
        {
            fixed (byte* pfData = fixedData)
            {
                Span<uint> uintData = new Span<uint>(pfData, fixedData.Length / 4);
                AddLength(uintData, originalLength);
                Encrypt(uintData);
            }
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="data">The fixed data.</param>
        /// <returns>The encrypted data.</returns>
        public unsafe byte[] Encrypt(byte[] data)
        {
            byte[] fixedData = FixData(data);
            EncryptInternal(fixedData, data.Length);
            return fixedData;
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="data">The UTF-8 data string.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] EncryptString(string data) => EncryptString(data, Encoding.UTF8);

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="data">The data string.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] EncryptString(string data, Encoding encoding)
        {
            byte[] fixedData = new byte[GetFixedDataLength(encoding.GetByteCount(data))];
            int originalLength = encoding.GetBytes(data, 0, data.Length, fixedData, 0);
            EncryptInternal(fixedData, originalLength);
            return fixedData;
        }

        private unsafe int DecryptInternal(byte[] fixedData)
        {
            fixed (byte* pfData = fixedData)
            {
                Span<uint> uintData = new Span<uint>(pfData, fixedData.Length / 4);
                Decrypt(uintData);
                return GetLength(uintData);
            }
        }

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <returns>The fixed data.</returns>
        public byte[] Decrypt(byte[] data)
        {
            if (data.Length % 4 != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }
            byte[] fixedData = (byte[])data.Clone();
            int originalLength = DecryptInternal(fixedData);
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
        /// <returns>The UTF-8 data string.</returns>
        public string DecryptString(byte[] data) => DecryptString(data, Encoding.UTF8);

        /// <summary>
        /// Decrypts the data to string.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <returns>The data string.</returns>
        public string DecryptString(byte[] data, Encoding encoding) => encoding.GetString(Decrypt(data));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void AddLength(Span<uint> data, int originalLength)
        {
            data[data.Length - 1] = (uint)originalLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe int GetLength(Span<uint> data)
        {
            int n = data.Length * 4 - 4;
            int m = (int)data[data.Length - 1];
            if (m < n - 3 || m > n)
            {
                m = 0;
            }
            return m;
        }
    }
}
