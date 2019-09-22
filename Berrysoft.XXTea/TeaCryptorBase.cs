using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Berrysoft.XXTea
{
    public abstract class TeaCryptorBase
    {
        public static readonly uint[] EmptyKey = new uint[4];

        protected const uint Delta = 0x9E3779B9;

        protected uint[] uintKey = EmptyKey;

        protected TeaCryptorBase() { }

        protected TeaCryptorBase(byte[] key)
        {
            if (key.Length != 0) ConsumeKey(key);
        }

        protected TeaCryptorBase(string key) : this(key, Encoding.UTF8) { }

        protected TeaCryptorBase(string key, Encoding encoding) : this(encoding.GetBytes(key)) { }

        public void ConsumeKey(byte[] key) => uintKey = ToUInt32Array(FixKey(key), false);

        protected virtual byte[] FixKey(byte[] key)
        {
            if (key.Length == 16)
            {
                return key;
            }
            else
            {
                byte[] fixedKey = new byte[16];
                Unsafe.CopyBlock(ref fixedKey[0], ref key[0], (uint)Math.Min(key.Length, 16));
                return fixedKey;
            }
        }

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
                Unsafe.CopyBlock(ref fixedData[0], ref data[0], (uint)length);
                return fixedData;
            }
        }

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

        protected abstract uint[] Encrypt(uint[] data);

        protected abstract uint[] Decrypt(uint[] data);

        public byte[] Encrypt(byte[] data) => ToByteArray(Encrypt(ToUInt32Array(FixData(data), true)), false);

        public byte[] EncryptString(string data) => EncryptString(data, Encoding.UTF8);

        public byte[] EncryptString(string data, Encoding encoding) => Encrypt(encoding.GetBytes(data));

        public byte[] Decrypt(byte[] data) => RestoreData(ToByteArray(Decrypt(ToUInt32Array(data, false)), true));

        public string DecryptString(byte[] data) => DecryptString(data, Encoding.UTF8);

        public string DecryptString(byte[] data, Encoding encoding) => encoding.GetString(Decrypt(data));

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
