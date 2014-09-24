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

            Assert.AreEqual(@"[{""Param1"":""Test"",""Param2"":""Test""}]", jsonString);
        }

        [TestMethod()]
        public void DeserializeTest()
        {
            var jsonString = @"[{""Param1"":""Test"",""Param2"":""Test""}]";

            var listofobjects = JsonUtility.Deserialize<List<TestObject>>(jsonString);

            Assert.AreEqual("Test", listofobjects.First().Param2);
        }

        [TestMethod()]
        public void DeserializeListTest()
        {
            var jsonString = @"{""Param1"":""Test"",""List1"": [ ""Alpha"", ""Beta"", ""Gamma"" ] }";

            // Act
            var testObject = JsonUtility.Deserialize<TestList>(jsonString);

            Assert.AreEqual("Test", testObject.Param1);
            Assert.AreEqual(3, testObject.List1.Count);
            Assert.AreEqual("Gamma", testObject.List1[2]);            
        }

        [TestMethod()]
        public void DeserializeListIsNotFixedSizeTest()
        {
            var jsonString = @"{""Param1"":""Test"",""List1"": [ ""Alpha"", ""Beta"", ""Gamma"" ] }";
            var testObject = JsonUtility.Deserialize<TestList>(jsonString);
            Console.WriteLine(testObject.List1.GetType().FullName);

            // Act
            testObject.List1.Add("test");

            Assert.AreEqual(4, testObject.List1.Count);
        }

        [TestMethod()]
        public void DeserializeListNoDataStillWorksTest()
        {
            var jsonString = @"{""Param1"":""Test""}";
            var testObject = JsonUtility.Deserialize<TestList>(jsonString);
            Console.WriteLine(testObject.List1.GetType().FullName);

            // Act
            testObject.List1.Add("test");

            Assert.AreEqual(1, testObject.List1.Count);
        }

        public class TestObject
        {
            public string Param1 { get; set; }
            public string Param2 { get; set; }
        }

        public class TestList
        {
            public string Param1 { get; set; }
            public IList<string> List1 { get; set; }

            public TestList()
            {
                List1 = new List<string>();
            }
        }
    }
}
