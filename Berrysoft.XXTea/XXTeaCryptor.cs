using System;
using System.Runtime.CompilerServices;

namespace Berrysoft.XXTea
{
    /// <summary>
    /// The base class of XXTEA algorithm.
    /// </summary>
    public abstract class XXTeaCryptorBase : TeaCryptorBase
    {
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
    }

    /// <summary>
    /// Represents a cryptor with XXTEA algorithm.
    /// </summary>
    public sealed class XXTeaCryptor : XXTeaCryptorBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MX(uint sum, uint y, uint z, int p, uint e, ReadOnlySpan<uint> k)
        {
            return (((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4))) ^ ((sum ^ y) + (k[(p & 3) ^ (int)e] ^ z));
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
        protected override void Encrypt(Span<uint> data, ReadOnlySpan<uint> key, int _) => EncryptInternal(data, key);

        /// <inhertidoc/>
        protected override void Decrypt(Span<uint> data, ReadOnlySpan<uint> key, int _) => DecryptInternal(data, key);
    }

    /// <summary>
    /// Represents a cryptor with a slightly modified XXTEA algorithm.
    /// If you don't know what it is for, don't use it.
    /// </summary>
    public sealed class AuthTeaCryptor : XXTeaCryptorBase
    {
        // Not using inheritance for speed.

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
        protected override void Encrypt(Span<uint> data, ReadOnlySpan<uint> key, int _) => EncryptInternal(data, key);

        /// <inhertidoc/>
        protected override void Decrypt(Span<uint> data, ReadOnlySpan<uint> key, int _) => DecryptInternal(data, key);
    }
}
