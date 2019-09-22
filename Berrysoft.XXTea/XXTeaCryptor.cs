using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace Berrysoft.XXTea
{
    /// <summary>
    /// Represents a cryptor with XXTEA algorithm.
    /// </summary>
    public sealed class XXTeaCryptor : TeaCryptorBase
    {
        /// <inhertidoc/>
        public XXTeaCryptor() : base() { }
        /// <inhertidoc/>
        public XXTeaCryptor(byte[] key) : base(key) { }
        /// <inhertidoc/>
        public XXTeaCryptor(string key) : base(key) { }
        /// <inhertidoc/>
        public XXTeaCryptor(string key, Encoding encoding) : base(key, encoding) { }

        /// <inhertidoc/>
        protected override byte[] FixData(byte[] data) => data;

        /// <inhertidoc/>
        protected override byte[] RestoreData(byte[] data) => data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MX(uint sum, uint y, uint z, int p, uint e, ImmutableArray<uint> k)
        {
            return ((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4) ^ (sum ^ y)) + (k[(p & 3) ^ (int)e] ^ z);
        }

        private static uint[] EncryptInternal(uint[] v, ImmutableArray<uint> k)
        {
            int n = v.Length - 1;
            uint z = v[n];
            uint y;
            int q = 6 + 52 / (n + 1);
            uint sum = 0;
            unchecked
            {
                while (q-- > 0)
                {
                    sum += Delta;
                    uint e = (sum >> 2) & 3;
                    for (int p = 0; p <= n; p++)
                    {
                        y = v[(p + 1) % (n + 1)];
                        z = v[p] += MX(sum, y, z, p, e, k);
                    }
                }
            }
            return v;
        }

        private static uint[] DecryptInternal(uint[] v, ImmutableArray<uint> k)
        {
            int n = v.Length - 1;
            int q = 6 + 52 / (n + 1);
            uint sum = unchecked((uint)q * Delta);
            uint y = v[0];
            uint z;
            unchecked
            {
                do
                {
                    uint e = (sum >> 2) & 3;
                    for (int p = n; p >= 0; p--)
                    {
                        z = v[(p + n) % (n + 1)];
                        y = v[p] -= MX(sum, y, z, p, e, k);
                    }
                    sum -= Delta;
                } while (--q > 0);
            }
            return v;
        }

        /// <inhertidoc/>
        protected override uint[] Encrypt(uint[] data) => EncryptInternal(data, UintKey);

        /// <inhertidoc/>
        protected override uint[] Decrypt(uint[] data) => DecryptInternal(data, UintKey);
    }
}
