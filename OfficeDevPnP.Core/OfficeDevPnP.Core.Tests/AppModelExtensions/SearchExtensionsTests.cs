using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass]
    public class SearchExtensionsTests
    {

        [TestMethod]
        public void SetSiteCollectionSearchCenterUrlTest()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                // Set search center url
                clientContext.Web.SetSiteCollectionSearchCenterUrl("/search/pages");
                string url = clientContext.Web.GetSiteCollectionSearchCenterUrl();

                Assert.AreEqual(url, "/search/pages");

                // Clear search center url
                clientContext.Web.SetSiteCollectionSearchCenterUrl("");
                url = clientContext.Web.GetSiteCollectionSearchCenterUrl();
                Assert.AreEqual(url, "");
            }
        }
    }
}
