using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berrysoft.XXTea.Test
{
    [TestClass]
    public class TeaTest
    {
        private const string Data = "Hello world";
        private const string Key = "abcdefghijklmn";

        [TestMethod]
        public void CryptTest()
        {
            TeaCryptor cryptor = new TeaCryptor(Key);
            Assert.AreEqual(Data, cryptor.DecryptString(cryptor.EncryptString(Data)));
        }

        [TestMethod]
        public void XCryptTest()
        {
            XTeaCryptor cryptor = new XTeaCryptor(Key);
            Assert.AreEqual(Data, cryptor.DecryptString(cryptor.EncryptString(Data)));
        }

        [TestMethod]
        public void XRoundCryptTest()
        {
            XTeaCryptor cryptor = new XTeaCryptor(Key);
            Assert.AreEqual(Data, cryptor.DecryptString(cryptor.EncryptString(Data, 1024), 1024));
        }

        [TestMethod]
        public void XXCryptTest()
        {
            XXTeaCryptor cryptor = new XXTeaCryptor(Key);
            Assert.AreEqual(Data, cryptor.DecryptString(cryptor.EncryptString(Data)));
        }
    }
}
