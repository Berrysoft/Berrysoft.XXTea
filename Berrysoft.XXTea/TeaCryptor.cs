using System;

namespace Berrysoft.XXTea
{
    /// <summary>
    /// Represents a cryptor with TEA algorithm.
    /// </summary>
    public sealed class TeaCryptor : TeaCryptorBase
    {
        private static void EncryptInternal(ref uint v0, ref uint v1, ReadOnlySpan<uint> k, int round)
        {
            uint sum = 0;
            int n = round;
            unchecked
            {
                while (n-- > 0)
                {
                    sum += Delta;
                    v0 += ((v1 << 4) + k[0]) ^ (v1 + sum) ^ ((v1 >> 5) + k[1]);
                    v1 += ((v0 << 4) + k[2]) ^ (v0 + sum) ^ ((v0 >> 5) + k[3]);
                }
            }
        }

        private static void DecryptInternal(ref uint v0, ref uint v1, ReadOnlySpan<uint> k, int round)
        {
            uint sum = unchecked((uint)(round * Delta));
            int n = round;
            unchecked
            {
                while (n-- > 0)
                {
                    v1 -= ((v0 << 4) + k[2]) ^ (v0 + sum) ^ ((v0 >> 5) + k[3]);
                    v0 -= ((v1 << 4) + k[0]) ^ (v1 + sum) ^ ((v1 >> 5) + k[1]);
                    sum -= Delta;
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
