using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.Core.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace OfficeDevPnP.Core.Utilities.Tests
{
    [TestClass()]
    public class JsonUtilityTests
    {
        [TestMethod()]
        public void SerializeTest()
        {
            List<TestObject> testObjects = new List<TestObject>();
            testObjects.Add(new TestObject() { Param1 = "Test", Param2 = "Test" });

            var jsonString = JsonUtility.Serialize(testObjects);

            Assert.AreEqual(jsonString, @"[{""Param1"":""Test"",""Param2"":""Test""}]");
        }

        [TestMethod()]
        public void DeserializeTest()
        {
            var jsonString = @"[{""Param1"":""Test"",""Param2"":""Test""}]";

            var listofobjects = JsonUtility.Deserialize<List<TestObject>>(jsonString);

            Assert.IsInstanceOfType(listofobjects, typeof(List<TestObject>));

            Assert.IsTrue(listofobjects.First().Param2 == "Test");

        }

        public class TestObject
        {
            public string Param1 { get; set; }
            public string Param2 { get; set; }
        }
    }
}
