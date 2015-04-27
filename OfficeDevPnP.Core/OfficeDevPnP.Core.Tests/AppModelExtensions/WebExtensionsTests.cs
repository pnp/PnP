using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Tests;
using System.IO;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass()]
    public class WebExtensionsTests
    {
        const string INDEXED_PROPERTY_KEY = "vti_indexedpropertykeys";
        private string _key = null;
        private string _value_string = null;
        private int _value_int = 12345;
        const string APPNAME = "HelloWorldApp";
        private ClientContext clientContext;

        #region Test initialize and cleanup
        [TestInitialize()]
        public void Initialize()
        {
            clientContext = TestCommon.CreateClientContext();

            _key = "TEST_KEY_" + DateTime.Now.ToFileTime();
            _value_string = "TEST_VALUE_" + DateTime.Now.ToFileTime();

            // Activate sideloading in order to test apps
            clientContext.Load(clientContext.Site, s => s.Id);
            clientContext.ExecuteQueryRetry();
            clientContext.Site.ActivateFeature(OfficeDevPnP.Core.Constants.APPSIDELOADINGFEATUREID);
        }

        [TestCleanup()]
        public void Cleanup()
        {
            // Deactivate sideloading
            clientContext.Load(clientContext.Site);
            clientContext.ExecuteQueryRetry();
            clientContext.Site.DeactivateFeature(OfficeDevPnP.Core.Constants.APPSIDELOADINGFEATUREID);

            var props = clientContext.Web.AllProperties;
            clientContext.Load(props);
            clientContext.ExecuteQueryRetry();

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
            clientContext.ExecuteQueryRetry();

            var instances = AppCatalog.GetAppInstances(clientContext, clientContext.Web);
            clientContext.Load(instances);
            clientContext.ExecuteQueryRetry();

            string appToRemove = APPNAME;
            #if CLIENTSDKV15
            appToRemove += "15";
            #endif            

            foreach (var instance in instances)
            {
                if (string.Equals(instance.Title, appToRemove, StringComparison.OrdinalIgnoreCase))
                {
                    instance.Uninstall();
                    clientContext.ExecuteQueryRetry();
                    break;
                }
            }
            clientContext.Dispose();
        }
        #endregion

        #region Property bag tests
        [TestMethod()]
        public void SetPropertyBagValueIntTest()
        {
            clientContext.Web.SetPropertyBagValue(_key, _value_int);

            var props = clientContext.Web.AllProperties;
            clientContext.Load(props);
            clientContext.ExecuteQueryRetry();
            Assert.IsTrue(props.FieldValues.ContainsKey(_key));
            Assert.AreEqual(_value_int, props.FieldValues[_key] as int?);
        }

        [TestMethod()]
        public void SetPropertyBagValueStringTest()
        {
            clientContext.Web.SetPropertyBagValue(_key, _value_string);

            var props = clientContext.Web.AllProperties;
            clientContext.Load(props);
            clientContext.ExecuteQueryRetry();
            Assert.IsTrue(props.FieldValues.ContainsKey(_key), "Entry not added");
            Assert.AreEqual(_value_string, props.FieldValues[_key] as string, "Entry not set with correct value");
        }

        [TestMethod()]
        public void SetPropertyBagValueMultipleRunsTest()
        {
            string key2 = _key + "_multiple";
            clientContext.Web.SetPropertyBagValue(key2, _value_string);
            clientContext.Web.SetPropertyBagValue(_key, _value_string);

            var props = clientContext.Web.AllProperties;
            clientContext.Load(props);
            clientContext.ExecuteQueryRetry();
            Assert.IsTrue(props.FieldValues.ContainsKey(_key), "Entry not added");
            Assert.AreEqual(_value_string, props.FieldValues[_key] as string, "Entry not set with correct value");
        }

        [TestMethod()]
        public void RemovePropertyBagValueTest()
        {
            var web = clientContext.Web;
            var props = web.AllProperties;
            web.Context.Load(props);
            web.Context.ExecuteQueryRetry();

            props[_key] = _value_string;

            web.Update();
            web.Context.ExecuteQueryRetry();

            web.RemovePropertyBagValue(_key);

            props.RefreshLoad();
            clientContext.ExecuteQueryRetry();
            Assert.IsFalse(props.FieldValues.ContainsKey(_key), "Entry not removed");
        }

        [TestMethod()]
        public void GetPropertyBagValueIntTest()
        {
            var web = clientContext.Web;
            var props = web.AllProperties;
            web.Context.Load(props);
            web.Context.ExecuteQueryRetry();

            props[_key] = _value_int;

            web.Update();
            web.Context.ExecuteQueryRetry();

            var intValue = web.GetPropertyBagValueInt(_key, -1);

            Assert.IsInstanceOfType(intValue, typeof(int?), "No int value returned");
            Assert.AreEqual(_value_int, intValue, "Incorrect value returned");

            // Check for non-existing key
            intValue = web.GetPropertyBagValueInt("_key_" + DateTime.Now.ToFileTime(), -12345);
            Assert.IsInstanceOfType(intValue, typeof(int?), "No int value returned");
            Assert.AreEqual(-12345, intValue, "Incorrect value returned");
        }

        [TestMethod()]
        public void GetPropertyBagValueStringTest()
        {
            var notExistingKey = "NOTEXISTINGKEY";
            var web = clientContext.Web;
            var props = web.AllProperties;
            web.Context.Load(props);
            web.Context.ExecuteQueryRetry();

            props[_key] = _value_string;

            web.Update();
            web.Context.ExecuteQueryRetry();

            var stringValue = web.GetPropertyBagValueString(_key, notExistingKey);

            Assert.IsInstanceOfType(stringValue, typeof(string), "No string value returned");
            Assert.AreEqual(_value_string, stringValue, "Incorrect value returned");

            // Check for non-existing key
            stringValue = web.GetPropertyBagValueString("_key_" + DateTime.Now.ToFileTime(), notExistingKey);
            Assert.IsInstanceOfType(stringValue, typeof(string), "No string value returned");
            Assert.AreEqual(notExistingKey, stringValue, "Incorrect value returned");
        }

        [TestMethod()]
        public void PropertyBagContainsKeyTest()
        {
            var web = clientContext.Web;
            var props = web.AllProperties;
            web.Context.Load(props);
            web.Context.ExecuteQueryRetry();

            props[_key] = _value_string;

            web.Update();
            web.Context.ExecuteQueryRetry();

            Assert.IsTrue(web.PropertyBagContainsKey(_key));
        }

        [TestMethod()]
        public void GetIndexedPropertyBagKeysTest()
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
                web.Context.ExecuteQueryRetry();

                props[INDEXED_PROPERTY_KEY] = encodedValues;

                web.Update();
                clientContext.ExecuteQueryRetry();
            }
            keys = web.GetIndexedPropertyBagKeys();
            Assert.IsTrue(keys.Contains(_key), "Key not present");

            // Local Cleanup
            props.RefreshLoad();
            clientContext.ExecuteQueryRetry();
            props[INDEXED_PROPERTY_KEY] = null;
            props.FieldValues.Remove(INDEXED_PROPERTY_KEY);
            web.Update();
            clientContext.ExecuteQueryRetry();
        }

        [TestMethod()]
        public void AddIndexedPropertyBagKeyTest()
        {
            var web = clientContext.Web;
            var props = web.AllProperties;
            clientContext.Load(props);
            clientContext.ExecuteQueryRetry();

            web.AddIndexedPropertyBagKey(_key);

            props.RefreshLoad();
            clientContext.ExecuteQueryRetry();

            Assert.IsTrue(props.FieldValues.ContainsKey(INDEXED_PROPERTY_KEY));

            // Local cleanup
            props[INDEXED_PROPERTY_KEY] = null;
            props.FieldValues.Remove(INDEXED_PROPERTY_KEY);
            web.Update();
            clientContext.ExecuteQueryRetry();
        }

        [TestMethod()]
        public void RemoveIndexedPropertyBagKeyTest()
        {
            var web = clientContext.Web;
            var props = web.AllProperties;

            // Manually add an indexed property bag value
            var encodedValues = GetEncodedValueForSearchIndexProperty(new List<string>() { _key });

            web.Context.Load(props);
            web.Context.ExecuteQueryRetry();

            props[INDEXED_PROPERTY_KEY] = encodedValues;

            web.Update();
            clientContext.ExecuteQueryRetry();

            // Remove the key
            Assert.IsTrue(web.RemoveIndexedPropertyBagKey(_key));
            props.RefreshLoad();
            clientContext.ExecuteQueryRetry();
            // All keys should be gone
            Assert.IsFalse(props.FieldValues.ContainsKey(_key), "Key still present");
        }
        #endregion

        #region Provisioning Tests

        [TestMethod]
        public void GetProvisioningTemplateTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var template = clientContext.Web.GetProvisioningTemplate();
                Assert.IsInstanceOfType(template, typeof (ProvisioningTemplate));
            }
        }
        #endregion

        #region App instance tests
        [TestMethod()]
        public void GetAppInstancesTest()
        {
            var web = clientContext.Web;

            var instances = web.GetAppInstances();
            Assert.IsInstanceOfType(instances, typeof(ClientObjectList<AppInstance>), "Incorrect return value");
            int instanceCount = instances.Count;

            #if !CLIENTSDKV15
            byte[] appToLoad = OfficeDevPnP.Core.Tests.Properties.Resources.HelloWorldApp;
            #else
            byte[] appToLoad = OfficeDevPnP.Core.Tests.Properties.Resources.HelloWorldApp15;
            #endif

            using (MemoryStream stream = new MemoryStream(appToLoad))
            {
                web.LoadApp(stream, 1033);
                clientContext.ExecuteQueryRetry();
            }

            instances = web.GetAppInstances();
            Assert.AreNotEqual(instances.Count, instanceCount, "App count is same after upload");
        }

        [TestMethod()]
        public void RemoveAppInstanceByTitleTest()
        {
            var web = clientContext.Web;

            var instances = web.GetAppInstances();
            Assert.IsInstanceOfType(instances, typeof(ClientObjectList<AppInstance>), "Incorrect return value");
            int instanceCount = instances.Count;

            #if !CLIENTSDKV15
            byte[] appToLoad = OfficeDevPnP.Core.Tests.Properties.Resources.HelloWorldApp;
            #else
            byte[] appToLoad = OfficeDevPnP.Core.Tests.Properties.Resources.HelloWorldApp15;
            #endif

            using (MemoryStream stream = new MemoryStream(appToLoad))
            {
                web.LoadApp(stream, 1033);
                clientContext.ExecuteQueryRetry();
            }

            string appToRemove = APPNAME;
            
            #if CLIENTSDKV15
            appToRemove += "15";
            #endif
            
            Assert.IsTrue(web.RemoveAppInstanceByTitle(appToRemove));

            instances = web.GetAppInstances();

            Assert.AreEqual(instances.Count, instanceCount);
        }
        #endregion

        #region Install solution tests
        // DO NOT RUN. The DesignPackage.Install() function, used by this test, wipes the composed look gallery, breaking other tests.")]
        [Ignore()]
        [TestMethod()]
        public void InstallSolutionTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Set up

                // Write the test solution to a local temporary file
                string solutionpath = Path.Combine(Path.GetTempPath(), "testsolution.wsp");
                System.IO.File.WriteAllBytes(solutionpath, OfficeDevPnP.Core.Tests.Properties.Resources.TestSolution);

                clientContext.Site.InstallSolution(new Guid(OfficeDevPnP.Core.Tests.Properties.Resources.TestSolutionGuid), solutionpath);

                // Check if the solution file is uploaded
                var solutionGallery = clientContext.Site.RootWeb.GetCatalog((int)ListTemplateType.SolutionCatalog);

                var camlQuery = new CamlQuery();
                camlQuery.ViewXml = string.Format(
      @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='SolutionId' /><Value Type='Guid'>{0}</Value></Eq></Where> 
            </Query> 
             <ViewFields><FieldRef Name='ID' /></ViewFields> 
      </View>", new Guid(OfficeDevPnP.Core.Tests.Properties.Resources.TestSolutionGuid));

                var solutions = solutionGallery.GetItems(camlQuery);
                clientContext.Load(solutions);
                clientContext.ExecuteQueryRetry();

                // Test

                Assert.IsTrue(solutions.Any(),"No solution files available");

                // Check if we can activate Test Feature on rootweb
                clientContext.Load(clientContext.Web);
                clientContext.ExecuteQueryRetry();

              //  clientContext.Web.ActivateFeature(new Guid(OfficeDevPnP.Core.Tests.Properties.Resources.TestSolutionFeatureGuid));
              //  Assert.IsTrue(clientContext.Web.IsFeatureActive(new Guid(OfficeDevPnP.Core.Tests.Properties.Resources.TestSolutionFeatureGuid)), "Test feature not activated");
             
                // Teardown
                // Done using the local file, remove it
                System.IO.File.Delete(solutionpath);
                clientContext.Site.UninstallSolution(new Guid(OfficeDevPnP.Core.Tests.Properties.Resources.TestSolutionGuid),"testsolution.wsp");
            }
        }

        // DO NOT RUN. The DesignPackage.Install() function, used by this test, wipes the composed look gallery, breaking other tests.")]
        [Ignore()]
        [TestMethod()]
        public void UninstallSolutionTest()
        {
            // Set up
            string solutionpath = Path.Combine(Path.GetTempPath(), "testsolution.wsp");
            System.IO.File.WriteAllBytes(solutionpath, OfficeDevPnP.Core.Tests.Properties.Resources.TestSolution);

            clientContext.Site.InstallSolution(new Guid(OfficeDevPnP.Core.Tests.Properties.Resources.TestSolutionGuid), solutionpath);

            // Execute test

            clientContext.Site.UninstallSolution(new Guid(OfficeDevPnP.Core.Tests.Properties.Resources.TestSolutionGuid),"testsolution.wsp");

            // Check if the solution file is uploaded
            var solutionGallery = clientContext.Site.RootWeb.GetCatalog((int)ListTemplateType.SolutionCatalog);

            var camlQuery = new CamlQuery();
            camlQuery.ViewXml = string.Format(
  @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='SolutionId' /><Value Type='Guid'>{0}</Value></Eq></Where> 
            </Query> 
             <ViewFields><FieldRef Name='ID' /></ViewFields> 
      </View>", new Guid(OfficeDevPnP.Core.Tests.Properties.Resources.TestSolutionGuid));

            var solutions = solutionGallery.GetItems(camlQuery);
            clientContext.Load(solutions);
            clientContext.ExecuteQueryRetry();
            Assert.IsFalse(solutions.Any(),"There are still solutions installed");

            Assert.IsFalse(clientContext.Web.IsFeatureActive(new Guid(OfficeDevPnP.Core.Tests.Properties.Resources.TestSolutionFeatureGuid)));
      
            // Teardown
            System.IO.File.Delete(solutionpath);
        }
        #endregion

        #region Helper methods
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
