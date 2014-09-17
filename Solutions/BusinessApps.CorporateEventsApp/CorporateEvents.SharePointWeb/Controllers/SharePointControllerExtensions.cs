using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateEvents.SharePointWeb.Controllers {
    public static class SharePointControllerExtensions {
        internal static ClientContext GetUserClientContextForSPHost(this HttpContextBase context) {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(context);
            var clientContext = spContext.CreateUserClientContextForSPHost();
            return clientContext;
        }
    }
}