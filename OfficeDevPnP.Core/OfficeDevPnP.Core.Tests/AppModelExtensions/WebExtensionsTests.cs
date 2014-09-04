using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Tests;
namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass()]
    public class WebExtensionsTests
    {
        const string INDEXED_PROPERTY_KEY = "vti_indexedpropertykeys";
        private string _key = null;
        private string _value_string = null;
        private int _value_int = 12345;

        #region SETUP AND TEARDOWN
        [TestInitialize()]
        public void Initialize()
        {
            _key = "TEST_KEY_" + DateTime.Now.ToFileTime();
            _value_string = "TEST_VALUE_" + DateTime.Now.ToFileTime();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var props = clientContext.Web.AllProperties;
                clientContext.Load(props);
                clientContext.ExecuteQuery();

                if (props.FieldValues.ContainsKey(_key))
                {
                    props[_key] = null;
                    props.FieldValues.Remove(_key);
                }
                if (props.FieldValues.ContainsKey(INDEXED_PROPERTY_KEY))
                {
                    props[INDEXED_PROPERTY_KEY] = null;
                    props.FieldValues.Remove(INDEXED_PROPERTY_KEY);
                }
                clientContext.Web.Update();
                clientContext.ExecuteQuery();
            }
        }
        #endregion

        #region PROPBAG tests
        [TestMethod()]
        public void SetPropertyBagValueTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.SetPropertyBagValue(_key, _value_int);

                var props = clientContext.Web.AllProperties;
                clientContext.Load(props);
                clientContext.ExecuteQuery();
                Assert.IsTrue(props.FieldValues.ContainsKey(_key));
                Assert.AreEqual(_value_int, props.FieldValues[_key] as int?);
            }
        }

        [TestMethod()]
        public void SetPropertyBagValueTest1()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.SetPropertyBagValue(_key, _value_string);

                var props = clientContext.Web.AllProperties;
                clientContext.Load(props);
                clientContext.ExecuteQuery();
                Assert.IsTrue(props.FieldValues.ContainsKey(_key), "Entry not added");
                Assert.AreEqual(_value_string, props.FieldValues[_key] as string, "Entry not set with correct value");
            }
        }

        [TestMethod()]
        public void RemovePropertyBagValueTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                var props = web.AllProperties;
                web.Context.Load(props);
                web.Context.ExecuteQuery();

                props[_key] = _value_string;

                web.Update();
                web.Context.ExecuteQuery();

                web.RemovePropertyBagValue(_key);

                props.RefreshLoad();
                clientContext.ExecuteQuery();
                Assert.IsFalse(props.FieldValues.ContainsKey(_key), "Entry not removed");
            }
        }

        [TestMethod()]
        public void GetPropertyBagValueIntTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                var props = web.AllProperties;
                web.Context.Load(props);
                web.Context.ExecuteQuery();

                props[_key] = _value_int;

                web.Update();
                web.Context.ExecuteQuery();

                var intValue = web.GetPropertyBagValueInt(_key, -1);

                Assert.IsInstanceOfType(intValue, typeof(int?), "No int value returned");
                Assert.AreEqual(_value_int, intValue, "Incorrect value returned");

                // Check for non-existing key
                intValue = web.GetPropertyBagValueInt("_key_" + DateTime.Now.ToFileTime(), -12345);
                Assert.IsInstanceOfType(intValue, typeof(int?), "No int value returned");
                Assert.AreEqual(-12345, intValue, "Incorrect value returned");
            }
        }

        [TestMethod()]
        public void GetPropertyBagValueStringTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var notExistingKey = "NOTEXISTINGKEY";
                var web = clientContext.Web;
                var props = web.AllProperties;
                web.Context.Load(props);
                web.Context.ExecuteQuery();

                props[_key] = _value_string;

                web.Update();
                web.Context.ExecuteQuery();

                var stringValue = web.GetPropertyBagValueString(_key, notExistingKey);

                Assert.IsInstanceOfType(stringValue, typeof(string), "No string value returned");
                Assert.AreEqual(_value_string, stringValue, "Incorrect value returned");

                // Check for non-existing key
                stringValue = web.GetPropertyBagValueString("_key_" + DateTime.Now.ToFileTime(), notExistingKey);
                Assert.IsInstanceOfType(stringValue, typeof(string), "No string value returned");
                Assert.AreEqual(notExistingKey, stringValue, "Incorrect value returned");
            }
        }

        [TestMethod()]
        public void PropertyBagContainsKeyTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                var props = web.AllProperties;
                web.Context.Load(props);
                web.Context.ExecuteQuery();

                props[_key] = _value_string;

                web.Update();
                web.Context.ExecuteQuery();

                Assert.IsTrue(web.PropertyBagContainsKey(_key));
            }
        }

        [TestMethod()]
        public void GetIndexedPropertyBagKeysTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                var props = web.AllProperties;
                var keys = web.GetIndexedPropertyBagKeys();

                Assert.IsInstanceOfType(keys, typeof(IEnumerable<string>), "No correct object returned");

                var keysList = keys.ToList();
                // Manually add an indexed property bag value
                if (!keysList.Contains(_key))
                {
                    keysList.Add(_key);
                    var encodedValues = GetEncodedValueForSearchIndexProperty(keysList);

                    web.Context.Load(props);
                    web.Context.ExecuteQuery();

                    props[INDEXED_PROPERTY_KEY] = encodedValues;

                    web.Update();
                    clientContext.ExecuteQuery();
                }
                keys = web.GetIndexedPropertyBagKeys();
                Assert.IsTrue(keys.Contains(_key), "Key not present");

                // Local Cleanup
                props.RefreshLoad();
                clientContext.ExecuteQuery();
                props[INDEXED_PROPERTY_KEY] = null;
                props.FieldValues.Remove(INDEXED_PROPERTY_KEY);
                web.Update();
                clientContext.ExecuteQuery();
            }

        }

        [TestMethod()]
        public void AddIndexedPropertyBagKeyTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                var props = web.AllProperties;
                clientContext.Load(props);
                clientContext.ExecuteQuery();

                web.AddIndexedPropertyBagKey(_key);

                props.RefreshLoad();
                clientContext.ExecuteQuery();

                Assert.IsTrue(props.FieldValues.ContainsKey(INDEXED_PROPERTY_KEY));

                // Local cleanup
                props[INDEXED_PROPERTY_KEY] = null;
                props.FieldValues.Remove(INDEXED_PROPERTY_KEY);
                web.Update();
                clientContext.ExecuteQuery();
            }
        }

        [TestMethod()]
        public void RemoveIndexedPropertyBagKeyTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                var props = web.AllProperties;

                // Manually add an indexed property bag value
                var encodedValues = GetEncodedValueForSearchIndexProperty(new List<string>() { _key });

                web.Context.Load(props);
                web.Context.ExecuteQuery();

                props[INDEXED_PROPERTY_KEY] = encodedValues;

                web.Update();
                clientContext.ExecuteQuery();

                // Remove the key
                Assert.IsTrue(web.RemoveIndexedPropertyBagKey(_key));
                props.RefreshLoad();
                clientContext.ExecuteQuery();
                // All keys should be gone
                Assert.IsFalse(props.FieldValues.ContainsKey(_key), "Key still present");
            }
        }

        private static string GetEncodedValueForSearchIndexProperty(IEnumerable<string> keys)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string current in keys)
            {
                stringBuilder.Append(Convert.ToBase64String(Encoding.Unicode.GetBytes(current)));
                stringBuilder.Append('|');
            }
            return stringBuilder.ToString();
        }

        #endregion
    }
}
