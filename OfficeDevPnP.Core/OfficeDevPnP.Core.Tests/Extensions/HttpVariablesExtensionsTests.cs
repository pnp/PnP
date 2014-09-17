using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
namespace System.Web.Tests
{
    [TestClass()]
    public class HttpVariablesExtensionsTests
    {
        private NameValueCollection nameValueCollection;

        [TestInitialize()]
        public void Initialize()
        {
            nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("DateTime", DateTime.Now.ToString());
            nameValueCollection.Add("String", "StringValue");
            nameValueCollection.Add("Int", int.MaxValue.ToString());
            nameValueCollection.Add("Guid", "9cf951cd-973e-4953-bba5-75704affb24c");
            nameValueCollection.Add("Bool", "true");
            nameValueCollection.Add("Long", long.MaxValue.ToString());
            nameValueCollection.Add("Enum", "Value1");
        }
        [TestMethod()]
        public void GetQueryStringTest()
        {
            Func<string, DateTime> convertToDate = delegate(string s)
               {
                   return DateTime.Parse(s);
               };

            var returnValue = nameValueCollection.GetQueryString("DateTime", convertToDate, DateTime.Now.AddDays(-1));

            Assert.AreEqual(returnValue.ToString("yyyy-MM-dd HH:mm"), DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        }

        [TestMethod()]
        public void HasVariableTest()
        {
            Assert.IsTrue(nameValueCollection.HasVariable("DateTime"));
        }

        [TestMethod()]
        public void AsStringTest()
        {
            var stringValue = nameValueCollection.AsString("String");
            Assert.IsInstanceOfType(stringValue, typeof(string));
        }

        [TestMethod()]
        public void AsStringTest1()
        {
            var stringValue = nameValueCollection.AsString("String", "DefaultValue");

            Assert.AreNotEqual(stringValue, "DefaultValue");
            Assert.AreEqual(stringValue, "StringValue");

            stringValue = nameValueCollection.AsString("StringNotExisting", "DefaultValue");

            Assert.AreEqual(stringValue, "DefaultValue");
        }

        [TestMethod()]
        public void AsIntTest()
        {
            Assert.AreEqual(nameValueCollection.AsInt("Int"), int.MaxValue);
        }

        [TestMethod()]
        public void AsIntTest1()
        {
            Assert.AreEqual(nameValueCollection.AsInt("Int", int.MinValue), int.MaxValue);
            Assert.AreEqual(nameValueCollection.AsInt("IntNotExisting", int.MinValue), int.MinValue);
        }

        [TestMethod()]
        public void AsLongTest()
        {
            Assert.AreEqual(nameValueCollection.AsLong("Long"), long.MaxValue);
        }

        [TestMethod()]
        public void AsLongTest1()
        {
            Assert.AreEqual(nameValueCollection.AsLong("Long", long.MinValue), long.MaxValue);
            Assert.AreEqual(nameValueCollection.AsLong("LongNotExisting", long.MinValue), long.MinValue);
        }

        [TestMethod()]
        public void AsBoolTest()
        {
            Assert.IsTrue(nameValueCollection.AsBool("Bool"));
        }

        [TestMethod()]
        public void AsBoolTest1()
        {
            Assert.IsTrue(nameValueCollection.AsBool("Bool", false));
            Assert.IsFalse(nameValueCollection.AsBool("BoolNotExists", false));

        }

        [TestMethod()]
        public void AsGuidTest()
        {
            Assert.IsInstanceOfType(nameValueCollection.AsGuid("Guid"), typeof(Guid));
        }

        [TestMethod()]
        public void AsGuidTest1()
        {
            Guid newGuid = Guid.NewGuid();
            Assert.IsInstanceOfType(nameValueCollection.AsGuid("Guid", newGuid), typeof(Guid));
            Assert.AreEqual(nameValueCollection.AsGuid("GuidNotExists", newGuid), newGuid);

        }

        [TestMethod()]
        public void AsEnumTest()
        {
            var enumValue = nameValueCollection.AsEnum<TestEnum>("Enum", TestEnum.Value2);
            Assert.AreEqual(enumValue, TestEnum.Value1);

            enumValue = nameValueCollection.AsEnum<TestEnum>("EnumNotExists", TestEnum.Value2);
            Assert.AreEqual(enumValue, TestEnum.Value2);

        }

        private enum TestEnum
        {
            Value1 = 0,
            Value2 = 1
        }
    }
}
