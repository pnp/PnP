using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace System.Tests
{
    [TestClass()]
    public class DateTimeExtensionsTests
    {
        [TestMethod()]
        public void AsTimeAgoStringTest()
        {
            DateTime now = DateTime.Now.AddDays(-1);

            Assert.AreEqual(now.AsTimeAgoString(), "yesterday");

            now = DateTime.Now.AddMonths(-1);
            Assert.AreEqual(now.AsTimeAgoString(), "a month ago");
        }

        [TestMethod()]
        public void AsTimeAgoStringTest1()
        {
            DateTime now = DateTime.Now.AddDays(-1);

            Assert.AreEqual(now.AsTimeAgoString(true), "yesterday");

            now = DateTime.Now.AddMonths(-1);
            Assert.AreEqual(now.AsTimeAgoString(true), "a month ago");
        }

        [TestMethod()]
        public void AsTimeAgoStringTest2()
        {
            DateTimeOffset offset = DateTimeOffset.Now.AddDays(-1);

            Assert.AreEqual(offset.AsTimeAgoString(), "yesterday");

            offset = DateTimeOffset.Now.AddMonths(-1);

            Assert.AreEqual(offset.AsTimeAgoString(), "a month ago");
        }
    }
}
