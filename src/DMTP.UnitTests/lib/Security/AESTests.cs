using DMTP.lib.Security;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DMTP.UnitTests.lib.Security
{
    [TestClass]
    public class AESTests
    {
        private string keyPassword = "4236C8DF278CC4431069B522E695D4F2";

        [TestMethod]
        public void EncryptString()
        {
            var result = AES.EncryptString("test", keyPassword);

            Assert.IsNotNull(result);
            Assert.AreEqual("i6UCWV6XLuK0125qFJ0qIw==", result);
        }

        [TestMethod]
        public void DecryptString()
        {
            var originalText = "test";

            var result = AES.EncryptString(originalText, keyPassword);

            Assert.IsNotNull(result);
            Assert.AreEqual("i6UCWV6XLuK0125qFJ0qIw==", result);

            var decryptResult = AES.DecryptString(result, keyPassword);

            Assert.IsNotNull(decryptResult);
            Assert.AreEqual(originalText, decryptResult);
        }
    }
}