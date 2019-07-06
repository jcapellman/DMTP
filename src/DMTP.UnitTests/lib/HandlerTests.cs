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
            var hHandler = new HostsHandler("none");

            await hHandler.AddUpdateHostAsync(null);
        }

        [TestMethod]
        public async Task HostsHandler_DefaultAddUpdateHostAsync()
        {
            var hHandler = new HostsHandler("none");

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