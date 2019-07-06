using System;
using System.Threading.Tasks;

using DMTP.lib.Handlers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DMTP.UnitTests.lib
{
    [TestClass]
    public class HandlerTests
    {
        private const string TEST_VALID_URL = "http://165.22.8.132/api/";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HostsHandler_Null()
        {
            var hHandler = new HostsHandler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HostsHandler_EmptyString()
        {
            var hHandler = new HostsHandler(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HostsHandler_NullAddUpdateHostAsync()
        {
            var hHandler = new HostsHandler(TEST_VALID_URL);

            await hHandler.AddUpdateHostAsync(null);
        }

        [TestMethod]
        public async Task HostsHandler_DefaultAddUpdateHostAsync()
        {
            var hHandler = new HostsHandler(TEST_VALID_URL);

            await hHandler.AddUpdateHostAsync(new DMTP.lib.Databases.Tables.Hosts());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task JobHandler_Null()
        {
            var jHandler = new JobHandler(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WorkerHandler_Null()
        {
            var wHandler = new WorkerHandler(null);
        }
    }
}