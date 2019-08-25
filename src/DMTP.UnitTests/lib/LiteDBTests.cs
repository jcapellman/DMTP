using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

using DMTP.lib.dal.Databases;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DMTP.UnitTests.lib
{
    [TestClass]
    public class LiteDBTests
    {
        private DatabaseManager _dbManager;

        [TestInitialize]
        public void Setup()
        {
            _dbManager = new DatabaseManager(new LiteDBDatabase(), new InMemoryCache());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddJob_Null()
        {
            new JobManager(_dbManager).AddJob(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void AddJob_Default()
        {
            new JobManager(_dbManager).AddJob(new Jobs());
        }

        [TestMethod]
        public void AddJob_Valid()
        {
            var job = new Jobs
            {
                Name = DateTime.Now.ToLongDateString(),
                ModelType = "Testing",
                TrainingDataPath = Path.GetRandomFileName()
            };

            new JobManager(_dbManager).AddJob(job);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateJob_Null()
        {
            new JobManager(_dbManager).UpdateJob(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void UpdateJob_Default()
        {
            new JobManager(_dbManager).UpdateJob(new Jobs());
        }
    }
}