using System;

namespace Berrysoft.XXTea
{
    /// <summary>
    /// Represents a cryptor with XTEA algorithm.
    /// </summary>
    public sealed class XTeaCryptor : TeaCryptorBase
    {
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

        /// <inhertidoc/>
        protected override void Encrypt(Span<uint> data, ReadOnlySpan<uint> key, int round)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                EncryptInternal(ref data[i], ref data[i + 1], key, round);
            }
        }

        /// <inhertidoc/>
        protected override void Decrypt(Span<uint> data, ReadOnlySpan<uint> key, int round)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                DecryptInternal(ref data[i], ref data[i + 1], key, round);
            }
        }
    }
}
