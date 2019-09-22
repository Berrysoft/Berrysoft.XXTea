using System.Collections.Immutable;
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

        private static void EncryptInternal(ref uint v0, ref uint v1, ImmutableArray<uint> key, int n)
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

        private static void DecryptInternal(ref uint v0, ref uint v1, ImmutableArray<uint> key, int n)
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

        private uint[] Encrypt(uint[] data, int round)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                EncryptInternal(ref data[i], ref data[i + 1], UintKey, round);
            }
            return data;
        }

        /// <inhertidoc/>
        protected override uint[] Encrypt(uint[] data) => Encrypt(data, DefaultRound);

        private uint[] Decrypt(uint[] data, int round)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                DecryptInternal(ref data[i], ref data[i + 1], UintKey, round);
            }
            return data;
        }

        /// <inhertidoc/>
        protected override uint[] Decrypt(uint[] data) => Decrypt(data, DefaultRound);

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="data">The fixed data.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] Encrypt(byte[] data, int round) => ToByteArray(Encrypt(ToUInt32Array(FixData(data), true), round), false);

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
        public byte[] EncryptString(string data, Encoding encoding, int round) => Encrypt(encoding.GetBytes(data), round);

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The fixed data.</returns>
        public byte[] Decrypt(byte[] data, int round) => RestoreData(ToByteArray(Decrypt(ToUInt32Array(data, false), round), true));

        /// <summary>
        /// Decrypts the data to string.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The UTF-8 data string.</returns>
        public string DecryptString(byte[] data, int round) => DecryptString(data, Encoding.UTF8, round);

        /// <summary>
        /// Decrypts the data to string.
        /// </summary>
        /// <param name="data">The encrypted data.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <param name="round">The round of loop.</param>
        /// <returns>The data string.</returns>
        public string DecryptString(byte[] data, Encoding encoding, int round) => encoding.GetString(Decrypt(data, round));
    }
}
