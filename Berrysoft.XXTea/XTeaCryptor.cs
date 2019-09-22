using System.Text;

namespace Berrysoft.XXTea
{
    public sealed class XTeaCryptor : TeaCryptorBase
    {
        private const int DefaultRound = 32;

        public XTeaCryptor() : base() { }
        public XTeaCryptor(byte[] key) : base(key) { }
        public XTeaCryptor(string key) : base(key) { }
        public XTeaCryptor(string key, Encoding encoding) : base(key, encoding) { }

        private static void EncryptInternal(ref uint v0, ref uint v1, uint[] key, int n)
        {
            uint sum = 0;
            unchecked
            {
                while (n-- > 0)
                {
                    v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
                    sum += Delta;
                    v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
                }
            }
        }

        private static void DecryptInternal(ref uint v0, ref uint v1, uint[] key, int n)
        {
            uint sum = unchecked((uint)n * Delta);
            unchecked
            {
                while (n-- > 0)
                {
                    v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
                    sum -= Delta;
                    v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
                }
            }
        }

        private uint[] Encrypt(uint[] data, int round)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                EncryptInternal(ref data[i], ref data[i + 1], uintKey, round);
            }
            return data;
        }

        protected override uint[] Encrypt(uint[] data) => Encrypt(data, DefaultRound);

        private uint[] Decrypt(uint[] data, int round)
        {
            for (int i = 0; i < data.Length; i += 2)
            {
                DecryptInternal(ref data[i], ref data[i + 1], uintKey, round);
            }
            return data;
        }

        protected override uint[] Decrypt(uint[] data) => Decrypt(data, DefaultRound);

        public byte[] Encrypt(byte[] data, int round) => ToByteArray(Encrypt(ToUInt32Array(FixData(data), true), round), false);

        public byte[] EncryptString(string data, int round) => EncryptString(data, Encoding.UTF8, round);

        public byte[] EncryptString(string data, Encoding encoding, int round) => Encrypt(encoding.GetBytes(data), round);

        public byte[] Decrypt(byte[] data, int round) => RestoreData(ToByteArray(Decrypt(ToUInt32Array(data, false), round), true));

        public string DecryptString(byte[] data, int round) => DecryptString(data, Encoding.UTF8, round);

        public string DecryptString(byte[] data, Encoding encoding, int round) => encoding.GetString(Decrypt(data, round));
    }
}
