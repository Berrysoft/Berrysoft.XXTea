using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berrysoft.XXTea.Test
{
    [TestClass]
    public class TeaTest
    {
        private const string Data0 = "Hello world!";
        private const string Key0 = "abcdefghijklmnop";
        private const string Data1 = "Hello world";
        private const string Key1 = "abcdefghijklmn";
        private const string Data2 = "Hello world!!";
        private const string Key2 = "abcdefghijklmnopqr";
        private const string Data3 = "";
        private const string Key3 = "";

        [DataTestMethod]
        [DataRow(Data0, Key0)]
        [DataRow(Data1, Key1)]
        [DataRow(Data2, Key2)]
        [DataRow(Data3, Key3)]
        public void CryptTest(string data, string key)
        {
            TeaCryptor cryptor = new TeaCryptor();
            Assert.AreEqual(data, cryptor.DecryptString(cryptor.EncryptString(data, key), key));
        }

        [DataTestMethod]
        [DataRow(Data0, Key0)]
        [DataRow(Data1, Key1)]
        [DataRow(Data2, Key2)]
        [DataRow(Data3, Key3)]
        public void XCryptTest(string data, string key)
        {
            XTeaCryptor cryptor = new XTeaCryptor();
            Assert.AreEqual(data, cryptor.DecryptString(cryptor.EncryptString(data, key), key));
        }

        [DataTestMethod]
        [DataRow(Data0, Key0, 1024)]
        [DataRow(Data1, Key1, 1024)]
        [DataRow(Data2, Key2, 1024)]
        [DataRow(Data3, Key3, 1024)]
        [DataRow(Data0, Key0, 0)]
        [DataRow(Data1, Key1, 0)]
        [DataRow(Data2, Key2, 0)]
        [DataRow(Data3, Key3, 0)]
        public void XRoundCryptTest(string data, string key, int round)
        {
            XTeaCryptor cryptor = new XTeaCryptor();
            Assert.AreEqual(data, cryptor.DecryptString(cryptor.EncryptString(data, key, round), key, round));
        }

        [DataTestMethod]
        [DataRow(Data0, Key0)]
        [DataRow(Data1, Key1)]
        [DataRow(Data2, Key2)]
        [DataRow(Data3, Key3)]
        public void XXCryptTest(string data, string key)
        {
            XXTeaCryptor cryptor = new XXTeaCryptor();
            Assert.AreEqual(data, cryptor.DecryptString(cryptor.EncryptString(data, key), key));
        }
    }
}
