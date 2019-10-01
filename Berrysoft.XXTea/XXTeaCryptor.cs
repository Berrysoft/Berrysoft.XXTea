using System;
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
        public override int GetFixedDataLength(int length) => ((length + 3) / 4 + 1) * 4;

        /// <inhertidoc/>
        protected override int GetOriginalDataLength(int m, int n)
        {
            n -= 4;
            if (m < n - 3 || m > n)
            {
                m = 0;
            }
            return m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MX(uint sum, uint y, uint z, int p, uint e, ReadOnlySpan<uint> k)
        {
            return ((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4) ^ (sum ^ y)) + (k[(p & 3) ^ (int)e] ^ z);
        }

        private static void EncryptInternal(Span<uint> v, ReadOnlySpan<uint> k)
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
        }

        private static void DecryptInternal(Span<uint> v, ReadOnlySpan<uint> k)
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
        }

        /// <inhertidoc/>
        protected override void Encrypt(Span<uint> data) => EncryptInternal(data, UInt32Key);

        /// <inhertidoc/>
        protected override void Decrypt(Span<uint> data) => DecryptInternal(data, UInt32Key);
    }
}
