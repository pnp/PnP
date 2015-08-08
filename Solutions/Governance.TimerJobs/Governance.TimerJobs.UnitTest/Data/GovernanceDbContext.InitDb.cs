using System;
using System.Collections.Generic;
using System.Configuration;
using Governance.TimerJobs.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;

namespace Governance.TimerJobs.UnitTest
{
    [TestClass]
    public class GovernanceDbContext_InitDb
    {
        [TestMethod]
        public void DropCreateDbTest()
        {
            Database.SetInitializer<GovernanceDbContext>(new DropCreateDatabaseIfModelChanges<GovernanceDbContext>());
            var connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            var context = new GovernanceDbContext(connectionString);
            var site = context.Sites.FirstOrDefault();
            Debug.WriteLine(connectionString);
            Assert.Inconclusive(
                site != null ? "No changed in data model!" : "DB is empty now!");
        }
    }
}