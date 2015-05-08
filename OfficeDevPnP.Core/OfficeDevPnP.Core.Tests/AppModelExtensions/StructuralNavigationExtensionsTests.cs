using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Enums;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass()]
    public class StructuralNavigationExtensionsTests
    {

        static string CurrentDynamicChildLimit = "__CurrentDynamicChildLimit";
        static string GlobalDynamicChildLimit = "__GlobalDynamicChildLimit";

        #region Test initialize and cleanup
        static bool deactivateSiteFeatureOnTeardown = false;
        static bool deactivateWebFeatureOnTeardown = false;
        static Guid publishingSiteFeatureId = new Guid("f6924d36-2fa8-4f0b-b16d-06b7250180fa");
        static Guid publishingWebFeatureId = new Guid("94c94ca6-b32f-4da9-a9e3-1f3d343d7ecb");

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                Web web;
                Site site;
                site = ctx.Site;
                web = ctx.Site.RootWeb;
                if (!site.IsFeatureActive(publishingSiteFeatureId))
                {
                    site.ActivateFeature(publishingSiteFeatureId);
                    deactivateSiteFeatureOnTeardown = true;
                }
                if (!web.IsFeatureActive(publishingWebFeatureId))
                {
                    site.RootWeb.ActivateFeature(publishingWebFeatureId);
                    deactivateWebFeatureOnTeardown = true;
                }
            }
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                if (deactivateSiteFeatureOnTeardown)
                {
                    ctx.Site.DeactivateFeature(publishingSiteFeatureId);
                }
                if (deactivateWebFeatureOnTeardown)
                {
                    ctx.Web.DeactivateFeature(publishingWebFeatureId);
                }
            }
        }
        #endregion

        #region Area navigation settings tests (AreaNavigationSettings.aspx) / only applies to publishing sites
        [TestMethod]
        public void GetNavigationSettingsTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                //Set MaxDynamicItems upfront to the default value
                clientContext.Load(clientContext.Web, w => w.AllProperties);
                clientContext.ExecuteQueryRetry();
                clientContext.Web.AllProperties[CurrentDynamicChildLimit] = 18;
                clientContext.Web.AllProperties[GlobalDynamicChildLimit] = 22;
                clientContext.Web.Update();
                clientContext.ExecuteQueryRetry();

                var web = clientContext.Web;
                AreaNavigationEntity nav = web.GetNavigationSettings();

                Assert.AreEqual(18, (int)nav.CurrentNavigation.MaxDynamicItems);
                Assert.AreEqual(22, (int)nav.GlobalNavigation.MaxDynamicItems);

            }
        }

        [TestMethod]
        public void UpdateNavigationSettingsTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                //Set MaxDynamicItems upfront to the default value
                clientContext.Load(clientContext.Web, w => w.AllProperties);
                clientContext.ExecuteQueryRetry();
                clientContext.Web.AllProperties[CurrentDynamicChildLimit] = 20;
                clientContext.Web.AllProperties[GlobalDynamicChildLimit] = 20;
                clientContext.Web.Update();
                clientContext.ExecuteQueryRetry();
                
                AreaNavigationEntity nav = new AreaNavigationEntity();
                nav.GlobalNavigation.ManagedNavigation = false;
                nav.GlobalNavigation.MaxDynamicItems = 13;
                nav.GlobalNavigation.ShowSubsites = true;
                nav.GlobalNavigation.ShowPages = true;

                nav.CurrentNavigation.ManagedNavigation = false;
                nav.CurrentNavigation.MaxDynamicItems = 15;
                nav.CurrentNavigation.ShowSubsites = true;
                nav.CurrentNavigation.ShowPages = true;

                nav.Sorting = StructuralNavigationSorting.Automatically;
                nav.SortBy = StructuralNavigationSortBy.Title ;
                nav.SortAscending = true;

                clientContext.Web.UpdateNavigationSettings(nav);

                clientContext.Load(clientContext.Web, w => w.AllProperties);
                clientContext.ExecuteQueryRetry();
                int currentDynamicChildLimit = -1;
                int.TryParse(clientContext.Web.AllProperties[CurrentDynamicChildLimit].ToString(), out currentDynamicChildLimit);
                int globalDynamicChildLimit = -1;
                int.TryParse(clientContext.Web.AllProperties[GlobalDynamicChildLimit].ToString(), out globalDynamicChildLimit);

                Assert.AreEqual(13, globalDynamicChildLimit);
                Assert.AreEqual(15, currentDynamicChildLimit);

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Sorting was set to ManuallyButPagesAutomatically without pages being shown in structural navigation")]
        public void UpdateNavigationSettings2Test()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                var web = clientContext.Web;
                AreaNavigationEntity nav = new AreaNavigationEntity();
                nav.GlobalNavigation.MaxDynamicItems = 12;
                nav.GlobalNavigation.ShowSubsites = true;
                nav.GlobalNavigation.ShowPages = false;

                nav.CurrentNavigation.MaxDynamicItems = 14;
                nav.CurrentNavigation.ShowSubsites = false;
                nav.CurrentNavigation.ShowPages = false;

                // setting this throws an exception
                nav.Sorting = StructuralNavigationSorting.ManuallyButPagesAutomatically;
                nav.SortBy = StructuralNavigationSortBy.LastModifiedDate;
                nav.SortAscending = false;

                web.UpdateNavigationSettings(nav);

            }
        }
        #endregion

    }
}
