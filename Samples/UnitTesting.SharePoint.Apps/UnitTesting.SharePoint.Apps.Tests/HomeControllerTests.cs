using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UnitTesting.SharePoint.AppsWeb.Controllers;

namespace UnitTesting.SharePoint.AppsWeb.Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        Mock<HttpServerUtilityBase> server;
        Mock<HttpResponseBase> response;
        Mock<HttpRequestBase> request;
        Mock<HttpSessionStateBase> session;
        Mock<HttpContextBase> context;

        NameValueCollection formData, queryString;

        string remoteSiteUrl, hostWebUrl, contextToken;


        [TestInitialize]
        public void SetUp()
        {
            remoteSiteUrl = ConfigurationManager.AppSettings["RemoteSiteUrl"];
            Assert.IsFalse(String.IsNullOrEmpty(remoteSiteUrl), "Remote Site Url not found in app.config");

            hostWebUrl = ConfigurationManager.AppSettings["HostWebUrl"];
            Assert.IsFalse(String.IsNullOrEmpty(hostWebUrl), "Host Web Url not found in app.config");

            contextToken = ConfigurationManager.AppSettings["ContextToken"];
            Assert.IsFalse(String.IsNullOrEmpty(contextToken), "Context Token not found in app.config");

            server = new Mock<HttpServerUtilityBase>(MockBehavior.Loose);

            response = new Mock<HttpResponseBase>(MockBehavior.Default);

            request = new Mock<HttpRequestBase>(MockBehavior.Strict);

            request.Setup(r => r.Url).Returns(new Uri(remoteSiteUrl));

            formData = new NameValueCollection();

            formData.Add("SPAppToken", contextToken);

            request.Setup(r => r.Form).Returns(formData);

            queryString = new NameValueCollection();
            queryString.Add("SPHostUrl", hostWebUrl);
            queryString.Add("SPLanguage", "en-US");
            queryString.Add("SPClientTag", "0");
            queryString.Add("SPProductNumber", "16.0.3403.1219");

            request.Setup(r => r.QueryString).Returns(queryString);

            session = new Mock<HttpSessionStateBase>();

            context = new Mock<HttpContextBase>();

            context.SetupGet(c => c.Request).Returns(request.Object);
            context.SetupGet(c => c.Response).Returns(response.Object);
            context.SetupGet(c => c.Server).Returns(server.Object);
            context.SetupGet(c => c.Session).Returns(session.Object);
        }

        [TestMethod]
        public void GetHostWebTitle()
        {
            // Arrange
            HomeController controller = new HomeController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            // Act
            string result = controller.GetHostWebTitle();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Vardhaman", result);
        }

        [TestMethod]
        public void GetCurrentUserTitle()
        {
            // Arrange
            HomeController controller = new HomeController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            // Act
            string result = controller.GetCurrentUserTitle();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Content and Code Dev 3", result);
        }

        [TestMethod]
        public void GetAppOnlyCurrentUserTitle()
        {
            // Arrange
            HomeController controller = new HomeController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            // Act
            string result = controller.GetAppOnlyCurrentUserTitle();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SharePoint App", result);
        }
    }
}
