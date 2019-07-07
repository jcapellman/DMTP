using System;
using System.ComponentModel.DataAnnotations;

using DMTP.lib.Databases;
using DMTP.lib.Databases.Tables;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DMTP.UnitTests.lib
{
    [TestClass]
    public class LiteDBTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddJob_Null()
        {
            var liteDb = new LiteDBDatabase();

            liteDb.AddJob(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void AddJob_Default()
        {
            var liteDb = new LiteDBDatabase();

            liteDb.AddJob(new Jobs());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateJob_Null()
        {
            var liteDb = new LiteDBDatabase();

            liteDb.UpdateJob(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void UpdateJob_Default()
        {
            var liteDb = new LiteDBDatabase();

            liteDb.UpdateJob(new Jobs());
        }
    }
}