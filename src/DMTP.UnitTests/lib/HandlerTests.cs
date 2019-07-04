using System;
using System.Threading.Tasks;

using DMTP.lib.Handlers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DMTP.UnitTests.lib
{
    [TestClass]
    public class HandlerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HostsHandler_Null()
        {
            var hHandler = new HostsHandler(null);
        }
    }
}