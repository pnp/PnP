using System;
using System.Security.Cryptography;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Tests;

namespace Branding.DetectMasterPageChanges
{
    [TestClass]
    public class MasterPageTests
    {
        private ClientContext _clientContext;
        private const string KnownHashOfSeattle = "16-E7-18-F5-6E-59-DE-6A-BA-5C-5F-19-52-BB-F4-08";
        private const string SeattleSiteRelativeUrl = "/_catalogs/masterpage/seattle.master";

        [TestInitialize]
        public void Initialisze()
        {
            _clientContext = TestCommon.CreateClientContext();
        }

        [TestMethod]
        public void SeattleMasterPageIsUnchanged()
        {
            var web = _clientContext.Web;
            //need to get the server relative url
            _clientContext.Load(web, w => w.ServerRelativeUrl);
            _clientContext.ExecuteQuery();
            //Use the existing context to directly get a copy of the seattle master page
            FileInformation seattle = File.OpenBinaryDirect(_clientContext, web.ServerRelativeUrl + SeattleSiteRelativeUrl);
            Assert.IsNotNull(seattle);

            //Compute an MD5 hash of the file
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(seattle.Stream);
            //Convert to a hex string for human consumption
            string hex = BitConverter.ToString(hash);
            //Check against last known MD5 hash
            Assert.AreEqual(KnownHashOfSeattle , hex);
        }
    }
}
