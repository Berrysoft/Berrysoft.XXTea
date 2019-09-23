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
        public ImmutableArray<uint> UintKey { get; private set; } = EmptyKey;

        /// <summary>
        /// Initializes a new instance of cryptor.
        /// </summary>
        protected TeaCryptorBase() { }

        /// <summary>
        /// Initializes a new instance of cryptor with key.
        /// </summary>
        /// <param name="key">The key.</param>
        protected TeaCryptorBase(byte[] key)
        {
            if (key.Length != 0) ConsumeKey(key);
        }

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
        public void ConsumeKey(byte[] key) => UintKey = ImmutableArray.Create(ToUInt32Array(FixKey(key), false));

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
        /// Fixes the key to at least 16B.
        /// </summary>
        /// <param name="key">The original key.</param>
        /// <returns>The fixed key.</returns>
        protected virtual byte[] FixKey(byte[] key)
        {
            if (key.Length == 16)
            {
                return key;
            }
            else
            {
                byte[] fixedKey = new byte[16];
                if (key.Length > 0)
                {
                    Unsafe.CopyBlock(ref fixedKey[0], ref key[0], (uint)Math.Min(key.Length, 16));
                }
                return fixedKey;
            }
        }

        /// <summary>
        /// Fixes the data to odd times of 4.
        /// </summary>
        /// <param name="data">The original data.</param>
        /// <returns>The fixed data.</returns>
        protected virtual byte[] FixData(byte[] data)
        {
            if ((data.Length + 4) % 8 == 0)
            {
                return data;
            }
            else
            {
                int length = ((data.Length + 4) / 8 + 1) * 8 - 4;
                byte[] fixedData = new byte[length];
                if (data.Length > 0)
                {
                    Unsafe.CopyBlock(ref fixedData[0], ref data[0], (uint)data.Length);
                }
                return fixedData;
            }
        }

        /// <summary>
        /// Removes the padding zero at the end of the fixed data.
        /// </summary>
        /// <param name="data">The fixed data.</param>
        /// <returns>The original data.</returns>
        protected virtual byte[] RestoreData(byte[] data)
        {
            int i = data.Length - 1;
            for (; i >= 0; i--)
            {
                if (data[i] != 0)
                {
                    break;
                }
            }
            if (i == -1)
            {
                return Array.Empty<byte>();
            }
            else
            {
                Array.Resize(ref data, i + 1);
                return data;
            }
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="data">The fixed data.</param>
        /// <returns>The encrypted data.</returns>
        protected abstract uint[] Encrypt(uint[] data);

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <returns>The fixed data.</returns>
        protected abstract uint[] Decrypt(uint[] data);

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="data">The fixed data.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] Encrypt(byte[] data) => ToByteArray(Encrypt(ToUInt32Array(FixData(data), true)), false);

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
        public byte[] EncryptString(string data, Encoding encoding) => Encrypt(encoding.GetBytes(data));

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <returns>The fixed data.</returns>
        public byte[] Decrypt(byte[] data) => RestoreData(ToByteArray(Decrypt(ToUInt32Array(data, false)), true));

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

        /// <summary>
        /// Converts data to <see cref="uint"/> array.
        /// </summary>
        /// <param name="data">The original data.</param>
        /// <param name="includeLength">Whether the end of the array contains the length of the data.</param>
        /// <returns>The converted data.</returns>
        protected static uint[] ToUInt32Array(byte[] data, bool includeLength)
        {
            int length = data.Length;
            int n = (length + 3) / 4;
            uint[] result;
            if (includeLength)
            {
                result = new uint[n + 1];
                result[n] = (uint)length;
            }
            else
            {
                result = new uint[n];
            }
            Unsafe.CopyBlock(ref Unsafe.As<uint, byte>(ref result[0]), ref data[0], (uint)length);
            return result;
        }

        /// <summary>
        /// Converts <see cref="uint"/> array to original data.
        /// </summary>
        /// <param name="data">The array.</param>
        /// <param name="includeLength">Whether the array is created with length.</param>
        /// <returns>The original data.</returns>
        protected static byte[] ToByteArray(uint[] data, bool includeLength)
        {
            int d = data.Length;
            uint n = (uint)(d << 2);
            if (includeLength)
            {
                uint m = data[d - 1];
                n -= 4;
                if (m < n - 3 || m > n)
                {
                    return Array.Empty<byte>();
                }
                n = m;
            }
            byte[] result = new byte[n];
            Unsafe.CopyBlock(ref result[0], ref Unsafe.As<uint, byte>(ref data[0]), n);
            return result;
        }
    }
}
