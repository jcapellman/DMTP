using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

using DMTP.lib.dal.Databases;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.Managers;

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

            new JobManager(liteDb).AddJob(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void AddJob_Default()
        {
            var liteDb = new LiteDBDatabase();

            new JobManager(liteDb).AddJob(new Jobs());
        }

        [TestMethod]
        public void AddJob_Valid()
        {
            var liteDb = new LiteDBDatabase();

            var job = new Jobs
            {
                Name = DateTime.Now.ToLongDateString(),
                ModelType = "Testing",
                TrainingDataPath = Path.GetRandomFileName()
            };

            new JobManager(liteDb).AddJob(job);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateJob_Null()
        {
            var liteDb = new LiteDBDatabase();

            new JobManager(liteDb).UpdateJob(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void UpdateJob_Default()
        {
            var liteDb = new LiteDBDatabase();

            new JobManager(liteDb).UpdateJob(new Jobs());
        }
    }
}