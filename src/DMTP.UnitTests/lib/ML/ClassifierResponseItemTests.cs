using System;

using DMTP.lib.ML;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DMTP.UnitTests.lib.ML
{
    [TestClass]
    public class ClassifierResponseItemTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest()
        {
            var response = new ClassifierResponseItem(null, null);
        }
    }
}